using System.Linq;
using System.Threading;
using EntityFrameworkCore3Mock;
using hr_api;
using hr_api.Models;
using hr_api.Services;
using hr_api.Services.Exceptions;
using Moq;
using Xunit;

namespace hr_api_tests.ServiceTests
{
    public class EmployeesServiceTests
    {
        private readonly DbContextMock<EmployeesDbContext> mockDb;
        private readonly DbSetMock<Employee> dbSetMock;

        public EmployeesServiceTests()
        {
            var initialEntities = new[]
            {
                new Employee
                {
                    EmployeeId = "existingId1",
                    Status = true
                },
                new Employee
                {
                    EmployeeId = "existingId2",
                    Status = false
                }
            };
            mockDb = new DbContextMock<EmployeesDbContext>();
            dbSetMock = mockDb.CreateDbSetMock(x => x.Employees, initialEntities);
        }

        [Fact]
        public async void Add_ShouldThrowException_IfIdAlreadyExists()
        {
            var existingEmployee = new Employee
            {
                EmployeeId = "existingId1"
            };

            var service = new EmployeesService(mockDb.Object);

            await Assert.ThrowsAsync<EntityDuplicationException>(() => service.Add(existingEmployee));
        }

        [Fact]
        public async void Add_ShouldAddNewEntityAndSaveChanges()
        {
            var newEmployee = new Employee
            {
                EmployeeId = "newId"
            };

            var service = new EmployeesService(mockDb.Object);

            var result = await service.Add(newEmployee);

            mockDb.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            dbSetMock.Verify(x => x.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public void GetAll_ShouldReturnDbSet()
        {
            var service = new EmployeesService(mockDb.Object);

            var result = service.GetAll();

            mockDb.Verify(x => x.Employees, Times.Once());

            Assert.Equal(dbSetMock.Object, result);
        }

        [Fact]
        public void GetByStatus_ShouldFilteredDbSet()
        {
            var service = new EmployeesService(mockDb.Object);

            var result = service.GetByStatus("active");

            Assert.Equal(dbSetMock.Object.Where(x => x.Status == true), result);
        }

        [Fact]
        public async void UpdateStatus_ShouldReturnNull_OnNonExisting()
        {
            var service = new EmployeesService(mockDb.Object);

            var result = await service.UpdateStatus("no id", "active");

            Assert.Null(result);
        }

        [Fact]
        public async void UpdateStatus_ShouldUpdateEmployeeStatus()
        {
            var service = new EmployeesService(mockDb.Object);

            var result = await service.UpdateStatus("existingId2", "active");

            Assert.True(result.Status);
        }
    }
}
