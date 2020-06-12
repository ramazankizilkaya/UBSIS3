using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using UBSIS3.Web.Data.Context;
using UBSIS3.Web.Data.Interfaces;
using UBSIS3.Web.Data.Mapper;
using UBSIS3.Web.Data.Repositories;

namespace UBSIS3.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>
                (options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddHttpClient();

            services.AddLocalization(opts => opts.ResourcesPath = "Resources");
            services.AddMvc()
                .AddViewLocalization(x => x.ResourcesPath = "Resources")
                .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(); ;

            services.Configure<RequestLocalizationOptions>(opts =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("tr"),
                    new CultureInfo("ru"),
                };

                opts.DefaultRequestCulture = new RequestCulture("en");
                opts.SupportedCultures = supportedCultures;
                opts.SupportedUICultures = supportedCultures;
            });

            services.AddScoped<IContactUsRepository, ContactUsRepository>();
            services.AddScoped<ICareerRepository, CareerRepository>();
            services.AddScoped<IVacancyRepository, VacancyRepository>();
            services.AddScoped<IErrorLogRepository, ErrorLogRepository>();
            services.AddScoped<IEmailNewsletterRepository, EmailNewsletterRepository>();
            services.AddScoped<ICovRepository, CovRepository>();
            
            services.AddTransient<ApplicationContext>();
            services.AddAutoMapper(typeof(UbsisMapper));
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true; // make the session cookie Essential
                options.IdleTimeout = TimeSpan.FromMinutes(15);
                //options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = "LoggedUserId";
            });
        }

         // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Exception");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            
        }
    }
}
