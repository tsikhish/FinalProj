//using Data;
//using Domain;
//using Domain.Post;
//using Final.Services;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Threading.Tasks;

//namespace Final.Controllers
//{
//    [Route("api/[controller]")]

//    public class AccountantController : Controller
//    {
//        private readonly PersonContext _personcontext;
//        private readonly IUserServices _userservices;
//        public AccountantController(PersonContext personcontext, UserServices userservices)
//        {
//            _personcontext = personcontext;
//            _userservices = userservices;
//        }
        //public async Task RegisterAccount() {
        //    var newAccountant1 = new Accountant { Id = 1, FirstName = "Tom", LastName = "Brown" ,Role="Accountant"};
        //        var newAccountant2 = new Accountant { Id = 2, FirstName = "John", LastName = "Doe" ,Role= "Accountant" };
        //        _personcontext.Accountants.Add(newAccountant1);
        //        _personcontext.Accountants.Add(newAccountant2);
        //        await _personcontext.SaveChangesAsync();
        //}
        //[HttpPost("/loginAccountant")]
        //public async Task<IActionResult> Login([FromBody] LoginUser loginModel)
        //{

//        //}
//    }
//}
