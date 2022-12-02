using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Configuration;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Controllers;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Extensions;
using Tips.SalesService.Api.Repository;

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
builder.Services.AddScoped<IRfqSourcingRepository, RfqSourcingRepository>();
builder.Services.AddScoped<IRfqRepository,RfqRepository>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
