using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using Repository;
using Tips.Master.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureMSSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryWrapper();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IReleaseProductBomRepository, ReleaseProductBomRepository>();
builder.Services.AddScoped<IReleaseCostBomRepository, ReleaseCostBomRepository>();
builder.Services.AddScoped<IReleaseEnggBomRepository, ReleaseEnggBomRepository>();
builder.Services.AddScoped<IEnggBomGroupRepository, EnggBomGroupRepository>();
builder.Services.AddScoped<IReleaseEnggBomRepository, ReleaseEnggBomRepository>();


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
