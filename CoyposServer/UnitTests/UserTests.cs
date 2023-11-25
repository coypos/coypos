using System.Net;
using CoyposServer.Controllers;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace CoyposServer.UnitTests;

[TestFixture]
public class UserTests
{
	private DatabaseContext _dbContext;
	private UserController _userController;
    
	[SetUp]
	public async Task Setup()
	{
		_dbContext = await TestHelpers.GenerateDatabaseContext();
		_userController = new UserController(_dbContext);
	}

	[Test]
	public void GetUsers()
	{
		var req = _userController.GetUsersWithFilter(new User(), itemsPerPage: _dbContext.Users.Count());
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<RichResponse<List<User>>>();
		result.Response.Count.Should().Be(_dbContext.Users.Count());
	}

	[Test]
	public void SensitiveDataIsTruncated()
	{
		var req = _userController.GetUsersWithFilter(new User(), itemsPerPage: _dbContext.Users.Count());
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<RichResponse<List<User>>>();
		var firstResult = result.Response.First();

		firstResult.Password.Should().BeNull();
		firstResult.Salt.Should().BeNull();
		firstResult.LoginToken.Should().BeNull();
	}

	[Test]
	public async Task UpdateUserNonSensitiveData()
	{
		var req = await _userController.Put(new User() {Name = "Snowflake"}, (int)_dbContext.Users.First().ID);
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<User>();
		result.Name.Should().Be("Snowflake");
		_dbContext.Users.First().Name.Should().Be("Snowflake");
	}
	
	[Test]
	public async Task UpdateUserPassword()
	{
		var cachedPassword = _dbContext.Users.First().Password;
		var req = await _userController.Put(new User() {Password = "Secret"}, (int)_dbContext.Users.First().ID);
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<User>();
		result.Password.Should().BeNull();
		var newPassword = _dbContext.Users.First().Password;
		newPassword.Should().NotBe(cachedPassword);
	}

	[Test]
	public async Task DeleteUser()
	{
		var cachedName = _dbContext.Users.First().Name;
		var req = await _userController.Delete((int)_dbContext.Users.First().ID);
		req.CheckStatusCode(HttpStatusCode.OK);
		_dbContext.Users.Count(_ => _.Name == cachedName).Should().Be(0);
	}

	[Test]
	public async Task CreateUser()
	{
		var req = await _userController.Post(new User()
		{
			Name = "Snowflake",
			Email = "snowflake@gmail.com",
			Password = "guess",
			PhoneNumber = "+48123123123",
		});
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<User>();
		result.Name.Should().Be("Snowflake");
		_dbContext.Users.Count(_ => _.Name == "Snowflake").Should().Be(1);
	}
	
	[Test]
	public async Task BanUser()
	{
		var req = await _userController.Ban((int)_dbContext.Users.First().ID);
		req.CheckStatusCode(HttpStatusCode.OK);
		_dbContext.Users.First().Role.Should().Be("Banned");
	}
	
	[Test]
	public async Task BanAlreadyBannedUser()
	{
		_dbContext.Users.First().Role = "Banned";
		await _dbContext.SaveChangesAsync();
		
		var req = await _userController.Ban((int)_dbContext.Users.First().ID);
		var result = req.YeldExpectedResult<ProblemDetails>();
		result.Title.Should().Be("User is already banned");
	}
	
	[Test]
	public async Task UnbanUser()
	{
		_dbContext.Users.First().Role = "Banned";
		await _dbContext.SaveChangesAsync();
		
		var req = await _userController.UnBan((int)_dbContext.Users.First().ID);
		req.CheckStatusCode(HttpStatusCode.OK);
		_dbContext.Users.First().Role.Should().BeNullOrEmpty();
	}
	
	[Test]
	public async Task UnbanAlreadyUnbannedUser()
	{
		var req = await _userController.UnBan((int)_dbContext.Users.First().ID);
		var result = req.YeldExpectedResult<ProblemDetails>();
		result.Title.Should().Be("User is not banned");
	}
}