using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore.Internal;
using NLog;
using Contracts;
using LoggerService;
using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using System.Text.Json.Serialization;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Extensions;
using Tips.Grin.Api.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
//builder.Services.ConfigureMSSqlContext(builder.Configuration);
builder.Services.ConfigureMySqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
//builder.Services.AuthenticateByJwtToken(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddControllers();
builder.Services.Configure<KestrelServerOptions>(option =>
{
    option.Limits.MaxRequestBodySize = 1073741824;
});
builder.Services.Configure<IISServerOptions>(option =>
{
    option.MaxRequestBodySize = 1073741824;
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tips.GRIN.Api",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "Jwt",
        In = ParameterLocation.Header,
        Description = "Enter the User Token with Bearer Format"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
         new OpenApiSecurityScheme
         {
             Reference = new OpenApiReference
             {
                 Type=ReferenceType.SecurityScheme,
                 Id="Bearer"
             }
         },
         new string[]{}
        }
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes("yX%z@1&*U$3#sP9!")), // Use the same secret key as the one in https://localhost:7016
        };
    });

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGrinRepository, GrinRepository>();
builder.Services.AddScoped<IIQCConfirmationRepository, IQCConfirmationRepository>();
builder.Services.AddScoped<IBinningRepository, BinningRepository>();
builder.Services.AddScoped<IReturnGrinRepository, ReturnGrinRepository>();
builder.Services.AddScoped<IBinningItemsRepository, BinningItemsRepository>();
builder.Services.AddScoped<IGrinPartsRepository, GrinPartsRepository>();
builder.Services.AddScoped<IOpenGrinRepository, OpenGrinRepository>();
builder.Services.AddScoped<IBinningLocationRepository, BinningLocationRepository>();
builder.Services.AddScoped<IWeightedAvgCostRepository, WeightedAvgCostRepository>();
builder.Services.AddScoped<IIQCConfirmationItemsRepository, IQCConfirmationItemsRepository>();
builder.Services.AddScoped<IDocumentUploadRepository, UploadDocumentRepository>();

var app = builder.Build();
app.UseSwagger();
app.UseHttpsRedirection();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tips.GRIN.Api");
    c.RoutePrefix = "swagger";
    c.DisplayRequestDuration();
});

app.MapControllers();

app.Run();