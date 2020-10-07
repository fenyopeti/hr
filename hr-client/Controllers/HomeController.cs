using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using hr_client.Models;
using hr_client.Services;

namespace hr_client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeesService _employeesService;

        public HomeController(ILogger<HomeController> logger, IEmployeesService employeesService)
        {
            _logger = logger;
            _employeesService = employeesService;
        }

        public async Task<IActionResult> Index()
        {
            var allEmployees = await _employeesService.GetAll();
            return View(allEmployees);
        }

        public async Task<IActionResult> Active()
        {
            var activeEmployees = await _employeesService.GetByStatus("active");
            return View("Index", activeEmployees);
        }

        public async Task<IActionResult> Inactive()
        {
            var inactiveEmployees = await _employeesService.GetByStatus("inactive");
            return View("Index", inactiveEmployees);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost, ActionName("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddConfirmed(EmployeeViewModel model)
        {
            await _employeesService.AddNewEmployee(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(string id, string oldStatus)
        {
            var newStatus = oldStatus.ToLower() == "true" ? "inactive" : "active";
            await _employeesService.ChangeStatus(id, newStatus);
            return RedirectToAction("Index"); 
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
