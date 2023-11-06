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
public class CategoryTests
{
	private DatabaseContext _dbContext;
	private CategoryController _categoryController;
    
	[SetUp]
	public async Task Setup()
	{
		_dbContext = await TestHelpers.GenerateDatabaseContext();
		_categoryController = new CategoryController(_dbContext);
	}

	[Test]
	public void GetCategories()
	{
		var req = _categoryController.GetCategories(new Category(), itemsPerPage: _dbContext.Categories.Count());
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<RichResponse<List<Category>>>();
		result.Response.Count.Should().Be(_dbContext.Categories.Count());
	}
	
	[Test]
	public async Task CreateCategory()
	{
		var req = await _categoryController.CreateCategory(new Category() {Name = "Snowflake", CreateDate = DateTime.Now, UpdateDate = DateTime.Now, IsVisible = true, ParentCategory = _dbContext.Categories.First()});
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<Category>();
		result.Name.Should().Be("Snowflake");
		_dbContext.Categories.Count(_ => _.Name == "Snowflake").Should().Be(1);
	}

	[Test]
	public async Task UpdateCategory()
	{
		var req = await _categoryController.UpdateCategory(new Category() {Name = "Snowflake"}, 0);
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<Category>();
		result.Name.Should().Be("Snowflake");
		_dbContext.Categories.First().Name.Should().Be("Snowflake");
	}
}