using System.Net;
using CoyposServer.Models;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CoyposServer.Controllers;

public class PromotionController : ControllerBase
{
	private readonly DatabaseContext _dbContext;

	public PromotionController(DatabaseContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	/// <summary>
	/// Returns promotions
	/// </summary>
	/// <param name="promotionFilter">filter to use</param>
	/// <param name="filter">filter type to use (AND/OR/NOR)</param>
	/// <param name="itemsPerPage">number of items per page</param>
	/// <param name="page">page number</param>
	[HttpGet]
	[Route("promotions")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(RichResponse<List<Promotion>>), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public ObjectResult GetPromotionsWithFilter([FromBody] Promotion promotionFilter, string filter = "AND", int itemsPerPage = 50, int page = 1)
	{
		try
		{
			var promotions = _dbContext.Promotions.ToList();
			var filteredPromotions = promotions.Filter(promotionFilter, filter);
			var pagefiedPromotions = filteredPromotions.Pagefy(itemsPerPage, page, out var totalPages);

			return StatusCode((int)HttpStatusCode.OK, new RichResponse<List<Promotion>>(pagefiedPromotions)
			{
				Page = page,
				TotalPages = totalPages,
				ItemsPerPage = itemsPerPage,
				TotalItems = promotions.Count,
				TotalItemsFiltered = filteredPromotions.Count
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
    /// Updates the promotion
    /// </summary>
    /// <param name="promotion">new promotion data</param>
    /// <param name="id">promotion ID</param>
    [HttpPut]
    [Route("promotion/{id}")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Promotion), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ObjectResult> Put([FromBody] Promotion promotion, int id)
    {
        Promotion? promotionFromDb;
        try
        {
	        promotionFromDb = _dbContext.Promotions.FirstOrDefault(p => p.ID == id);
            if (promotionFromDb is null)
                throw new Exception("No known promotion with such ID");

            promotion.ID = id;
            promotion.CreateDate = promotionFromDb.CreateDate;
            promotion.UpdateDate = DateTime.Now;
            promotion = ObjectHelpers.CopyNonNullValues(promotionFromDb, promotion);
            if (promotion.DiscountPercentage is < 1 or > 99)
	            throw new Exception("Promotion discount needs to be in range of 1-99 (incl.)");
            if (promotion.StartDate == promotion.EndDate)
	            throw new Exception("Start and End date cannot be the same");
            if (promotion.StartDate > promotion.EndDate)
	            throw new Exception("Promotion cannot start after it ends");
            _dbContext.Entry(promotionFromDb).CurrentValues.SetValues(promotion);
            
            await _dbContext.ForceSaveChangesAsync("Promotions");
        }
        catch (Exception e)
        {
            var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
            var response = e.InnerException is not null
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.InternalServerError;
            return StatusCode((int)response, new ProblemDetails() { Title = message });
        }
        
        return StatusCode((int)HttpStatusCode.OK, promotionFromDb);
    }
	
	/// <summary>
	/// Deletes the promotion
	/// </summary>
	/// <param name="id">promotion ID</param>
	[HttpDelete]
	[Route("promotion/{id}")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public async Task<ObjectResult> Delete(int id)
	{
		try
		{
			var promotionFromDb = _dbContext.Promotions.FirstOrDefault(p => p.ID == id);
			if (promotionFromDb is null)
				throw new Exception("No known promotion with such ID");
			_dbContext.Remove(promotionFromDb);
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

		return StatusCode((int)HttpStatusCode.OK, $"Promotion {id} deleted");
	}
	
	/// <summary>
    /// Creates a promotion
    /// </summary>
    /// <param name="promotion">new promotion data</param>
    /// <returns></returns>
    [HttpPost]
    [Route("promotion")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Promotion), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ObjectResult> Post([FromBody] Promotion promotion)
    {
        EntityEntry<Promotion> result;

        try
        {
	        // overwritten values:
	        promotion.ID = null;
	        promotion.CreateDate = DateTime.Now;
	        promotion.UpdateDate = DateTime.Now;
	        if (promotion.DiscountPercentage is < 1 or > 99)
		        throw new Exception("Promotion discount needs to be in range of 1-99 (incl.)");
	        if (promotion.StartDate == promotion.EndDate)
		        throw new Exception("Start and End date cannot be the same");
	        if (promotion.StartDate > promotion.EndDate)
		        throw new Exception("Promotion cannot start after it ends");
            // </>

            result = _dbContext.Promotions.Add(promotion);
            
            await _dbContext.ForceSaveChangesAsync("Promotions");
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