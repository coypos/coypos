using System.Net;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CoyposServer.Controllers;

public class SettingsController : ControllerBase
{
	private DatabaseContext _dbContext;

	public SettingsController(DatabaseContext dbContext)
	{
		_dbContext = dbContext;
	}

	/// <summary>
	/// Returns all settings
	/// </summary>
	/// <param name="settingFilter">filter to use</param>
	/// <param name="filter">filter type to use (AND/OR/NOR)</param>
	/// <param name="itemsPerPage">number of items per page</param>
	/// <param name="page">page number</param>
	[HttpGet]
	[Route("settings")]
	[ProducesResponseType(typeof(RichResponse<List<Setting>>), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public ObjectResult Settings([FromBody] Setting settingFilter, string filter = "AND", int itemsPerPage = 50, int page = 1)
	{
		try
		{
			var settings = _dbContext.Settings.ToList();
			var filteredSettings = settings.Filter(settingFilter, filter);
			var pagefiedSettings = filteredSettings.Pagefy(itemsPerPage, page, out var totalPages).ToList();
			return StatusCode((int)HttpStatusCode.OK, new RichResponse<List<Setting>>(pagefiedSettings)
			{
				Page = page,
				TotalPages = totalPages,
				ItemsPerPage = itemsPerPage,
				TotalItems = settings.Count,
				TotalItemsFiltered = filteredSettings.Count
			});
		}
		catch (Exception e)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
		}
	}

	/// <summary>
	/// Returns a specific setting
	/// </summary>
	/// <param name="query">ID or Key</param>
	[HttpGet]
	[Route("setting/{query}")]
	[ProducesResponseType(typeof(Setting), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public ObjectResult Setting(string query)
	{
		try
		{
			var foundSetting = _dbContext.Settings.ToList()
				.FirstOrDefault(_ => _.ID.ToString() == query || _.Key == query);
			if (foundSetting is null)
				return StatusCode((int)HttpStatusCode.BadRequest, new ProblemDetails() { Title = "No such setting" });
			return StatusCode((int)HttpStatusCode.OK, foundSetting);
		}
		catch (Exception e)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
		}
	}

	/// <summary>
	/// Adds a new setting or updates an existing one
	/// </summary>
	/// <param name="setting">key/value setting</param>
	[HttpPost]
	[Route("setting")]
	[ProducesResponseType(typeof(Setting), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public ObjectResult AddSetting([FromBody] SettingRequestModel setting)
	{
		try
		{
			var foundSetting = _dbContext.Settings.FirstOrDefault(_ => _.Key == setting.Key);
			if (foundSetting is null)
				_dbContext.Settings.Add(new Setting() { Key = setting.Key, Value = setting.Value });
			else
				foundSetting.Value = setting.Value;

			_dbContext.SaveChanges();
			return StatusCode((int)HttpStatusCode.OK, _dbContext.Settings.First(_ => _.Key == setting.Key));
		}
		catch (Exception e)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
		}
	}
	
	/// <summary>
	/// Deletes a specific setting
	/// </summary>
	/// <param name="query">ID or Key</param>
	[HttpDelete]
	[Route("setting/{query}")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public ObjectResult DeleteSetting(string query)
	{
		try
		{
			var foundSetting = _dbContext.Settings.ToList()
				.FirstOrDefault(_ => _.ID.ToString() == query || _.Key == query);
			if (foundSetting is null)
				return StatusCode((int)HttpStatusCode.BadRequest, new ProblemDetails() { Title = "No such setting" });
			_dbContext.Settings.Remove(foundSetting);
			_dbContext.SaveChanges();
			return StatusCode((int)HttpStatusCode.OK, "deleted");
		}
		catch (Exception e)
		{
			return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
		}
	}
}