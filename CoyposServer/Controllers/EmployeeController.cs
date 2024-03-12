using System.Net;
using System.Text;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CoyposServer.Controllers;

public class EmployeeController : ControllerBase
{
	private readonly DatabaseContext _dbContext;

	public EmployeeController(DatabaseContext dbContext) => _dbContext = dbContext;
	
	/// <summary>
	/// Returns employees
	/// </summary>
	/// <param name="employeeFilter">filter to use</param>
	/// <param name="filter">filter type to use (AND/OR/NOR)</param>
	/// <param name="itemsPerPage">number of items per page</param>
	/// <param name="page">page number</param>
	/// <param name="language">language to use</param>
	[HttpGet]
	[Route("employees")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(RichResponse<List<Employee>>), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public ObjectResult GetEmployees([FromBody] Employee employeeFilter, string filter = "AND", int itemsPerPage = 50, int page = 1, string language = "")
	{
		try
		{
			var employees = _dbContext.Employees.ToList();
			var filteredEmployees = employees.Filter(employeeFilter, filter);
			var pagefiedEmployees = filteredEmployees.Pagefy(itemsPerPage, page, out var totalPages);
            
			for (var i = 0; i < pagefiedEmployees.Count; i++)
			{
				var t = pagefiedEmployees[i];
				if (!t.Name.IsNullOrEmpty())
					t.Name = LanguageHelpers.Translate(t.Name, language);
			}
            
			return StatusCode((int)HttpStatusCode.OK, new RichResponse<List<Employee>>(pagefiedEmployees)
			{
				Page = page,
				TotalPages = totalPages,
				ItemsPerPage = itemsPerPage,
				TotalItems = employees.Count,
				TotalItemsFiltered = filteredEmployees.Count
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
	/// Updates an employee
	/// </summary>
	/// <param name="employee">new employee data</param>
	/// <param name="id">employee ID</param>
	[HttpPut]
	[Route("employee/{id}")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(Employee), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public async Task<ObjectResult> Put([FromBody] Employee employee, int id)
	{
		Employee? employeeFromDb;
		try
		{
			employeeFromDb = _dbContext.Employees.FirstOrDefault(p => p.ID == id);
			if (employeeFromDb is null)
				throw new Exception("No known employee with such ID");
			employee.ID = id;
			employee = ObjectHelpers.CopyNonNullValues(employeeFromDb, employee);
			//_dbContext.AttachVirtualProperties(productFromDb);
			_dbContext.Entry(employeeFromDb).CurrentValues.SetValues(employee);
            
			await _dbContext.ForceSaveChangesAsync("Employees");
		}
		catch (Exception e)
		{
			var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
			var response = e.InnerException is not null
				? HttpStatusCode.BadRequest
				: HttpStatusCode.InternalServerError;
			return StatusCode((int)response, new ProblemDetails() { Title = message });
		}

		return StatusCode((int)HttpStatusCode.OK, employeeFromDb);
	}
	
	/// <summary>
	/// Deletes an employee
	/// </summary>
	/// <param name="id">employee ID</param>
	[HttpDelete]
	[Route("employee/{id}")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public async Task<ObjectResult> Delete(int id)
	{
		try
		{
			var employeeFromDb = _dbContext.Employees.FirstOrDefault(p => p.ID == id);
			if (employeeFromDb is null)
				throw new Exception("No known employee with such ID");
			_dbContext.AttachVirtualProperties(id);
			_dbContext.Remove(employeeFromDb);
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

		return StatusCode((int)HttpStatusCode.OK, $"Employee {id} deleted");
	}
	
	/// <summary>
	/// Creates an employee entry
	/// </summary>
	/// <param name="employee">new employee data</param>
	[HttpPost]
	[Route("employee")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(Employee), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
	public async Task<ObjectResult> Post([FromBody] Employee employee)
	{
		EntityEntry<Employee> result;

		try
		{
			result = _dbContext.Employees.Add(employee);
			await _dbContext.ForceSaveChangesAsync("Employees");
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
	/// Validates employee to log in to the kiosk
	/// </summary>
	/// <param name="employeeValidation">validation data</param>
	[HttpGet]
	[Route("employee_validate")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(Employee), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
	public async Task<ObjectResult> Validate([FromBody] EmployeeValidationModel employeeValidation)
	{
		try
		{
			var found = _dbContext.Employees.FirstOrDefault(_ =>
				_.CardId == employeeValidation.CardId && _.PIN == employeeValidation.PIN && _.Enabled == true);
			if (found is null)
				return StatusCode((int)HttpStatusCode.Unauthorized, "");
			return StatusCode((int)HttpStatusCode.OK, found);
		}
		catch (Exception e)
		{
			var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
			var response = e.InnerException is not null
				? HttpStatusCode.BadRequest
				: HttpStatusCode.InternalServerError;
			return StatusCode((int)response, new ProblemDetails() { Title = message });
		}
	}
	
	/// <summary>
	/// Validates employee to log in to the admin panel
	/// </summary>
	/// <param name="employeeValidation">validation data</param>
	[HttpGet]
	[Route("employee_validate_admin")]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
	[ProducesResponseType(typeof(Employee), (int)HttpStatusCode.OK)]
	[ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
	public async Task<ObjectResult> ValidateAdmin([FromBody] EmployeeValidationModel employeeValidation)
	{
		try
		{
			var found = _dbContext.Employees.FirstOrDefault(_ =>
				_.CardId == employeeValidation.CardId && _.PIN == employeeValidation.PIN && _.Enabled == true && _.Admin == true);
			if (found is null)
				return StatusCode((int)HttpStatusCode.Unauthorized, "");
			return StatusCode((int)HttpStatusCode.OK, found);
		}
		catch (Exception e)
		{
			var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
			var response = e.InnerException is not null
				? HttpStatusCode.BadRequest
				: HttpStatusCode.InternalServerError;
			return StatusCode((int)response, new ProblemDetails() { Title = message });
		}
	}
}