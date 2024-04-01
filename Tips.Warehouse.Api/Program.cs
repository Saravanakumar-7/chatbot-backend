using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using NLog;
using System.Configuration;
using System.Text.Json.Serialization;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Extensions;
using Tips.Warehouse.Api.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));


builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.AddHttpContextAccessor();

//builder.Services.ConfigureMSSqlContext(builder.Configuration);
builder.Services.ConfigureMySqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            // ValidIssuer = "Wyzmindz", // Use the same issuer as the one in https://localhost:7016
            ValidateAudience = false,
            // ValidAudience = "Tips", // Use the same audience as the one in https://localhost:7016
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("yX%z@1&*U$3#sP9!")), // Use the same secret key as the one in https://localhost:7016
        };
    });
//builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
//builder.Services.AddScoped<IOpenDeliveryOrderRepository, OpenDeliveryOrderRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddScoped<IOpenDeliveryOrderRepository, OpenDeliveryOrderRepository>();
builder.Services.AddScoped<IBTODeliveryOrderRepository, BTODeliveryOrderRepository>();
builder.Services.AddScoped<IDeliveryOrderRepository, DeliveryOrderRepository>();
builder.Services.AddScoped<IOpenDeliveryOrderHistoryRepository, OpenDeliveryOrderHistoryRepository>();
builder.Services.AddScoped<IReturnBtoDeliveryOrderRepository, ReturnBtoDeliveryOrderRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryTranctionRepository, InventoryTranctionRepository>();
builder.Services.AddScoped<IReturnInvoiceRepository, ReturnInvoiceRepository>();
builder.Services.AddScoped<IBTODeliveryOrderItemsRepository, BTODeliveryOrderItemRepository>();
builder.Services.AddScoped<IInvoiceChildRepository, InvoiceChildRepository>();
builder.Services.AddScoped<ILocationTransferRepository, LocationTransferRepository>();
builder.Services.AddScoped<IMaterialIssueTrackerRepository, MaterialIssueTrackerRepository>();
builder.Services.AddScoped<IBTODeliveryOrderHistoryRepository, BTODeliveryOrderHistoryRepository>();
builder.Services.AddScoped<IReturnOpenDeliveryOrderRepository, ReturnOpenDeliveryOrderRepository>();
builder.Services.AddScoped<IOpenDeliveryOrderPartsRepository, OpenDeliveryOrderPartsRepository>();
builder.Services.AddScoped<IReturnOpenDeliveryOrderPartsRepository, ReturnOpenDeliveryOrderPartsRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IOpenDeliveryOrderRepository, OpenDeliveryOrderRepository>();
builder.Services.AddScoped<IBTODeliveryOrderInventoryHistoryRepository, BTODeliveryOrderInventoryHistoryRepository>();


builder.Services.AddScoped<MySqlConnection>();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();


//builder.Services.A

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

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

app.MapControllers();

app.Run();