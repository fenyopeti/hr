using hr_api.Controllers;
using hr_api.Models;
using hr_api.Services;
using hr_api.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace hr_api_tests.ControllerTests
{
    public class EmployeesControllerTests
    {
        private readonly Mock<IEmployeesService> mockEmployeesService;

        public EmployeesControllerTests()
        {
            mockEmployeesService = new Mock<IEmployeesService>();
        }

        [Fact]
        public void GetEmployees_WithNoQuery_ShouldCallGetAllMethod()
        {
            mockEmployeesService.Setup(service => service.GetAll());

            var controller = new EmployeesController(mockEmployeesService.Object);

            controller.GetEmployees("");

            mockEmployeesService.Verify(x => x.GetAll(), Times.Once());
        }

        [Fact]
        public void GetEmployees_WithQuery_ShouldCallGetByStatus()
        {
            mockEmployeesService.Setup(service => service.GetByStatus(It.IsAny<string>()));

            var controller = new EmployeesController(mockEmployeesService.Object);

            controller.GetEmployees("active");

            mockEmployeesService.Verify(x => x.GetByStatus("active"), Times.Once());
        }

        [Fact]
        public async void GetById_WithNonExistentEmployeeId_ShouldReturnNotFoundResult()
        {
            mockEmployeesService.Setup(service => service.GetById(It.IsAny<string>())).ReturnsAsync(() => null);

            var controller = new EmployeesController(mockEmployeesService.Object);

            var result = await controller.GetById("non existent id");
            
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async void GetById_WithExistentEmployeeId_ShouldReturnEmployee()
        {
            var expectedEmployee = new hr_api.Models.Employee
            {
                EmployeeId = "some_id"
            };
            mockEmployeesService.Setup(service => service.GetById(It.IsAny<string>()))
                .ReturnsAsync(expectedEmployee);

            var controller = new EmployeesController(mockEmployeesService.Object);

            var result = await controller.GetById("some_id");

            Assert.Equal(expectedEmployee.EmployeeId, result.Value.EmployeeId);
        }

        [Fact]
        public async void Add_WithExistentEmployeeId_ShouldReturnBadRequestResult()
        {
            mockEmployeesService.Setup(service => service.Add(It.IsAny<hr_api.Models.Employee>()))
                .Throws<EntityDuplicationException>();

            var controller = new EmployeesController(mockEmployeesService.Object);

            var employeeToAdd = new hr_api.Models.Employee();
            var result = await controller.Add(employeeToAdd);

            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async void Add_WithNonExistentEmployeeId_ShouldReturnCreatedResult()
        {
            var employeeToAdd = new Employee
            {
                EmployeeId = "new_id"
            };
            mockEmployeesService.Setup(service => service.Add(It.IsAny<Employee>()));

            var controller = new EmployeesController(mockEmployeesService.Object);

            var result = await controller.Add(employeeToAdd);

            Assert.IsType<CreatedResult>(result.Result);
        }

        [Fact]
        public async void UpdateStatus_ShouldReturnNotFoundResult_WithNonExistentEmployee()
        {
            mockEmployeesService.Setup(service => service.UpdateStatus(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => null);

            var controller = new EmployeesController(mockEmployeesService.Object);

            var updateDto = new UpdateStatusDto { status = "active" };

            var result = await controller.UpdateStatus("non existent id", updateDto);
            
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void UpdateStatus_ShouldReturnBadRequest_OnNonValidStatus()
        {
            mockEmployeesService.Setup(service => service.UpdateStatus(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => null);

            var controller = new EmployeesController(mockEmployeesService.Object);

            var updateDto = new UpdateStatusDto { status = "non valid status" };

            var result = await controller.UpdateStatus("some id", updateDto);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void UpdateStatus_ShouldReturnOk_OnNoError()
        {
            var employee = new Employee
            {
                EmployeeId = "some_id"
            };
            mockEmployeesService.Setup(service => service.UpdateStatus(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(employee);

            var controller = new EmployeesController(mockEmployeesService.Object);

            var updateDto = new UpdateStatusDto { status = "active" };

            var result = await controller.UpdateStatus(employee.EmployeeId, updateDto);

            Assert.IsType<OkResult>(result);
        }
    }
}
