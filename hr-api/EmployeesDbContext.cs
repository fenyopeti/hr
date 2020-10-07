using hr_api.Models;
using Microsoft.EntityFrameworkCore;

namespace hr_api
{
    public class EmployeesDbContext : DbContext
    {
        public virtual DbSet<Employee> Employees { get; set; }

        public EmployeesDbContext(DbContextOptions<EmployeesDbContext> options) : base(options)
        {
        }

        public EmployeesDbContext()
        {
        }
    }
}
