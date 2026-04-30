using System;
using System.Threading.Tasks;
using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Models;
using EMS.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace EMS.Tests.Services;
[TestFixture]
public class AuthServiceTests
{
    private AppDbContext _db = null!;
    private IConfiguration _config = null!;
    private AuthService _service = null!;

    [SetUp]
    public void Setup()
    {
        // Fresh in-memory database per test
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);

        // Mock IConfiguration for JWT settings
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Key"]).Returns("TestSecretKey_32Chars_ForNUnit!!");
        mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("EMS.API");
        mockConfig.Setup(c => c["Jwt:Audience"]).Returns("EMS.Client");
        mockConfig.Setup(c => c["Jwt:ExpiryHours"]).Returns("8");
        _config = mockConfig.Object;

        _service = new AuthService(_db, _config);
    }

    [TearDown]
    public void TearDown()
    {
        _db.Dispose();
    }

    // ── RegisterAsync ─────────────────────────────────────────────────────

    [Test]
    public async Task RegisterAsync_NewUser_ReturnsSuccessWithToken()
    {
        // Arrange
        var dto = new RegisterRequestDto { Username = "testuser", Password = "pass123", Role = "Viewer" };

        // Act
        var result = await _service.RegisterAsync(dto);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Token, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Username, Is.EqualTo("testuser"));
    }

    [Test]
    public async Task RegisterAsync_DuplicateUsername_ReturnsFailure()
    {
        // Arrange — seed an existing user
        _db.AppUsers.Add(new AppUser
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        var dto = new RegisterRequestDto { Username = "admin", Password = "newpass", Role = "Viewer" };

        // Act
        var result = await _service.RegisterAsync(dto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Token, Is.Null);
    }

    [Test]
    public async Task RegisterAsync_ShortPassword_IsHandledByCaller()
    {
        // AuthService does not validate password length — that's done by DTO annotations.
        // This test confirms BCrypt still hashes any string passed to it.
        var dto = new RegisterRequestDto { Username = "shortpass", Password = "ab", Role = "Viewer" };
        var result = await _service.RegisterAsync(dto);
        Assert.That(result.Success, Is.True); // service itself doesn't re-validate
    }

    // ── LoginAsync ────────────────────────────────────────────────────────

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        // Arrange — seed a user with known password
        _db.AppUsers.Add(new AppUser
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        var dto = new LoginRequestDto { Username = "admin", Password = "admin123" };

        // Act
        var result = await _service.LoginAsync(dto);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Token, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Role, Is.EqualTo("Admin"));
    }

    [Test]
    public async Task LoginAsync_WrongPassword_ReturnsFailure()
    {
        // Arrange
        _db.AppUsers.Add(new AppUser
        {
            Username = "viewer",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("viewer123"),
            Role = "Viewer",
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        var dto = new LoginRequestDto { Username = "viewer", Password = "wrongpass" };

        // Act
        var result = await _service.LoginAsync(dto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Token, Is.Null);
    }

    [Test]
    public async Task LoginAsync_NonExistentUser_ReturnsFailure()
    {
        var dto = new LoginRequestDto { Username = "ghost", Password = "anypass" };
        var result = await _service.LoginAsync(dto);
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public void GenerateToken_ValidUser_ReturnsNonEmptyString()
    {
        var user = new AppUser { Id = 1, Username = "admin", Role = "Admin", PasswordHash = "x", CreatedAt = DateTime.UtcNow };
        var token = _service.GenerateToken(user);
        Assert.That(token, Is.Not.Null.And.Not.Empty);
    }
}