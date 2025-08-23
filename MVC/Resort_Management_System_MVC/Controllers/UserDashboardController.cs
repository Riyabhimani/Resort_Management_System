using Microsoft.AspNetCore.Mvc;

namespace Resort_Management_System_MVC.Controllers
{
    public class UserDashboardController : Controller
    {
        public IActionResult UserIndex()
        {
            return View();
        }
    }
}
