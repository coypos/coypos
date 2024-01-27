using System.Net;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CoyposServer.Controllers;

public class ReceiptController : ControllerBase
{
	private readonly DatabaseContext _dbContext;

	public ReceiptController(DatabaseContext dbContext) =>
		_dbContext = dbContext;
	
	/// <summary>
	/// Returns the due amount for a specified product ID list.
	/// </summary>
	/// <param name="basketProducts">product IDs</param>
	[HttpGet]
	[Route("receipt_calculate")]
	[ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public ObjectResult ReceiptCalculate([FromBody] List<ReceiptPostItemModel> basketProducts)
	{
		try
		{
			var products = _dbContext.Products.Where(_ => basketProducts.Any(_b => _b.ProductId == _.ID)).ToList();
			
			var promotions = _dbContext.Promotions.ToList();
			for (var i = 0; i < products.Count; i++)
			{
				PromotionsHelper.GetBestPromotion(promotions, products[i], out var bestPromotion, out var discountedPrice);
				if (bestPromotion is null)
					continue;
				products[i].AppliedPromotion = bestPromotion;
				products[i].DiscountedPrice = discountedPrice;
			}

			decimal totalPrice = basketProducts.Sum(basketProduct =>
				((decimal)products.First(_ => _.ID == basketProduct.ProductId).DiscountedPrice * basketProduct.Quantity));

			// one more time, to prevent the floating-point error:
			totalPrice = Math.Round(totalPrice, 2); 

			return StatusCode((int)HttpStatusCode.OK, totalPrice);
		}
		catch (Exception e)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
		}
	}

	/// <summary>
	/// Creates a new receipt and transaction entries
	/// </summary>
	/// <param name="receiptPost">receipt data</param>
	/// <returns>a finalised receipt</returns>
	[HttpPost]
	[Route("receipt")]
	[ProducesResponseType(typeof(Receipt), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public async Task<ObjectResult> ReceiptPost([FromBody] ReceiptPostModel receiptPost)
	{
		try
		{
			if (!new[] { "PAID_CASH", "PAID_CARD", "PAID_BLIK", "PAID_EXTERNAL" }.Contains(receiptPost.Action))
				throw new Exception("Not supported Action type");

			if (receiptPost.Action != "PAID_CASH" && receiptPost.TransactionId is null)
				throw new Exception("Non-cash transactions require a transaction ID");

			if (receiptPost.BasketItems.Count == 0)
				throw new Exception("The basket cannot be empty");

			#region [Receipt]
			
				var receipt = new Receipt();
				receipt.Action = receiptPost.Action;
				if (receiptPost.TransactionId is not null)
					receipt.TransactionId = receiptPost.TransactionId;
				receipt.CreateDate = DateTime.Now;
				receipt.UpdateDate = DateTime.Now;

				if (receiptPost.UserId is not null)
				{
					var found = _dbContext.Users.FirstOrDefault(_ => _.ID == receiptPost.UserId);
					if (found is null)
						throw new Exception($"User with ID of {receiptPost.UserId} does not exist");
					receipt.User = found;
				}

				var promotions = _dbContext.Promotions.ToList();
				var outReceipt = _dbContext.Receipts.Add(receipt);
			
			#endregion

			#region [Adding transactions]
			
				var transactions = new List<Transaction>();
				foreach (var item in receiptPost.BasketItems)
				{
					var product = _dbContext.Products.First(_ => item.ProductId == _.ID);
					var quantity = item.Quantity;
					
					PromotionsHelper.GetBestPromotion(promotions, product, out var _, out var discountedPrice);
					if (discountedPrice is null)
						discountedPrice = product.Price;
					transactions.Add(new Transaction()
					{
						Product = product,
						Quantity = quantity,
						TotalPrice = Math.Round((decimal)(quantity * discountedPrice), 2),
						OriginalPrice = Math.Round((decimal)(quantity * product.Price), 2),
						Receipt = receipt,
						CreateDate = DateTime.Now,
						UpdateDate = DateTime.Now
					});
				}
				
				foreach (var transaction in transactions)
					_dbContext.Transactions.Add(transaction);
			
			#endregion
			

			await _dbContext.SaveChangesAsync();

			// add transactions to the non-mapped transaction list...
			// ...but REMOVE all references to the target entity, to prevent
			// self referencing loops. Do it AFTER saving changes.
			var outEntity = outReceipt.Entity;
			foreach (var transaction in transactions)
				transaction.Receipt = null;
			outEntity.Transactions = transactions;

			return StatusCode((int)HttpStatusCode.OK, outReceipt.Entity);
		}
		catch (Exception e)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
		}
	}
	
	/// <summary>
	/// Refunds the receipt (one-way ticket!)
	/// </summary>
	/// <param name="id">Receipt ID</param>
	[HttpPost]
	[Route("receipt/refund/{id:int}")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public async Task<ObjectResult> Refund(int id)
	{
		try
		{
			var found = _dbContext.Receipts.FirstOrDefault(_ => _.ID == id);
			if (found is null)
				throw new Exception($"Receipt with ID of {id} does not exist");

			if (found.Action.Contains("REFUNDED"))
				throw new Exception($"This receipt is already refunded");

			found.Action = found.Action.Replace("PAID_", "REFUNDED_");

			await _dbContext.SaveChangesAsync();
			
			return StatusCode((int)HttpStatusCode.OK, "Refunded!");
		}
		catch (Exception e)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
		}
	}
	
	/// <summary>
    /// Returns receipts
    /// </summary>
    /// <param name="receiptFilter">filter to use</param>
    /// <param name="filter">filter type to use (AND/OR/NOR)</param>
    /// <param name="itemsPerPage">number of items per page</param>
    /// <param name="page">page number</param>
    /// <param name="loadImages">should load images?</param>
    [HttpGet]
    [Route("receipts")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(RichResponse<List<Receipt>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public ObjectResult GetReceipts([FromBody] Receipt receiptFilter, string filter = "AND", int itemsPerPage = 50, int page = 1, string language = "", bool loadImages = false)
    {
        try
        {
            var images = loadImages ? _dbContext.Images.ToList() : new List<Image>();
            var transactions = _dbContext.Receipts.ToList();
            var filteredReceipts = transactions.Filter(receiptFilter, filter);
            var pagefiedReceipts = filteredReceipts.Pagefy(itemsPerPage, page, out var totalPages);

            for (var i = 0; i < pagefiedReceipts.Count; i++)
            {
	            var r = pagefiedReceipts[i];
	            r.Transactions = new();
	            r.Transactions = _dbContext.Transactions.Where(_ => _.Receipt.ID == r.ID).ToList();
	            for (var i1 = 0; i1 < r.Transactions.Count; i1++)
	            {
		            var t = r.Transactions[i1];
		            t.Receipt = null; // important! prevents self-referencing in json parsing
		            if (!t.Product.Image.IsNullOrEmpty() && loadImages)
			            t.Product.Image =
				            images.FirstOrDefault(_ => _.ID.ToString() == t.Product.Image).Img;
		            else
			            t.Product.Image = null;
                
		            if (!t.Product.Name.IsNullOrEmpty())
			            t.Product.Name = LanguageHelpers.Translate(t.Product.Name, language);
	            }
            }
            
            return StatusCode((int)HttpStatusCode.OK, new RichResponse<List<Receipt>>(pagefiedReceipts)
            {
                Page = page,
                TotalPages = totalPages,
                ItemsPerPage = itemsPerPage,
                TotalItems = transactions.Count,
                TotalItemsFiltered = filteredReceipts.Count
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