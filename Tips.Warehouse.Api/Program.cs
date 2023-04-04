using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using System.Text.Json.Serialization;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Extensions;
using Tips.Warehouse.Api.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));


builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
//builder.Services.ConfigureMSSqlContext(builder.Configuration);
builder.Services.ConfigureMySqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IOpenDeliveryOrderRepository, OpenDeliveryOrderRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IOpenDeliveryOrderRepository, OpenDeliveryOrderRepository>();  
builder.Services.AddScoped<IBTODeliveryOrderRepository, BTODeliveryOrderRepository>();
builder.Services.AddScoped<IDeliveryOrderRepository, DeliveryOrderRepository>();
builder.Services.AddScoped<IReturnBtoDeliveryOrderRepository, ReturnBtoDeliveryOrderRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryTranctionRepository, InventoryTranctionRepository>();
builder.Services.AddScoped<IReturnInvoiceRepository, ReturnInvoiceRepository>();
builder.Services.AddScoped<IBTODeliveryOrderItemsRepository, BTODeliveryOrderItemRepository>();
builder.Services.AddScoped<IInvoiceChildRepository, InvoiceChildRepository>();

builder.Services.AddScoped<IBTODeliveryOrderHistoryRepository, BTODeliveryOrderHistoryRepository>();




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


app.UseAuthorization();

app.MapControllers();

app.Run();
