using AuthenticationMicroservice.Business.Service.Abstractions;
using AuthenticationMicroservice.Contracts.DTOs;
using AuthenticationMicroservice.Api.Controllers;
using Moq;
using Xunit;
using AuthenticationMicroservice.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using AuthenticationMicroservice.Api.Validators;

namespace AuthenticationMicroservice.Tests.UnitTests;

public class AuthControllerTests
{
    private readonly Mock<IUsersService> _usersService;
    private readonly AuthenticationController _controller;
    private readonly LoginRequestValidator _loginRequestValidator;

    public AuthControllerTests()
    {
        _usersService = new Mock<IUsersService>();
        _controller = new AuthenticationController(_usersService.Object);
        _loginRequestValidator = new LoginRequestValidator();
    }

    [Fact]
    public async Task ReturnsSuccessWhenAllGood()
    {
        LoginRequest request = new LoginRequest
        {
            Email = "test@gmail.com",
            Password = "Monkey2008!_"
        };

        _usersService
        .Setup(x => x.Login("test@gmail.com", "Monkey2008!_"))
        .ReturnsAsync(new AccessAndRefreshToken
        {
            AccessToken = "asda",
            RefreshToken = "asd"
        });

        ActionResult result = await _controller.Login(request);

        OkObjectResult? okObjectResult = result as OkObjectResult;

        okObjectResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ReturnsNotFoundWhenAccountDoesntExist()
    {
        LoginRequest request = new LoginRequest
        {
            Email = "test@gmail.com",
            Password = "Monkey2008!_"
        };

        _usersService
        .Setup(x => x.Login("test@gmail.com", "Monkey2008!_"))
        .ReturnsAsync(new AccessAndRefreshToken { });

        ActionResult result = await _controller.Login(request);

        NotFoundObjectResult? notFoundObjectResult = result as NotFoundObjectResult;

        notFoundObjectResult!.StatusCode.Should().Be(404);
    }
}
