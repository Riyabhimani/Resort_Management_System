using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Resort_Management_System_MVC.Models;

namespace Resort_Management_System_MVC.Controllers
{
    public class UserController : Controller
    {

        private readonly HttpClient client;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5159/api/User");
        }

        public async Task<IActionResult> UserList()
        {
            var response = await client.GetAsync("User");
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<UserModel>>(json);
            return View(list);
        }

        public async Task<IActionResult> UserAddEdit(int? id)
        {
            try
            {
                UserModel user = new UserModel();

                if (id != null)
                {
                    var response = await client.GetAsync($"User/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "User not found.";
                        return RedirectToAction("UserList");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<UserModel>(json);
                }

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to load form.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UserAddEdit(UserModel user)
        {
            if (!ModelState.IsValid)
                return View("UserList");

            try
            {
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
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to save user.";
                return View("UserList");
            }
        }

        public async Task<IActionResult> UserDelete(int id)
        {

            try
            {
                await client.DeleteAsync($"User/{id}");
                TempData["SuccessMessage"] = "User deleted successfully.";
                return RedirectToAction("UserList");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the User: " + ex.Message;
                return RedirectToAction("UserList");
            }
        }

    }
}
