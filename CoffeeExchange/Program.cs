using CoffeeExchange.Configs;
using CoffeeExchange.Data.Context;
using CoffeeExchange.Mapper;
using CoffeeExchange.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
});

services.AddAutoMapper(typeof(MapperProfile));
services.AddSingleton<AuthenticationConfig>(authConfig);
services.AddScoped<IAuthenticationService, AuthenticationService>();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();