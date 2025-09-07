using Microsoft.AspNetCore.Mvc;

namespace Resort_Management_System_MVC.Controllers
{
    public class AdminPanelDashboardController : Controller
    {
        public IActionResult AdminPanelDashboard()
        {
            return View();
        }
    }
}
