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


            var mainDataConnString = Configuration.GetConnectionString("MainDataConnection");
            services.AddSingleton<IConfiguration>(Configuration);

            var virtualCatalogId = Convert.ToInt64(Configuration["AppOptions:virtualId"]);
            var redisCache = new Context();
            services.AddSingleton(redisCache);

            EntityConfigDictionary.AddConfig("city",
                new EntityConfig(mainDataConnString)
                    .AddSqlCommandSelect("select t.*, JSON_OBJECT('id' value id, 'name' value name, 'id_region' value id_region) as value from CITIES t")
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
                    .AddSqlCommandWhere("where t.id in (6293680, 6280637, 6293680, 6294898, 6325585, 6324182, 6252121, 6202929, 6324216, " +
                                        "6324213, 6161537, 6307814,6343804, 6337167, 6291460, 6316576, 6310491, " +
                                        "6312913, 6363302, 6337781, 5857818, 6309865, 5936214 )")
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

            services.AddScoped<IGeoRepository, FSGeoRepository>();
            services.AddScoped<ILocalizationRepository, FSLocalizationRepository>();
            services.AddScoped<IMeasureUnitRepository, FSMeasureUnitRepository>();
            services.AddScoped<IProductRepository, FSProductRepository>();
            services.AddScoped<IQuotationProductRepository, FSQuotationProductRepository>();
            services.AddScoped<ICurrencyRepository, FSCurrencyRepository>();
            services.AddScoped<ISupplierRepository, FSSupplierRepository>();
            services.AddScoped<IManufacturerRepository, FSManufacturerRepository>();
            services.AddScoped<IProductGroupRepository, FSProductGroupRepository>();

            //services.AddScoped<IDictionaryRepository, FSDictionaryRepository>();
            //services.AddScoped<IOrdersRepository, FSOrdersRepository>();
            //services.AddScoped<IUserRepository, FSUserRepository>();
            //services.AddScoped<IRoleRepository, FSRoleRepository>();
            //services.AddScoped<IBrandRepository, FSBrandRepository>();

            services.Add(new ServiceDescriptor(typeof(ICacheService<Country_DTO>),
                    p => new CacheService<Country_DTO>
                    ("country", 51000000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<City_DTO>),
                    p => new CacheService<City_DTO>
                    ("city", 5000000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Lang_DTO>),
                    p => new CacheService<Lang_DTO>
                    ("lang", 50000000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Measure_Unit_DTO>),
                    p => new CacheService<Measure_Unit_DTO>
                    ("measure_unit", 320000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Product_DTO>),
                    p => new CacheService<Product_DTO>
                    ("products", 300000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Currency_DTO>),
                    p => new CacheService<Currency_DTO>
                    ("currencies", 200000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Supplier_DTO>),
                    p => new CacheService<Supplier_DTO>
                    ("suppliers", 100000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Manufacturer_DTO>),
                    p => new CacheService<Manufacturer_DTO>
                    ("manufacturers", 600000, redisCache), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Quotation_Product_DTO>),
                    p => new CacheService<Quotation_Product_DTO>
                    ("quotation_product", 620000, redisCache, true), ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Product_Groups_DTO>),
                p => new CacheService<Product_Groups_DTO>
                    ("product_groups", 620000, redisCache, true), ServiceLifetime.Singleton));
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

            // create global dependency collection ICacheService
            /*
            var services = app.ApplicationServices.GetServices<ICacheService>();
            AppDepResolver.InitCollection(services);
             */ 

            IdentityInit(app.ApplicationServices).Wait();
        }

        public async Task IdentityInit(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var defaultRoles = new[] { "admin", "user" };

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
