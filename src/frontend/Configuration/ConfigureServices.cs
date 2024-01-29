// using System.Configuration;
using CafeReadConf.Frontend.Models;
using CafeReadConf.Frontend.Service;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using CafeReadConf.EasyAuth;

namespace CafeReadConf.Configure
{
    public static class ServiceCollectionBuilder
    {
        public static IServiceCollection ConfigureServices(
            this IServiceCollection services,
            IConfiguration config)
        {

            services.AddRazorPages();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AppServicesAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = AppServicesAuthenticationDefaults.AuthenticationScheme;
            })
            .AddAppServicesAuthentication();

            // Add Identity Service to the container
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministrator", policy
                    => policy.RequireRole("app.admin"));
            });

            services.AddSingleton<UserEntityFactory>();

            // UserService Conditional Dependency Injection based on Backend API URL configuration 
            if (string.IsNullOrEmpty(config["BACKEND_API_URL"]))
            {
                // If no backend API URL is provided, we assume we are connecting to TableStorage directly from the frontend
                services.AddSingleton<IUserService, UserServiceTableStorage>();
            }
            else
            {
                services.AddHttpClient("ApiBaseAddress", client =>
                {
                    client.BaseAddress = new Uri(config["BACKEND_API_URL"]);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("User-Agent", "AppInno Frontend");
                });
                // If backend API URL is provided, we assume we are connecting to the Azure Function backend API
                services.AddSingleton<IUserService, UserServiceAPI>();
            }

            return services;
        }

        public static void ValidateCriticalConfig(this IConfiguration config)
        {
            //Fail fast if some of the mandatory configurations are missing
            var missingKey = config
                .GetChildren()
                .FirstOrDefault(c => string.IsNullOrEmpty(c.Value))?.Key;

            //Checking the value of SECRET key to validate its dependancies : 
            var secretKeyValue = config.GetValue<string>("SECRET");
            if (!String.IsNullOrEmpty(secretKeyValue))
            {
                switch (missingKey)
                {
                    //If secret is set, we need to check if all the other mandatory configurations are set
                    case "AZURE_TABLE_STORAGE_URI":
                    case "AZURE_TABLE_STORAGE_TABLENAME":
                    case "AZURE_TABLE_PARTITION_KEY":
                        //And fail fast in case any of these is missing
                        throw new ConfigurationErrorsException("Missing configuration values for the table storage backend");
                    default:
                        // All configurations are present...
                        return; // Exit the method
                }
            }
        }
    }
}