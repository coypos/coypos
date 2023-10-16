using CoyposServer.Controllers;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace CoyposServer.UnitTests;

[TestFixture]
public class FilterTests
{
    private DatabaseContext _dbContext;
    private ProductController _productController;
    
    [SetUp]
    public async Task Setup()
    {
        _dbContext = await TestHelpers.GenerateDatabaseContext();
        _productController = new ProductController(_dbContext);
    }

    [Test]
    public void FilterAND_Results()
    {
        var categories = _dbContext.Categories.ToList();
        var filtered = categories.Filter(new Category() { Name = categories[0].Name }, ListExtensions.FilterType.AND);
        filtered.Count().Should().Be(1);
        filtered.First().ID.Should().Be(categories[0].ID);
    }

    [Test]
    public void FilterAND_NoResults()
    {
        var categories = _dbContext.Categories.ToList();
        var filtered = categories.Filter(new Category() { Name = "No way I exist!" }, ListExtensions.FilterType.AND);
        filtered.Count().Should().Be(0);
    }

    [Test]
    public void FilterOR_Results()
    {
        var categories = _dbContext.Categories.ToList();
        var filtered = categories.Filter(new Category() { Name = categories[0].Name, ID = categories[1].ID}, ListExtensions.FilterType.OR);
        filtered.Count().Should().Be(2);
        filtered.Contains(categories[0]).Should().BeTrue();
        filtered.Contains(categories[1]).Should().BeTrue(); 
        
        var filtered2 = categories.Filter(new Category() { Name = "No way I exist!", ID = categories[1].ID}, ListExtensions.FilterType.OR);
        filtered2.Count().Should().Be(1);
        filtered2.Contains(categories[1]).Should().BeTrue(); 
    }

    [Test]
    public void FilterOR_NoResults()
    {
        var categories = _dbContext.Categories.ToList();
        var filtered = categories.Filter(new Category() { Name = "No way I exist!", ID = categories.Count()}, ListExtensions.FilterType.OR);
        filtered.Count().Should().Be(0);
    }

    [Test]
    public void FilterNOR_Results()
    {
        var categories = _dbContext.Categories.ToList();
        var filtered = categories.Filter(new Category() { Name = "No way I exist!"}, ListExtensions.FilterType.NOR);
        filtered.Count().Should().Be(categories.Count());
    }

    [Test]
    public void FilterNOR_NoResults()
    {
        var categories = _dbContext.Categories.ToList();
        var filtered = categories.Filter(new Category() { Name = categories[0].Name}, ListExtensions.FilterType.NOR);
        filtered.Count().Should().Be(categories.Count() - 1);
    }

    [Test]
    public void FilterISNULL_Results()
    {
        var categories = _dbContext.Categories.ToList();
        var filtered = categories.Filter(new Category() { ParentCategory = new Category() },
            ListExtensions.FilterType.ISNULL);
        filtered.Count().Should().Be(1);
    }

    [Test]
    public void FilterISNULL_NoResults()
    {
        var categories = _dbContext.Categories.ToList();
        var filtered = categories.Filter(new Category() { Name = "nope!" }, ListExtensions.FilterType.ISNULL);
        filtered.Count().Should().Be(0);
    }

    [Test]
    public void Pages()
    {
        var categories = _dbContext.Categories.ToList();
        categories.Pagefy(1, 1).Count.Should().Be(1);
        categories.Pagefy(3, 2).Count.Should().Be(3);
        categories.Pagefy(20, _dbContext.Categories.Count()).Count.Should().Be(0);
        categories.Pagefy(3, 2).Should().Contain(_dbContext.Categories.ToList()[5]);
    }
}