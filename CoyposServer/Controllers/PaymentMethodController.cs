using System.Net;
using System.Text;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace CoyposServer.Controllers;

public class PaymentMethodController : ControllerBase
{
	private readonly DatabaseContext _dbContext;

	public PaymentMethodController(DatabaseContext dbContext) => _dbContext = dbContext;

	/// <summary>
	/// Returns payment methods
	/// </summary>
	/// <param name="paymentMethodFilter">filter to use</param>
	/// <param name="filter">filter type to use (AND/OR/NOR)</param>
	/// <param name="itemsPerPage">number of items per page</param>
	/// <param name="page">page number</param>
	/// <param name="language">language to use</param>
	/// <param name="loadImages">should we load images?</param>
	/// <returns></returns>
	[HttpGet]
	[Route("payment_methods")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(RichResponse<List<PaymentMethod>>), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public ObjectResult GetPaymentMethods([FromBody] PaymentMethod paymentMethodFilter, string filter = "AND", int itemsPerPage = 50, int page = 1, string language = "", bool loadImages = false)
	{
		try
		{
			var images = loadImages ? _dbContext.Images.ToList() : new List<Image>();
			var paymentMethods = _dbContext.PaymentMethods.ToList();
			var filteredPaymentMethods = paymentMethods.Filter(paymentMethodFilter, filter);
			var pagefiedPaymentMethods = filteredPaymentMethods.Pagefy(itemsPerPage, page, out var totalPages);
            
			for (var i = 0; i < pagefiedPaymentMethods.Count; i++)
			{
				var t = pagefiedPaymentMethods[i];
				if (!t.Image.IsNullOrEmpty() && loadImages)
					t.Image =
						images.FirstOrDefault(_ => _.ID.ToString() == t.Image).Img;
				else
					t.Image = null;
                
				if (!t.Name.IsNullOrEmpty())
					t.Name = LanguageHelpers.Translate(t.Name, language);
			}
            
			return StatusCode((int)HttpStatusCode.OK, new RichResponse<List<PaymentMethod>>(pagefiedPaymentMethods)
			{
				Page = page,
				TotalPages = totalPages,
				ItemsPerPage = itemsPerPage,
				TotalItems = paymentMethods.Count,
				TotalItemsFiltered = filteredPaymentMethods.Count
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
	/// Updates a payment method
	/// </summary>
	/// <param name="paymentMethod">new payment method data</param>
	/// <param name="id">payment method ID</param>
	[HttpPut]
	[Route("payment_method/{id}")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(PaymentMethod), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public async Task<ObjectResult> Put([FromBody] PaymentMethod paymentMethod, int id)
	{
		PaymentMethod? paymentMethodFromDb;
		try
		{
			if (!paymentMethod.Image.IsNullOrEmpty())
			{
				var imageSize = Encoding.UTF8.GetBytes(paymentMethod.Image).Length;
				if (imageSize > 25*1024)
					throw new Exception(
						$"Provided image is too large ({imageSize} bytes). Maximum image size: {25*1024} bytes.");
				var imageResult = _dbContext.Images.Add(new Image() { Img = paymentMethod.Image });
				await _dbContext.ForceSaveChangesAsync("Images");
				paymentMethod.Image = imageResult.Entity.ID.ToString();
			}
            
			paymentMethodFromDb = _dbContext.PaymentMethods.FirstOrDefault(p => p.ID == id);
			if (paymentMethodFromDb is null)
				throw new Exception("No known payment method with such ID");
			paymentMethod.ID = id;
			paymentMethod = ObjectHelpers.CopyNonNullValues(paymentMethodFromDb, paymentMethod);
			//_dbContext.AttachVirtualProperties(productFromDb);
			_dbContext.Entry(paymentMethodFromDb).CurrentValues.SetValues(paymentMethod);
            
			await _dbContext.ForceSaveChangesAsync("PaymentMethods");
		}
		catch (Exception e)
		{
			var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
			var response = e.InnerException is not null
				? HttpStatusCode.BadRequest
				: HttpStatusCode.InternalServerError;
			return StatusCode((int)response, new ProblemDetails() { Title = message });
		}

		return StatusCode((int)HttpStatusCode.OK, paymentMethodFromDb);
	}
	
	
	/// <summary>
	/// Deletes a payment method
	/// </summary>
	/// <param name="id">payment method ID</param>
	[HttpDelete]
	[Route("payment_method/{id}")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public async Task<ObjectResult> Delete(int id)
	{
		try
		{
			var paymentMethodFromDb = _dbContext.PaymentMethods.FirstOrDefault(p => p.ID == id);
			if (paymentMethodFromDb is null)
				throw new Exception("No known payment method with such ID");
			_dbContext.AttachVirtualProperties(id);
			_dbContext.Remove(paymentMethodFromDb);
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

		return StatusCode((int)HttpStatusCode.OK, $"Payment method {id} deleted");
	}
	
	/// <summary>
	/// Creates a payment method entry
	/// </summary>
	/// <param name="paymentMethod">new payment method data</param>
	[HttpPost]
	[Route("payment_method")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(PaymentMethod), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public async Task<ObjectResult> Post([FromBody] PaymentMethod paymentMethod)
	{
		EntityEntry<PaymentMethod> result;

		try
		{
			if (!paymentMethod.Image.IsNullOrEmpty())
			{
				var imageSize = Encoding.UTF8.GetBytes(paymentMethod.Image).Length;
				if (imageSize > 25*1024)
					throw new Exception(
						$"Provided image is too large ({imageSize} bytes). Maximum image size: {25*1024} bytes.");
				var imageResult = _dbContext.Images.Add(new Image() { Img = paymentMethod.Image });
				await _dbContext.ForceSaveChangesAsync("Images");
				paymentMethod.Image = imageResult.Entity.ID.ToString();
			}

			result = _dbContext.PaymentMethods.Add(paymentMethod);
            
			await _dbContext.ForceSaveChangesAsync("PaymentMethods");
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