using ApiFinalProject.BLL.DTOs.Auth;
using ApiFinalProject.BLL.Managers;
using Microsoft.AspNetCore.Mvc;

namespace ApiFinalProject.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthManager _authManager;

    public AuthController(IAuthManager authManager)
    {
        _authManager = authManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authManager.RegisterAsync(dto);
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authManager.LoginAsync(dto);
        if (!result.IsSuccess)
        {
            return Unauthorized(result);
        }
        return Ok(result);
    }
}
