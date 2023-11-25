using System.Net;
using System.Text;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CoyposServer.Controllers;

public class UserController : ControllerBase
{
	private DatabaseContext _dbContext;

	public UserController(DatabaseContext dbContext)
	{
		_dbContext = dbContext;
	}

    /// <summary>
    /// Returns users
    /// </summary>
    /// <param name="userFilter">filter to use</param>
    /// <param name="filter">filter type to use (AND/OR/NOR)</param>
    /// <param name="itemsPerPage">number of items per page</param>
    /// <param name="page">page number</param>
    [HttpGet]
    [Route("users")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(RichResponse<List<User>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public ObjectResult GetUsersWithFilter([FromBody] User userFilter, string filter = "AND", int itemsPerPage = 50, int page = 1)
    {
        try
        {
            var users = _dbContext.Users.ToList();
            var filteredUsers = users.Filter(userFilter, filter);
            var pagefiedUsers = filteredUsers.Pagefy(itemsPerPage, page, out var totalPages);

            foreach (var user in pagefiedUsers)
            {
                user.Password = null;
                user.Salt = null;
                user.LoginToken = null;
            }
            
            return StatusCode((int)HttpStatusCode.OK, new RichResponse<List<User>>(pagefiedUsers)
            {
                Page = page,
                TotalPages = totalPages,
                ItemsPerPage = itemsPerPage,
                TotalItems = users.Count,
                TotalItemsFiltered = filteredUsers.Count
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
    /// Updates the user
    /// </summary>
    /// <param name="user">new user data</param>
    /// <param name="id">user ID</param>
    [HttpPut]
    [Route("user/{id}")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ObjectResult> Put([FromBody] User user, int id)
    {
        User? userFromDb;
        try
        {
            // these values cannot be set.
            user.LoginToken = null;
            user.LoginTokenValidDate = null;
            if (user.Password is not null)
            {
                user.Salt = BCrypt.Net.BCrypt.GenerateSalt();
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, user.Salt);
            }
            else
            {
                user.Salt = null;
            }

            userFromDb = _dbContext.Users.FirstOrDefault(p => p.ID == id);
            if (userFromDb is null)
                throw new Exception("No known user with such ID");
            user.ID = id;
            user.CreateDate = userFromDb.CreateDate;
            user = ObjectHelpers.CopyNonNullValues(userFromDb, user);
            //_dbContext.AttachVirtualProperties(productFromDb);
            _dbContext.Entry(userFromDb).CurrentValues.SetValues(user);
            
            await _dbContext.ForceSaveChangesAsync("Users");
        }
        catch (Exception e)
        {
            var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
            var response = e.InnerException is not null
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.InternalServerError;
            return StatusCode((int)response, new ProblemDetails() { Title = message });
        }

        userFromDb.Password = null;
        userFromDb.LoginToken = null;
        userFromDb.Salt = null;
        return StatusCode((int)HttpStatusCode.OK, userFromDb);
    }

    /// <summary>
    /// Deletes the user
    /// </summary>
    /// <param name="id">user ID</param>
    [HttpDelete]
    [Route("user/{id}")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ObjectResult> Delete(int id)
    {
        try
        {
            var userFromDb = _dbContext.Users.FirstOrDefault(p => p.ID == id);
            if (userFromDb is null)
                throw new Exception("No known user with such ID");
            _dbContext.AttachVirtualProperties(id);
            _dbContext.Remove(userFromDb);
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

        return StatusCode((int)HttpStatusCode.OK, $"User {id} deleted");
    }

    /// <summary>
    /// Creates a user entry
    /// </summary>
    /// <param name="user">new user data</param>
    /// <returns></returns>
    [HttpPost]
    [Route("user")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ObjectResult> Post([FromBody] User user)
    {
        EntityEntry<User> result;

        try
        {
            if (_dbContext.Users.Any(_ => _.Email == user.Email))
            {
                throw new Exception("Email is already taken");
            }

            if (_dbContext.Users.Any(_ => _.PhoneNumber == user.PhoneNumber))
            {
                throw new Exception("Phone number is already taken");
            }
            
            // these values cannot be set.
            user.LoginToken = null;
            user.LoginTokenValidDate = null;
            user.Salt = BCrypt.Net.BCrypt.GenerateSalt();
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, user.Salt);
            
            var random = new Random();
            user.CardNumber = (random.Next((int)(100000000000 / 1000000), (int)(999999999999 / 1000000 + 1)) * 1000000 +
                               random.Next(1000000)).ToString();
            
            // overwritten values:
            user.CreateDate = DateTime.Now;
            user.UpdateDate = DateTime.Now;
            _dbContext.AttachVirtualProperties(user);
            // </>

            result = _dbContext.Users.Add(user);
            
            await _dbContext.ForceSaveChangesAsync("Users");
        }
        catch (Exception e)
        {
            var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
            var response = e.InnerException is not null
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.InternalServerError;
            return StatusCode((int)response, new ProblemDetails() { Title = message });
        }

        result.Entity.Password = null;
        result.Entity.Salt = null;
        result.Entity.LoginToken = null;
        return StatusCode((int)HttpStatusCode.OK, result.Entity);
    }

    /// <summary>
    /// Bans the user
    /// </summary>
    /// <param name="id">user ID</param>
    /// <returns></returns>
    [HttpPost]
    [Route("user/{id}/ban")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ObjectResult> Ban(int id)
    {
        try
        {
            var foundUser = _dbContext.Users.FirstOrDefault(_ => _.ID == id);
            if (foundUser is null)
                throw new Exception("No known user with such ID");
            if (foundUser.Role == "Banned")
                throw new Exception("User is already banned");

            foundUser.Role = "Banned";
            await _dbContext.ForceSaveChangesAsync("Users");
        }
        catch (Exception e)
        {
            var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
            var response = e.InnerException is not null
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.InternalServerError;
            return StatusCode((int)response, new ProblemDetails() { Title = message });
        }
        
        return StatusCode((int)HttpStatusCode.OK, "Banned!");
    }
    
    /// <summary>
    /// Unbans the user
    /// </summary>
    /// <param name="id">user ID</param>
    /// <returns></returns>
    [HttpPost]
    [Route("user/{id}/unban")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ObjectResult> UnBan(int id)
    {
        try
        {
            var foundUser = _dbContext.Users.FirstOrDefault(_ => _.ID == id);
            if (foundUser is null)
                throw new Exception("No known user with such ID");
            if (foundUser.Role != "Banned")
                throw new Exception("User is not banned");

            foundUser.Role = "";
            await _dbContext.ForceSaveChangesAsync("Users");
        }
        catch (Exception e)
        {
            var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
            var response = e.InnerException is not null
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.InternalServerError;
            return StatusCode((int)response, new ProblemDetails() { Title = message });
        }
        
        return StatusCode((int)HttpStatusCode.OK, "Unbanned!");
    }
}