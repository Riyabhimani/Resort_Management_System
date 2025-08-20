using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Resort_Management_System_MVC.Service;

namespace Resort_Management_System_MVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly AuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginController(AuthService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password, string Role)
        {
            var jsonData = await _authService.AuthenticateUserAsync(Username, Password, Role);

            if (jsonData == null)
            {
                ViewBag.Error = "Invalid credentials.";
                return View();
            }

            var data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonData);
            string token = data["token"];

            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "Invalid credentials.";
                return View();
            }

            _httpContextAccessor.HttpContext.Session.SetString("JWTToken", token);
            _httpContextAccessor.HttpContext.Session.SetString("UserRole", Role);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
