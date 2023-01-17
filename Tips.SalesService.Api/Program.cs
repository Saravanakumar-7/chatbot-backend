using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using NLog;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Configuration;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Controllers;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Extensions;
using Tips.SalesService.Api.Repository;
using static Tips.SalesService.Api.Repository.RfqEnggItemRepository;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureMSSqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
//builder.Services.AddDbContext<DbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Server=DESKTOP-EFBBM74;Database=TipsSalesService;Trusted_Connection=True;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true;")));

//builder.Services.AddDbContext<TipsSalesServiceDbContext>();
//builder.Services.AddScoped<IRfqRepository, RfqRepository>();

builder.Services.AddScoped<IFgOqcRepository, FgOqcRepository>();
builder.Services.AddScoped<IFinalOqcRepository, FinalOqcRepository>();

builder.Services.AddScoped<IRfqLPCostingRepository,RfqLPCostingRepository>();
builder.Services.AddScoped<IRfqSourcingRepository, RfqSourcingRepository>();
builder.Services.AddScoped<IRfqEnggRepository, RfqEnggRepository>();
builder.Services.AddScoped<IRfqCustomerSupportRepository,RfqCustomerSupportRepository>();
builder.Services.AddScoped<IRfqCustomerSupportItemRepository, RfqCustomerSupportItemsRepository>();
builder.Services.AddScoped<IRfqRepository, RfqRepository>();

builder.Services.AddScoped<ISalesOrderRepository,SalesOrderRepository>();
builder.Services.AddScoped<IQuoteRepository,QuoteRepository>();


builder.Services.AddScoped<IForeCastRepository, ForeCastRepository>();
builder.Services.AddScoped<IForeCastCustomerSupportRepository, ForeCastCustomerSupportRepository>();
builder.Services.AddScoped<IForeCastCustomerSupportItemRepository, ForeCastCustomerSupportItemsRepository>();
builder.Services.AddScoped<IForeCastEnggRepository, ForeCastEnggRepository>();
builder.Services.AddScoped<IForeCastEnggItemsRepository, ForeCastEnggItemRepository>();
builder.Services.AddScoped<IForeCastReleaseLpRepository, ForeCastLPReleaseRepository>();

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

//builder.Services.AddScoped<IRfqCustomerSupportNotesRepository, RfqCustomerSupportNotes>();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
 