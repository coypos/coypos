using System.Net;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CoyposServer.Controllers;

public class TransactionController : ControllerBase
{
    private readonly DatabaseContext _dbContext;

    public TransactionController(DatabaseContext dbContext) =>
        _dbContext = dbContext;
    
	/// <summary>
    /// Returns transactions
    /// </summary>
    /// <param name="transactionFilter">filter to use</param>
    /// <param name="filter">filter type to use (AND/OR/NOR)</param>
    /// <param name="itemsPerPage">number of items per page</param>
    /// <param name="page">page number</param>
    /// <param name="loadImages">should load images?</param>
    [HttpGet]
    [Route("transactions")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(RichResponse<List<Transaction>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public ObjectResult GetTransactions([FromBody] Transaction transactionFilter, string filter = "AND", int itemsPerPage = 50, int page = 1, string language = "", bool loadImages = false)
    {
        try
        {
            var images = loadImages ? _dbContext.Images.ToList() : new List<Image>();
            var transactions = _dbContext.Transactions.ToList();
            var filteredTransactions = transactions.Filter(transactionFilter, filter);
            var pagefiedTransactions = filteredTransactions.Pagefy(itemsPerPage, page, out var totalPages);
            
            for (var i = 0; i < pagefiedTransactions.Count; i++)
            {
                var t = pagefiedTransactions[i];
                if (!t.Product.Image.IsNullOrEmpty() && loadImages)
                    t.Product.Image =
                        images.FirstOrDefault(_ => _.ID.ToString() == t.Product.Image).Img;
                else
                    t.Product.Image = null;
                
                if (!t.Product.Name.IsNullOrEmpty())
                    t.Product.Name = LanguageHelpers.Translate(t.Product.Name, language);
            }
            
            return StatusCode((int)HttpStatusCode.OK, new RichResponse<List<Transaction>>(pagefiedTransactions)
            {
                Page = page,
                TotalPages = totalPages,
                ItemsPerPage = itemsPerPage,
                TotalItems = transactions.Count,
                TotalItemsFiltered = filteredTransactions.Count
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