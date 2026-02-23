using System.Security.Claims;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

namespace AWM.Service.Tests;

public class HttpAuthorizationContextTests
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpAuthorizationContext _sut;
    private readonly CurrentUserProvider _currentUserProvider;

    public HttpAuthorizationContextTests()
    {
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _currentUserProvider = new CurrentUserProvider(_httpContextAccessor);
        _sut = new HttpAuthorizationContext(_httpContextAccessor, _currentUserProvider);
    }

    [Fact]
    public void UserId_ShouldReturnCorrectId_WhenClaimExists()
    {
        // Arrange
        var userId = 123;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        var result = _sut.UserId;

        // Assert
        result.Should().Be(userId);
    }

    [Fact]
    public void UserId_ShouldMatchCurrentUserProvider_WhenClaimExists()
    {
        // Arrange
        var userId = 456;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        var sutResult = _sut.UserId;
        var providerResult = _currentUserProvider.UserId;

        // Assert
        sutResult.Should().Be(userId);
        providerResult.Should().Be(userId);
        sutResult.Should().Be(providerResult);
    }

    [Fact]
    public void UniversityId_ShouldReturnCorrectId_WhenClaimExists()
    {
        // Arrange
        var uniId = 789;
        var claims = new List<Claim>
        {
            new Claim(AuthorizationConstants.UniversityIdClaimType, uniId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        var result = _sut.UniversityId;

        // Assert
        result.Should().Be(uniId);
    }

    [Fact]
    public void UniversityId_ShouldReturnCorrectId_WhenLegacyClaimExists()
    {
        // Arrange
        var uniId = 999;
        var claims = new List<Claim>
        {
            new Claim("UniversityId", uniId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        var result = _sut.UniversityId;

        // Assert
        result.Should().Be(uniId);
    }

    [Fact]
    public void UniversityId_CurrentUserProvider_ShouldSupportLegacyClaim()
    {
        // Arrange
        var uniId = 999;
        var claims = new List<Claim>
        {
            new Claim("UniversityId", uniId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        var result = _currentUserProvider.UniversityId;

        // Assert
        result.Should().Be(uniId);
    }
}
