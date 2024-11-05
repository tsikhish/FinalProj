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
using System;
using Final.Services.ServiceHelper;
using static Final.Services.ILoanService;
using System.Net.Http;
using Xunit.Sdk;

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
                .UseInMemoryDatabase(databaseName: "UpdatedFinal")
                .Options;
            var dbContextMock = new PersonContext(_dbContextOptions);

            _loggerMock = new Mock<ILogger<LoanController>>();
            _loancontroller = new LoanController(dbContextMock, _loanTesting.Object, _loggerMock.Object);
        }
        [Fact]
        public async Task DeleteLoanByLoanId_WhenLoanExists_ReturnsOk()
        {
            //Arrange
            ClearDatabase();
            int loanId = 5;
            AddLoanToDatabase(loanId);
            //Act
            var result = await _loancontroller.DeleteLoanByUserId(loanId);
            //Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<User>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(200, okResult.StatusCode);
        }
        

        [Fact]
        public async Task DeleteLoanWithNoNameIdentifier_ReturnsBadRequest()
        {
            //Arrange
            ClearDatabase();
            CreateClaimsWithNameIdentifier(2);
            //Act
            var result = await _loancontroller.DeleteLoanByUserId(5);
            //Assert
            var objectResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, objectResult.StatusCode);

        }
        [Fact]
        public async Task DeleteLoanByUserId_WhenLoanDoesntExist_ReturnsUnauthorized()
        {
            //Arrange
            ClearDatabase();
            int userId = 2;
            CreateClaimsWithNameIdentifier(userId);
            var loanId = 100;
            AddLoanToDatabase(5);
            //Act
            var result = await _loancontroller.DeleteLoanByUserId(loanId);
            //Assert
            var objectResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, objectResult.StatusCode);
        }

        [Fact]
        public async Task AddLoanWhenUserIsBlocked_ReturnsBadRequest()
        {
            //Arrange
            ClearDatabase();
            int userId = 2;
            CreateClaimsWithNameIdentifier(userId);
            var bankLoan = new AddLoans { Ammount = 300 };
            _loanTesting.Setup(x => x.AddingLoan(userId, bankLoan))
                .ThrowsAsync(new Exception("Can't be added, user is Blocked"));
            //Act
            var result = await _loancontroller.AddLoan(bankLoan);
            //Assert
            var objectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }
        [Fact]
        public async Task AddLoanByNonAuthorized_ReturnsBadRequest()
        {
            //Arrange
            ClearDatabase();
            CreateClaimsWithNameIdentifier(2);
            var bankLoan = new AddLoans { Ammount = 200 };
            _loanTesting.Setup(x => x.AddingLoan(2,bankLoan))
                .ThrowsAsync(new Exception("Unauthorized"));

            //Act
            var result = await _loancontroller.AddLoan(bankLoan);
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
            var result = await _loancontroller.AddLoan(bankLoan);
            //Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
        }
        [Fact]
        public async Task UpdateLoan_WhenLoanNotFound_ReturnsBadRequest()
        {
            //Arrange
            ClearDatabase();
            int loanId = 100;
            var userId = 2;
            AddLoanToDatabase(userId);
            var updateLoan = UpdateLoanForTesting();
            _loanTesting.Setup(x => x.UpdatingLoan(It.IsAny<HttpContext>(), userId, loanId, updateLoan))
                .ThrowsAsync(new KeyNotFoundException("Loan not found"));
            //Act
            var result = await _loancontroller.UpdateLoanByUserId(loanId, updateLoan);
            //Assert
            var objectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, objectResult.StatusCode);
        }
        [Fact]
        public async Task UpdateLoan_WhenNoIdentifier_ReturnsUnauthorized()
        {
            //Arrange
            ClearDatabase();
            int loanId = 1;
            int idOfUser = 1;
            var updateLoan = UpdateLoanForTesting();
            CreateClaimsWithNoNameIdentifier();
            _loanTesting.Setup(x => x.UpdatingLoan(It.IsAny<HttpContext>(), idOfUser, loanId, updateLoan))
                           .ThrowsAsync(new Exception("Unauthorized"));
            //Act
            var result = await _loancontroller.UpdateLoanByUserId(loanId, updateLoan);
            //Assert
            var objectResult = Assert.IsType<UnauthorizedResult>(result.Result);
            Assert.Equal(401, objectResult.StatusCode);
        }
        [Fact]
        public async Task UpdateLoan_WhenValid_ReturnsOk()
        {
            // Arrange
            ClearDatabase();
            int loanId = 1;
            int idOfUser = 1;
            CreateClaimsWithNameIdentifier(idOfUser);
            AddLoansToDatabaseForTesting(loanId, idOfUser);
            var updateLoan = UpdateLoanForTesting();
            _loanTesting.Setup(x => x.UpdatingLoan(It.IsAny<HttpContext>(), idOfUser, loanId, updateLoan))
            .Returns(Task.CompletedTask);
            // Act
            var result = await _loancontroller.UpdateLoanByUserId(loanId, updateLoan);

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal($"{updateLoan} updated successfully", objectResult.Value);
        }
        //[Fact]
        //public async Task AddPaymentNotForAuthorizedUser_ReturnsBadRequest()
        //{
        //    ClearDBForPayments();
        //    CreateClaimsWithNameIdentifier(1);
        //    var paymentForLoan = new PaymentForLoan { loanId = 1, PaidAmount = 500 };
        //    _loanTesting.Setup(x => x.ProcessLoanPaymentAsync(It.IsAny<HttpContext>(), paymentForLoan, 1))
        //      .ThrowsAsync(new Exception(("The specified loan does not exist in loan records.")));
        //}
        [Fact]
        public async Task AddPaymentWithNonLoanId_ReturnsBadRequest()
        {
            ClearDBForPayments();
            CreateClaimsWithNameIdentifier(1);
            var paymentForLoan = new PaymentForLoan {loanId=99, PaidAmount = 500 };
            _loanTesting.Setup(x => x.AddingPayment(It.IsAny<HttpContext>(), paymentForLoan, 1))
               .ThrowsAsync(new Exception(("The specified loan does not exist in loan records.")));
            // Act
            var result = await _loancontroller.AddPayment(paymentForLoan);

            // Assert
            var objectResult = Assert.IsType<BadRequestObjectResult>(result); 
            Assert.Equal(400, objectResult.StatusCode); 
            Assert.Equal("The specified loan does not exist in loan records.", objectResult.Value); 
    }
        [Fact]
        public async Task AddPaymentWithExistingPayment_ReturnsException()
        {
            //Arrange
            ClearDBForPayments();
            CreateClaimsWithNameIdentifier(1);
            var paymentForLoan = new PaymentForLoan {PaidAmount = 500 };
            AddPaymentsToDB(2);
            _loanTesting.Setup(x => x.AddingPayment(It.IsAny<HttpContext>(), paymentForLoan, 1))
               .ThrowsAsync(new Exception("Payment already exists for this loan."));
            //Act
            var result = await _loancontroller.AddPayment(paymentForLoan);
            //Assert
            var objectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal($"Payment already exists for this loan.", objectResult.Value);

        }
        [Fact]
        public async Task AddPaymentWithNoIdentifier_ReturnsUnauthorized()
        {
            //Arrange
            ClearDatabase();
            CreateClaimsWithNoNameIdentifier();
            var paymentForLoan = new PaymentForLoan { PaidAmount = 500 };
            //Act
            var result = await _loancontroller.AddPayment(paymentForLoan);
            //Assert
            var objectResult = Assert.IsType<UnauthorizedResult>(result);
            Assert.Equal(401, objectResult.StatusCode);
        }
        [Fact]
        public async Task AddingPayment_ReturnsOk()
        {
            // Arrange
            ClearDBForPayments();
            var userId = 1;
            CreateClaimsWithNameIdentifier(userId);
            var paymentForLoan = new PaymentForLoan { PaidAmount = 500 };
            AddPaymentToDatabase();
            _loanTesting.Setup(x => x.AddingPayment(It.IsAny<HttpContext>(), paymentForLoan, userId))
                .Returns(Task.CompletedTask);
            //Act
            var result = await _loancontroller.AddPayment(paymentForLoan);
            //Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal("Payment has added successfully", objectResult.Value);
        }
        private PaymentHistory AddPaymentToDatabase()
        {
            using (var context = new PersonContext(_dbContextOptions))
            {
                var payment = new PaymentHistory { Id = 5, FullPayment = 1000 };
                context.Payment.Add(payment);
                context.SaveChanges();
                return payment;
            }
        }
        private Loan AddLoanToDatabase(int loanId)
        {
            using (var context = new PersonContext(_dbContextOptions))
            {
        var loan = new Loan { Id = loanId, UserId = 1, Ammount = 1000 }; 
                context.Loans.Add(loan);
                context.SaveChanges();
                return loan;
            }

        }
        private void ClearDBForPayments()
        {
            using (var context = new PersonContext(_dbContextOptions))
            {
                context.Payment.RemoveRange(context.Payment);
                context.SaveChanges();
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
        private PaymentHistory AddPaymentsToDB(int loanId)
        {
            using (var context = new PersonContext(_dbContextOptions))
            {
                var existingPayment = new PaymentHistory
                {
                    Id=1,
                    LoanId = loanId,
                    FullPayment=3000
                };

                context.Payment.Add(existingPayment);
                context.SaveChanges();
                return existingPayment;
            }
        }
        private void AddLoansToDatabaseForTesting(int loanId, int userId)
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

