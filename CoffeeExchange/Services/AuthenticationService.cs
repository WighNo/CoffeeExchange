using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using CoffeeExchange.Configs;
using CoffeeExchange.Data.Context;
using CoffeeExchange.Data.Context.Entities;
using CoffeeExchange.Data.Requests.Models;
using CoffeeExchange.Data.Response.Models;
using CoffeeExchange.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoffeeExchange.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly AuthenticationConfig _config;
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public AuthenticationService(AuthenticationConfig config, DataContext dataContext, IMapper mapper)
    {
        _config = config;
        _dataContext = dataContext;
        _mapper = mapper;
    }

    public async Task<(bool, string)> Register(AuthenticationRequest request)
    {
        if (_dataContext.Users.Any(user => user.Login == request.Login) == true)
            return (false, "User already registered");

        var user = _mapper.Map<User>(request);
        user.ProvideSaltAndHash();

        await _dataContext.Users.AddAsync(user);
        await _dataContext.SaveChangesAsync();
        
        return (true, "Success");
    }

    public async Task<(bool, string)> RegisterWithRole(RegistrationWithRoleRequest request)
    {
        if (_dataContext.Users.Any(user => user.Login == request.Login) == true)
            return (false, "User already registered");

        var user = _mapper.Map<User>(request);
        user.ProvideSaltAndHash();
        
        await _dataContext.Users.AddAsync(user);
        await _dataContext.SaveChangesAsync();
        
        return (true, "Success");
    }

    public async Task<(bool, string)> Login(AuthenticationRequest request)
    {
        var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Login == request.Login);

        if (user is null)
            return (false, "Invalid Email");
        
        if (user.PasswordHash != AuthenticationHelpers.ComputeHash(request.Password, user.Salt)) 
            return (false, "Invalid Password");

        return (true, GenerateJwtToken(AssembleClaimsIdentity(user)));
    }

    private string GenerateJwtToken(ClaimsIdentity subject)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        byte[] key = Encoding.ASCII.GetBytes(_config.BearerKey);

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = subject,
            Expires = DateTime.Now.AddMinutes(_config.LifeTime),
            SigningCredentials = new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private ClaimsIdentity AssembleClaimsIdentity(User user)
    {
        ClaimsIdentity subject = new ClaimsIdentity(new[]
        {
            new Claim("id", user.Id.ToString()),
            new Claim(ClaimsIdentity.DefaultIssuer, _config.Issuer),
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString()),
        });

        return subject;
    }
}