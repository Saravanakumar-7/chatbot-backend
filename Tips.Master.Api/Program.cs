using Accounts;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore.Internal;
using NLog;
using Repository;
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

var key = builder.Configuration["Jwt:key"];
builder.Services.ConfigureJwtToken(builder.Configuration);
builder.Services.AddTransient<IJwtAuth, Auth>();
//builder.Services.AuthenticateByJwtToken(builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
builder.Services.AddHttpContextAccessor();
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
app.UseAuthentication();

app.UseAuthorization();

app.UseRouting();

app.MapControllers();

app.Run();