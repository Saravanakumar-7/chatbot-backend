using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using System.Text;
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
        Title = "Tips.Purchase.Api",
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
builder.Services.AddScoped<IPurchaseRequisitionRepository, PurchaseRequisitionRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IDocumentUploadRepository, UploadDocumentRepository>();
builder.Services.AddScoped<IPRItemsDocumentUploadRepository, PRItemsUploadDocumentRepository>();
builder.Services.AddScoped<IPoItemsRepository, PurchaseOrderItemRepository>();
builder.Services.AddScoped<IPOCollectionTrackerRepository, POCollectionTrackerRepository>();
builder.Services.AddScoped<IPOCollectionTrackerForAviRepository, POCollectionTrackerForAviRepository>();
builder.Services.AddScoped<IPOBreakDownRepository, POBreakDownRepository>();
builder.Services.AddScoped<IPOBreakDownForAviRepository, POBreakDownForAviRepository>();
builder.Services.AddScoped<IPoConfirmationDateHistoryRepository, PoConfirmationDateHistoryRepository>();
builder.Services.AddScoped<IPoConfirmationDateRepository, PoConfirmationDateRepository>();
builder.Services.AddScoped<IPoConfirmationHistoryRepository, PoConfirmationHistoryRepository>();
builder.Services.AddScoped<IPrItemsRepository, PurchaseRequisitionItemRepository>();
builder.Services.AddScoped<IPoAddprojectRepository, PoAddprojectRepository>();
builder.Services.AddScoped<IPoItemHistoryRepository, PoItemHistoryRepository>();
builder.Services.AddScoped<IPOInitialConfirmationDateHistoryRepository, POInitialConfirmationDateHistoryRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
//}

app.UseHttpsRedirection();

//app.UseStaticFiles();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});
app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tips.Purchase.Api");
    c.RoutePrefix = "swagger";
    c.DisplayRequestDuration();
});


app.MapControllers();

app.Run();