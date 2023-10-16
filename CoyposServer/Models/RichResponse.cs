using Newtonsoft.Json;

namespace CoyposServer.Models;

[JsonObject]
public class RichResponse<T>
{
    public RichResponse(T responseModel)
    {
        Response = responseModel;
    }

    [JsonProperty("response")] public T Response;
    [JsonProperty("page")] public int? Page;
    [JsonProperty("totalPages")] public int? TotalPages;
    [JsonProperty("itemsPerPage")] public int? ItemsPerPage;
    [JsonProperty("totalItems")] public int? TotalItems;
    [JsonProperty("totalItemsFiltered")] public int? TotalItemsFiltered;
}