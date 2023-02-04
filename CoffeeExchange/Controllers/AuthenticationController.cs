using CoffeeExchange.Data.Context;
using CoffeeExchange.Data.Requests.Models;
using CoffeeExchange.Data.Response.Models;
using CoffeeExchange.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeExchange.Controllers;

[ApiController]
[Route("authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly DataContext _dataContext;

    public AuthenticationController(IAuthenticationService authenticationService, DataContext dataContext)
    {
        _authenticationService = authenticationService;
        _dataContext = dataContext;
    }

    [HttpGet("get-user-info")]
    public async Task<IActionResult> GetUserInfo([FromQuery] int id)
    {
        var result = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthenticationRequest request)
    {
        var (success, content) = await _authenticationService.Register(request);
        
        if (success == false)
            return BadRequest(content);

        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register-with-role")]
    public async Task<IActionResult> RegisterWithRole([FromBody] RegistrationWithRoleRequest request)
    {
        var (success, content) = await _authenticationService.RegisterWithRole(request);

        if (success == false)
            return BadRequest(content);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthenticationRequest request)
    {
        var (success, content) = await _authenticationService.Login(request);

        if (success == false)
            return BadRequest(content);

        return Ok(new AuthenticationResponse(content, "None"));
    }
}