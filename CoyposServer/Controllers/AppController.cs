using System.Diagnostics;
using System.Net;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Attributes;
using CoyposServer.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CoyposServer.Controllers;

public class AppController : ControllerBase
{
	private DatabaseContext _dbContext;

	public AppController(DatabaseContext dbContext)
	{
		_dbContext = dbContext;
	}

	private List<Tuple<int /*user ID*/, int /* attempts */>> _failedAttemptsCache = new();
	private List<Tuple<int /*user ID*/, Stopwatch /* timeout */>> _timedOutUsers = new();

	/// <summary>
	/// User login
	/// </summary>
	/// <param name="email">User email</param>
	/// <param name="password">User password (plain)</param>
	/// <returns>login token (OK) if successful. Otherwise, an error code (LOCKED)</returns>
	[HttpPost]
	[Route("app/login")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Locked)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	[NoApiKey]
	public async Task<ObjectResult> Login(string email, string password)
	{
		try
		{
			if (email.IsNullOrEmpty() || password.IsNullOrEmpty())
				return StatusCode((int)HttpStatusCode.Locked, "1");

			var user = _dbContext.Users.FirstOrDefault(_ => _.Email == email);
			if (user is null)
				return StatusCode((int)HttpStatusCode.Locked, "2");

			if (_timedOutUsers.Any(_ => _.Item1 == user.ID && _.Item2.Elapsed.TotalMinutes < 15))
				return StatusCode((int)HttpStatusCode.Locked, "3");

			if (BCrypt.Net.BCrypt.Verify(password, user.Password))
			{
				var token = "";
				// if token is still valid, renew and return it
				if (!user.LoginToken.IsNullOrEmpty() && user.LoginTokenValidDate > DateTime.Now)
				{
					token = user.LoginToken;
				}
				// otherwise, create a new one
				else
				{
					token = Guid.NewGuid().ToString();
				}

				user.LoginTokenValidDate = DateTime.Now + TimeSpan.FromDays(365);
				user.LoginToken = token;
				await _dbContext.ForceSaveChangesAsync("Users");
				return StatusCode((int)HttpStatusCode.OK, token);
			}
			else
			{
				return StatusCode((int)HttpStatusCode.Locked, "2");
			}
		}
		catch (Exception e)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
		}
	}
	
	/// <summary>
	/// User logout
	/// </summary>
	/// <param name="token">Token</param>
	/// <returns>nothing if successful (OK). Otherwise, an error code (LOCKED)</returns>
	[HttpPost]
	[Route("app/logout")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Locked)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	[NoApiKey]
	public async Task<ObjectResult> Logout(string token)
	{
		try
		{
			if (token.IsNullOrEmpty())
				return StatusCode((int)HttpStatusCode.Locked, "1");

			var user = _dbContext.Users.FirstOrDefault(_ => _.LoginToken == token);
			if (user is null)
				return StatusCode((int)HttpStatusCode.Locked, "2");

			user.LoginToken = null;
			user.LoginTokenValidDate = null;
			await _dbContext.ForceSaveChangesAsync("Users");
			return StatusCode((int)HttpStatusCode.OK, "Logged out");
		}
		catch (Exception e)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
		}
	}
	
	
	/// <summary>
	/// Get active promotions
	/// </summary>
	/// <param name="token">Token</param>
	/// <returns>a list of active promotions (after start date but before end date)</returns>
	[HttpGet]
	[Route("app/promotions")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Locked)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	[NoApiKey]
	public async Task<ObjectResult> Promotions(string token, int itemsPerPage = 50, int page = 1, string language = "", bool loadImages = false)
	{
		try
		{
			if (token.IsNullOrEmpty() || _dbContext.Users.FirstOrDefault(_ => _.LoginToken == token) is null)
				return StatusCode((int)HttpStatusCode.Unauthorized, "");
			
			var images = loadImages ? _dbContext.Images.ToList() : new List<Image>();

			var promotions = _dbContext.Promotions.ToList();
			promotions = promotions.Where(_ => _.StartDate < DateTime.Now && _.EndDate > DateTime.Now).ToList();
			
			var pagefiedPromotions = promotions.Pagefy(itemsPerPage, page, out var totalPages);
			
			for (var i = 0; i < pagefiedPromotions.Count; i++)
			{
				pagefiedPromotions[i].AffectedProducts = new List<Product>();
				foreach (var s in pagefiedPromotions[i].Ids.Split(','))
					foreach (var product in _dbContext.Products.Where(_ => _.ID.ToString() == s).ToList())
						pagefiedPromotions[i].AffectedProducts.Add(product);
				
				foreach (var affectedProduct in pagefiedPromotions[i].AffectedProducts)
				{
					if (!affectedProduct.Image.IsNullOrEmpty() && loadImages)
						affectedProduct.Image = images.FirstOrDefault(_ => _.ID.ToString() == affectedProduct.Image)!.Img;
					else
						affectedProduct.Image = null;

					affectedProduct.Name = LanguageHelpers.Translate(affectedProduct.Name, language);
				}
			}

			return StatusCode((int)HttpStatusCode.OK, new RichResponse<List<Promotion>>(pagefiedPromotions)
			{
				Page = page,
				TotalPages = totalPages,
				ItemsPerPage = itemsPerPage,
				TotalItems = promotions.Count,
				TotalItemsFiltered = promotions.Count
			});
		}
		catch (Exception e)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
		}
	}
}