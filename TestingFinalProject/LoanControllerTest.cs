using Data;
using Domain.Post;
using Domain;
using Final.Controllers;
using Final.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using static Domain.Post.AddLoans;

namespace TestingFinalProject
{
    public class LoanControllerTest
    {

        private readonly Mock<ILoanService> _loanTesting;
        private readonly LoanController _loancontroller;
        private readonly Mock<ILogger<LoanController>> _loggerMock;
        private readonly DbContextOptions<PersonContext> _dbContextOptions;
        public LoanControllerTest()
        {
            _loanTesting = new Mock<ILoanService>();
            _dbContextOptions = new DbContextOptionsBuilder<PersonContext>()
                .UseInMemoryDatabase(databaseName: "FinalResult")
                .Options;
            var dbContextMock = new PersonContext(_dbContextOptions);

            _loggerMock = new Mock<ILogger<LoanController>>();
            _loancontroller = new LoanController(dbContextMock, _loanTesting.Object, _loggerMock.Object);
        }
        [Fact]
        public async Task DeleteLoanByUserId_WhenLoanExists_ReturnsOk()
        {
            //Arrange
            ClearDatabase();  
            int userId = 2;
            AddLoanToDatabase(userId);
            //Act
            var result =await _loancontroller.DeleteLoanByUserId(userId);
            //Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<User>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(200, okResult.StatusCode);
        }
        public async Task DeleteLoanByUserId_WhenLoanDoesntExist_ReturnsUnauthorized()
        {
            //Arrange
            ClearDatabase();
            int userId = 100;
            AddLoanToDatabase(2);
            //Act
            var result = await _loancontroller.DeleteLoanByUserId(userId);
            //Assert
            var objectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, objectResult.StatusCode);
        }
        [Fact]
        public async Task AddLoanByNonAdminPerson_ReturnsBadRequest()
        {
            //Arrange
            ClearDatabase();
            CreateClaimsWithNoNameIdentifier();
            int userId = 4;
            var bankLoan = new AddLoans { Ammount = 200 };
            //Act
            var result = await _loancontroller.AddLoan(userId, bankLoan);
            //Assert
            var objectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }
        [Fact]
        public async Task AddLoan_WhenValid_ReturnsOk()
        {
            //Arrange
            ClearDatabase();
            int userId = 2;
            CreateClaimsWithNameIdentifier(userId);
            var bankLoan = new AddLoans { Ammount = 200 };
            _loanTesting.Setup(x => x.AddingLoan(userId, bankLoan))
                           .Returns(Task.CompletedTask);
            //Act
            var result = await _loancontroller.AddLoan(userId, bankLoan);
            //Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
        }
        [Fact]
        public async Task UpdateLoan_WhenLoanNotFound_ReturnsBadRequest()
        {
            //Arrange
            ClearDatabase();
            int userId = 1;
            int loanId = 100;
            AddLoanToDatabase(userId);
            var updateLoan = UpdateLoanForTesting();
            //Act
            var result = await _loancontroller.UpdateLoanByUserId(userId, loanId, updateLoan);
            //Assert
            var objectResult = Assert.IsType<BadRequestObjectResult>(result.Result); 
            Assert.Equal(400, objectResult.StatusCode);
        }
        [Fact]
        public async Task UpdateLoan_WhenNoIdentifier_ReturnsUnauthorized()
        {
            //Arrange
            ClearDatabase();
            int userId = 1;
            int loanId = 1;
            var updateLoan = UpdateLoanForTesting();
            CreateClaimsWithNoNameIdentifier();
            //Act
            var result = await _loancontroller.UpdateLoanByUserId(userId, loanId, updateLoan);
            //Assert
            var objectResult = Assert.IsType<UnauthorizedResult>(result.Result);
            Assert.Equal(401, objectResult.StatusCode);
        }
        [Fact]
        public async Task UpdateLoan_WhenValid_ReturnsOk()
        {
            // Arrange
            ClearDatabase();
            int userId = 1;
            int loanId = 1;
            int idOfUser = 1;
            CreateClaimsWithNameIdentifier(idOfUser);
            AddLoansToDatabaseForTesting(loanId, userId);
            var updateLoan = UpdateLoanForTesting();
            _loanTesting.Setup(x => x.UpdatingLoan(It.IsAny<HttpContext>(), userId, loanId, updateLoan, idOfUser))
           .Returns(Task.CompletedTask); 

            // Act
            var result = await _loancontroller.UpdateLoanByUserId(userId, loanId, updateLoan);

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, objectResult.StatusCode); 
            Assert.Equal($"{updateLoan} updated successfully", objectResult.Value);
        }
        private Loan AddLoanToDatabase(int userId)
        {
            using (var context = new PersonContext(_dbContextOptions))
            {
                var loan = new Loan { UserId = userId, Ammount = 1000 };
                context.Loans.Add(loan);
                context.SaveChanges();
                return loan;
            }
        }

        private void ClearDatabase()
        {
            using (var context = new PersonContext(_dbContextOptions))
            {
                context.Loans.RemoveRange(context.Loans);  
                context.SaveChanges();
            }
        }

        private void CreateClaimsWithNoNameIdentifier()
        {
            var identity = new ClaimsIdentity(); 
            var user = new ClaimsPrincipal(identity);
            _loancontroller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

        }
        private void CreateClaimsWithNameIdentifier(int idOfUser)
        {
            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier, idOfUser.ToString()),
            new Claim(ClaimTypes.Role, Role.Admin.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);
            _loancontroller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }
        private void AddLoansToDatabaseForTesting(int loanId,int userId)
        {

            using (var context = new PersonContext(_dbContextOptions))
            {
                var existingLoan = new Loan
                {
                    Id = loanId,
                    UserId = userId,
                    Status = LoanStatus.Proccessing
                };

                context.Loans.Add(existingLoan);
                context.SaveChanges();
            }
        }
        private AddLoans UpdateLoanForTesting()
        {
            var updateLoan = new AddLoans
            {
                Ammount = 2000,
                LoanPeriod = 12,
                LoanType = TypeOfLoan.CarLoan,
                Currency = CurrencyType.USD,
                Status = LoanStatus.Proccessing
            };
            return updateLoan;
        }
    }
}