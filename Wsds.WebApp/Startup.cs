using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Wsds.DAL.Providers;
using Wsds.DAL.Identity;
using Wsds.DAL.Entities;
using Wsds.DAL.ORM;
using Wsds.DAL.Repository.Abstract;
using Wsds.DAL.Repository.Specific;
using Wsds.WebApp.Auth;
using Microsoft.AspNetCore.HttpOverrides;

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
                options.Filters.Add(new RequireHttpsAttribute());
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
            var virtualCatalogId = Convert.ToInt64(Configuration["AppOptions:virtualId"]);

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

            services.AddScoped<IDictionaryRepository, FSDictionaryRepository>();
            services.AddScoped<IOrdersRepository, FSOrdersRepository>();
            services.AddScoped<IUserRepository, FSUserRepository>();
            services.AddScoped<IRoleRepository, FSRoleRepository>();
            services.AddScoped<IBrandRepository, FSBrandRepository>();

            services.Add(new ServiceDescriptor(typeof(ICacheService<Product>),
                p => new CacheService<Product>
                (mainDataConnString, 60000, (pr) => pr.PRODUCTS_IN_GROUPS.Any(pg => pg.ProductGroup.ID_PRODUCT_CAT == virtualCatalogId)
                    , (prg) => prg.PRODUCTS_IN_GROUPS
                    , (m) => m.MANUFACTURER
                    , (prVal) => prVal.PRODUCT_PROP_VALUES),
                ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Product_Group>),
                p => new CacheService<Product_Group>(mainDataConnString, 600000, // 10 min
                    (f) => f.ID_PRODUCT_CAT == virtualCatalogId,
                    (pg) => pg.PRODUCTS_IN_GROUPS),
                ServiceLifetime.Singleton));

            services.Add(new ServiceDescriptor(typeof(ICacheService<Product_Template>),
                p => new CacheService<Product_Template>(mainDataConnString, 600000,
                    (pt) => pt.ProductGroup.ID_PRODUCT_CAT == virtualCatalogId,
                    (pt) => pt.PROP,
                    (pt) => pt.PROP.PROP_ENUMS_LISTS,
                    (pt) => pt.PROP.PRODUCT_PROP_VALUES),
                ServiceLifetime.Singleton));
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
