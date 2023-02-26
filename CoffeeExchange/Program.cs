using CoffeeExchange.Configs;
using CoffeeExchange.Data.Context;
using CoffeeExchange.Mapper;
using CoffeeExchange.Middlewares;
using CoffeeExchange.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

AuthenticationConfig? authConfig = configuration.GetSection(AuthenticationConfig.SectionKey).Get<AuthenticationConfig>();
if (authConfig is null)
    throw new NullReferenceException("Can't parse auth config");

services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString("Database"));
});

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.DescribeAllParametersInCamelCase();
    
    var filePath = Path.Combine(AppContext.BaseDirectory, "CoffeeExchange.xml");
    options.IncludeXmlComments(filePath);
    
    options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

services.AddAutoMapper(typeof(MapperProfile));
services.AddSingleton<AuthenticationConfig>(authConfig);
services.AddScoped<IAuthenticationService, AuthenticationService>();

string webRootPath = builder.Environment.WebRootPath;
services.AddScoped<ProductPhotosSaveService>(_ => new ProductPhotosSaveService(webRootPath));

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // ValidateIssuer = true,
        ValidateIssuer = false,
        // ValidIssuer = authConfig.Issuer,
        
        // ValidateAudience = true,
        ValidateAudience = false,
        // ValidAudience = authConfig.Audience,
        
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = authConfig.SymmetricSecurityKey(),
        
        ClockSkew = TimeSpan.Zero,
    };
});

services.AddAuthorization();

var app = builder.Build();

app.UseRequestLogging();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseClaimsDetermination();

app.MapControllers();

app.Run();