using System;
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
using Wsds.DAL.ORM;
using Wsds.DAL.Repository.Abstract;
using Wsds.DAL.Repository.Specific;
using Wsds.WebApp.Auth;
using Microsoft.AspNetCore.HttpOverrides;
using CachingFramework.Redis;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Infrastructure.Facade;
using Wsds.DAL.Repository;
using StackExchange.Redis;
using Wsds.DAL.Services.Abstract;
using Wsds.DAL.Services.Specific;
using Wsds.WebApp.Auth.Protection;

namespace Wsds.WebApp
{
    public class Startup
    {
        private IHostingEnvironment _environment;
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            _environment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        

        public void ConfigureServices(IServiceCollection services)
        {
            // fox ssl enable
            services.AddMvc(options =>
            {
                //options.Filters.Add(new RequireHttpsAttribute());
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
                        .AllowAnyMethod();
                });
            });

            services.AddDataProtection();

            var mainDataConnString = Configuration.GetConnectionString("MainDataConnection");
            services.AddSingleton<IConfiguration>(Configuration);

            var virtualCatalogId = Convert.ToInt64(Configuration["AppOptions:virtualId"]);

            var redisConfig = new ConfigurationOptions
            {
                EndPoints =
                            {
                                { "localhost", 6379 }
                            },

                SyncTimeout = int.MaxValue //,
                //Ssl = true,
                //Password = "mypassword"
            };

            var redisCache = new Context(redisConfig);
            services.AddSingleton(redisCache);

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
                    .AddSqlCommandSelect("select id, Json_object('id' value id, 'name' value name," +
                                         "'dateStart' value date_start, 'dateEnd' value date_end," +
                                         "'img_url' value img_url,'priority' value priority," +
                                         "'isActive' value is_active, 'sketch_content' value sketch_content," +
                                         "'action_content' value action_content) as value from actions")
                    .AddSqlCommandWhere("where is_active = 1")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Action2Json")
            );

            EntityConfigDictionary.AddConfig("pages",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select id, Json_object('id' value id, 'name' value name,'content' value content) as value " +
                                         "from pages")
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
                    .AddSqlCommandSelect("select t.id, select JSON_OBJECT ('id' value id, 'firstName' value first_name," +
                                         "'lastName' value last_name, 'middleName' value middle_name, " +
                                         "'passportSeries' value passport_Series, 'passportNum' value passport_Num, " +
                                         "'issuedAuthority' value issued_Authority, 'taxNumber' value tax_number, 'birthDate' value birth_date) as value from person t")
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
                    .AddSqlCommandSelect("select t.id, JSON_OBJECT('id' value id, 'idSupplier' value id_supplier," +
                                         "'name' value name, 'idCity' value id_city, 'zip' value zip, " +
                                         "'address_line' value address_line, 'lat' value lat,'lng' value lng," +
                                         "'type' value type) as value from STORE_PLACES t ")
                    .AddSqlCommandWhere("where t.id in (select sp.id_store_place from product_store_places sp where sp.qty>0) " +
                                        "and t.type=1")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Store_place2Json")
                );

            EntityConfigDictionary.AddConfig("stores",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, JSON_OBJECT('id' value id, 'idCity' value id_city, 'address' value address_line, 'lat' value lat,'lng' value lng," +
                                         "'openTime' value open_time, 'closeTime' value close_time, 'rating' value rating, 'idFeedbacks' value id_feedbacks) as value from STORE_PLACES t ")
                    .AddSqlCommandWhere("where t.type=1  and t.lat is not null  and t.lng is not null  and t.is_active=1 and t.open_time is not null and t.close_time is not null")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Store2Json")
                );

            EntityConfigDictionary.AddConfig("favorite_stores",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.id, JSON_OBJECT('id' value id, 'idClient' value id_client, 'idStore' value id_store_places) as value from FAVORITE_STORES t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.FavoriteStore2Json")
                    .SetSequence("FAVORITE_STORES_SEQ")
                    .SetBaseTable("FAVORITE_STORES")
                );

            EntityConfigDictionary.AddConfig("product_store_place",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.*, " +
                                         "JSON_OBJECT ('id' value t.id, 'idStorePlace' value id_Store_Place, " +
                                         "'idQuotationProduct' value id_Quotation_Product, 'qty' value qty) as value " +
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
                    .AddSqlCommandSelect("select t.*, JSON_OBJECT('id' value t.id, 'idOrder' value id_order, " +
                                         "'idQuotationProduct' value id_quotation, 'price' value price, 'qty' value qty, " +
                                         "'idStorePlace' value id_store_place, 'idLoEntity' value id_lo_entity, " +
                                         "'loTrackTicket' value lo_track_ticket, 'loDeliveryCost' value lo_delivery_cost, " +
                                         "'loDeliveryCompleted' value lo_delivery_completed, " +
                                         "'loEstimatedDeliveryDate' value lo_estimated_delivery_date, " +
                                         "'loDeliveryCompletedDate' value lo_delivery_completed_date, 'errorMessage' value error_message, " +
                                         "'warningMessage' value warning_message, 'payPromoCode' value pay_promocode, " +
                                         "'payPromoCodeDiscount' value pay_promocode_discount, 'payBonusCnt' value pay_bonus_cnt, " +
                                         "'payPromoBonusCnt' value pay_promobonus_cnt, 'earnedBonusCnt' value earned_bonus_cnt, " +
                                         "'warningRead' value warning_read) as value from ORDER_SPEC_PRODUCTS t , client_orders o")
                    .AddSqlCommandWhere("where o.id = t.id_order")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSequence("SEQ_ORDER_SPEC_PRODUCTS")
                    .SetSerializerFunc("Serialization.ORDER_SPEC_PRODUCT2Json")
                );



            EntityConfigDictionary.AddConfig("client_order_product",
                new EntityConfig(mainDataConnString)
                    .SetBaseTable("ORDER_SPEC_PRODUCTS")
                    .AddSqlCommandSelect("select t.*, JSON_OBJECT('id' value t.id, 'idOrder' value id_order, " +
                                         "'idQuotationProduct' value id_quotation, 'price' value price, 'qty' value qty, " +
                                         "'idStorePlace' value id_store_place, 'idLoEntity' value id_lo_entity, " +
                                         "'loTrackTicket' value lo_track_ticket, 'loDeliveryCost' value lo_delivery_cost, " +
                                         "'loDeliveryCompleted' value lo_delivery_completed, " +
                                         "'loEstimatedDeliveryDate' value lo_estimated_delivery_date, " +
                                         "'loDeliveryCompletedDate' value lo_delivery_completed_date, 'errorMessage' value error_message, " +
                                         "'warningMessage' value warning_message, 'payPromoCode' value pay_promocode, " +
                                         "'payPromoCodeDiscount' value pay_promocode_discount, 'payBonusCnt' value pay_bonus_cnt, " +
                                         "'payPromoBonusCnt' value pay_promobonus_cnt, 'earnedBonusCnt' value earned_bonus_cnt, " +
                                         "'warningRead' value warning_read) as value from ORDER_SPEC_PRODUCTS t , client_orders o")
                    .AddSqlCommandWhere("where o.id = t.id_order and o.id_status=0")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSequence("SEQ_ORDER_SPEC_PRODUCTS")
                    .SetSerializerFunc("Serialization.ORDER_SPEC_PRODUCT2Json")
                );

            EntityConfigDictionary.AddConfig("client",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.*, JSON_OBJECT('id' value id,'userId' value user_id, " +
                                         "'name' value name, 'phone' value phone, 'login' value login, 'email' value email, " +
                                         "'fname' value fname, 'lname' value lname, 'barcode' value barcode,  'bonusBalance' " +
                                         "value null, 'actionBonusBalance' value null, 'id_currency' value id_currency,'id_lang' value id_lang, " +
                                         "'app_key' value app_key) as value from CLIENTS t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Client2Json")
                );

            EntityConfigDictionary.AddConfig("quotation",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.*, " +
                                         "JSON_OBJECT('id' value id, 'idSupplier' value id_Supplier, 'dateStart' value date_start, " +
                                         "'dateEnd' value date_end, 'currencyId' value currency_id) as value from QUOTATIONS t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Quot2Json")
                );


            EntityConfigDictionary.AddConfig("loentity",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.*, JSON_OBJECT('id' value id, 'name' value name) as value from LO_ENTITIES t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.loEntity2Json")
                );


            EntityConfigDictionary.AddConfig("pmtmethod",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.*, JSON_OBJECT('id' value id, 'name' value name) as value from ENUM_PAYMENT_METHODS t")
                    .AddSqlCommandOrderBy("order by t.id")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.pmtmethod2Json")
                );

            EntityConfigDictionary.AddConfig("region",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.*, JSON_OBJECT('id' value id, 'name' value name ) as value from REGIONS t  ")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.region2Json")
                );

            EntityConfigDictionary.AddConfig("city",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.*, JSON_OBJECT('id' value id, 'name' value name, " +
                                         "'idRegion' value id_region) as value from CITIES t ")
                    .AddSqlCommandWhere("where /*t.id in (select sp.id_city from product_store_places psp, " +
                                        "store_places sp where psp.id_store_place = sp.id and psp.qty > 0)*/ t.id_region is not null")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.city2Json")
                );

            EntityConfigDictionary.AddConfig("city_with_store",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.*, JSON_OBJECT('id' value id, 'name' value name, " +
                                         "'idRegion' value id_region) as value from CITIES t ")
                    .AddSqlCommandWhere("where t.id in (select sp.id_city from product_store_places psp, " +
                                        "store_places sp where psp.id_store_place = sp.id and psp.qty > 0 and sp.type=1)")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.city2Json")
                );

            EntityConfigDictionary.AddConfig("country",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.*, JSON_OBJECT('id' value id, 'name' value name) as value from COUNTRIES t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.country2Json")
                );


            EntityConfigDictionary.AddConfig("lang",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.*, JSON_OBJECT('id' value id, 'name' value name) as value from LOCALES t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.Lang2Json")
                );


            EntityConfigDictionary.AddConfig("measure_unit",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.*, JSON_OBJECT('id' value id, 'name' value name) as value from measure_unit t")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.MeasureUnit2Json")
                );

            EntityConfigDictionary.AddConfig("products", 
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, json_data as value from products t") 
                    .AddSqlCommandWhere("where /* t.id in (6293680, 6280637, 6293680, 6294898, 6325585, 6324182, 6252121, 6202929, 6324216, " +
                                        "6324213, 6161537, 6307814,6343804, 6337167, 6291460, 6316576, 6310491, " +
                                        "6312913, 6363302, 6337781, 5857818, 6309865, 5936214 ) and */ t.price<>0 " +
                                        " and t.id_manufacturer in (220, 107996, 196, 253, 114733, 47, 109483, 114183, 158, 199, 216)") 

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
                    .AddSqlCommandSelect("SELECT t.id, Serialization_Branch.ProductGroups2Json(t.id) as value from product_groups t")
                    .AddSqlCommandWhere($"where t.id_product_cat ={virtualCatalogId}")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization_Branch.ProductGroups2Json")
            );

            EntityConfigDictionary.AddConfig("product_reviews",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization.ProductReviews2Json(t.id) as value from product_reviews t")
                    .AddSqlCommandOrderBy("order by t.review_date")
                    .SetKeyField("id")
                    .SetValueField("value")
                    .SetSerializerFunc("Serialization.ProductReviews2Json")
                    .SetSequence("SEQ_PRODUCT_REVIEWS")
                    .SetBaseTable("product_reviews")
            );

            EntityConfigDictionary.AddConfig("store_reviews",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("SELECT t.id, Serialization.StoreReviews2Json(t.id) as value from store_reviews t")
                    .AddSqlCommandOrderBy("order by t.review_date")
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

            services.AddScoped<FoxStoreDBContext>(_ =>
                new FoxStoreDBContext(mainDataConnString));

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
            //services.AddScoped<IDictionaryRepository, FSDictionaryRepository>();
            //services.AddScoped<IOrdersRepository, FSOrdersRepository>();
            //services.AddScoped<IUserRepository, FSUserRepository>();
            //services.AddScoped<IRoleRepository, FSRoleRepository>();
            //services.AddScoped<IBrandRepository, FSBrandRepository>();

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
                    ("products", 3000000, redisCache, false), ServiceLifetime.Singleton));

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
                    ("pages", 100000000, redisCache, true), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Action_DTO>),
                p => new CacheService<Action_DTO>
                    ("actions", 100000000, redisCache, true), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<BannerSlide_DTO>),
                p => new CacheService<BannerSlide_DTO>
                    ("banner_slides", 600000, redisCache, true), ServiceLifetime.Singleton));

            // add roles
            IdentityInit(services.BuildServiceProvider()).Wait();
        }

        public void Configure(IApplicationBuilder app,
                              IHostingEnvironment env,
                              ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseStatusCodePages();

            app.UseCors("AnyOrigin");
            app.UseJwtBearerAuthentication(AuthOpt.InitToken(Configuration.GetSection("AuthToken")));
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

          
            // this is obsolete call 
            //TODO: check this method when we will be create admin panel
           // IdentityInit(app.ApplicationServices).Wait();

            // create global dependency collection ICacheService
            /*
            var services = app.ApplicationServices.GetServices<ICacheService>();
            AppDepResolver.InitCollection(services);
             */ 

            //IdentityInit(app.ApplicationServices).Wait();
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

    }
}
