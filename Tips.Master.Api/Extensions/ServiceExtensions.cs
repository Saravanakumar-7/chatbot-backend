using Contracts;
using Entities;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MySql.EntityFrameworkCore.Extensions;
//using MySql.EntityFrameworkCore.Extensions;
using Repository;

namespace Tips.Master.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .WithExposedHeaders("X-Pagination")
                    .AllowAnyHeader());
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {

            });
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureMSSqlContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config["MSSqlconnection:connectionString"];
            services.AddDbContext<TipsMasterDbContext>(o => o.UseSqlServer(connectionString));

        }

        //public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        //{

        //    var connectionString = config["MySqlconnection:connectionString"];
        //    services.AddDbContext<TipsMasterDbContext>(o => o.UseMySQL(connectionString));
        //}

        public class MysqlEntityFrameworkDesignTimeServices : IDesignTimeServices
        {
            public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
            {
                serviceCollection.AddEntityFrameworkMySQL();
                new EntityFrameworkRelationalDesignServicesBuilder(serviceCollection)
                    .TryAddCoreServices();
            }
        }

        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapperForMaster, RepositoryWrapperForMaster>();
        }
    }
}
