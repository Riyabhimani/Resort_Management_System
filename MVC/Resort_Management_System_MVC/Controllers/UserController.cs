//using System.Text;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
//using Resort_Management_System_MVC.Models;

//namespace Resort_Management_System_MVC.Controllers
//{
//    public class UserController : Controller
//    {

//        private readonly HttpClient client;
//        private readonly IHttpContextAccessor _httpContextAccessor;

//        // ✅ Constructor injection for HttpClient and IHttpContextAccessor
//        public UserController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
//        {
//            client = httpClientFactory.CreateClient();
//            client.BaseAddress = new Uri("https://localhost:5159/api/");
//            _httpContextAccessor = httpContextAccessor;
//        }

//        // ✅ Adds JWT token from session to request headers
//        private void AddJwtToken()
//        {
//            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
//            if (!string.IsNullOrEmpty(token))
//            {
//                client.DefaultRequestHeaders.Authorization =
//                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
//            }
//        }

//        //public async Task<IActionResult> UserList()
//        //{
//        //    var response = await client.GetAsync("User");
//        //    var json = await response.Content.ReadAsStringAsync();
//        //    var list = JsonConvert.DeserializeObject<List<UserModel>>(json);
//        //    return View(list);
//        //}

//        public async Task<IActionResult> UserList()
//        {
//            try
//            {
//                AddJwtToken(); // 🔑 Add token before calling API

//                var response = await client.GetAsync("User");

//                // 🔒 Redirect to login if unauthorized
//                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
//                {
//                    return RedirectToAction("Login", "Login");
//                }

//                // ✅ Deserialize response into user list
//                response.EnsureSuccessStatusCode();
//                var json = await response.Content.ReadAsStringAsync();
//                var list = JsonConvert.DeserializeObject<List<UserModel>>(json);

//                return View(list);
//            }
//            catch
//            {
//                TempData["Error"] = "Unable to load users.";
//                return View(new List<UserModel>());
//            }
//        }

//        public async Task<IActionResult> UserAddEdit(int? id)
//        {
//            try
//            {
//                AddJwtToken();
//                UserModel user = new UserModel();

//                if (id != null)
//                {
//                    var response = await client.GetAsync($"User/{id}");
//                    if (!response.IsSuccessStatusCode)
//                    {
//                        TempData["ErrorMessage"] = "User not found.";
//                        return RedirectToAction("UserList");
//                    }

//                    var json = await response.Content.ReadAsStringAsync();
//                    user = JsonConvert.DeserializeObject<UserModel>(json);
//                }

//                return View(user);
//            }
//            catch (Exception ex)
//            {
//                TempData["ErrorMessage"] = "Unable to load form.";
//                return RedirectToAction("Index");
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> UserAddEdit(UserModel user)
//        {
//            if (!ModelState.IsValid)
//                return View(user);

//            try
//            {
//                AddJwtToken();
//                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

//                if (user.UserId == 0)
//                {
//                    var response = await client.PostAsync("User", content);
//                    response.EnsureSuccessStatusCode();
//                }
//                else
//                {
//                    var response = await client.PutAsync($"User/{user.UserId}", content);
//                    response.EnsureSuccessStatusCode();
//                }

//                return RedirectToAction("UserList");
//            }
//            catch (Exception ex)
//            {
//                TempData["ErrorMessage"] = "Unable to save user.";
//                return View("UserList");
//            }
//        }

//        public async Task<IActionResult> UserDelete(int id)
//        {
//            AddJwtToken();
//            var response = await client.DeleteAsync($"User/{id}");

//            if (response.IsSuccessStatusCode)
//            {
//                TempData["SuccessMessage"] = "User deleted successfully.";
//            }
//            else
//            {
//                TempData["ErrorMessage"] = $"Failed to delete user. Status: {response.StatusCode}";
//            }

//            return RedirectToAction("UserList");
//        }

//    }
//}

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Resort_Management_System_MVC.Models;
using System.Text;

namespace Resort_Management_System_MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5159/api/");
            _httpContextAccessor = httpContextAccessor;
        }

        // ✅ Adds JWT token from session to request headers
        private void AddJwtToken()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        // ✅ Helper to check if user is logged in
        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Session.GetString("JWTToken"));
        }

        // ✅ User List (only for logged-in users)
        public async Task<IActionResult> UserList()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Login");

            try
            {
                AddJwtToken();
                var response = await client.GetAsync("User");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Login");

                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<UserModel>>(json);

                return View(list);
            }
            catch
            {
                TempData["Error"] = "Unable to load users.";
                return View(new List<UserModel>());
            }
        }

        public async Task<IActionResult> UserAddEdit(int? id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Login");

            try
            {
                AddJwtToken();
                UserModel user = new UserModel();

                if (id != null)
                {
                    var response = await client.GetAsync($"User/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "User not found.";
                        return RedirectToAction("UserList");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<UserModel>(json);
                }

                return View(user);
            }
            catch
            {
                TempData["ErrorMessage"] = "Unable to load form.";
                return RedirectToAction("UserList");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UserAddEdit(UserModel user)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Login");

            if (!ModelState.IsValid)
                return View(user);

            try
            {
                AddJwtToken();
                var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                if (user.UserId == 0)
                {
                    var response = await client.PostAsync("User", content);
                    response.EnsureSuccessStatusCode();
                }
                else
                {
                    var response = await client.PutAsync($"User/{user.UserId}", content);
                    response.EnsureSuccessStatusCode();
                }

                return RedirectToAction("UserList");
            }
            catch
            {
                TempData["ErrorMessage"] = "Unable to save user.";
                return View("UserList");
            }
        }

        public async Task<IActionResult> UserDelete(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Login");

            AddJwtToken();
            var response = await client.DeleteAsync($"User/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to delete user. Status: {response.StatusCode}";
            }

            return RedirectToAction("UserList");
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("UserRole");

            // Role-based access
            if (role != "Admin")
                return RedirectToAction("AccessDenied", "Login");

            AddJwtToken(); // attach token from Session

            var userId = HttpContext.Session.GetString("UserId");

            var response = await client.GetAsync($"list/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Login");
            }

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<UserModel>>(json);
            return View(list);
        }

        // Optional AccessDenied action
        public IActionResult AccessDenied()
        {
            return View(); // Show a view saying "You don't have permission"
        }
    }
}
