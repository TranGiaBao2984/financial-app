﻿namespace App {
    using System;
    using System.Diagnostics;
    using System.Linq;
    using App.Models.Domain;
    using App.Models.Domain.Identity;
    using App.Support.Integration;
    using AutoMapper;
    using Microsoft.AspNet.Authentication.Cookies;
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.FileProviders;
    using Microsoft.AspNet.Hosting;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Mvc.Formatters;
    using Microsoft.Data.Entity;
    using Microsoft.Data.Entity.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.PlatformAbstractions;
    using Models.Domain.Repositories;
    using Models.Domain.Services;
    using Newtonsoft.Json.Serialization;
    using Support;

    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment applicationEnvironment)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddUserSecrets()
                .AddApplicationInsightsSettings(developerMode:env.IsDevelopment())
                .AddEnvironmentVariables();

            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddApplicationInsightsTelemetry(this.Configuration);
            services.AddMvc(options => {
                JsonOutputFormatter jsonFormatter = options.OutputFormatters.OfType<JsonOutputFormatter>().First();
                jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AppDbContext, int>()
                .AddDefaultTokenProviders()
                .AddUserValidator<AppUserValidator>()
                .AddPasswordValidator<AppPasswordValidator>()
                .AddUserManager<AppUserManager>()
                .AddUserStore<AppUserStore>();

            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<AppDbContext>(options => options.UseSqlServer(this.Configuration["Data:AppDbConnection:ConnectionString"]));

            // DI
            services.AddScoped<AppDbContext>();
            services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());
            services.AddScoped<AppUserManager>();
            services.AddScoped<AppUserStore>();

            services.AddTransient<AppOwnerRepository>();
            RepositoryRegistry.InsertIn(services);

            services.AddScoped<SheetRetrievalService>();
            services.AddScoped<EntityOwnerService>();
            services.AddScoped<SheetOffsetCalculationService>();
            services.AddScoped<SheetStatisticsService>();
            services.AddScoped<AutoMapperEngineFactory.SheetOffsetCalculationResolver>();
            services.AddScoped<AutoMapperEngineFactory.EntityResolver<Category>>();
            
            services.AddSingleton<IMappingEngine>(AutoMapperEngineFactory.Create);
            services.AddSingleton<IETagGenerator, ETagGenerator>();
            services.AddSingleton<IStaticFileUrlGenerator, StaticFileUrlGenerator>();
            services.AddSingleton<IAppVersionService, AppVersionService>();

            services.AddTransient<IBrowserDetector, DefaultBrowserDetector>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
            if (env.IsDevelopment()) {
                loggerFactory.AddDebug(LogLevel.Debug);
            } else {
                loggerFactory.AddTraceSource(new SourceSwitch("Financial-App"), new DefaultTraceListener());
            }

            app.UseApplicationInsightsRequestTelemetry();
            app.MapApplicationCacheManifest();
            app.MapAngularViewPath(env);

            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                LoginPath = new PathString("/Account/Login"),
                ExpireTimeSpan = TimeSpan.FromDays(30),
                SlidingExpiration = true,

                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                AuthenticationScheme = IdentityCookieOptions.ApplicationCookieAuthenticationType
            });

            if (env.IsDevelopment()) {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseRuntimeInfoPage();
            } else {
                app.UseExceptionHandler("/");
                app.UseApplicationInsightsExceptionTelemetry();
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseStaticFiles();

            app.UseMvc(routes => {
                // We only match one controller since we will want
                // all requests to go to the controller which renders
                // the initial view.

                routes.MapRoute(
                    name: "default",
                    template: "{*.}",
                    defaults: new {
                        controller = "Home",
                        action = "Index"
                    });
            });

            app.UseApplicationInsightsExceptionTelemetry();

        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
