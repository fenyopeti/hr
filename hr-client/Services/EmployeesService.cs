using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using hr_client.Models;

namespace hr_client.Services
{
    public class EmployeesService : IEmployeesService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly HttpClient _client;

        public EmployeesService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _client = _clientFactory.CreateClient("hr-api");
        }

        public async Task<IEnumerable<EmployeeViewModel>> GetAll()
        {
            var response = await _client.GetAsync("employees");

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var employees = await JsonSerializer.DeserializeAsync<IEnumerable<EmployeeViewModel>>(responseStream);
            return employees;
        }

        public async Task<IEnumerable<EmployeeViewModel>> GetByStatus(string status)
        {
            var response = await _client.GetAsync($"employees?status={status}");

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var employees = await JsonSerializer.DeserializeAsync<IEnumerable<EmployeeViewModel>>(responseStream);
            return employees;
        }

        public async Task AddNewEmployee(EmployeeViewModel employee)
        {
            var payload = new StringContent(JsonSerializer.Serialize(employee), Encoding.UTF8, "application/json");
            await _client.PutAsync("employees", payload);
        }

        public async Task ChangeStatus(string id, string status)
        {
            var payload = new StringContent(JsonSerializer.Serialize(new { status = status }), Encoding.UTF8, "application/json");
            await _client.PostAsync($"employees/{id}", payload);
        }
    }

    public interface IEmployeesService
    {
        Task<IEnumerable<EmployeeViewModel>> GetAll();
        Task<IEnumerable<EmployeeViewModel>> GetByStatus(string status);
        Task AddNewEmployee(EmployeeViewModel employee);
        Task ChangeStatus(string id, string status);
    }
}
