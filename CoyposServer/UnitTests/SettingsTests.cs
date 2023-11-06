using System.Net;
using CoyposServer.Controllers;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace CoyposServer.UnitTests;

[TestFixture]
public class SettingsTests
{
	private DatabaseContext _dbContext;
	private SettingsController _settingsController;
    
	[SetUp]
	public async Task Setup()
	{
		_dbContext = await TestHelpers.GenerateDatabaseContext();
		_settingsController = new SettingsController(_dbContext);
	}
    
	[Test]
	public void ListSettingsWithoutFilter()
	{
		var req = _settingsController.Settings(new Setting() { }, "AND", _dbContext.Products.Count(), 1);
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<RichResponse<List<Setting>>>();
		result.Response.Count.Should().Be(_dbContext.Settings.Count());
	}
	
	[Test]
	public void ListSettingsWithFilter()
	{
		var req = _settingsController.Settings(new Setting() { Key = _dbContext.Settings.First().Key}, "AND", _dbContext.Products.Count(), 1);
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<RichResponse<List<Setting>>>();
		result.Response.Count.Should().Be(1);
	}

	[Test]
	public void GetSettingById()
	{
		var req = _settingsController.Setting("1");
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<Setting>();
		result.ID.Should().Be(1);
	}
	
	[Test]
	public void GetSettingByKey()
	{
		var req = _settingsController.Setting(_dbContext.Settings.First().Key);
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<Setting>();
		result.ID.Should().Be(0);
		result.Key.Should().Be(_dbContext.Settings.First().Key);
	}

	[Test]
	public void AddSetting()
	{
		var req = _settingsController.AddSetting(new SettingRequestModel() {Key = "te", Value = "st"});
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<Setting>();
		result.Key.Should().Be("te");
		result.Value.Should().Be("st");
	}

	[Test]
	public void UpdateSetting()
	{
		var req = _settingsController.AddSetting(new SettingRequestModel() {Key = _dbContext.Settings.First().Key, Value = "test"});
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<Setting>();
		result.Key.Should().Be(_dbContext.Settings.First().Key);
		result.Value.Should().Be("test");
	}

	[Test]
	public void DeleteSetting()
	{
		var cachedCount = _dbContext.Settings.Count();
		var req = _settingsController.DeleteSetting("1");
		req.CheckStatusCode(HttpStatusCode.OK);
		(_dbContext.Settings.Count() + 1).Should().Be(cachedCount);
	}
}