using hr_client.Controllers;
using hr_client.Models;
using hr_client.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace hr_client_tests
{
    public class HomeControllerTests
    {
        private readonly Mock<IEmployeesService> serviceMock;
        private readonly HomeController controller;

        public HomeControllerTests()
        {
            serviceMock = new Mock<IEmployeesService>();
            controller = new HomeController(serviceMock.Object);
            
        }

        [Fact]
        public async void Index_ShouldCallServiceAndReturnView()
        {
            var result = await controller.Index();

            serviceMock.Verify(x => x.GetAll(), Times.Once());
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public async void Active_ShouldCallServiceAndReturnView()
        {
            var result = await controller.Active();

            serviceMock.Verify(x => x.GetByStatus("active"), Times.Once());
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public async void Inactive_ShouldCallServiceAndReturnView()
        {
            var result = await controller.Inactive();

            serviceMock.Verify(x => x.GetByStatus("inactive"), Times.Once());
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void Add_ShouldReturnView()
        {
            var result = controller.Add();

            Assert.IsAssignableFrom<IActionResult>(result);
        }

        //[HttpPost, ActionName("Add")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddConfirmed(EmployeeViewModel model)
        //{
        //    await _employeesService.AddNewEmployee(model);
        //    return RedirectToAction(nameof(Index));
        //}

        [Fact]
        public async void AddConfirmed_ShouldCallServiceAndRedirectHome()
        {
            var newEmployee = new EmployeeViewModel
            {
                employeeId = "some_id"
            };

            var result = await controller.AddConfirmed(newEmployee);

            serviceMock.Verify(x => x.AddNewEmployee(newEmployee), Times.Once());
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public async void ChangeStatus_ShouldCallServiceWithCorrectStatusAndReturnView()
        {
            var result = await controller.ChangeStatus("some_id", "false");

            serviceMock.Verify(x => x.ChangeStatus("some_id", "active"), Times.Once());
            Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
