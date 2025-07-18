using Microsoft.AspNetCore.Mvc;
using AuthenticationMicroservice.Api.Routes;
using AuthenticationMicroservice.Business.Service.Abstractions;
using AuthenticationMicroservice.Contracts.Requests;
using AuthenticationMicroservice.Domain.Models;
using AuthenticationMicroservice.Contracts.DTOs;
using Serilog;

namespace AuthenticationMicroservice.Api.Controllers;

[ApiController]
[Route(ApiRoutes.AuthenticationController.Base)]
public class AuthenticationController : ControllerBase
{
    private readonly IUsersService _usersService;

    public AuthenticationController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpPost(ApiRoutes.AuthenticationController.Register)]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        Log.Information("Register Request called");

        User user = await _usersService.Register(request.Email, request.Password);

        if (user.Id == 0)
        {
            return Conflict();
        }

        return Ok(user);
    }

    [HttpPost(ApiRoutes.AuthenticationController.Login)]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        Log.Information("Login Request called");

        AccessAndRefreshToken result = await _usersService.Login(request.Email, request.Password);

        if (result.AccessToken == string.Empty)
        {
            return NotFound(
                new
                {
                    Message = "Account Not Found"
                }
            );
        }

        return Ok(result);
    }

    [HttpDelete(ApiRoutes.AuthenticationController.Logout)]
    public async Task<ActionResult> Logout([FromBody] LogoutRequest request)
    {
        Log.Information("Logout Request Called");

        await _usersService.Logout(request.Id);

        return Ok();
    }

    [HttpPost(ApiRoutes.AuthenticationController.RefreshAccessToken)]
    public async Task<ActionResult> RefreshAccessToken([FromBody] RefreshTokenRequest request)
    {
        Log.Information("Refresh Request Called");

        string result = await _usersService.RefreshAccessToken(request.RefreshToken);

        return Ok(
            new
            {
                AccessToken = result
            }
        );
    }

    [HttpPost(ApiRoutes.AuthenticationController.GetNewRefreshToken)]
    public async Task<ActionResult> GetNewRefreshToken([FromBody] LogoutRequest request)
    {
        Log.Information("Get New Refresh Token Request Called");

        string result = await _usersService.GetNewRefreshToken(request.Id);

        return Ok(
            new
            {
                RefreshToken = result
            }
        );
    }
}
