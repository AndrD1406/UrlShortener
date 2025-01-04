using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PolynomSolver.DataAccess.Dtos;
using System.Security.Claims;
using UrlShortener.BusinessLogic.Models;
using UrlShortener.BusinessLogic.Services;
using UrlShortener.DataAccess.Models;

namespace UrlShortener.WebApi.Controllers;

[Route("api/[controller]")]
[AllowAnonymous]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;
    
    private readonly ITokenService tokenService;
    private readonly IMapper mapper;

    public AccountController(
        UserManager<User> userManager, 
        SignInManager<User> signInManager,
        ITokenService tokenService,
        IMapper mapper)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.tokenService = tokenService;
        this.mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDTO)
    {
        // Validation 
        if (ModelState.IsValid == false)
        {
            string errorMessages = string.Join(" | ", ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage));
            return Problem(errorMessages);
        }

        // Create user
        User user = mapper.Map<User>(registerDTO);

        IdentityResult result = await userManager.CreateAsync(user, registerDTO.Password);

        if (result.Succeeded == true)
        {
            //adding the default role
            await userManager.AddToRoleAsync(user, "User");

            // sign-in
            // isPersistent: false - must be deleted automatically when the browser is closed
            await signInManager.SignInAsync(user, isPersistent: false);

            var authenticationResponse = tokenService.CreateJwtToken(user);
            user.RefreshToken = authenticationResponse.RefreshToken;

            user.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;
            await userManager.UpdateAsync(user);

            return Ok(authenticationResponse);
        }

        string errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description));
        return Problem(errorMessage);
    }

    [HttpGet]
    public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
    {
        User? user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return Ok(true);
        }
        return Ok(false);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDTO)
    {
        // Validation 
        if (ModelState.IsValid == false)
        {
            string errorMessages = string.Join(" | ", ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage));
            return Problem(errorMessages);
        }

        var result = await signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            User? user = await userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null)
                return NoContent();

            await signInManager.SignInAsync(user, isPersistent: false);

            var authenticationResponse = tokenService.CreateJwtToken(user);
            user.RefreshToken = authenticationResponse.RefreshToken;

            user.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;
            try
            {
                await userManager.UpdateAsync(user);
            }
            catch (Exception exc)
            {
                Console.WriteLine($"await userManager.UpdateAsync(user): {exc.Message}");
            }


            return Ok(authenticationResponse);
        }
        return Problem("Invalid email or password");
    }

    [HttpGet("logout")]
		public async Task<IActionResult> GetLogout()
		{
			await signInManager.SignOutAsync();

			return NoContent();
		}

    [HttpPost("generate-new-jwt-token")]
    public async Task<IActionResult> GenerateNewAccessToken(TokenModel tokenModel)
    {
        if (tokenModel == null)
        {
            return BadRequest("Invalid client request");
        }

        string? token = tokenModel.Token;
        string? refreshToken = tokenModel.RefreshToken;


        ClaimsPrincipal? principal = tokenService.GetPrincipalFromJwtToken(token);
        if (principal == null)
        {
            return BadRequest("Invalid access token");
        }

        string? email = principal.FindFirstValue(ClaimTypes.Email);

        User? user = await userManager.FindByEmailAsync(email);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpirationDateTime <= DateTime.UtcNow)
        {
            return BadRequest("Invalid refresh token");
        }

        AuthenticationResponse authenticationResponse = tokenService.CreateJwtToken(user);

        user.RefreshToken = authenticationResponse.RefreshToken;
        user.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;

        await userManager.UpdateAsync(user);

        return Ok(authenticationResponse);
    }
}
