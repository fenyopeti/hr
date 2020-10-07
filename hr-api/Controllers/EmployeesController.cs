using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using hr_api.Models;
using hr_api.Services;
using hr_api.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace hr_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController
    {
        private readonly IEmployeesService _employeesService;

        public EmployeesController(IEmployeesService employeesService)
        {
            _employeesService = employeesService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Employee>> GetEmployees([FromQuery(Name = "status")] string status)
        {
            if (String.IsNullOrEmpty(status))
            {
                return new OkObjectResult(_employeesService.GetAll());
            }

            return new OkObjectResult(_employeesService.GetByStatus(status));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetById(string id)
        {
            var employee = await _employeesService.GetById(id);

            if (employee == null)
            {
                return new NotFoundResult();
            }

            return employee;
        }

        [HttpPut]
        public async Task<ActionResult<Employee>> Add(Employee newEmployee)
        {
            try
            {
                var addedEmployee = await _employeesService.Add(newEmployee);
                return new CreatedResult($"/Employees/{addedEmployee?.Entity.EmployeeId}", addedEmployee?.Entity);
            }
            catch (EntityDuplicationException)
            {
                return new BadRequestResult();
            }
            
        }

        [HttpPost("{id}")]
        public async Task<ActionResult> UpdateStatus([FromRoute] string id, UpdateStatusDto updateStatus)
        {
            var availableStatuses = new List<string> { EmployeesService.ACTIVE, EmployeesService.INACTIVE };
            if (!availableStatuses.Contains(updateStatus.status.ToLower()))
            {
                return new BadRequestResult();
            }

            var employee = await _employeesService.UpdateStatus(id, updateStatus.status);

            if (employee == null)
            {
                return new NotFoundResult();
            }

            return new OkResult();
        }
    }

    public class UpdateStatusDto
    {
        public string status { get; set; }
    }
}
