using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Domain;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Final.Services;
using System.Linq;
using Domain.Post;
using Final.helper;
using Microsoft.Extensions.Logging;
namespace Final.Controllers
{
    [Route("api/[controller]")]

    public class UserController : Controller
    {
        private readonly PersonContext _personcontext;
        private readonly IUserServices _userservices;
        private readonly ILogger _logger;
        public UserController(PersonContext personcontext, IUserServices userservices,ILogger<UserController> logger)
        {
            _personcontext = personcontext;
            _userservices = userservices;
            _logger = logger;
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistration user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Modelstate is not valid");
                    return BadRequest(ModelState);
                }
                var newuser = await _userservices.UserRegister(user);
                _logger.LogInformation($"User {user.UserName} registered successfully.");
                return Ok($"{user.UserName} registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred during user registration for {user.UserName}.");
                var errorResponse = new ErrorResponse
                {
                    Message = "An error occurred while processing your request.",
                    Detail = ex.Message
                };
                return BadRequest(errorResponse);
            }

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser loginModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Modelstate is not valid");
                    return BadRequest(ModelState);
                }
                var tokenstring = await _userservices.LoginUser(loginModel);
                if (tokenstring != null)
                {
                    return Ok(tokenstring);
                }
                else
                {
                    _logger.LogWarning("Invalid username or password");
                    return Unauthorized("Invalid username or password");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred during user login for {loginModel.UserName}.");
                var errorResponse = new ErrorResponse
                {
                    Message = "An error occurred while processing your request.",
                    Detail = ex.Message
                };
                return BadRequest(errorResponse);
            }

        }
        [Authorize(Roles = nameof(Role.Admin))]
        [HttpGet("getuser")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllPerson()
        {
            try
            {
                _logger.LogDebug("Getting all users");
                var persons = await _userservices.GetAll();
                if (!persons.Any())
                {
                    _logger.LogWarning("warning");
                    return NotFound("there are no person");
                }
                else
                {
                    return Ok(persons);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred during getting all the users.");
                var errorResponse = new ErrorResponse
                {
                    Message = "An error occurred while processing your request.",
                    Detail = ex.Message
                };
                return BadRequest(errorResponse);
            }

        }
        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPut("userUpdate")]
        public async Task<ActionResult<IEnumerable<User>>> UpdateUser(int userId)
        {
            try
            {
                _logger.LogDebug("Starting updating all users");
                var user = await _userservices.UserIsBlocked(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred during user update.");

                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("user/{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            try
            {
                _logger.LogDebug("Getting user's id");
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    _logger.LogWarning($"User with this {id} isnt authorized or token is null");
                    return Unauthorized();
                }
                var user = await _personcontext.AppUsers.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                {
                    _logger.LogWarning($"{user} not found");
                    return NotFound();
                }
                if (User.IsInRole(nameof(Role.Admin)) || (userId == id))
                {
                    _logger.LogInformation("You got User by its Id");
                    return Ok(user);
                }
                else
                {
                    _logger.LogWarning("Somethings wrong");
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred during user update.");
                var errorResponse = new ErrorResponse
                {
                    Message = "An error occurred while processing your request.",
                    Detail = ex.Message
                };
                return BadRequest(errorResponse);
            }

        }
    }
}
