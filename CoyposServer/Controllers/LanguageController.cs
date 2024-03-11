using System.Net;
using System.Text;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CoyposServer.Controllers;

public class LanguageController : ControllerBase
{
	private readonly DatabaseContext _dbContext;

	public LanguageController(DatabaseContext dbContext) => _dbContext = dbContext;
	
	/// <summary>
	/// Returns languages
	/// </summary>
	/// <param name="languageFilter">filter to use</param>
	/// <param name="filter">filter type to use (AND/OR/NOR)</param>
	/// <param name="itemsPerPage">number of items per page</param>
	/// <param name="page">page number</param>
	/// <param name="language">language to use</param>
	/// <param name="loadImages">should we load images?</param>
	[HttpGet]
	[Route("languages")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(RichResponse<List<Language>>), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public ObjectResult GetLanguages([FromBody] Language languageFilter, string filter = "AND", int itemsPerPage = 50, int page = 1, string language = "", bool loadImages = false)
	{
		try
		{
			var images = loadImages ? _dbContext.Images.ToList() : new List<Image>();
			var languages = _dbContext.Languages.ToList();
			var filteredLanguages = languages.Filter(languageFilter, filter);
			var pagefiedLanguages = filteredLanguages.Pagefy(itemsPerPage, page, out var totalPages);
            
			for (var i = 0; i < pagefiedLanguages.Count; i++)
			{
				var t = pagefiedLanguages[i];
				if (!t.Image.IsNullOrEmpty() && loadImages)
					t.Image =
						images.FirstOrDefault(_ => _.ID.ToString() == t.Image).Img;
				else
					t.Image = null;
                
				if (!t.Name.IsNullOrEmpty())
					t.Name = LanguageHelpers.Translate(t.Name, language);
			}
            
			return StatusCode((int)HttpStatusCode.OK, new RichResponse<List<Language>>(pagefiedLanguages)
			{
				Page = page,
				TotalPages = totalPages,
				ItemsPerPage = itemsPerPage,
				TotalItems = languages.Count,
				TotalItemsFiltered = filteredLanguages.Count
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
	/// Updates a language
	/// </summary>
	/// <param name="language">new language data</param>
	/// <param name="id">language ID</param>
	[HttpPut]
	[Route("language/{id}")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(Language), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public async Task<ObjectResult> Put([FromBody] Language language, int id)
	{
		Language? languageFromDb;
		try
		{
			if (!language.Image.IsNullOrEmpty())
			{
				var imageSize = Encoding.UTF8.GetBytes(language.Image).Length;
				if (imageSize > 25*1024)
					throw new Exception(
						$"Provided image is too large ({imageSize} bytes). Maximum image size: {25*1024} bytes.");
				var imageResult = _dbContext.Images.Add(new Image() { Img = language.Image });
				await _dbContext.ForceSaveChangesAsync("Images");
				language.Image = imageResult.Entity.ID.ToString();
			}
            
			languageFromDb = _dbContext.Languages.FirstOrDefault(p => p.ID == id);
			if (languageFromDb is null)
				throw new Exception("No known language with such ID");
			language.ID = id;
			language = ObjectHelpers.CopyNonNullValues(languageFromDb, language);
			//_dbContext.AttachVirtualProperties(productFromDb);
			_dbContext.Entry(languageFromDb).CurrentValues.SetValues(language);
            
			await _dbContext.ForceSaveChangesAsync("Languages");
		}
		catch (Exception e)
		{
			var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
			var response = e.InnerException is not null
				? HttpStatusCode.BadRequest
				: HttpStatusCode.InternalServerError;
			return StatusCode((int)response, new ProblemDetails() { Title = message });
		}

		return StatusCode((int)HttpStatusCode.OK, languageFromDb);
	}
	
	/// <summary>
	/// Deletes a language
	/// </summary>
	/// <param name="id">language ID</param>
	[HttpDelete]
	[Route("language/{id}")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public async Task<ObjectResult> Delete(int id)
	{
		try
		{
			var languageFromDb = _dbContext.Languages.FirstOrDefault(p => p.ID == id);
			if (languageFromDb is null)
				throw new Exception("No known language with such ID");
			_dbContext.AttachVirtualProperties(id);
			_dbContext.Remove(languageFromDb);
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

		return StatusCode((int)HttpStatusCode.OK, $"Language {id} deleted");
	}
	
	/// <summary>
	/// Creates a language entry
	/// </summary>
	/// <param name="language">new language data</param>
	[HttpPost]
	[Route("language")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(Language), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public async Task<ObjectResult> Post([FromBody] Language language)
	{
		EntityEntry<Language> result;

		try
		{
			if (!language.Image.IsNullOrEmpty())
			{
				var imageSize = Encoding.UTF8.GetBytes(language.Image).Length;
				if (imageSize > 25*1024)
					throw new Exception(
						$"Provided image is too large ({imageSize} bytes). Maximum image size: {25*1024} bytes.");
				var imageResult = _dbContext.Images.Add(new Image() { Img = language.Image });
				await _dbContext.ForceSaveChangesAsync("Images");
				language.Image = imageResult.Entity.ID.ToString();
			}

			result = _dbContext.Languages.Add(language);
            
			await _dbContext.ForceSaveChangesAsync("Languages");
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