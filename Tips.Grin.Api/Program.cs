using Contracts;
using LoggerService;
using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using System.Text.Json.Serialization;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Extensions;
using Tips.Grin.Api.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureMSSqlContext(builder.Configuration);
//builder.Services.ConfigureMySqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AuthenticateByJwtToken(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGrinRepository, GrinRepository>();
builder.Services.AddScoped<IIQCConfirmationRepository, IQCConfirmationRepository>();
builder.Services.AddScoped<IBinningRepository, BinningRepository>();
builder.Services.AddScoped<IReturnGrinRepository, ReturnGrinRepository>();
builder.Services.AddScoped<IBinningItemsRepository, BinningItemsRepository>();
builder.Services.AddScoped<IGrinPartsRepository, GrinPartsRepository>();
builder.Services.AddScoped<IIQCConfirmationItemsRepository, IQCConfirmationItemsRepository>();
//builder.Services.AddScoped<IReturnGrinDocumentUploadRepository, ReturnGrinDocumentUpload>();
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


builder.Services.AddScoped<IDocumentUploadRepository, UploadDocumentRepository>();
builder.Services.AddHttpClient();

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

app.UseAuthorization();

app.MapControllers();

app.Run();

