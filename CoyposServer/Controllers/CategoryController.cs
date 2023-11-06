using System.Net;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NUnit.Framework;

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

    /// <summary>
    /// Creates a category
    /// </summary>
    /// <param name="category">category data</param>
    [HttpPost]
    [Route("category")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Category), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ObjectResult> CreateCategory([FromBody] Category category)
    {
        EntityEntry<Category> result;

        try
        {
            // overwritten values:
            category.UpdateDate = DateTime.Now;
            category.CreateDate = DateTime.Now;
            _dbContext.AttachVirtualProperties(category);
            // </>
            

            result = _dbContext.Categories.Add(category);
            await _dbContext.ForceSaveChangesAsync("Categories");
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

    /// <summary>
    /// Updates a category
    /// </summary>
    /// <param name="category">category data</param>
    /// <param name="id">category ID</param>
    [HttpPut]
    [Route("category/{id:int}")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Category), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ObjectResult> UpdateCategory([FromBody] Category category, int id)
    {
        Category? categoryFromDb;
        try
        {
            categoryFromDb = _dbContext.Categories.FirstOrDefault(p => p.ID == id);
            if (categoryFromDb is null)
                throw new Exception("No known category with such ID");
            category.ID = id;
            category.CreateDate = category.CreateDate;
            category.UpdateDate = DateTime.Now;
            category = ObjectHelpers.CopyNonNullValues(categoryFromDb, category);
            //_dbContext.AttachVirtualProperties(productFromDb);
            _dbContext.Entry(categoryFromDb).CurrentValues.SetValues(category);
            
            await _dbContext.ForceSaveChangesAsync("Categories");
        }
        catch (Exception e)
        {
            var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
            var response = e.InnerException is not null
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.InternalServerError;
            return StatusCode((int)response, new ProblemDetails() { Title = message });
        }
        
        return StatusCode((int)HttpStatusCode.OK, categoryFromDb);
    }
    
    /// <summary>
    /// Deletes a category
    /// </summary>
    /// <param name="id">category ID</param>
    [HttpDelete]
    [Route("category/{id:int}")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Category), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ObjectResult> DeleteCategory(int id)
    {
        try
        {
            var categoryFromDb = _dbContext.Categories.FirstOrDefault(p => p.ID == id);
            if (categoryFromDb is null)
                throw new Exception("No known category with such ID");
            _dbContext.Remove(categoryFromDb);
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

        return StatusCode((int)HttpStatusCode.OK, $"Category {id} deleted");
    }
}