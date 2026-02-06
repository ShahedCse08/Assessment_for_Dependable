using Microsoft.Extensions.DependencyInjection;
using PMS.Application.Interfaces;
using PMS.Application.Services;
using PMS.Infrastructure.Repositories;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace PharmacyManagementSystem
{
    public class Global : HttpApplication
    {
        private static IServiceProvider _serviceProvider;
        void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ConfigureServices();
        }

        private void ConfigureServices()
        {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            services.AddScoped<IDbConnection>(sp =>
            {
                var connStr = ConfigurationManager.ConnectionStrings["PharmacyDB"].ConnectionString;
                var connection = new SqlConnection(connStr);
                return connection;
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMedicineRepository, MedicineRepository>();
            services.AddScoped<ISalesRepository, SalesRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMedicineService, MedicineService>();
            services.AddScoped<ISalesService, SalesService>();


            _serviceProvider = services.BuildServiceProvider();
        }
        public static T GetService<T>()
        {
            return _serviceProvider.GetService<T>();
        }
    }
}