using Application;
using Application.Impl;
using Domain.Repositories;
using Infrastructure.SQLServer;
using Infrastructure.GraphApi;
using AplicaAclFunction;
using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace AplicaAclFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            ConfigurarAplicaAcl(builder);
        }

        public static void ConfigurarAplicaAcl(IFunctionsHostBuilder builder)
        {         
            builder.Services.AddSingleton<IAplicaAclService, AplicaAclService>();           
            builder.Services.AddSingleton<AplicaAclSQLRepository>();
            builder.Services.AddSingleton<AplicaAclGraphApiRepository>();
            
            
            builder.Services.AddTransient<IAplicaAclSQLRepository>(ServiceProvider =>
            {
                return ServiceProvider.GetService<AplicaAclSQLRepository>();
            });
            
            builder.Services.AddTransient<IAplicaAclGraphApiRepository>(ServiceProvider =>
            {
                return ServiceProvider.GetService<AplicaAclGraphApiRepository>();
            });
            

            /*
            builder.Services.AddTransient<Func<string, IAplicaAclSQLRepository>>(ServiceProvider => key =>
            {
                switch(key)
                {
                    case "SQL":
                        return ServiceProvider.GetService<AplicaAclSQLRepository>();
                    default:
                        return ServiceProvider.GetService<AplicaAclSQLRepository>();
                }
            } );
            */
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .SetBasePath(context.ApplicationRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{context.EnvironmentName}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}