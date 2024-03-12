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
	/// List of Products in the basket
	/// </summary>
	[JsonProperty("basket_items")] public List<ReceiptPostItemModel> BasketItems;
	
	/// <summary>
	/// Transaction ID (if applicable)
	/// </summary>
	[JsonProperty("transaction_id")] public string? TransactionId;

	/// <summary>
	/// Payment method ID (from the database)
	/// </summary>
	[JsonProperty("payment_method")] public int PaymentMethodId;
}