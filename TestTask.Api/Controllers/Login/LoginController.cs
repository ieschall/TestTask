using System.Security.Authentication;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using TestTask.Api.Controllers.Login.Models;
using TestTask.Logic.Services.Authentication;
using TestTask.Logic.Services.Authentication.Responses;

namespace TestTask.Api.Controllers.Login;

[AllowAnonymous]
public class LoginController : Controller
{
    private readonly AuthenticationService _authService = new();
    
    [HttpPost("api/login")]
    [ProducesResponseType(typeof(AuthenticationResult), 200)]
    [ProducesResponseType(typeof(string), 401)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var username = request.Username;
        var password = request.Password;
        
        try
        {
            var authResult = _authService.Login(username, password);
            return Ok(authResult);
        }
        catch (AuthenticationException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
    
    [HttpPost("api/refresh")]
    [ProducesResponseType(typeof(AuthenticationResult), 200)]
    [ProducesResponseType(typeof(string), 401)]
    public IActionResult Login([FromBody] RefreshRequest request)
    {
        try
        {
            var authResult = _authService.Refresh(request.RefreshToken);
            return Ok(authResult);
        }
        catch (AuthenticationException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}