using CoyposServer.Models.Sql;

namespace CoyposServer.Utils.Extensions;

public static class PromotionsHelper
{
	public static void GetBestPromotion(List<Promotion> promotions, Product product, out Promotion? bestPromotion,
		out decimal? discountedPrice)
	{
		bestPromotion = null;
		discountedPrice = null;
		
		var promotionsWithThisItem = promotions.Where(_ =>
		{
			var id = product.ID.ToString();
			return _.Ids is not null 
			       && _.Ids.Split(',').Any(v => v == id);
		}).ToList();
		var activePromotions =
			promotionsWithThisItem.Where(_ => _.StartDate < DateTime.Now && _.EndDate > DateTime.Now).ToList();
		bestPromotion = activePromotions.MinBy(_ => _.DiscountPercentage);
		if (bestPromotion is null)
			return;
#pragma warning disable CS8629
		discountedPrice = Math.Round((decimal)(product.Price - product.Price * bestPromotion.DiscountPercentage / 100), 2);
#pragma warning restore CS8629
	}
}