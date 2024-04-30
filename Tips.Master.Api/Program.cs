using Accounts;
using Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using Repository;
using System.Text;
using Tips.Master.Api.Extensions;
using static Repository.ReleaseCostBomRepository;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
//builder.Services.ConfigureMSSqlContext(builder.Configuration);
builder.Services.ConfigureMySqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryWrapper();
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
//var key = builder.Configuration["Jwt:key"];
//builder.Services.ConfigureJwtToken(builder.Configuration);
builder.Services.AddTransient<IJwtAuth, Auth>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tips.Master.Api",
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
            ValidateLifetime = false,
            //ValidIssuer = "Wyzmindz",
            //ValidAudience = "Tips",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("yX%z@1&*U$3#sP9!")), // Use the same secret key as the one in https://localhost:7016
        };
    });
//builder.Services.AuthenticateByJwtToken(builder.Configuration);
//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IReleaseProductBomRepository, ReleaseProductBomRepository>();
builder.Services.AddScoped<IAdditionalChargesRepository, AdditionalChargesRepository>();
builder.Services.AddScoped<IReleaseCostBomRepository, ReleaseCostBomRepository>();
builder.Services.AddScoped<IReleaseEnggBomRepository, ReleaseEnggBomRepository>();
builder.Services.AddScoped<IEnggBomGroupRepository, EnggBomGroupRepository>();
builder.Services.AddScoped<IEnggBomRepository, EngineeringBomRepository>();
builder.Services.AddScoped<ILeadRepository, LeadRepository>();
builder.Services.AddScoped<IFileUploadRepository, FileUploadDocumentRepository>();
builder.Services.AddScoped<IUOMRepository, UOMRepository>();
builder.Services.AddScoped<IConvertionrateRepository, ConvertionrateRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IImageUploadRepository, ImageUploadDocumentRepository>();
builder.Services.AddScoped<ILeadWebsiteRepository, LeadWebsiteRepository>();
builder.Services.AddScoped<ITypeOfHomeRepository, TypeOfHomeRepository>();
builder.Services.AddScoped<IBHKRepository, BHKRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ISFTRepository, SFTRepository>();
builder.Services.AddScoped<IProjectNameRepository, ProjectNameRepository>();
builder.Services.AddScoped<IPmcContractorRepository, PmcContractorRepository>();
builder.Services.AddScoped<IArchitectureRepository, ArchitectureRepository>();
builder.Services.AddScoped<ILightningDesignerRepository, LightningDesignerRepository>();
builder.Services.AddScoped<IStageOfConstructionRepository, StageOfConstructionRepository>();
builder.Services.AddScoped<IStateRepository, StateRepository>();
builder.Services.AddScoped<ISourceDetailsRepository, SourceDetailsRepository>();


builder.Services.AddScoped<IRoomNameRepository, RoomNameRepository>();
builder.Services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
builder.Services.AddScoped<ITypeSolutionRepository, TypeSolutionRepository>();
builder.Services.AddScoped<ITypeOfRoomRepository, TypeOfRoomRepository>();
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddHttpClient();
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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tips.Master.Api");
    c.RoutePrefix = "swagger";
    c.DisplayRequestDuration();
});
//app.UseRouting();

app.MapControllers();

app.Run();