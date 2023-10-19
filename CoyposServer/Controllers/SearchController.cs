using System.Globalization;
using System.Net;
using System.Text;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CoyposServer.Controllers;

public class SearchController : ControllerBase
{
    private readonly DatabaseContext _dbContext;
    
    public SearchController(DatabaseContext dbContext) =>
        _dbContext = dbContext;
    
    /// <summary>
    /// Searches for a product
    /// </summary>
    /// <param name="query">search query (not case sensitive, replaces diacritics with standard unicode)</param>
    /// <param name="categoryId">additional category filter. -1: disabled; 0: return all nulls; anything else: category ID</param>
    /// <param name="itemsPerPage">number of items per page</param>
    /// <param name="page">page number</param>
    [HttpGet]
    [Route("search/{query}")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(RichResponse<List<Product>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public ObjectResult SearchProduct(string query = "", int categoryId = -1, int itemsPerPage = 50, int page = 1)
    {
        try
        {
            var products = _dbContext.Products.ToList();
            products = categoryId switch
            {
                -1 => products,
                0 => products.Where(_ => _.Category is null).ToList(),
                _ => products.Where(_ => _.Category is not null && _.Category.ID == categoryId).ToList()
            };
            var filteredProducts = query == "" ? products : products = products.Where(_ =>
            {
                var name = new string(_.Name.Normalize(NormalizationForm.FormD)
                    .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    .ToArray());
                var q = new string(query.Normalize(NormalizationForm.FormD)
                    .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    .ToArray());
                return name.ToLower().Contains(q.ToLower());
            }).ToList();
            var pagefiedProducts = filteredProducts.Pagefy(itemsPerPage, page, out var totalPages);

            return StatusCode((int)HttpStatusCode.OK, new RichResponse<List<Product>>(pagefiedProducts)
            {
                Page = page,
                TotalPages = totalPages,
                ItemsPerPage = itemsPerPage,
                TotalItems = products.Count,
                TotalItemsFiltered = filteredProducts.Count
            });
        }
        catch (Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
        }
    }
}