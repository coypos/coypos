using System.Net;
using CoyposServer.Controllers;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CoyposServer.UnitTests;

[TestFixture]
public class ProductTests
{
    private DatabaseContext _dbContext;
    private ProductController _productController;
    private CategoryController _categoryController;
    
    [SetUp]
    public async Task Setup()
    {
        _dbContext = await TestHelpers.GenerateDatabaseContext();
        _productController = new ProductController(_dbContext);
        _categoryController = new CategoryController(_dbContext);
    }

    [Test]
    public void GetProductsWithoutFilter()
    {
        var req = _productController.GetProductsWithFilter(new Product() { }, "AND", _dbContext.Products.Count(), 1);
        req.CheckStatusCode(HttpStatusCode.OK);
        var result = req.YeldExpectedResult<RichResponse<List<Product>>>();
        result.Response.Count.Should().Be(_dbContext.Products.Count());
    }

    [Test]
    public void GetProductsWithFilter()
    {
        var req = _productController.GetProductsWithFilter(new Product() { Name = _dbContext.Products.First().Name});
        req.CheckStatusCode(HttpStatusCode.OK);
        var result1 = req.YeldExpectedResult<RichResponse<List<Product>>>().Response;

        var result2 = _dbContext.Products.ToList().Filter(new Product() { Name = _dbContext.Products.First().Name },
            ListExtensions.FilterType.AND);

        foreach (var product in result1)
            result2.Any(p => p.ID == product.ID).Should().BeTrue();

        foreach (var product in result2)
            result1.Any(p => p.ID == product.ID).Should().BeTrue();
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
    public async Task UpdateProduct()
    {
        var product = _dbContext.Products.ToList()[3];
        var req = await _productController.Put(new Product() { Name = "A new fancy name" }, (int) product.ID);
        req.CheckStatusCode(HttpStatusCode.OK);
        var result = req.YeldExpectedResult<Product>();
        result.ID.Should().Be(product.ID);
        result.Name.Should().Be("A new fancy name");
        _dbContext.Products.ToList()[3].Name.Should().Be("A new fancy name");
    }
    
    [Test]
    public async Task UpdateNonExistentProduct()
    {
        var req = await _productController.Put(new Product() { Name = "A new fancy name" }, _dbContext.Products.Count());
        req.CheckStatusCode(HttpStatusCode.InternalServerError);
        var result = req.YeldExpectedResult<ProblemDetails>();
    }

    [Test]
    public async Task DeleteProduct()
    {
        var product = _dbContext.Products.ToList()[3];
        var req = await _productController.Delete((int) product.ID);
        req.CheckStatusCode(HttpStatusCode.OK);
        var result = req.YeldExpectedResult<string>();
        _dbContext.Products.Any(p => p.ID == product.ID).Should().BeFalse();
    }
    
    [Test]
    public async Task DeleteNonExistentProduct()
    {
        var cachedCount = _dbContext.Products.Count();
        var req = await _productController.Delete(_dbContext.Products.Count());
        req.CheckStatusCode(HttpStatusCode.InternalServerError);
        var result = req.YeldExpectedResult<ProblemDetails>();
        _dbContext.Products.Count().Should().Be(cachedCount);
    }

    [Test]
    public async Task CreateProduct()
    {
        var name = "A new fancy product";
        var createDate = DateTime.Now + TimeSpan.FromSeconds(10);
        var product = new Product()
        {
            Name = name,
            Barcode = "1000",
            CreateDate = createDate,
            Enabled = true,
            IsLoose = false,
            Price = (decimal)21.37
        };

        var req = await _productController.Post(product);
        req.CheckStatusCode(HttpStatusCode.OK);
        var result = req.YeldExpectedResult<Product>();

        result.Name.Should().Be(name);
        result.CreateDate.Should().NotBe(createDate);
    }
}