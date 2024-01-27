using Newtonsoft.Json;

namespace CoyposServer.Models;

[JsonObject]
public class ReceiptPostModel
{
	/// <summary>
	/// User ID, if the transaction included a loyalty card. Can be null
	/// </summary>
	[JsonProperty("user_id")] public int? UserId;
	
	/// <summary>
	/// Receipt action. For POST can only be PAID_CASH, PAID_CARD, PAID_BLIK or PAID_EXTERNAL.
	/// For PUT: the above, plus REFUNDED_CASH, REFUNDED_CARD, REFUNDED_BLIK or REFUNDED_EXTERNAL
	/// </summary>
	[JsonProperty("action")] public string Action;
	
	/// <summary>
	/// List of Products in the basket
	/// </summary>
	[JsonProperty("basket_items")] public List<ReceiptPostItemModel> BasketItems;
	
	/// <summary>
	/// Transaction ID (only for PAID_CARD)
	/// </summary>
	[JsonProperty("transaction_id")] public string? TransactionId;
}