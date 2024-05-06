using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using System.Text;
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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tips.Production.Api",
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
//var key = builder.Configuration["Jwt:key"];
//builder.Services.ConfigureJwtToken(builder.Configuration);
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            // ValidIssuer = "Wyzmindz", // Use the same issuer as the one in https://localhost:7016
//            ValidateAudience = false,
//            // ValidAudience = "Tips", // Use the same audience as the one in https://localhost:7016
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes("yX%z@1&*U$3#sP9!")), // Use the same secret key as the one in https://localhost:7016
//        };
//    });
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IShopOrderRepository, ShopOrderRepository>();
builder.Services.AddTransient<IShopOrderItemRepository, ShopOrderItemRepository>();
builder.Services.AddTransient<IShopOrderConfirmationRepository, ShopOrderConfirmationRepository>();
builder.Services.AddTransient<ISAShopOrderRepository, SAShopOrderRepository>();
builder.Services.AddTransient<ISAShopOrderMaterialIssueRepository, SAShopOrderMaterialIssueRepository>();
builder.Services.AddTransient<IFGShopOrderMaterialIssueRepository, FGShopOrderMaterialIssueRepository>();
builder.Services.AddTransient<IMaterialReturnNoteRepository, MaterialReturnNoteRepository>();
builder.Services.AddScoped<IMaterialIssueRepository, MaterialIssueRepository>();
builder.Services.AddScoped<IMaterialRequestsRepository, MaterialRequestsRepository>();
builder.Services.AddScoped<IOQCRepository, OQCRepository>();
builder.Services.AddScoped<IMaterialIssueHistoryRepository, MaterialIssueHistoryRepository>();
builder.Services.AddScoped<IMaterialIssueItemRepository, MaterialIssueItemRepository>();
builder.Services.AddScoped<IOQCBinningRepository, OQCBinningRepository>();

//builder.Services.AddTransient<IMaterialReturnNoteItemRepository, Mater>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
//app.UseSwaggerUI();
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
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tips.Production.Api");
    c.RoutePrefix = "swagger";
    c.DisplayRequestDuration();
});

app.MapControllers();

app.Run();