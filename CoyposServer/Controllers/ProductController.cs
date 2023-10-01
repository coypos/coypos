﻿using System.Net;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CoyposServer.Controllers;

public class ProductController : ControllerBase
{
    private readonly DatabaseContext _dbContext;

    public ProductController(DatabaseContext dbContext) =>
        _dbContext = dbContext;

    /// <summary>
    /// Returns products
    /// </summary>
    /// <param name="productFilter">filter to use</param>
    /// <param name="filter">filter type to use (AND/OR/NOR)</param>
    /// <param name="itemsPerPage">number of items per page</param>
    /// <param name="page">page number</param>
    [HttpGet]
    [Route("products")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(List<Product>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public ObjectResult GetProductsWithFilter([FromBody] Product productFilter, string filter = "AND", int itemsPerPage = 50, int page = 1)
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
    /// Returns product categories
    /// </summary>
    /// <param name="categoryFilter">filter to use</param>
    /// <param name="filter">filter type to use (AND/OR/NOR)</param>
    /// <param name="itemsPerPage">number of items per page</param>
    /// <param name="page">page number</param>
    [HttpGet]
    [Route("categories")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(List<Product>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public ObjectResult GetCategories([FromBody] Category categoryFilter, string filter = "AND", int itemsPerPage = 50, int page = 1)
    {
        try
        {
            var categories = _dbContext.Categories.ToList().Filter(categoryFilter, filter).Pagefy(itemsPerPage, page);
            return StatusCode((int)HttpStatusCode.OK, categories);
        }
        catch (Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
        }
    }

    /// <summary>
    /// Updates a product
    /// </summary>
    /// <param name="product">new product data</param>
    /// <param name="id">product ID</param>
    [HttpPut]
    [Route("product/{id}")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ObjectResult> Put([FromBody] Product product, int id)
    {
        Product? productFromDb;
        try
        {
            productFromDb = _dbContext.Products.FirstOrDefault(p => p.ID == id);
            if (productFromDb is null)
                throw new Exception("No known product with such ID");
            product.ID = id;
            product.CreateDate = productFromDb.CreateDate;
            product = ObjectHelpers.CopyNonNullValues(productFromDb, product);
            //_dbContext.AttachVirtualProperties(productFromDb);
            _dbContext.Entry(productFromDb).CurrentValues.SetValues(product);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
            var response = e.InnerException is not null
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.InternalServerError;
            return StatusCode((int)response, new ProblemDetails() { Title = message });
        }

        return StatusCode((int)HttpStatusCode.OK, productFromDb);
    }

    /// <summary>
    /// Deletes a product
    /// </summary>
    /// <param name="id">product ID</param>
    [HttpDelete]
    [Route("product/{id}")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ObjectResult> Delete(int id)
    {
        try
        {
            var productFromDb = _dbContext.Products.FirstOrDefault(p => p.ID == id);
            if (productFromDb is null)
                throw new Exception("No known product with such ID");
            _dbContext.AttachVirtualProperties(id);
            _dbContext.Remove(productFromDb);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
            var response = e.InnerException is not null
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.InternalServerError;
            return StatusCode((int)response, new ProblemDetails() { Title = message });
        }

        return StatusCode((int)HttpStatusCode.OK, $"Product {id} deleted");
    }

    /// <summary>
    /// Creates a product entry
    /// </summary>
    /// <param name="product">new product data</param>
    /// <returns></returns>
    [HttpPost]
    [Route("product")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ObjectResult> Post([FromBody] Product product)
    {
        EntityEntry<Product> result;

        try
        {
            // overwritten values:
            product.CreateDate = DateTime.Now;
            _dbContext.AttachVirtualProperties(product);
            // </>

            result = _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
            var response = e.InnerException is not null
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.InternalServerError;
            return StatusCode((int)response, new ProblemDetails() { Title = message });
        }

        return StatusCode((int)HttpStatusCode.OK, result.Entity);
    }
}