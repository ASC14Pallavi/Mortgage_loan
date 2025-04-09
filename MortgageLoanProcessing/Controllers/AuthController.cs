using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MortgageLoanProcessing.DTOs;
using MortgageLoanProcessing.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null) return Unauthorized("Invalid credentials");

        var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);
        if (!result.Succeeded) return Unauthorized("Invalid credentials");

        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
    }

    //[HttpPost("logout")]
    //public async Task<IActionResult> Logout()
    //{
    //    await _signInManager.SignOutAsync();
    //    return Ok("User logged out successfully");
    //}

    //[HttpGet("admin")]
    //[Authorize(Roles = "Admin")]
    //public async Task<IActionResult> GetAdminData()
    //{
    //    var adminUserName = User.Identity.Name; // Get logged-in admin username from JWT
    //    var adminUser = await _userManager.FindByNameAsync(adminUserName);

    //    if (adminUser == null)
    //    {
    //        return NotFound("Admin user not found");
    //    }

    //    return Ok(new
    //    {
    //        Username = adminUser.UserName,
    //        Email = adminUser.Email,
    //        Roles = await _userManager.GetRolesAsync(adminUser),
    //        //CreatedDate = adminUser.CreatedDate // Assuming you have a CreatedDate field
    //    });
    //}


    // User Endpoint - Fetch registered user data (Only User can access)
    [HttpGet("user")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetUserData()
    {
        var userName = User.Identity.Name; // Get logged-in username from the JWT token
        var user = await _userManager.FindByNameAsync(userName);

        if (user == null)
        {
            return NotFound("User not found");
        }

        // Return user details
        return Ok(new
        {
            Username = user.UserName,
            Email = user.Email,
            Roles = await _userManager.GetRolesAsync(user)
        });
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return BadRequest("User already exists");

        var user = new User
        {
            UserName = model.Username,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Assign the default "User" role only (Admin cannot self-register)
        await _userManager.AddToRoleAsync(user, "User");

        return Ok(new { message = "User registered successfully" });

    }

}