using Contracts;
using Entities;
using Microsoft.AspNetCore.Authentication;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MySql.EntityFrameworkCore.Extensions;
//using MySql.EntityFrameworkCore.Extensions;
using Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        {

            var connectionString = config["MySqlconnection:connectionString"];
            services.AddDbContext<TipsMasterDbContext>(o => o.UseMySQL(connectionString));
        }

        public class MysqlEntityFrameworkDesignTimeServices : IDesignTimeServices
        {
            public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
            {
                serviceCollection.AddEntityFrameworkMySQL();
                new EntityFrameworkRelationalDesignServicesBuilder(serviceCollection)
                    .TryAddCoreServices();
            }
        }

        //public static void AuthenticateByJwtToken(this IServiceCollection services, IConfiguration config)
        //{
        //    var key = config["Jwt:key"];

        //    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //     .AddJwtBearer(options =>
        //     {
        //         options.TokenValidationParameters = new TokenValidationParameters
        //         {
        //             ValidateIssuer = false,
        //             ValidateAudience = false,
        //             ValidateLifetime = false,
        //             ValidateIssuerSigningKey = true,
        //             //ValidIssuer = "[Issuer name]", // replace with the actual issuer name used by the Master API Microservice
        //             //ValidAudience = "[Audience name]", // replace with the actual audience name used by the Grin Service
        //             IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)) // replace with the actual secret key used by the Master API Microservice
        //         };
        //     });
        //}

        //public static void ConfigureJwtToken(this IServiceCollection services, IConfiguration config)
        //{
        //    /// security key for token generation
        //    var key = config["Jwt:key"];
        //    services.AddAuthentication(x =>
        //    {
        //        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    }).AddJwtBearer(x =>
        //    {
        //        x.RequireHttpsMetadata = false;
        //        x.SaveToken = true;
        //        x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            ValidateIssuer = false,
        //            ValidateAudience = false,
        //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))
        //        };
        //    });
        //}

        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapperForMaster, RepositoryWrapperForMaster>();
        }
    }
}
