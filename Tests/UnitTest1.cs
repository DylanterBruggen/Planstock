using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Backend.Controllers;
using Backend.Areas.Identity.Data;
using Backend.Authentication;
using System.Threading.Tasks;
using System.Collections.Generic;

public class AuthenticateControllerTests
{
    private readonly Mock<UserManager<BackendUser>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly IConfiguration _configuration;
    private readonly AuthenticateController _controller;

    public AuthenticateControllerTests()
    {
        var store = new Mock<IUserStore<BackendUser>>();
        _userManagerMock = new Mock<UserManager<BackendUser>>(store.Object, null, null, null, null, null, null, null, null);
        var roleStore = new Mock<IRoleStore<IdentityRole>>();
        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

        var configValues = new Dictionary<string, string>
{
    { "JWT:Secret", "ThisIsAReallyLongSecureTestKey1234567890!!!" }, // ≥ 32 chars
    { "JWT:ValidIssuer", "testissuer" },
    { "JWT:ValidAudience", "testaudience" }
};
        _configuration = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();

        _controller = new AuthenticateController(_userManagerMock.Object, _roleManagerMock.Object, _configuration);
    }

    [Fact]
    public async Task Login_Successful()
    {
        var model = new LoginModel { Email = "test@voorbeeld.nl", Password = "GeldigWacht123!" };
        var user = new BackendUser { Email = model.Email, UserName = "testuser" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, model.Password)).ReturnsAsync(true);
        _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

        var result = await _controller.Login(model);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Login_InvalidPassword()
    {
        var model = new LoginModel { Email = "test@voorbeeld.nl", Password = "foutWachtwoord" };
        var user = new BackendUser { Email = model.Email, UserName = "testuser" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, model.Password)).ReturnsAsync(false); // password invalid

        var result = await _controller.Login(model);

        Assert.IsType<UnauthorizedResult>(result);
    }



    [Fact]
    public async Task Login_NonExistentEmail()
    {
        var model = new LoginModel { Email = "nep@voorbeeld.nl", Password = "watDanOok123" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync((BackendUser)null);

        var result = await _controller.Login(model);

        var notFound = Assert.IsType<NotFoundObjectResult>(result); // 
        Assert.Contains("User not found", notFound.Value.ToString());
    }



    [Fact]
    public async Task Login_EmptyFields()
    {
        var model = new LoginModel { Email = "", Password = "" };

        _controller.ModelState.AddModelError("Email", "Required");
        _controller.ModelState.AddModelError("Password", "Required");

        var result = await _controller.Login(model);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequest.Value);
    }
}
