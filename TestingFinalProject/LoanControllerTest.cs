using Data;
using Domain.Post;
using Domain;
using Final.Controllers;
using Final.helper;
using Final.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace TestingFinalProject
{
    public class LoanControllerTest
    {

        private readonly Mock<ILoanService> _loanTesting;
        private readonly LoanController _loancontroller;
        public LoanControllerTest()
        {
            _loanTesting = new Mock<ILoanService>();
            var dbContextOptions = new DbContextOptionsBuilder<PersonContext>()
                .UseInMemoryDatabase(databaseName: "FinalResult")
                .Options;
            var dbContextMock = new Mock<PersonContext>(dbContextOptions);
            _loancontroller = new LoanController(dbContextMock.Object, _loanTesting.Object);

        }
        [Fact]
        public async Task UpdateLoans_ReturnsOK()
        {
            // Arrange
            int userId = 1;
            int loanId = 1;
            int idOfUser = 1;
            var updateLoan = new AddLoans { };
            _loanTesting.Setup(x => x.UpdatingLoan(It.IsAny<HttpContext>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<AddLoans>(), It.IsAny<int>()))
                .Returns((HttpContext httpcontext,int userId, int loanId, AddLoans updateLoan, int idOfUser) => null);
            //Act
            var result = await _loancontroller.UpdateLoanByUserId(userId, loanId, updateLoan);
            //Assert
            Assert.NotNull(result);
            Assert.IsType<ObjectResult>(result.Result);
        }



    }
}
