using Data;
using Domain;
using Final.helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Final.Validation;
using Domain.Post;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using FluentValidation;

namespace Final.Services
{
    public interface IUserServices
    {
        Task<string> LoginUser([FromBody] LoginUser loginUser);
        Task<IEnumerable<User>> GetAll();
        Task<User> UserRegister([FromBody] UserRegistration userRegistration);
        Task<ActionResult<User>> UserIsBlocked(int id);

    }
    public class UserServices : IUserServices
    {
        private readonly PersonContext _personcontext;
        private readonly AppSettings _appsettings;

        public UserServices(PersonContext personcontext, IOptions<AppSettings> appsettings)
        {
            _appsettings = appsettings.Value;
            _personcontext = personcontext;
        }
        public async Task<User> UserRegister([FromBody] UserRegistration user)
        {
            await ValidateRegistration(user);
            var existingUser = await _personcontext.Users.FirstOrDefaultAsync(x => x.UserName == user.UserName);
            if (existingUser != null)
            {
                throw new Exception($"{existingUser} already exists,please try another informations");
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
                Role = user.Role,
                IsBlocked = false
            };
            await _personcontext.Users.AddAsync(newUser);
            await _personcontext.SaveChangesAsync();
            return newUser;
        }
        public async Task<IEnumerable<User>> GetAll() => await _personcontext.Users.ToListAsync();
        public async Task<string> LoginUser([FromBody] LoginUser loginmodel)
        {
            await ValidateLogin(loginmodel);
            var user = await _personcontext.Users.FirstOrDefaultAsync(x => x.UserName == loginmodel.UserName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginmodel.Password, user.Password))
            {
                return null;
            }
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Role,user.Role),
            };
            var tokenstring = GenerateToken(authClaims);


            return tokenstring;
        }
        public async Task<ActionResult<User>> UserIsBlocked(int id)
        {
            var user = await _personcontext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                throw new Exception($"{id} not found");
            }
            user.IsBlocked = !user.IsBlocked;
            _personcontext.Users.Update(user);
            await _personcontext.SaveChangesAsync();
            return user;
        }
        private async Task ValidateLogin(LoginUser loginUser)
        {
            var validator = new UserLoginValidation();
            var valid =  validator.Validate(loginUser);
            var errorMessage = "";
            if (!valid.IsValid)
            {
                foreach (var item in valid.Errors)
                {
                    errorMessage += item.ErrorMessage + " , ";
                }
                throw new Exception(errorMessage);
            }
        }

        private async Task ValidateRegistration( UserRegistration userRegistration)
        {
            var validator = new UserRegisterValidation();
            var valid =  validator.Validate(userRegistration);
            var errorMessage = "";
            if (!valid.IsValid)
            {
                foreach (var item in valid.Errors)
                {
                    errorMessage += item.ErrorMessage + " , ";
                }
                throw new Exception (errorMessage);
            }
        }
        private string GenerateToken(List<Claim> claims)
        {
            var key = Encoding.ASCII.GetBytes(_appsettings.Secret);
            var authSecret = new SymmetricSecurityKey(key);
            var tokenObject = new JwtSecurityToken(
                expires: DateTime.Now.AddDays(1),
                claims: claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                );
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.WriteToken(tokenObject);
            return token;
        }

    }
}

