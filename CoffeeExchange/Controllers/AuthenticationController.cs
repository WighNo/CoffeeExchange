using CoffeeExchange.Configs;
using CoffeeExchange.Data.Requests.Models;
using CoffeeExchange.Data.Response.Models;
using CoffeeExchange.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeExchange.Controllers;

/// <summary>
/// Контроллер аутентификации
/// </summary>
[ApiController]
[Route("authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="authenticationService">Сервис аутентификации</param>
    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    /// <param name="request">Данные пользователя</param>
    /// <returns></returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> Register([FromBody] AuthenticationRequest request)
    {
        var (success, content) = await _authenticationService.Register(request);
        
        if (success is false)
            return BadRequest(content);

        return Ok();
    }

    /// <summary>
    /// Регистрация нового пользователя с ролью
    /// </summary>
    /// <param name="request">Параметры регистрации</param>
    /// <returns></returns>
    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost("register-with-role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterWithRole([FromBody] RegistrationWithRoleRequest request)
    {
        var (success, content) = await _authenticationService.RegisterWithRole(request);

        if (success is false)
            return BadRequest(content);

        return Ok();
    }

    /// <summary>
    /// Получение JWT-токена
    /// </summary>
    /// <param name="request">Данные пользователя</param>
    /// <returns></returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] AuthenticationRequest request)
    {
        var (success, content) = await _authenticationService.Login(request);

        if (success is false)
            return BadRequest(content);

        return Ok(new AuthenticationResponse(content, "None"));
    }
}