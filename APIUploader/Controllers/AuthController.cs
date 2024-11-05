using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using BCrypt.Net;
using WebApplication1.Services;
using Microsoft.AspNetCore.Authorization;


[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    // Endpoint de registro
    [HttpPost("register")]
    public IActionResult Register(UserRegister request)
    {
        
        try
        {
            _userService.RegisterUser(request);
        }
        catch (Exception e)
        {

            return BadRequest(e.Message);
        }

        return Ok("User registered");
    }

    // Endpoint de login
    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLogin request)
    {
        string token;
        try
        {
            token = _userService.LoginUser(request);

        }
        catch (Exception e)
        {

            return Unauthorized(e.Message);
        }

        return Ok(new { Token = token });

    }


}


