using Moq;
using Final.Services;
using Xunit;
using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using Final.Controllers;
using Data;
using Final.helper;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Post;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
namespace TestingFinalProject
{
    public class IUserController
    {
        private readonly Mock<IUserServices> _repositoryStub;
        private readonly UserController _usercontroller;
        public IUserController()
        {
            _repositoryStub = new Mock<IUserServices>();
            var dbContextOptions = new DbContextOptionsBuilder<PersonContext>()
                .UseInMemoryDatabase(databaseName: "FinalResult")
                .Options;
            var dbContextMock = new Mock<PersonContext>(dbContextOptions);
            _usercontroller = new UserController(dbContextMock.Object, Mock.Of<IOptions<AppSettings>>(), _repositoryStub.Object);

        }

        [Fact]
        public void GetAllPerson_whenPersonDoesntExist_ReturnsNotFound()
        {   
            // Arrange
            _repositoryStub.Setup(x => x.GetAll()).ReturnsAsync(new List<User>());
            // Act
            var result = _usercontroller.GetAllPerson().Result;
            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("there are no person", notFoundResult.Value);
        }
        [Fact]
        public void GetAllPerson_whenPersonExists_ReturnsOk()
        {
            //Arrange
            _repositoryStub.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { GetUser() });

            //Act
            var result = _usercontroller.GetAllPerson().Result;
            //Assert
           Assert.IsType<OkObjectResult>(result.Result);
        }
        [Fact]
        public void RegisterPerson_ReturnsOk()
        {
            //Arrange
            var registerUser = RegisterFakeUser();
            _repositoryStub.Setup(x => x.UserRegister(It.IsAny<UserRegistration>())).ReturnsAsync(new User());
            //Act
            var result = _usercontroller.RegisterUser(registerUser);
            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }
        [Fact]
        public void LoginPersonWhenPersonExists_ReturnsOk()
        {
            //Arrange
            var loginUser = LoginFakeUser();
            _repositoryStub.Setup(x => x.LoginUser(It.IsAny<LoginUser>())).ReturnsAsync("fake_token");
            //Act
            var result =  _usercontroller.Login(loginUser);
            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);

        }
        [Fact]
        public async Task LoginPerson_WhenPersonDoesntExists_NotFound()
        {
            // Arrange
            _repositoryStub.Setup(x => x.LoginUser(It.IsAny<LoginUser>())).ReturnsAsync((string)null);
            var fakeUser = LoginFakeUser();

            // Act
            var result = await _usercontroller.Login(fakeUser);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result); 
        }

        private LoginUser LoginFakeUser()
            {
                return new LoginUser
                {
                    UserName = "tsikhish",
                    Password = "tsikhish"
                };
            }
        private UserRegistration RegisterFakeUser()
        {
            return new UserRegistration
            {
                FirstName = "mariam",
                LastName = "tsik",
                UserName = "tsikhish",
                Age = 20,
                Salary = 5000,
                Email = "tsikhish@gmail.com",
                Password = "tsikhish",
            };
        }
        private User GetUser()
        {
            return new User
            {
                Id = 1,
                FirstName = "mariam",
                LastName = "tsik",
                UserName = "tsikhish",
                Age = 20,
                Salary = 5000,
                Email = "tsikhish@gmail.com",
                Password = "tsikhish",
                Role ="User",
                IsBlocked=false
            };
        }

    }
}
