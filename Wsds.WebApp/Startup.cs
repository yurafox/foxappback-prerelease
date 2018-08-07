﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wsds.DAL.Providers;
using Wsds.DAL.Identity;
using Wsds.DAL.Entities;
using Wsds.DAL.Repository.Abstract;
using Wsds.DAL.Repository.Specific;
using Wsds.WebApp.Auth;
using Microsoft.AspNetCore.HttpOverrides;
using CachingFramework.Redis;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Infrastructure.Facade;
using StackExchange.Redis;
using Wsds.DAL.Services.Abstract;
using Wsds.DAL.Services.Specific;
using Wsds.WebApp.Auth.Protection;
using RabbitMQ.Client;
using Serilog;
using Wsds.WebApp.Filters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Wsds.WebApp
{
    public class Startup
    {
        private IHostingEnvironment _environment;
        public IConfigurationRoot Configuration { get; }
        private Version _apiVersion;

        public Startup(IHostingEnvironment env)
        {
            _environment = env;
            _apiVersion = typeof(Startup).Assembly.GetName().Version;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            #region Serilog Configuration https://serilog.net/

            string environment = "dev";

            #if !DEBUG
                   environment = "prod";
            #endif

            var seqConfig = Configuration.GetSection("SeqConfig").GetSection(environment);
            var seqServer = seqConfig["host"];

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.Seq(seqServer) //it can be either http://dit-seq-10-80:5341/#/events or http://localhost:5341
                .CreateLogger();

            #endregion
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // add versioning
            services.AddApiVersioning(options =>
            {
                options.ApiVersionReader = new QueryStringApiVersionReader();
                options.ReportApiVersions = false;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
                options.DefaultApiVersion = new ApiVersion(_apiVersion.Major, _apiVersion.Minor);
            });

            // fox ssl enable
            services.AddMvc(options =>
            {
                //options.Filters.Add(new RequireHttpsAttribute());
                options.Filters.Add(typeof(CustomErrorFilter));
            });

            // fox add code for close many http redirects
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
            });

            // fox cors options
            services.AddCors(options =>
            {
                // foxtrot open policy will be change
                options.AddPolicy("AnyOrigin", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithExposedHeaders("X-SCN");
                });
            });

            services.AddDataProtection();

            var mainDataConnString = Configuration.GetConnectionString("MainDataConnection");
            services.AddSingleton<IConfiguration>(Configuration);
            
            //creating Serilog Singleton
            services.AddSingleton<Serilog.ILogger>(Log.Logger);

            var virtualCatalogId = Convert.ToInt64(Configuration["AppOptions:virtualId"]);
            var langId = Convert.ToInt64(Configuration["AppOptions:lang"]);


            // Redis singleton instantiation
            var redisConfig = GetRedisConfigBySolutionConfiguration();
            var redisCache = new Context(redisConfig);
            services.AddSingleton(redisCache);

            // RabbitMQ singleton instantiation
            IConfigurationSection mqOpts = Configuration.GetSection("rabbit");
            var rabbitConnFactory = new ConnectionFactory() {
                UserName = mqOpts.GetValue<string>("username"),
                Password = mqOpts.GetValue<string>("password"),
                HostName = mqOpts.GetValue<string>("hostname"),
                Port = mqOpts.GetValue<int>("port")
            };
            IConnection rabbitConnection = rabbitConnFactory.CreateConnection();
            services.AddSingleton<IConnection>(rabbitConnection);

            #region EntityProvider section
            
            EntityConfigDictionary.AddConfig("client_credit_cards",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.CreditCards2Json(t.id) as value from client_credit_cards t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.CreditCards2Json")
                    .SetSequence("SEQ_CLIENT_CREDIT_CARDS")
                    .SetBaseTable("CLIENT_CREDIT_CARDS")
                );

            EntityConfigDictionary.AddConfig("lo_entity_delivery_type",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.LoEntityDeliveryType2Json(t.id) as value " +
                                            "from lo_entity_delivery_type t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetBaseTable("lo_entity_delivery_type")
                    .SetSerializerFunc("Serialization.LoEntityDeliveryType2Json")
            );

            EntityConfigDictionary.AddConfig("lo_entity_office",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.LoEntityOffice2Json(t.id) as value " +
                                            "from lo_entity_office t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetBaseTable("lo_entity_office")
                    .SetSerializerFunc("Serialization.LoEntityOffice2Json")
            );


            EntityConfigDictionary.AddConfig("lo_delivery_types",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.LoDeliveryType2Json(t.id) as value " +
                                            "from LO_DELIVERY_TYPES t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetBaseTable("LO_DELIVERY_TYPES")
                    .SetSerializerFunc("Serialization.LoDeliveryType2Json")
            );

            EntityConfigDictionary.AddConfig("shipment",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.Shipment2Json(t.id) as value " +
                                            "from shipment t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetBaseTable("shipment")
                    .SetSerializerFunc("Serialization.Shipment2Json")
            );


            EntityConfigDictionary.AddConfig("app_params",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Json_object('id' value t.id, 'propName' value t.prop_name, 'propVal' value t.prop_value) as value " +
                                            "from APP_PROPS t")
                    .SetKeyField("id")
                    .SetValueField("value")
            );

            EntityConfigDictionary.AddConfig("application_keys",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select k.id, JSON_OBJECT ('id' value id, 'key' value key," +
                                         "'dateStart' value date_start, 'dateEnd' value date_end,'IdClient' value id_client) as value " +
                                         "from application_keys k")
                    .AddSqlCommandOrderBy("order by k.date_end desc fetch first 1 row only")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSequence("APPLICATION_KEYS_SEQ")
                    .SetBaseTable("APPLICATION_KEYS")
            );

            EntityConfigDictionary.AddConfig("actions",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select a.id, Serialization.Action2Json(a.id) as value from actions a")
                    .AddSqlCommandWhere("where a.is_active = 1 and a.date_start <= trunc(sysdate) and trunc(sysdate) <= a.date_end")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Action2Json")
            );

            EntityConfigDictionary.AddConfig("pages",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select p.id, Serialization.Page2Json(p.id) as value from pages p")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Page2Json")
            );

            EntityConfigDictionary.AddConfig("poll_question_answers",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select id, Json_object('id' value id, 'idPollQuestions' value id_poll_questions,'answer' value answer) as value " +
                                         "from poll_question_answers")
                    .SetKeyField("id")
                    .SetValueField("value")
            );

            EntityConfigDictionary.AddConfig("poll_questions",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select id, Json_object('id' value id, 'idPoll' value id_poll," +
                                         "'order' value priority, 'question' value question_text,'answerType' value answer_type) as value from poll_questions")
                    .SetKeyField("id")
                    .SetValueField("value")
            );

            EntityConfigDictionary.AddConfig("polls",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select id, Json_object('id' value id, 'dateStart' value date_start," +
                                         "'dateEnd' value date_end, 'urlBanner' value banner_url,'bannerText' value banner_text) as value from polls")
                    .SetKeyField("id")
                    .SetValueField("value")
            );

            EntityConfigDictionary.AddConfig("client_poll_answers",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select id, Json_object('id' value id, 'idPoll' value id_poll," +
                                         "'idPollQuestions' value id_poll_question, 'idClient' value id_client," +
                                         "'clientAnswer' value client_answer) as value from client_poll_answers")
                    .SetKeyField("id")
                    .SetValueField("value")
            );

            EntityConfigDictionary.AddConfig("client_address",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.*, Json_object('id' value id, 'idClient' value id_client, " +
                                         "'idCity' value id_city, 'zip' value zip, 'street' value street, 'lat' value lat, " +
                                         "'lng' value lng, 'isPrimary' value is_primary, 'idCountry' value id_country, " +
                                         "'city' value city, 'bldApp' value bld_app, 'recName' value recname, " +
                                         "'phone' value phone) as value from client_address t")
                    .AddSqlCommandWhere("where nvl(is_deleted,0)<>1")
                    .SetKeyField("id")
                    .SetSequence("SEQ_CLIENT_ADDRESS")
                    .SetValueField("value")
                    .SetBaseTable("CLIENT_ADDRESS")
                );

            EntityConfigDictionary.AddConfig("credit_product",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.s_id as sId, json_object('sId' value s_Id, 'sName' value s_name,'sDefProdId' value t.s_def_prod_id, "+
                                         "'sPartPay' value t.s_part_pay, 'sGracePeriod' value t.s_grace_period, 'maxTerm' value t.max_term, "+
                                         "'firstPay' value t.first_pmt,'monthCommissionPct' value t.month_commission_pct, 'yearPct' value t.year_pct, "+
                                         "'kpcPct' value t.kpc_pct, 'minAmt' value APP_CORE.Get_Min_Credit_Amt, 'maxAmt' value APP_CORE.Get_Max_Credit_Amt, "+
                                         "'minTerm' value t.min_term "+
                                         ") as value from CREDIT_PRODUCTS t  ")
                    .SetKeyField("sId")
                    .SetValueField("value")
                );


            EntityConfigDictionary.AddConfig("person_info",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.Person2Json (t.id) as value from person t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Person2Json")
                    .SetSequence("SEQ_PERSON")
                    .SetBaseTable("PERSON")
                );

            EntityConfigDictionary.AddConfig("lo_track_log",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, JSON_OBJECT('id' value id, " +
                                         "'idOrderSpecProd' value id_order_spec_prod, " +
                                         "'trackDate' value Track_Date, 'trackString' value Track_String) as value " +
                                         "from LO_ORDER_SPEC_TRACKING t")
                    .SetKeyField("id")
                    .SetValueField("value")
                );

            EntityConfigDictionary.AddConfig("lo_suppl_entity",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.*, JSON_OBJECT('id' value id, 'idSupplier' value id_Supplier, " +
                                         "'idLoEntity' value id_lo_entity) as value from LO_SUPPL_ENTITIES t")
                    .SetKeyField("id")
                    .SetValueField("value")
                );

            EntityConfigDictionary.AddConfig("store_place",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.Store_place2Json(t.id) as value from STORE_PLACES t ")
                    .AddSqlCommandWhere("where t.id in (select sp.id_store_place from product_store_places sp where sp.qty>0) " +
                                        "and t.type=1")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Store_place2Json")
                );

            EntityConfigDictionary.AddConfig("stores",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.Store2Json(t.id) as value from STORE_PLACES t ")
                    .AddSqlCommandWhere("where t.type=1  and t.lat is not null  and t.lng is not null  and t.is_active=1")
                    .AddSqlCommandOrderBy("order by t.address_line")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Store2Json")
                );

            EntityConfigDictionary.AddConfig("favorite_stores",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.FavoriteStore2Json(t.id) as value from FAVORITE_STORES t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.FavoriteStore2Json")
                    .SetSequence("FAVORITE_STORES_SEQ")
                    .SetBaseTable("FAVORITE_STORES")
                );

            EntityConfigDictionary.AddConfig("product_store_place",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, " +
                                         "Serialization.Product_Store_Place2Json(t.id) as value " +
                                         "from PRODUCT_STORE_PLACES t, store_places sp, cities c")
                    .AddSqlCommandWhere("where t.id_store_place = sp.id and sp.id_city = c.id")
                    .AddSqlCommandOrderBy("order by c.name asc, sp.name asc, sp.address_line asc")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Product_Store_Place2Json")
                );

            EntityConfigDictionary.AddConfig("client_order_all",
                new EntityConfig(mainDataConnString)
                    .SetBaseTable("CLIENT_ORDERS")
                    .AddSqlCommandSelect("select t.*, Serialization.CLIENT_ORDER2JSON(t.id) as value " +
                                         "from client_orders t")
                    .AddSqlCommandWhere("where t.id_status>0")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSequence("SEQ_CLIENT_ORDERS")
                    .SetSerializerFunc("Serialization.CLIENT_ORDER2JSON")
                );



            EntityConfigDictionary.AddConfig("client_order",
                new EntityConfig(mainDataConnString)
                    .SetBaseTable("CLIENT_ORDERS")
                    .AddSqlCommandSelect("select t.*, Serialization.CLIENT_ORDER2JSON(t.id) as value " +
                                         "from client_orders t")
                    .AddSqlCommandWhere("where t.id_status=0")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSequence("SEQ_CLIENT_ORDERS")
                    .SetSerializerFunc("Serialization.CLIENT_ORDER2JSON")
                );

            EntityConfigDictionary.AddConfig("client_order_product_all",
                new EntityConfig(mainDataConnString)
                    .SetBaseTable("ORDER_SPEC_PRODUCTS")
                    .AddSqlCommandSelect("select t.*, Serialization.ORDER_SPEC_PRODUCT2Json(t.id) as value from ORDER_SPEC_PRODUCTS t , client_orders o")
                    .AddSqlCommandWhere("where o.id = t.id_order")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSequence("SEQ_ORDER_SPEC_PRODUCTS")
                    .SetSerializerFunc("Serialization.ORDER_SPEC_PRODUCT2Json")
                );


            EntityConfigDictionary.AddConfig("client_order_hist",
                new EntityConfig(mainDataConnString)
                    .SetBaseTable("client_order_history")
                    .AddSqlCommandSelect("select t.*, Serialization.CLIENT_ORDERHIST2JSON(t.id) as value " +
                                            "from CLIENT_ORDER_HISTORY t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.CLIENT_ORDERHIST2JSON")
                );


            EntityConfigDictionary.AddConfig("client_order_product_hist",
                new EntityConfig(mainDataConnString)
                    .SetBaseTable("order_history_spec_products")
                    .AddSqlCommandSelect("select t.*, JSON_OBJECT('id' value t.id, 'idOrder' value id_order, " +
                                         "'idQuotationProduct' value id_quotation_product, 'price' value price, " +
                                         "'qty' value qty, 'idLoEntity' value id_lo_entity, " +
                                         "'loEstimatedDeliveryDate' value lo_estimated_delivery_date, " +
                                         "'loDeliveryCompletedDate' value lo_delivery_completed_date, " +
                                         "'earnedBonusCnt' value earned_bonus_cnt) as value from order_history_spec_products t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.ORDER_SPEC_HIST_PRODUCT2Json")
                );


            EntityConfigDictionary.AddConfig("client_order_product",
                new EntityConfig(mainDataConnString)
                    .SetBaseTable("ORDER_SPEC_PRODUCTS")
                    .AddSqlCommandSelect("select t.id, Serialization.ORDER_SPEC_PRODUCT2Json(t.id) as value from ORDER_SPEC_PRODUCTS t , client_orders o")
                    .AddSqlCommandWhere("where o.id = t.id_order and o.id_status=0")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSequence("SEQ_ORDER_SPEC_PRODUCTS")
                    .SetSerializerFunc("Serialization.ORDER_SPEC_PRODUCT2Json")
                );

            EntityConfigDictionary.AddConfig("client",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.Client2Json(t.id) as value from CLIENTS t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Client2Json")
                );

            EntityConfigDictionary.AddConfig("quotation",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, " +
                                         "Serialization.Quot2Json(t.id) as value from QUOTATIONS t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Quot2Json")
                );


            EntityConfigDictionary.AddConfig("loentity",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.loEntity2Json(t.id) as value from LO_ENTITIES t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.loEntity2Json")
                );


            EntityConfigDictionary.AddConfig("pmtmethod",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.pmtmethod2Json(t.id) as value from ENUM_PAYMENT_METHODS t")
                    .AddSqlCommandOrderBy("order by t.id")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.pmtmethod2Json")
                );

            EntityConfigDictionary.AddConfig("region",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.region2Json(id) as value from REGIONS t  ")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.region2Json")
                );

            EntityConfigDictionary.AddConfig("city",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.city2Json(t.id) as value from CITIES t ")
                    .AddSqlCommandWhere("where t.id_region is not null")
                    .AddSqlCommandOrderBy("order by t.name")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.city2Json")
                );

            EntityConfigDictionary.AddConfig("city_with_store",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.city2Json(t.id) as value from CITIES t ")
                    .AddSqlCommandWhere("where t.id in (select sp.id_city from product_store_places psp, " +
                                        "store_places sp where psp.id_store_place = sp.id and psp.qty > 0 and sp.type=1)")
                    .AddSqlCommandOrderBy("order by t.name")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.city2Json")
                );

            EntityConfigDictionary.AddConfig("country",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.country2Json(t.id) as value from COUNTRIES t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.country2Json")
                );


            EntityConfigDictionary.AddConfig("lang",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.Lang2Json(t.id) as value from LOCALES t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Lang2Json")
                );


            EntityConfigDictionary.AddConfig("measure_unit",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.MeasureUnit2Json(t.id) as value from measure_unit t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.MeasureUnit2Json")
                );

            EntityConfigDictionary.AddConfig("products", 
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, json_data as value from products t")
                    .AddSqlCommandWhere("where nvl(t.price,0)<>0 and json_data is not null") 

                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetPreserializedJSONField("json_data")
                    .SetSerializerFunc("Serialization.Product2Json")
                );

            EntityConfigDictionary.AddConfig("quotation_product", 
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization.QuotProduct2Json(t.id) as value from QUOTATIONS_PRODUCTS t ")
                    .AddSqlCommandWhere("where t.stock_qty>0")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.QuotProduct2Json")
                );

            EntityConfigDictionary.AddConfig("currencies", 
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization.Currency2Json(t.id) as value from currencies t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Currency2Json")
                );

            EntityConfigDictionary.AddConfig("suppliers", 
                  new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization.Supplier2Json(t.id) as value from suppliers t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Supplier2Json")
                );

            EntityConfigDictionary.AddConfig("manufacturers", 
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization.Manufacturer2Json(t.id) as value from manufacturers t ")
                    .AddSqlCommandWhere("where t.id in (select p.id_manufacturer from products p)")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Manufacturer2Json")
                );

            EntityConfigDictionary.AddConfig("product_groups",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization_Branch.ProductGroups2Json_2(t.id) as value from v_product_groups_tree t")
                    .AddSqlCommandWhere($"where t.id_product_cat ={virtualCatalogId} and t.is_active=1")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization_Branch.ProductGroups2Json_2")
            );

            EntityConfigDictionary.AddConfig("product_reviews",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization.ProductReviews2Json(t.id) as value from product_reviews t")
                    .AddSqlCommandOrderBy("order by t.review_date desc")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.ProductReviews2Json")
                    .SetSequence("SEQ_PRODUCT_REVIEWS")
                    .SetBaseTable("product_reviews")
            );

            EntityConfigDictionary.AddConfig("store_reviews",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization.StoreReviews2Json(t.id) as value from store_reviews t")
                    .AddSqlCommandOrderBy("order by t.review_date desc")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.StoreReviews2Json")
                    .SetSequence("SEQ_STORE_REVIEWS")
                    .SetBaseTable("store_reviews")
            );

            EntityConfigDictionary.AddConfig("novelty_details",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization.NoveltyDetails2Json(t.id) as value from novelty_details t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.NoveltyDetails2Json")
            );

            EntityConfigDictionary.AddConfig("device_data",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization.DeviceData2Json(t.id) as value from device_data t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.DeviceData2Json")
                    .SetSequence("SEQ_DEVICE_DATA")
                    .SetBaseTable("device_data")
            );

            EntityConfigDictionary.AddConfig("banner_slides",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization.BannerSlide2Json(t.id) as value from banner_slides t")
                    .AddSqlCommandWhere("where t.is_active=1")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.BannerSlide2Json")
            );

            EntityConfigDictionary.AddConfig("client_messages",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization.ClientMessage2Json(t.id) as value from client_messages_to_support t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.ClientMessage2Json")
                    .SetSequence("SEQ_CLIENT_MESSAGES")
                    .SetBaseTable("client_messages_to_support")
            );

            EntityConfigDictionary.AddConfig("global_localization",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization.GlobalLocalization2Json(t.id) as value from global_localization t")
                    .AddSqlCommandWhere($"WHERE t.id_lang={langId}")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.GlobalLocalization2Json")
                    .SetSequence("SEQ_GLOBAL_LOCALIZATION")
                    .SetBaseTable("global_localization")
            );

            EntityConfigDictionary.AddConfig("currency_rate",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization.Rates2Json(t.id) as value from currencies t")
                    .AddSqlCommandWhere($"WHERE t.id<>4")
                    .SetKeyField("targetId")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Rates2Json")
            );

            EntityConfigDictionary.AddConfig("product_review_votes",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization.ProductReviewVote2Json(t.id) as value from PRODUCT_REVIEW_VOTES t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.ProductReviewVote2Json")
                    .SetSequence("SEQ_PRODUCT_REVIEW_VOTES")
                    .SetBaseTable("PRODUCT_REVIEW_VOTES")
            );

            EntityConfigDictionary.AddConfig("store_review_votes",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization.StoreReviewVote2Json(t.id) as value from STORE_REVIEW_VOTES t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.StoreReviewVote2Json")
                    .SetSequence("SEQ_STORE_REVIEW_VOTES")
                    .SetBaseTable("STORE_REVIEW_VOTES")
            );

            EntityConfigDictionary.AddConfig("product_reviews_add_vote",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select pr.id,Serialization_Branch.ProductReviews2Json(pr.id,prv.vote) as value from product_reviews pr " +
                                         "left join product_review_votes prv on pr.id = prv.id_review " +
                                         "and prv.id_client = :idClient")

                .AddSqlCommandWhere("where pr.is_moderated=1")
                .AddSqlCommandOrderBy("order by pr.review_date desc")
                .SetKeyField("id")
                .SetValueField("value")
                );

            EntityConfigDictionary.AddConfig("store_reviews_add_vote",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select sr.id,Serialization_Branch.StoreReviews2Json(sr.id,srv.vote) as value from store_reviews sr " +
                                         "left join store_review_votes srv on sr.id = srv.id_review " +
                                         "and srv.id_client = :idClient")

                .AddSqlCommandWhere("where sr.is_moderated=1")
                .AddSqlCommandOrderBy("order by sr.review_date desc")
                .SetKeyField("id")
                .SetValueField("value")
                );

            EntityConfigDictionary.AddConfig("news",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.News2Json(id) as value from news t")
                    .AddSqlCommandWhere("where t.is_active=1")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.News2Json")
            );

            EntityConfigDictionary.AddConfig("news_category",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, Serialization.NewsCategory2Json(id) as value from news_category t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.NewsCategory2Json")
            );

            #endregion

            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));

            services.AddIdentity<AppUser, IdentityRole>(opt =>
                {
                    opt.Password.RequiredLength = 5;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequireDigit = false;

                })
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            // limit the number of login attempts
            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 10;
            });


            services.AddScoped<IAppParamsRepository, FSAppParamsRepository>();
            services.AddScoped<ICreditRepository, FSCreditRepository>();
            services.AddScoped<IStorePlaceRepository, FSStorePlaceRepository>();
            services.AddScoped<ICartRepository, FSCartRepository>();
            services.AddScoped<IClientRepository, FSClientRepository>();
            services.AddScoped<IQuotationRepository, FSQuotationRepository>();
            services.AddScoped<ILORepository, FSLORepository>();
            services.AddScoped<IFinRepository, FSFinRepository>();
            services.AddScoped<IGeoRepository, FSGeoRepository>();
            services.AddScoped<ILocalizationRepository, FSLocalizationRepository>();
            services.AddScoped<IMeasureUnitRepository, FSMeasureUnitRepository>();
            services.AddScoped<IProductRepository, FSProductRepository>();
            services.AddScoped<IQuotationProductRepository, FSQuotationProductRepository>();
            services.AddScoped<ICurrencyRepository, FSCurrencyRepository>();
            services.AddScoped<ISupplierRepository, FSSupplierRepository>();
            services.AddScoped<IManufacturerRepository, FSManufacturerRepository>();
            services.AddScoped<IProductGroupRepository, FSProductGroupRepository>();
            services.AddScoped<IUserRepository, FSUserRepository>();
            services.AddScoped<IRoleRepository, FSRoleRepository>();
            services.AddScoped<AccountUserFacade>();
            services.AddScoped<ICrypto, FSCryptoProvider>();
            services.AddScoped<IAuthSender,FSAuthSender>();
            services.AddScoped<ISmsService, FSSmsService>();
            services.AddScoped<INoveltyRepository, FSNoveltyRepository>();
            services.AddScoped<IPollRepository, FSPollRepository>();
            services.AddScoped<IPageRepository, FSPageRepository>();
            services.AddScoped<IActionRepository, FSActionRepository>();
            services.AddScoped<IDeviceDataRepository, FSDeviceDataRepository>();
            services.AddScoped<IReviewRepository, FSReviewRepository>();
            services.AddScoped<IBannerSlideRepository, FSBannerSlideRepository>();
            services.AddScoped<IClientMessageRepository, FSClientMessageRepository>();
            services.AddScoped<IAppLocalizationRepository, FSAppLocalizationRepository>();
            services.AddScoped<ILegalPolicyRepository, FSLegalPolicyRepository>();
            services.AddScoped<INewsRepository, FSNewsRepository>();
            services.AddScoped<INewsCategoryRepository, FSNewsCategoryRepository>();
            services.AddScoped<ICreditCardRepository, FSCreditCardRepository>();
            services.AddScoped<ISaleRmmRepository, FSSaleRmmRepository>();
            //services.AddScoped<IDictionaryRepository, FSDictionaryRepository>();
            //services.AddScoped<IOrdersRepository, FSOrdersRepository>();
            //services.AddScoped<IUserRepository, FSUserRepository>();
            //services.AddScoped<IRoleRepository, FSRoleRepository>();
            //services.AddScoped<IBrandRepository, FSBrandRepository>();

            services.Add(new ServiceDescriptor(typeof(ICacheService<AppParam_DTO>),
                    p => new CacheService<AppParam_DTO>
                    ("app_params", 100000, redisCache, true), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Client_DTO>),
                    p => new CacheService<Client_DTO>
                    ("client", 1000000, redisCache, false), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Quotation_DTO>),
                    p => new CacheService<Quotation_DTO>
                    ("quotation", 1000000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<LoEntity_DTO>),
                    p => new CacheService<LoEntity_DTO>
                    ("loentity", 100000000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Enum_Pmt_Method_DTO>),
                    p => new CacheService<Enum_Pmt_Method_DTO>
                    ("pmtmethod", 50000000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Country_DTO>),
                    p => new CacheService<Country_DTO>
                    ("country", 51000000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<City_DTO>),
                    p => new CacheService<City_DTO>
                    ("city", 5000000, redisCache, true), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Region_DTO>),
                    p => new CacheService<Region_DTO>
                    ("region", 500000000, redisCache, true), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Lang_DTO>),
                    p => new CacheService<Lang_DTO>
                    ("lang", 50000000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Measure_Unit_DTO>),
                    p => new CacheService<Measure_Unit_DTO>
                    ("measure_unit", 3200000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Product_DTO>),
                    p => new CacheService<Product_DTO>
                    ("products", 3600000, redisCache, false), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Currency_DTO>),
                    p => new CacheService<Currency_DTO>
                    ("currencies", 2000000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Supplier_DTO>),
                    p => new CacheService<Supplier_DTO>
                    ("suppliers", 100000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Manufacturer_DTO>),
                    p => new CacheService<Manufacturer_DTO>
                    ("manufacturers", 600000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Quotation_Product_DTO>),
                    p => new CacheService<Quotation_Product_DTO>
                    ("quotation_product", 620000, redisCache, false), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Product_Groups_DTO>),
                p => new CacheService<Product_Groups_DTO>
                    ("product_groups", 620000, redisCache, true), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<StorePlace_DTO>),
                p => new CacheService<StorePlace_DTO>
                    ("store_place", 1000000, redisCache, true), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Store_DTO>),
                p => new CacheService<Store_DTO>
                    ("stores", 10000000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<LoSupplEntity_DTO>),
                p => new CacheService<LoSupplEntity_DTO>
                    ("lo_suppl_entity", 7200000, redisCache, true), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<CreditProduct_DTO>),
                p => new CacheService<CreditProduct_DTO>
                    ("credit_product", 7200000, redisCache, true), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<ProductReview_DTO>),
                p => new CacheService<ProductReview_DTO>
                    ("product_reviews", 10000000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<StoreReview_DTO>),
                p => new CacheService<StoreReview_DTO>
                    ("store_reviews", 10000000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<NoveltyDetails_DTO>),
                p => new CacheService<NoveltyDetails_DTO>
                    ("novelty_details", 1000000, redisCache, true), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Page_DTO>),
                p => new CacheService<Page_DTO>
                    ("pages", 10000000, redisCache, true), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Action_DTO>),
                p => new CacheService<Action_DTO>
                    ("actions", 10000000, redisCache, true), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<BannerSlide_DTO>),
                p => new CacheService<BannerSlide_DTO>
                    ("banner_slides", 600000, redisCache, true), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Localization_DTO>),
                p => new CacheService<Localization_DTO>
                    ("global_localization", 600000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<CurrencyRate_DTO>),
                p => new CacheService<CurrencyRate_DTO>
                    ("currency_rate", 10000000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<LoEntityOffice_DTO>),
                p => new CacheService<LoEntityOffice_DTO>
                    ("lo_entity_office", 12000000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<LoDeliveryType_DTO>),
                p => new CacheService<LoDeliveryType_DTO>
                    ("lo_delivery_types", 12000000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<LoEntityDeliveryType_DTO>),
                p => new CacheService<LoEntityDeliveryType_DTO>
                    ("lo_entity_delivery_type", 12000000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<News_DTO>),
                p => new CacheService<News_DTO>
                   ("news", 12000000, redisCache, true), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<NewsCategory_DTO>),
                p => new CacheService<NewsCategory_DTO>
                   ("news_category", 12000000, redisCache, true), ServiceLifetime.Singleton));

            // add roles
            IdentityInit(services.BuildServiceProvider()).Wait();
        }

        public void Configure(IApplicationBuilder app,
                              IHostingEnvironment env,
                              ILoggerFactory loggerFactory)
        {
            //add Serilog configuration
            loggerFactory.AddSerilog();

            app.UseExceptionHandler();
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            loggerFactory.AddConsole();
            app.UseStatusCodePages();

            app.UseCors("AnyOrigin");
            app.UseJwtBearerAuthentication(AuthOpt.InitToken(Configuration.GetSection("AuthToken")));
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/v{version:apiVersion}/{controller=Home}/{action=Index}/{id?}");
            });

        }

        //[Obsolete]
        public async Task IdentityInit(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var defaultRoles = new[] { "admin", "user","retail" };

            foreach (var role in defaultRoles)
            {
                if (await roleManager.FindByNameAsync(role) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

        }

        // switch redis config by pre-processor directive
        public ConfigurationOptions GetRedisConfigBySolutionConfiguration()
        {
            ConfigurationOptions redisConfig = null;

            // working pre-processor
            #if DEBUG
             redisConfig = new ConfigurationOptions() {EndPoints = {{"localhost", 6379}}};

            #else
            var endPointCollection = new EndPointCollection();

            var configSection = Configuration.GetSection("RedisConfig").GetChildren();
            foreach (var nodeSection in configSection)
            {
                endPointCollection.Add(nodeSection["host"], Convert.ToInt32(nodeSection["port"]));
            }
            
            redisConfig = new ConfigurationOptions
            {
                EndPoints = // EndPoint - don't have a setter method. Working with initialization block
                {
                    endPointCollection[0],
                    endPointCollection[1],
                    endPointCollection[2],
                    endPointCollection[3],
                    endPointCollection[4],
                    endPointCollection[5]

                }
            };
            #endif

            redisConfig.SyncTimeout = int.MaxValue;

            return redisConfig;
        }
    }
}