using Newtonsoft.Json;

namespace CoyposServer.Models;

[JsonObject]
public class ReceiptPostItemModel
{
	[JsonProperty("product_id")] public int ProductId;
	[JsonProperty("quantity")] public decimal Quantity;
}