using System.Diagnostics;
using System.Net;
using CoyposServer.Utils;
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
}