using Newtonsoft.Json;

namespace CoyposServer.Models;

[JsonObject]
public class EmployeeValidationModel
{
	/// <summary>
	/// Employee Card ID
	/// </summary>
	[JsonProperty("card_id")] public string CardId;

	/// <summary>
	/// Employee PIN
	/// </summary>
	[JsonProperty("pin")] public string PIN;
}