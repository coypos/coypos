using System.Diagnostics;
using System.Net;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using Microsoft.AspNetCore.Mvc;
using NickStrupat;

namespace CoyposServer.Controllers;

public class DashboardController : ControllerBase
{
	private readonly DatabaseContext _dbContext;

	public DashboardController(DatabaseContext dbContext) => _dbContext = dbContext;
	
	/// <summary>
	/// Returns data that is used for displaying the homepage dashboard
	/// </summary>
	[HttpGet]
	[Route("dashboard")]
	[ProducesResponseType(typeof(DashboardModel), (int)HttpStatusCode.OK)]
	public ObjectResult Get()
	{
		var dashboardModel = new DashboardModel();
		var computerInfo = new ComputerInfo();
		dashboardModel.CoyPos = new CoyposModel()
		{
			Version = "0.4", //todo
			Time = DateTime.Now,
			MemoryUsed = (ulong)Process.GetCurrentProcess().PrivateMemorySize64,
			MemoryFree = (computerInfo.AvailablePhysicalMemory),
			MemoryTotal = computerInfo.TotalPhysicalMemory,
			OsName = computerInfo.OSFullName,
			OsPlatform = computerInfo.OSPlatform,
			OsVersion = computerInfo.OSVersion,
			DockerContainerId = System.IO.File.ReadAllText("/etc/hostname").ReplaceLineEndings(""),
			Uptime = Program.UptimeStopwatch.Elapsed
		};

		dashboardModel.EmployeeCount = _dbContext.Employees.Count();
		dashboardModel.UserCount = _dbContext.Users.Count();
		dashboardModel.UserWithMostPoints = dashboardModel.UserCount == 0 ? null : _dbContext.Users.OrderByDescending(_ => _.Points).First();

		dashboardModel.ProductCount = _dbContext.Products.Count();

		dashboardModel.MostPopularProducts = (!_dbContext.Transactions.Any() || !_dbContext.Products.Any())
			? new List<StatEntry<Product>>()
			: _dbContext.Transactions.GroupBy(_ => _.Product).OrderByDescending(_ => _.Sum(a => a.Quantity)).Take(5)
				.Select(_ => new StatEntry<Product>()
					{ Item = _.Key, Value = Convert.ToInt32(_.Sum(a => a.Quantity)) }).ToList();
		

		dashboardModel.CategoriesCount = _dbContext.Categories.Count();

		dashboardModel.NumberOfReceipts = _dbContext.Receipts.Count();
		dashboardModel.ReceiptsCount = !_dbContext.Receipts.Any() ? new List<Receipt>() : _dbContext.Receipts.OrderByDescending(_ => _.CreateDate).Take(5).ToList();

		dashboardModel.PaymentMethodCount = _dbContext.PaymentMethods.Count();
		dashboardModel.MostPopularPaymentMethods = (!_dbContext.Receipts.Any() || !_dbContext.PaymentMethods.Any()) ? new List<StatEntry<PaymentMethod>>() : _dbContext.Receipts.GroupBy(_ => _.PaymentMethod)
			.OrderByDescending(_ => _.Count()).Take(5)
			.Select(_ => new StatEntry<PaymentMethod>() { Item = _.Key, Value = _.Count() }).ToList();

		dashboardModel.PromotionCount = _dbContext.Promotions.Count();
		dashboardModel.ActivePromotions = _dbContext.Promotions
			.Where(_ => _.StartDate < DateTime.Now && _.EndDate > DateTime.Now).ToList();

		dashboardModel.RevenueToday = _dbContext.Transactions.ToList().Where(_ => _.CreateDate.Value.Date == DateTime.Today)
			.Select(_ => _.TotalPrice).Sum() ?? 0;
		dashboardModel.RevenueYesterday = _dbContext.Transactions.ToList()
			.Where(_ => _.CreateDate.Value.Date == DateTime.Today - TimeSpan.FromDays(1)).Select(_ => _.TotalPrice).Sum() ?? 0;
		dashboardModel.RevenueTodayTrend = dashboardModel.RevenueToday - dashboardModel.RevenueYesterday;
		dashboardModel.RevenueTodayPromotionLoss = _dbContext.Transactions.ToList().Where(_ => _.CreateDate.Value.Date == DateTime.Today)
			.Select(_ => _.TotalPrice - _.OriginalPrice).Sum() ?? 0;

		dashboardModel.RevenueThisWeek = _dbContext.Transactions.ToList()
			.Where(_ => _.CreateDate >= GetStartOfWeek(DateTime.Today) && _.CreateDate <= GetEndOfWeek(DateTime.Today))
			.Select(_ => _.TotalPrice).Sum() ?? 0;
		dashboardModel.RevenuePreviousWeek = _dbContext.Transactions.ToList()
			.Where(_ => _.CreateDate >= GetStartOfWeek(DateTime.Today).AddDays(-7) && _.CreateDate <= GetEndOfWeek(DateTime.Today).AddDays(-7))
			.Select(_ => _.TotalPrice).Sum() ?? 0;
		dashboardModel.RevenueThisWeekTrend = dashboardModel.RevenueThisWeek - dashboardModel.RevenuePreviousWeek;
		dashboardModel.RevenueThisWeekPromotionLoss = _dbContext.Transactions.ToList()
			.Where(_ => _.CreateDate >= GetStartOfWeek(DateTime.Today) && _.CreateDate <= GetEndOfWeek(DateTime.Today))
			.Select(_ => _.TotalPrice - _.OriginalPrice).Sum() ?? 0;
		
		dashboardModel.RevenueThisMonth = _dbContext.Transactions.ToList()
			.Where(_ => _.CreateDate >= GetStartOfMonth(DateTime.Today) && _.CreateDate <= GetEndOfMonth(DateTime.Today))
			.Select(_ => _.TotalPrice).Sum() ?? 0;
		dashboardModel.RevenuePreviousMonth = _dbContext.Transactions.ToList()
			.Where(_ => _.CreateDate >= GetStartOfMonth(DateTime.Today).AddMonths(-1) && _.CreateDate <= GetEndOfMonth(DateTime.Today.AddMonths(-1)))
			.Select(_ => _.TotalPrice).Sum() ?? 0;
		dashboardModel.RevenueThisMonthTrend = dashboardModel.RevenueThisMonth - dashboardModel.RevenuePreviousMonth;
		dashboardModel.RevenueThisMonthPromotionLoss = _dbContext.Transactions.ToList()
			.Where(_ => _.CreateDate >= GetStartOfMonth(DateTime.Today) && _.CreateDate <= GetEndOfMonth(DateTime.Today))
			.Select(_ => _.TotalPrice - _.OriginalPrice).Sum() ?? 0;

		dashboardModel.RevenueThisYear = _dbContext.Transactions.ToList().Where(_ => _.CreateDate.Value.Year == DateTime.Today.Year)
			.Select(_ => _.TotalPrice).Sum() ?? 0;
		dashboardModel.RevenuePreviousYear = _dbContext.Transactions.ToList().Where(_ => _.CreateDate.Value.Year == DateTime.Today.Year - 1)
			.Select(_ => _.TotalPrice).Sum() ?? 0;
		dashboardModel.RevenueThisYearTrend = dashboardModel.RevenueThisYear - dashboardModel.RevenuePreviousYear;
		dashboardModel.RevenueThisYearPromotionLoss = _dbContext.Transactions.ToList().Where(_ => _.CreateDate.Value.Year == DateTime.Today.Year)
			.Select(_ => _.TotalPrice - _.OriginalPrice).Sum() ?? 0;

		return StatusCode((int)HttpStatusCode.OK, dashboardModel);
	}
	
	static DateTime GetStartOfWeek(DateTime date)
	{
		var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
		return date.AddDays(-1 * diff).Date;
	}

	static DateTime GetEndOfWeek(DateTime date)
	{
		return GetStartOfWeek(date).AddDays(6);
	}

	static DateTime GetStartOfMonth(DateTime date)
	{
		return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
	}

	static DateTime GetEndOfMonth(DateTime date)
	{
		return GetStartOfMonth(date).AddMonths(1).AddDays(-1);
	}
	
}