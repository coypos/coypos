using System.Net;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CoyposServer.Controllers;

public class ProductController : ControllerBase
{
    private readonly DatabaseContext _dbContext;

    public ProductController(DatabaseContext dbContext) =>
        _dbContext = dbContext;

    /// <summary>
    /// Returns all products
    /// </summary>
    [HttpGet]
    [Route("products")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(List<Product>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public ObjectResult GetProducts([FromBody] Product productFilter, string filter = "AND", int itemsPerPage = 50, int page = 1)
    {
        try
        {
            if (page <= 0)
                page = 1;
            
            var products = _dbContext.Products.ToList().Filter(productFilter, filter).Pagefy(itemsPerPage, page);
            return StatusCode((int)HttpStatusCode.OK, products);
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

    /// <summary>
    /// Returns all product categories that are present in the database
    /// </summary>
    [HttpGet]
    [Route("categories")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(List<Product>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public ObjectResult GetCategories()
    {
        try
        {
            var products = _dbContext.Products.ToList();
            var categories = products.Select(product => product.Category).ToList().Distinct();
            return StatusCode((int)HttpStatusCode.OK, categories);
        }
        catch (Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
        }
    }
}