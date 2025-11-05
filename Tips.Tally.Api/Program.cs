using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using System.Text;
using Tips.Tally.Api.Contracts;
using Tips.Tally.Api.Extensions;
using Tips.Tally.Api.Repository;

var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
//builder.Services.ConfigureMSSqlContext(builder.Configuration);
builder.Services.ConfigureMySqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
//builder.Services.AddDbContext<DbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Server=DESKTOP-EFBBM74;Database=TipsSalesService;Trusted_Connection=True;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true;")));
builder.Services.AddControllers();
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(15);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(15);
});
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
        Title = "Tips.Tally.Api",
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

builder.Services.AddScoped<ITallyRepository, TallyRepository>();
builder.Services.AddScoped<ITokenValidationService, TokenValidationService>();


var app = builder.Build();

app.UseSwagger();


app.UseHttpsRedirection();


app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");

app.UseRouting();

app.UseMiddleware<TokenValidationMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tips.Tally.Api");
    c.RoutePrefix = "swagger";
    c.DisplayRequestDuration();
});

app.MapControllers();

app.Run();
