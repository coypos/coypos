using System.Net;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CoyposServer.Controllers;

public class CategoryController : ControllerBase
{
    private readonly DatabaseContext _dbContext;
    
    public CategoryController(DatabaseContext dbContext) =>
        _dbContext = dbContext;
    
    /// <summary>
    /// Returns product categories
    /// </summary>
    /// <param name="categoryFilter">filter to use</param>
    /// <param name="filter">filter type to use (AND/OR/NOR)</param>
    /// <param name="itemsPerPage">number of items per page</param>
    /// <param name="page">page number</param>
    [HttpGet]
    [Route("categories")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(RichResponse<List<Category>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public ObjectResult GetCategories([FromBody] Category categoryFilter, string filter = "AND", int itemsPerPage = 50, int page = 1)
    {
        try
        {
            var categories = _dbContext.Categories.ToList();
            var filteredCategories = categories.Filter(categoryFilter, filter);
            var pagefiedCategories = filteredCategories.Pagefy(itemsPerPage, page, out var totalPages);
            
            return StatusCode((int)HttpStatusCode.OK, new RichResponse<List<Category>>(pagefiedCategories)
            {
                Page = page,
                TotalPages = totalPages,
                ItemsPerPage = itemsPerPage,
                TotalItems = categories.Count,
                TotalItemsFiltered = filteredCategories.Count
            });
        }
        catch (ArgumentOutOfRangeException e)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, new ProblemDetails() { Title = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
        }
    }
}