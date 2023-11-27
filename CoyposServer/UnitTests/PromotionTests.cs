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
public class PromotionTests
{
	private DatabaseContext _dbContext;
	private PromotionController _promotionController;
	private ProductController _productController;
    
	[SetUp]
	public async Task Setup()
	{
		_dbContext = await TestHelpers.GenerateDatabaseContext();
		_promotionController = new PromotionController(_dbContext);
		_productController = new ProductController(_dbContext);
	}
    
	[Test]
	public void GetPromotions()
	{
		var req = _promotionController.GetPromotionsWithFilter(new Promotion(), itemsPerPage: _dbContext.Promotions.Count());
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<RichResponse<List<Promotion>>>();
		result.Response.Count.Should().Be(_dbContext.Promotions.Count());
	}

	[Test]
	public void GetPromotionsWithFilter()
	{
		var req = _promotionController.GetPromotionsWithFilter(new Promotion() {Ids = "1"}, itemsPerPage: _dbContext.Promotions.Count());
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<RichResponse<List<Promotion>>>();
		result.Response.Count.Should().Be(1);
	}

	[Test]
	public async Task UpdatePromotion()
	{
		var req = await _promotionController.Put(new Promotion() {Ids = "3,4"}, 0);
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<Promotion>();
		result.Ids.Should().Be("3,4");
		_dbContext.Promotions.First().Ids.Should().Be("3,4");
	}

	[Test]
	public async Task UpdatePromotionSameDate()
	{
		var d = DateTime.Now;
		var req = await _promotionController.Put(new Promotion() {StartDate = d, EndDate = d}, 0);
		var result = req.YeldExpectedResult<ProblemDetails>();
		result.Title.Should().Be("Start and End date cannot be the same");
	}
	
	[Test]
	public async Task UpdatePromotionWithDateMissmatch()
	{
		var req = await _promotionController.Put(new Promotion() {StartDate = DateTime.Now + TimeSpan.FromDays(1), EndDate = DateTime.Now}, 0);
		var result = req.YeldExpectedResult<ProblemDetails>();
		result.Title.Should().Be("Promotion cannot start after it ends");
	}

	[Test]
	public async Task UpdatePromotionDiscountBelowZero()
	{
		var req = await _promotionController.Put(new Promotion() {DiscountPercentage = -30}, 0);
		var result = req.YeldExpectedResult<ProblemDetails>();
		result.Title.Should().Be("Promotion discount needs to be in range of 1-99 (incl.)");
	}

	[Test]
	public async Task UpdatePromotionDiscountAboveNinetyNine()
	{
		var req = await _promotionController.Put(new Promotion() {DiscountPercentage = 100}, 0);
		var result = req.YeldExpectedResult<ProblemDetails>();
		result.Title.Should().Be("Promotion discount needs to be in range of 1-99 (incl.)");
	}
	
	[Test]
	public async Task CreatePromotion()
	{
		var req = await _promotionController.Post(new Promotion()
		{
			CreateDate = DateTime.Now,
			UpdateDate = DateTime.Now,
			StartDate = DateTime.Now - TimeSpan.FromDays(1),
			EndDate = DateTime.Now + TimeSpan.FromDays(1),
			Ids = "10,19",
			DiscountPercentage = 50
		});
		req.CheckStatusCode(HttpStatusCode.OK);
		var result = req.YeldExpectedResult<Promotion>();
		result.Ids.Should().Be("10,19");
		_dbContext.Promotions.Count(_ => _.Ids == "10,19").Should().Be(1);
	}

	[Test]
	public async Task DeletePromotion()
	{
		var cachedIds = _dbContext.Promotions.First().Ids;
		var req = await _promotionController.Delete(0);
		req.CheckStatusCode(HttpStatusCode.OK);
		_dbContext.Promotions.Count(_ => _.Ids == cachedIds).Should().Be(0);
	}

	[Test]
	public void IsPromotionPresentInTheProduct()
	{
		var promo = _dbContext.Promotions.First();
		var req = _productController.GetProductsWithFilter(new Product() { }, "AND", _dbContext.Products.Count(), 1);
		var result = req.YeldExpectedResult<RichResponse<List<Product>>>();
		var r = result.Response.First();
		var pr = Math.Round((decimal)(r.Price - r.Price * promo.DiscountPercentage / 100), 2);
		r.AppliedPromotion = promo;
		r.DiscountedPrice.Should().Be(pr);
	}
}