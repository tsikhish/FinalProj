using Data;
using Final.helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System;
using Domain;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Final.Services;
using Final.Validation;
using System.Linq;
using Domain.Post;
using FluentValidation;
using static Domain.Loan;
using static Domain.Post.AddLoans;

namespace Final.Controllers
{
    [Route("api/[controller]")]

    public class UserController : Controller
    {
        private readonly PersonContext _personcontext;
        private readonly AppSettings _appsettings;
        private readonly IUserServices _userservices;

        public UserController(PersonContext personcontext, IOptions<AppSettings> appsettings, IUserServices userservices)
        {
            _personcontext = personcontext;
            _appsettings = appsettings.Value;
            _userservices = userservices;
        }
        [HttpPost("/user/registration")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistration user)
        {
            try
            {
                var validator = new UserRegisterValidation();
                var validationResult = validator.Validate(user);
                var errorMessage = "";
                if (!validationResult.IsValid)
                {
                    foreach(var item in validationResult.Errors)
                    {
                        errorMessage += item.ErrorMessage + " , ";
                    }
                    return BadRequest(errorMessage);
                }

                var existingUser = await _personcontext.Users.FirstOrDefaultAsync(x => x.UserName == user.UserName);
                if (existingUser != null)
                {
                    return Conflict($"{existingUser.UserName} already exists");
                }
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                var newUser = new User
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Password = hashedPassword,
                    Salary = user.Salary,
                    LastName = user.LastName,
                    Age = user.Age,
                    FirstName = user.FirstName,
                    Role = user.Role
                };

                await _personcontext.Users.AddAsync(newUser);
                await _personcontext.SaveChangesAsync();

                if (newUser.Role != "User")
                {
                    return BadRequest($"{newUser.UserName} should be a User");
                }

                return Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("/user/login")]
        public async Task<IActionResult> Login([FromBody] LoginUser loginModel)
        {
            var validator = new UserLoginValidation();
            var valid = validator.Validate(loginModel);
            var errorMessage = "";
            if (!valid.IsValid)
            {
                foreach (var item in valid.Errors)
                {
                    errorMessage += item.ErrorMessage + " , ";
                }
                return BadRequest(errorMessage);
            }
            var tokenstring = await _userservices.LoginUser(loginModel);
            if (tokenstring != null)
            {
                var user = await _personcontext.Users.FirstOrDefaultAsync(x => x.UserName == loginModel.UserName);
                
                return Ok(
                    new
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Token = tokenstring,
                        Role = user.Role,
                    }
                    );
            }
            else
            {
                return Unauthorized("Invalid username or password");
            }

        }
        [HttpGet("getuser")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllPerson()
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

        [Authorize]
        [HttpGet("/{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            if (userId != id)
            {
                return Forbid();
            }

            var user = await _personcontext.Users.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(user);

        }





    }
}
