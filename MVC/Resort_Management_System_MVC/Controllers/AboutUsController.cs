using Microsoft.AspNetCore.Mvc;

namespace Resort_Management_System_MVC.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult AboutUs()
        {
            return View();
        }
    }
}
