using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using NLog;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Extensions;
using Tips.Purchase.Api.Repository;

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
builder.Services.Configure<KestrelServerOptions>(option =>
{
    option.Limits.MaxRequestBodySize = 1073741824;
});
builder.Services.Configure<IISServerOptions>(option =>
{
    option.MaxRequestBodySize = 1073741824;
});

var key = builder.Configuration["Jwt:key"];
builder.Services.ConfigureJwtToken(builder.Configuration);
builder.Services.AddScoped<IPurchaseRequisitionRepository, PurchaseRequisitionRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IDocumentUploadRepository, UploadDocumentRepository>();
builder.Services.AddScoped<IPRItemsDocumentUploadRepository, PRItemsUploadDocumentRepository>();
builder.Services.AddScoped<IPoItemsRepository, PurchaseOrderItemRepository>();
builder.Services.AddScoped<IPOCollectionTrackerRepository, POCollectionTrackerRepository>();
builder.Services.AddScoped<IPOBreakDownRepository, POBreakDownRepository>();
builder.Services.AddScoped<IPoConfirmationDateHistoryRepository, PoConfirmationDateHistoryRepository>();
builder.Services.AddScoped<IPoConfirmationDateRepository, PoConfirmationDateRepository>();
builder.Services.AddScoped<IPoConfirmationHistoryRepository, PoConfirmationHistoryRepository>();
builder.Services.AddScoped<IPrItemsRepository, PurchaseRequisitionItemRepository>();
builder.Services.AddScoped<IPoAddprojectRepository, PoAddprojectRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
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

app.UseAuthentication();

app.UseAuthorization();

app.UseRouting();

app.MapControllers();

app.Run();