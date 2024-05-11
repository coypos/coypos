using CoyposServer.Models.Sql;
using Newtonsoft.Json;

namespace CoyposServer.Models;

/// <summary>
/// Data for the Dashboard
/// </summary>
[JsonObject]
public class DashboardModel
{
	/// <summary>
	/// Server info
	/// </summary>
	[JsonProperty("coypos")] public CoyposModel CoyPos;
	
	/// <summary>
	/// Total number of employees
	/// </summary>
	[JsonProperty("employee_count")] public int EmployeeCount;
	
	/// <summary>
	/// Total number of users
	/// </summary>
	[JsonProperty("user_count")] public int UserCount;
	
	/// <summary>
	/// User with most points
	/// </summary>
	[JsonProperty("user_most_points")] public User? UserWithMostPoints;
	
	/// <summary>
	/// Total number of products
	/// </summary>
	[JsonProperty("product_count")] public int ProductCount;
	
	/// <summary>
	/// 5 most popular products (all time)
	/// </summary>
	[JsonProperty("products_most_popular")] public List<StatEntry<Product>> MostPopularProducts = new();
	
	/// <summary>
	/// Total number of categories
	/// </summary>
	[JsonProperty("category_count")] public int CategoriesCount;
	
	/// <summary>
	/// Total number of receipts
	/// </summary>
	[JsonProperty("receipt_count")] public int NumberOfReceipts;
	
	/// <summary>
	/// Most recent receipts
	/// </summary>
	[JsonProperty("receipts_most_recent")] public List<Receipt> ReceiptsCount = new();

	/// <summary>
	/// Total number of payment methods
	/// </summary>
	[JsonProperty("payment_method_count")] public int PaymentMethodCount;
	
	/// <summary>
	/// 5 most popular payment methods (all time)
	/// </summary>
	[JsonProperty("payment_methods_most_popular")] public List<StatEntry<PaymentMethod>> MostPopularPaymentMethods = new();

	/// <summary>
	/// Total number of promotions
	/// </summary>
	[JsonProperty("promotion_count")] public int PromotionCount;
	
	/// <summary>
	/// Number of active promotions
	/// </summary>
	[JsonProperty("promotions_active")] public List<Promotion> ActivePromotions;

	/// <summary>
	/// Revenue from today
	/// </summary>
	[JsonProperty("revenue_today")] public decimal RevenueToday;
	
	/// <summary>
	/// Revenue from yesterday
	/// </summary>
	[JsonProperty("revenue_yesterday")] public decimal RevenueYesterday;
	
	/// <summary>
	/// Gain/loss compared to yesterday
	/// </summary>
	[JsonProperty("revenue_today_trend")] public decimal RevenueTodayTrend;
	
	/// <summary>
	/// Loss on promotions today
	/// </summary>
	[JsonProperty("revenue_today_promotion_loss")] public decimal RevenueTodayPromotionLoss;
	
	/// <summary>
	/// Revenue from this week
	/// </summary>
	[JsonProperty("revenue_this_week")] public decimal RevenueThisWeek;
	
	/// <summary>
	/// Revenue from previous week
	/// </summary>
	[JsonProperty("revenue_previous_week")] public decimal RevenuePreviousWeek;
	
	/// <summary>
	/// Gain/loss compared to last week
	/// </summary>
	[JsonProperty("revenue_this_week_trend")] public decimal RevenueThisWeekTrend;
	
	/// <summary>
	/// Loss on promotions this week
	/// </summary>
	[JsonProperty("revenue_this_week_promotion_loss")] public decimal RevenueThisWeekPromotionLoss;
	
	/// <summary>
	/// Revenue this month
	/// </summary>
	[JsonProperty("revenue_this_month")] public decimal RevenueThisMonth;
	
	/// <summary>
	/// Revenue previous month
	/// </summary>
	[JsonProperty("revenue_previous_month")] public decimal RevenuePreviousMonth;
	
	/// <summary>
	/// Gain/loss compared to last month
	/// </summary>
	[JsonProperty("revenue_this_month_trend")] public decimal RevenueThisMonthTrend;
	
	/// <summary>
	/// Loss on promotions this month
	/// </summary>
	[JsonProperty("revenue_this_month_promotion_loss")] public decimal RevenueThisMonthPromotionLoss;

	/// <summary>
	/// Revenue this year
	/// </summary>
	[JsonProperty("revenue_this_year")] public decimal RevenueThisYear;
	
	/// <summary>
	/// Revenue previous year
	/// </summary>
	[JsonProperty("revenue_previous_year")] public decimal RevenuePreviousYear;
	
	/// <summary>
	/// Gain/loss compared to last year
	/// </summary>
	[JsonProperty("revenue_this_year_trend")] public decimal RevenueThisYearTrend;
	
	/// <summary>
	/// Loss on promotions this year
	/// </summary>
	[JsonProperty("revenue_this_year_promotion_loss")] public decimal RevenueThisYearPromotionLoss;
}

[JsonObject]
public class StatEntry<T>
{
	/// <summary>
	/// Item in question
	/// </summary>
	[JsonProperty("item")] public T Item;
	
	/// <summary>
	/// Some value, most usually quantity
	/// </summary>
	[JsonProperty("value")] public int Value;
}