using FootballLeagueManagement.Infrastructure.Identity;
using FootballLeagueManagement.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterInputModel input)
    {
        var user = new ApplicationUser
        {
            UserName = input.Email,
            Email = input.Email,
            FullName = input.FullName,
            FavoriteClub = input.FavoriteClub
        };

        var result = await userManager.CreateAsync(user, input.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(error => error.Description));
        }

        await userManager.AddToRoleAsync(user, "User");
        return CreatedAtAction(nameof(Profile), new { }, new { user.Email, user.FullName, user.FavoriteClub });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginInputModel input)
    {
        var result = await signInManager.PasswordSignInAsync(input.Email, input.Password, isPersistent: false, lockoutOnFailure: true);
        return result.Succeeded ? Ok(new { Message = "Logged in." }) : Unauthorized();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return Ok(new { Message = "Logged out." });
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        var user = await userManager.GetUserAsync(User);
        return user is null
            ? Unauthorized()
            : Ok(new { user.Email, user.FullName, user.FavoriteClub });
    }
}
