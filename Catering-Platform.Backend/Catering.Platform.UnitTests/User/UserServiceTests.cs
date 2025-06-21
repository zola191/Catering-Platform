using AutoFixture;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.Services;
using Catering.Platform.Applications.ViewModels.Users;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests.User;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Catering.Platform.UnitTests.User;

public class UserServiceTests
{
    private readonly IUserService _userService;
    private readonly IUserRepository _mockUserRepository;
    private readonly IHashService _mockHashService;
    private readonly ILogger<IUserService> _mockLogger;
    private readonly Fixture _fixture;

    public UserServiceTests()
    {
        _mockUserRepository = Substitute.For<IUserRepository>();
        _mockHashService = Substitute.For<IHashService>();
        _mockLogger = NullLogger<IUserService>.Instance;

        _fixture = new Fixture();
        _userService = new UserService(_mockUserRepository, _mockHashService, _mockLogger);
    }

    [Fact]
    public async Task CreateUserAsync_ReturnsUserViewModel_WhenDataIsValid()
    {
        // Arrange
        var request = _fixture.Create<CreateUserRequest>();
        var hashedPassword = "hashed_password";
        var userDomain = CreateUserRequest.MapToDomain(request);
        userDomain.PasswordHash = hashedPassword;

        var expectedViewModel = UserViewModel.MapToViewModel(userDomain);

        _mockHashService.HashPassword(request.Password).Returns(hashedPassword);
        _mockUserRepository.AddAsync(userDomain).Returns(Task.CompletedTask);

        // Act
        var result = await _userService.CreateUserAsync(request);

        // Assert
        Assert.Equal(expectedViewModel.Id, result.Id);
        Assert.Equal(expectedViewModel.FirstName, result.FirstName);
        Assert.Equal(expectedViewModel.MiddleName, result.MiddleName);
        Assert.Equal(expectedViewModel.LastName, result.LastName);

        await _mockUserRepository.Received(1).AddAsync(Arg.Is<Domain.Models.User>(u =>
            u.FirstName == request.FirstName &&
            u.MiddleName == request.MiddleName &&
            u.LastName == request.LastName &&
            u.PasswordHash == hashedPassword));
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        var request = _fixture.Create<CreateUserRequest>();
        var hashedPassword = "hashed_password";

        _mockHashService.HashPassword(request.Password).Returns(hashedPassword);

        _mockUserRepository.AddAsync(Arg.Any<Domain.Models.User>()).Throws(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _userService.CreateUserAsync(request));
        Assert.Equal("Database error", exception.Message);
    }
}
