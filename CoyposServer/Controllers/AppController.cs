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
    [ProducesResponseType(typeof(List<Promotion>), (int)HttpStatusCode.OK)]
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
					{
						var foundImage = images.FirstOrDefault(_ => _.ID.ToString() == affectedProduct.Image);
						if (foundImage is null)
						{
							affectedProduct.Image = null;
							goto end;
						}

						affectedProduct.Image =
							images.FirstOrDefault(_ => _.ID.ToString() == affectedProduct.Image)!.Img;
					}
					else
						affectedProduct.Image = null;

					end:
					affectedProduct.Name = LanguageHelpers.Translate(affectedProduct.Name, language);
					
#pragma warning disable CS8629
					affectedProduct.DiscountedPrice = Math.Round((decimal)(affectedProduct.Price - affectedProduct.Price * pagefiedPromotions[i].DiscountPercentage / 100), 2);
#pragma warning restore CS8629
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

	/// <summary>
	/// Get user receipts
	/// </summary>
	/// <param name="token">Token</param>
	/// <returns>a list of receipts</returns>
	[HttpGet]
	[Route("app/receipts")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(List<Receipt>), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	[NoApiKey]
	public async Task<ObjectResult> Receipts(string token, int itemsPerPage = 50, int page = 1, string language = "",
		bool loadImages = false)
	{
		try
		{
			if (token.IsNullOrEmpty() || _dbContext.Users.FirstOrDefault(_ => _.LoginToken == token) is null)
				return StatusCode((int)HttpStatusCode.Unauthorized, "");

			var found = _dbContext.Users.First(_ => _.LoginToken == token);

			return new ReceiptController(_dbContext).GetReceipts(new Receipt() { User = found }, "AND", itemsPerPage,
				page,
				language, loadImages);
		}
		catch (Exception e)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
		}
	}
	
	
	/// <summary>
	/// Get user data
	/// </summary>
	/// <param name="token">Token</param>
	/// <returns>user data</returns>
	[HttpGet]
	[Route("app/user")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	[NoApiKey]
	public async Task<ObjectResult> User(string token)
	{
		try
		{
			if (token.IsNullOrEmpty() || _dbContext.Users.FirstOrDefault(_ => _.LoginToken == token) is null)
				return StatusCode((int)HttpStatusCode.Unauthorized, "");

			var found = _dbContext.Users.First(_ => _.LoginToken == token);

			found.Password = null;
			found.Salt = null;
			found.LoginToken = null;
			
			return StatusCode((int)HttpStatusCode.OK, found);
		}
		catch (Exception e)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
		}
	}


	/// <summary>
	/// Edit user data
	/// </summary>
	/// <param name="user">new user data</param>
	/// <param name="token">Token</param>
	[HttpPut]
	[Route("app/user")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
	[ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	[NoApiKey]
	public async Task<ObjectResult> UserEdit([FromBody] User user, [FromHeader] string password, [FromQuery] string token)
	{
		try
		{
			if (token.IsNullOrEmpty() || _dbContext.Users.FirstOrDefault(_ => _.LoginToken == token) is null)
				return StatusCode((int)HttpStatusCode.Unauthorized, "");

			var found = _dbContext.Users.First(_ => _.LoginToken == token);
			var pass = BCrypt.Net.BCrypt.HashPassword(password, found.Salt);
			if (pass != found.Password)
				return StatusCode((int)HttpStatusCode.BadRequest, new ProblemDetails() {Title = "Wrong password"});

			// prevent these values from being updatable by the user
			user.Role = null;
			user.CardNumber = null;
			user.Points = null;

			return (await new UserController(_dbContext).Put(user, (int)found.ID));
		}
		catch (Exception e)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
		}
	}
	

	/// <summary>
	/// register endpoint for the user
	/// </summary>
	[HttpPost]
	[NoApiKey]
	[Route("app/register")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public async Task<ObjectResult> Post([FromBody] User user)
	{
		return (await new UserController(_dbContext).Post(user));
	}

	/// <summary>
	/// returns app logo
	/// </summary>
	[HttpGet]
	[NoApiKey]
	[Route("app/logo")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public ObjectResult Logo()
	{
		return (new SettingsController(_dbContext).Setting("logo"));
	}
}