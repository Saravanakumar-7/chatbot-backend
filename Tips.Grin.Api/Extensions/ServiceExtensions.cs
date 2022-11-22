using LoggerService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Repository;

namespace Tips.Grin.Api.Extensions
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
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination"));
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
            services.AddDbContext<TipsGrinDbContext>(o => o.UseSqlServer(connectionString));
        }

        //public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        //{
        //    services.AddScoped<IRepositoryWrapperForMaster, RepositoryWrapperForMaster>();
        //}
    }
}
