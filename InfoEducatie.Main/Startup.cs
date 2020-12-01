using System;
using InfoEducatie.Contest;
using InfoEducatie.Main.Data;
using MCMS.Base.Helpers;
using MCMS.Builder;
using MCMS.Common;
using MCMS.Emailing;
using MCMS.Files;
using MCMS.SwaggerFormly.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfoEducatie.Main
{
    public class Startup
    {
        private readonly MApp _mApp;

        public Startup(IWebHostEnvironment currentEnvironment)
        {
            _mApp = new MAppBuilder(currentEnvironment)
                .AddSpecifications<MCommonSpecifications>()
                .AddSpecifications<MEmailingSpecifications>()
                .AddSpecifications<MFilesSpecifications>()
                .AddSpecifications<InfoEducatieSpecifications>()
                .AddSpecifications<InfoEducatieContestSpecifications>()
                .WithPostgres<ApplicationDbContext>()
                .WithSwagger(new SwaggerConfigOptions
                    {
                        Title = "Admin API",
                        Version = "v1"
                    }
                    // new SwaggerConfigOptions
                    // {
                    //     Title = "API",
                    //     Version = "v1"
                    // }
                )
                .Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _mApp.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, ILogger<Startup> logger)
        {
            _mApp.Configure(app, serviceProvider);
            var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
            if (!string.IsNullOrEmpty(Env.Get("ASPNETCORE_URLS")) && !env.IsProduction())
            {
                logger.LogInformation("Listening on " + Env.Get("ASPNETCORE_URLS") + "\nOpen your browser at " +
                                      Env.Get("ASPNETCORE_URLS").Replace("0.0.0.0", "localhost"));
                logger.LogInformation("Swagger url " + Env.Get("ASPNETCORE_URLS").Replace("0.0.0.0", "localhost") +
                                      "/api/docs");
            }
        }
    }
}