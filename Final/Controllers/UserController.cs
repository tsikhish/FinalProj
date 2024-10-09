using Data;
using Final.helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System;
using Domain;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Final.Services;
using System.Linq;
using Domain.Post;
namespace Final.Controllers
{
    [Route("api/[controller]")]

    public class UserController : Controller
    {
        private readonly PersonContext _personcontext;
        private readonly IUserServices _userservices;

        public UserController(PersonContext personcontext, IUserServices userservices)
        {
            _personcontext = personcontext;
            _userservices = userservices;
        }
   
        [HttpPost("/user/registration")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistration user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var newuser = await _userservices.UserRegister(user);

                return Ok($"{user.UserName} registered successfully");
            }
            catch (Exception ex)
            {

                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
        [HttpPost("/user/login")]
        public async Task<IActionResult> Login([FromBody] LoginUser loginModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var tokenstring = await _userservices.LoginUser(loginModel);
                if (tokenstring != null)
                {
                    return Ok(tokenstring);
                }
                else
                {
                    return Unauthorized("Invalid username or password");
                }
            }
            catch(Exception ex) 
            {
                return BadRequest($"An error occured: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("getuser")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllPerson()
        {
            try
            {
                var persons = await _userservices.GetAll();
                if (!persons.Any())
                {
                    return NotFound("there are no person");
                }
                else
                {
                    return Ok(persons);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occured: {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("/userUpdate")]
        public async Task<ActionResult<IEnumerable<User>>> UpdateUser(int userId)
        {
            try
            {
                var user = await _userservices.UserIsBlocked(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("/{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized();
                }
                var user = await _personcontext.AppUsers.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                {
                    return NotFound();
                }
                if (User.IsInRole(nameof(Role.Admin)) || (userId == id))
                {
                    return Ok(user);
                }
                else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occured: {ex.Message}");
            }
        }
    }
}
