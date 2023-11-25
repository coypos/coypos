using System.Net;
using CoyposServer.Controllers;
using CoyposServer.Models;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace CoyposServer.UnitTests;

[TestFixture]
public class AppTests
{
	private DatabaseContext _dbContext;
	private UserController _userController;
	private AppController _appController;
    
	[SetUp]
	public async Task Setup()
	{
		_dbContext = await TestHelpers.GenerateDatabaseContext();
		_userController = new UserController(_dbContext);
		_appController = new AppController(_dbContext);
	}

	[Test]
	public async Task LoginSuccessfully()
	{
		var req = await _appController.Login(_dbContext.Users.First().Email, TestHelpers.FirstUserPassword);
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<string>();
		result.Should().Be(_dbContext.Users.First().LoginToken);
	}
	
	[Test]
	public async Task LoginWithEmptyEmail()
	{
		var req = await _appController.Login("", TestHelpers.FirstUserPassword);
		req.CheckStatusCode(HttpStatusCode.Locked);
		var result = req.YeldExpectedResult<string>();
		result.Should().Be("1");
	}
	
	[Test]
	public async Task LoginWithEmptyPassword()
	{
		var req = await _appController.Login(_dbContext.Users.First().Email, "");
		req.CheckStatusCode(HttpStatusCode.Locked);
		var result = req.YeldExpectedResult<string>();
		result.Should().Be("1");
	}
	
	[Test]
	public async Task LoginWithInvalidEmail()
	{
		var req = await _appController.Login("invalid@email.com", TestHelpers.FirstUserPassword);
		req.CheckStatusCode(HttpStatusCode.Locked);
		var result = req.YeldExpectedResult<string>();
		result.Should().Be("2");
	}
	
	[Test]
	public async Task LoginWithBadPassword()
	{
		var req = await _appController.Login(_dbContext.Users.First().Email, TestHelpers.FirstUserPassword + "a");
		req.CheckStatusCode(HttpStatusCode.Locked);
		var result = req.YeldExpectedResult<string>();
		result.Should().Be("2");
	}
	
	[Test]
	public async Task LogoutSuccessfully()
	{
		await LoginSuccessfully();
		var req = await _appController.Logout(_dbContext.Users.First().LoginToken);
		req.CheckStatusCode(HttpStatusCode.OK);
		_dbContext.Users.First().LoginToken.Should().BeNull();
	}

	[Test]
	public async Task LogoutWithEmptyToken()
	{
		await LoginSuccessfully();
		var req = await _appController.Logout("");
		req.CheckStatusCode(HttpStatusCode.Locked);
		var result = req.YeldExpectedResult<string>();
		result.Should().Be("1");
		_dbContext.Users.First().LoginToken.Should().NotBeNull();
	}
	
	[Test]
	public async Task LogoutWithInvalidToken()
	{
		await LoginSuccessfully();
		var req = await _appController.Logout(Guid.NewGuid().ToString());
		req.CheckStatusCode(HttpStatusCode.Locked);
		var result = req.YeldExpectedResult<string>();
		result.Should().Be("2");
		_dbContext.Users.First().LoginToken.Should().NotBeNull();
	}
}