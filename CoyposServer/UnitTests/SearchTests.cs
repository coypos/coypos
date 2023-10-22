using System.Globalization;
using System.Net;
using System.Text;
using CoyposServer.Controllers;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace CoyposServer.UnitTests;

[TestFixture]
public class SearchTests
{
    private DatabaseContext _dbContext;
    private SearchController _searchController;
    
    [SetUp]
    public async Task Setup()
    {
        _dbContext = await TestHelpers.GenerateDatabaseContext();
        _searchController = new SearchController(_dbContext);
    }
    
    [Test]
    public void SearchNoQuery()
    {
        var req = _searchController.SearchProduct(itemsPerPage: _dbContext.Products.Count());
        req.CheckStatusCode(HttpStatusCode.OK);
        var result = req.YeldExpectedResult<RichResponse<List<Product>>>();
        result.Response.Count.Should().Be(_dbContext.Products.Count());
    }
    
    [Test]
    public void SearchMatchingQuerySingle()
    {
        var searchString = _dbContext.Products.First().Name.Split('-')[0];
        var howManyShouldWeHave = _dbContext.Products.Count(_ => _.Name.ToLower().Contains(searchString.ToLower()));
        
        var req = _searchController.SearchProduct(searchString,  -1, _dbContext.Products.Count(), 1);
        req.CheckStatusCode(HttpStatusCode.OK);
        var result = req.YeldExpectedResult<RichResponse<List<Product>>>();
        result.Response.Count.Should().Be(howManyShouldWeHave);
    }
    
    [Test]
    public void SearchMatchingQueryMultiple()
    {
        var searchString = "a";
        var howManyShouldWeHave = _dbContext.Products.Count(_ => _.Name.ToLower().Contains(searchString.ToLower()));
        
        var req = _searchController.SearchProduct(searchString, -1, _dbContext.Products.Count(), 1);
        req.CheckStatusCode(HttpStatusCode.OK);
        var result = req.YeldExpectedResult<RichResponse<List<Product>>>();
        result.Response.Count.Should().Be(howManyShouldWeHave);
    }
    
    [Test]
    public void SearchNoMatchingQuery()
    {
        var searchString = Guid.NewGuid().ToString();
        var req = _searchController.SearchProduct(searchString, -1, _dbContext.Products.Count());
        req.CheckStatusCode(HttpStatusCode.OK);
        var result = req.YeldExpectedResult<RichResponse<List<Product>>>();
        result.Response.Count.Should().Be(0);
    }
    
    [Test]
    public void SearchDiacritics()
    {
        var diacritics = new List<Tuple<char, int>>()
        {
            new('ą', 0), new('ę', 0), new('ś', 0), new('ó', 0), new('ł', 0),
            new('ż', 0), new('ź', 0), new('ć', 0), new('ń', 0)
        };
        
        for (var i = 0; i < diacritics.Count; i++)
            diacritics[i] = new(diacritics[i].Item1,  _dbContext.Products.Count(_ => _.Name.ToLower().Contains(diacritics[i].Item1.ToString().RemoveDiacritics())));
        
        foreach (var product in _dbContext.Products)
            product.Name = product.Name.AddDiacritics();
        
        _dbContext.SaveChanges();
        
        foreach (var (item1, howManyShouldWeHave) in diacritics)
        {
            var req = _searchController.SearchProduct(item1.ToString(), -1, _dbContext.Products.Count());
            req.CheckStatusCode(HttpStatusCode.OK);
            var result = req.YeldExpectedResult<RichResponse<List<Product>>>();
            result.Response.Count.Should().Be(howManyShouldWeHave);
        }
    }
}