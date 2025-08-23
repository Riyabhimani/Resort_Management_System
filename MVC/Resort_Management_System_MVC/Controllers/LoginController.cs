using System.Data;
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
public async Task<IActionResult> Login(string UserName, string Password, string Role)
{
    var jsonData = await _authService.AuthenticateUserAsync(UserName, Password,Role);

    if (string.IsNullOrEmpty(jsonData))
    {
        ViewBag.Error = "Invalid credentials.";
        return View();
    }

    try
    {
        // Deserialize JSON
        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

                if (data != null && data.ContainsKey("token"))
                {
                    string token = data["token"].ToString();

                    // Store token in session (or cookie)
                    HttpContext.Session.SetString("JWToken", token);
                    HttpContext.Session.SetString("UserRole", Role);
                    HttpContext.Session.SetString("UserName", UserName);

                    if (Role == "Admin")
                        return RedirectToAction("Index", "Home");  // goes to AdminController → Dashboard
                    else if (Role == "User")
                        return RedirectToAction("UserIndex", "UserDashboard");   // goes to UserController → Dashboard
                    else

                        //return RedirectToAction("AccessDenied", "Login");
                        return RedirectToAction("AccessDenied", "Login");
                }

                else
                {
                    // API did not return token → show error
                    ViewBag.Error = "Invalid API response: token missing.";
                    return View();
                }
    }
    catch (Exception ex)
    {
        // Log exception for debugging
        Console.WriteLine("Deserialization error: " + ex.Message);
        ViewBag.Error = "Unexpected error while processing login.";
        return View();
    }
}



        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
