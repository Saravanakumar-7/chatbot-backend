using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Extensions;
using Tips.Production.Api.Repository;
//using Tips.Warehouse.Api.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureMSSqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers();
builder.Services.AddTransient<IShopOrderRepository, ShopOrderRepository>();
builder.Services.AddTransient<IShopOrderConfirmationRepository, ShopOrderConfirmationRepository>();
builder.Services.AddTransient<ISAShopOrderRepository, SAShopOrderRepository>();
builder.Services.AddTransient<ISAShopOrderMaterialIssueRepository, SAShopOrderMaterialIssueRepository>();
builder.Services.AddTransient<IFGShopOrderMaterialIssueRepository, FGShopOrderMaterialIssueRepository>();
builder.Services.AddTransient<IMaterialReturnNoteRepository, MaterialReturnNoteRepository>();
builder.Services.AddScoped<IMaterialIssueRepository, MaterialIssueRepository>();

//builder.Services.AddTransient<IMaterialReturnNoteItemRepository, Mater>();
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

