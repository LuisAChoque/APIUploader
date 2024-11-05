using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Services;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

// Configuración de JWT
var key = Encoding.ASCII.GetBytes("TuClaveSecretaMuySeguraMuchoMasQueSegura!!!!!");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Configuración de Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Mi API",
        Version = "v1"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduzca 'Bearer' seguido de su token en el campo de texto. Ejemplo: 'Bearer 12345abcdef'"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//bbdd
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//AWS
// Cargar configuración de AWS desde appsettings.json
var awsOptions = builder.Configuration.GetSection("AWS");

// Configurar AWS S3
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    return new AmazonS3Client(
        awsOptions["AccessKey"],
        awsOptions["SecretKey"],
        Amazon.RegionEndpoint.USEast2
    );
});

// Registrar un servicio de transferencia para S3
builder.Services.AddSingleton<TransferUtility>();

// Opcional: Registrar un servicio de almacenamiento que use S3
builder.Services.AddSingleton<IFileStorageService, AwsStorageService>();

//azure
builder.Services.AddSingleton(sp =>
{
    var azureConnectionString = builder.Configuration.GetSection("Azure:ConnectionStrings:AzureBlobStorage").Value;
    return new BlobServiceClient(azureConnectionString);
});
builder.Services.AddSingleton<IFileStorageService, AzureStorageService>();

builder.Services.AddScoped<IFileStorageDataService, FileStorageDataService>();


builder.Services.AddScoped<IUserService, UserAuthService>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
