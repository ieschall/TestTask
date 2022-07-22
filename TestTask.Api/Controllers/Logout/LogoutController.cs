using Microsoft.AspNetCore.Mvc;
using TestTask.Api.Controllers.Logout.Models;
using TestTask.Logic.Services.Authentication;

namespace TestTask.Api.Controllers.Logout;

public class LogoutController : Controller
{
    private readonly AuthenticationService _authService = new();
    
    [HttpPost("api/logout")]
    [ProducesResponseType(200)]
    public IActionResult Login([FromBody] LogoutRequest request)
    {
        var token = request.RefreshToken;
        _authService.Logout(token);
        return Ok();
    }
}