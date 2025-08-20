using Microsoft.AspNetCore.Mvc;

namespace Resort_Management_System_MVC.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult News()
        {
            return View();
        }
    }
}
