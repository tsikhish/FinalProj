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

namespace Final.Services
{
    public interface IUserServices
    {
      //  public  Task GetPersonByUsername([FromBody] UserRegistration user);
        Task<string> LoginUser([FromBody] LoginUser loginUser);
        Task<IEnumerable<User>> GetAll();
        Task<ActionResult<User>> GetPersonById(int id);

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

        //public async Task GetPersonByUsername([FromBody] UserRegistration userRegistration)
        //{
        //    try
        //    {

        //        var user = _personcontext.Users.FirstOrDefault(x => x.UserName == userRegistration.UserName);
        //        if (user != null)
        //        var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
        //        var newuser = new User
        //        {
        //            UserName = user.UserName,
        //            Email = user.Email,
        //            Password = passwordHash,
        //            Salary = user.Salary,
        //            LastName = user.LastName,
        //            Age = user.Age,
        //            FirstName = user.FirstName,
        //            Role = user.Role,
        //        };
        //        await _personcontext.Users.AddAsync(newuser);
        //        await _personcontext.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"An error occurred: {ex.Message}");
        //    }
        //}
        public async Task<IEnumerable<User>> GetAll() => await _personcontext.Users.ToListAsync();
        //public async Task<ActionResult<User>> GetPersonById(int id)
        //{
        //    var user = await UserClaim(id);
        //    return new OkObjectResult(user);
        //}


        public async Task<string> LoginUser([FromBody] LoginUser loginmodel)
        {
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

        public Task<ActionResult<User>> GetPersonById(int id)
        {
            throw new NotImplementedException();
        }
    }
}

