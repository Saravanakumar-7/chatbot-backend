using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using NLog;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Configuration;
using System.Text.Json.Serialization;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Controllers;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Extensions;
using Tips.SalesService.Api.Repository;
using Microsoft.OpenApi.Models;
using static Tips.SalesService.Api.Repository.RfqEnggItemRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
//builder.Services.ConfigureMSSqlContext(builder.Configuration);
builder.Services.ConfigureMySqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
//builder.Services.AddDbContext<DbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Server=DESKTOP-EFBBM74;Database=TipsSalesService;Trusted_Connection=True;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true;")));
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tips.SalesService.Api",
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
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes("yX%z@1&*U$3#sP9!")), // Use the same secret key as the one in https://localhost:7016
        };
    });

builder.Services.AddEndpointsApiExplorer();


//builder.Services.AddDbContext<TipsSalesServiceDbContext>();
//builder.Services.AddScoped<IRfqRepository, RfqRepository>();

builder.Services.AddScoped<IFgOqcRepository, FgOqcRepository>();
builder.Services.AddScoped<IFinalOqcRepository, FinalOqcRepository>();

builder.Services.AddScoped<IRfqLPCostingRepository, RfqLPCostingRepository>();
builder.Services.AddScoped<IRfqSourcingRepository, RfqSourcingRepository>();
builder.Services.AddScoped<IRfqEnggRepository, RfqEnggRepository>();
builder.Services.AddScoped<IRfqCustomerSupportRepository, RfqCustomerSupportRepository>();
builder.Services.AddScoped<IRfqCustomerSupportItemRepository, RfqCustomerSupportItemsRepository>();
builder.Services.AddScoped<IRfqRepository, RfqRepository>();

builder.Services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();
builder.Services.AddScoped<ISalesAdditionalChargesRepository, SalesAdditionalChargesRepository>();
builder.Services.AddScoped<ISoConfirmationDateHistoryRepository, SoConfirmationDateHistoryRepository>();
builder.Services.AddScoped<ISoConfirmationDateRepository, SoConfirmationDateRepository>();
builder.Services.AddScoped<ISalesOrderAdditionalChargesHistoryRepository, SalesOrderAdditionalChargesHistoryRepository>();

builder.Services.AddScoped<IScheduleDateHistoryRepository, ScheduleDateHistoryRepository>();

builder.Services.AddScoped<IForeCastRepository, ForeCastRepository>();
builder.Services.AddScoped<IForeCastCustomerSupportRepository, ForeCastCustomerSupportRepository>();
builder.Services.AddScoped<IForeCastCustomerSupportItemRepository, ForeCastCustomerSupportItemsRepository>();
builder.Services.AddScoped<IForeCastEnggRepository, ForeCastEnggRepository>();
builder.Services.AddScoped<IForeCastEnggItemsRepository, ForeCastEnggItemRepository>();
builder.Services.AddScoped<IForeCastReleaseLpRepository, ForeCastLPReleaseRepository>();
builder.Services.AddScoped<ICollectionTrackerRepository, CollectionTrackerRepository>();
builder.Services.AddScoped<IForecastSourcingRepository, ForeCastSourcingRepository>();
builder.Services.AddScoped<IForecastLpCostingRepository, ForecastLpCostingRepository>();
builder.Services.AddScoped<IMaterialRequestRepository, MaterialRequestRepository>();
builder.Services.AddScoped<IMaterialTransactionNoteRepository, MaterialTransactionNoteRepository>();
builder.Services.AddScoped<ILocationTransferRepository, LocationTransferRepository>();
builder.Services.AddScoped<IReleaseLpRepository, RfqLPReleaseRepository>();
builder.Services.AddScoped<IRfqEnggItemRepository, RfqEnggItemRepository>();
builder.Services.AddScoped<IRfqCustomGroupRepository, RfqCustomGroupRepository>();
builder.Services.AddScoped<IRfqCustomFieldRepository, RfqCustomFieldRepository>();
builder.Services.AddScoped<IForeCastCustomGroupRepository, ForeCastCustomGroupRepository>();
builder.Services.AddScoped<IForeCastCustomFieldRepository, ForeCastCustomFieldRepository>();
builder.Services.AddScoped<ISalesOrderItemsRepository, SalesOrderItemRepository>();
builder.Services.AddScoped<IItemPriceListRepository, ItemPriceListRepository>();
builder.Services.AddScoped<ISalesOrderHistoryRepository, SalesOrderHistoryRepository>();
builder.Services.AddScoped<ICoverageReportRepository, CoverageReportRepository>();
builder.Services.AddScoped<IDocumentUploadRepository, UploadDocumentRepository>();
builder.Services.AddScoped<ISalesOrderAdditionalChargesHistoryRepository, SalesOrderAdditionalChargesHistoryRepository>();
builder.Services.AddScoped<IScheduleDateHistoryRepository, ScheduleDateHistoryRepository>();

//builder.Services.AddScoped<IRfqCustomerSupportNotesRepository, RfqCustomerSupportNotes>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();


app.UseHttpsRedirection();

//app.UseStaticFiles();

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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tips.SalesService.Api");
    c.RoutePrefix = "swagger";
    c.DisplayRequestDuration();
});

app.MapControllers();

app.Run();
