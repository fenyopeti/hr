using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hr_api.Models;
using hr_api.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace hr_api.Services
{
    public class EmployeesService : IEmployeesService
    {
        private readonly EmployeesDbContext _db;
        public static readonly string ACTIVE = "active";
        public static readonly string INACTIVE = "inactive";

        public EmployeesService(EmployeesDbContext db)
        {
            this._db = db;
        }

        public async Task<EntityEntry<Employee>> Add(Employee newEmployee)
        {
            var employee = await GetById(newEmployee.EmployeeId);

            if (employee != null)
            {
                throw new EntityDuplicationException();
            }

            var added = await _db.Employees.AddAsync(newEmployee);
            await _db.SaveChangesAsync();
            return added;
        }

        public IEnumerable<Employee> GetAll()
        {
            return _db.Employees;
        }

        public Task<Employee> GetById(string id)
        {
            return _db.Employees.FirstOrDefaultAsync(employee => employee.EmployeeId == id);
        }

        public IEnumerable<Employee> GetByStatus(string status)
        {
            var isActive = status.ToLower() == ACTIVE;
            return _db.Employees.Where(employee => employee.Status == isActive);
        }

        public async Task<Employee> UpdateStatus(string id, string status)
        {
            var employee = await GetById(id);

            if (employee == null)
            {
                return null;
            }

            employee.Status = status == ACTIVE;
            await _db.SaveChangesAsync();

            return employee;
        }
    }

    public interface IEmployeesService
    {
        Task<Employee> GetById(string id);
        Task<EntityEntry<Employee>> Add(Employee newEmployee);
        IEnumerable<Employee> GetAll();
        IEnumerable<Employee> GetByStatus(string status);
        Task<Employee> UpdateStatus(string id, string status);
    }
}
