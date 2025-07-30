using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Resort_Management_System_MVC.Models;
using System.Text;


namespace Resort_Management_System_MVC.Controllers
{
    public class StaffController : Controller
    {
        private readonly HttpClient client;

        public StaffController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5159/api/");
        }
        public async Task<IActionResult> StaffList()
        {
            var response = await client.GetAsync("Staff");
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<StaffModel>>(json);
            return View(list);
        }
        public async Task<IActionResult> StaffDelete(int id)
        {
            var response = await client.DeleteAsync($"Staff/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Staff Detail deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to delete staff. Status: {response.StatusCode}";
            }

            return RedirectToAction("StaffList");
        }

        public async Task<IActionResult> StaffAddEdit(int? id)
        {
            try
            {
                StaffModel staff = new StaffModel();

                if (id != null)
                {
                    var response = await client.GetAsync($"Staff/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "Staff not found.";
                        return RedirectToAction("StaffList");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    staff = JsonConvert.DeserializeObject<StaffModel>(json);
                }

                return View(staff);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to load form.";
                return RedirectToAction("StaffList");
            }
        }

        [HttpPost]
        public async Task<IActionResult> StaffAddEdit(StaffModel staff)
        {
            if (!ModelState.IsValid)
                return View(staff);

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(staff), Encoding.UTF8, "application/json");

                if (staff.StaffId == 0)
                {
                    staff.Created = DateTime.Now;
                    var response = await client.PostAsync("Staff", content);
                    response.EnsureSuccessStatusCode();
                }
                else
                {
                    staff.Modified = DateTime.Now;
                    var response = await client.PutAsync($"Staff/{staff.StaffId}", content);
                    response.EnsureSuccessStatusCode();
                }

                return RedirectToAction("StaffList");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unable to save staff: {ex.Message}";
                return View(staff);
            }
        }

    }
}


//using System.Text;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
//using Resort_Management_System_MVC.Models;

//namespace Resort_Management_System_MVC.Controllers
//{
//    public class StaffController : Controller
//    {
//        private readonly HttpClient client;

//        public StaffController(IHttpClientFactory httpClientFactory)
//        {
//            client = httpClientFactory.CreateClient();
//            client.BaseAddress = new Uri("http://localhost:5159/api/");
//        }
//        public async Task<IActionResult> StaffList()
//        {
//            var response = await client.GetAsync("Staff");
//            var json = await response.Content.ReadAsStringAsync();
//            var list = JsonConvert.DeserializeObject<List<StaffModel>>(json);
//            return View(list);
//        }
//        public async Task<IActionResult> StaffDelete(int id)
//        {
//            var response = await client.DeleteAsync($"Staff/{id}");

//            if (response.IsSuccessStatusCode)
//            {
//                TempData["SuccessMessage"] = "Staff Detail deleted successfully.";
//            }
//            else
//            {
//                TempData["ErrorMessage"] = $"Failed to delete staff. Status: {response.StatusCode}";
//            }

//            return RedirectToAction("StaffList");
//        }

//        public async Task<IActionResult> StaffAddEdit(int? id)
//        {
//            try
//            {
//                StaffModel staff = new StaffModel();

//                if (id != null)
//                {
//                    var response = await client.GetAsync($"Staff/{id}");
//                    if (!response.IsSuccessStatusCode)
//                    {
//                        TempData["Error"] = "Staff not found.";
//                        return RedirectToAction("StaffList");
//                    }

//                    var json = await response.Content.ReadAsStringAsync();
//                    staff = JsonConvert.DeserializeObject<StaffModel>(json);
//                }

//                return View(staff);
//            }
//            catch (Exception ex)
//            {
//                TempData["Error"] = "Unable to load form.";
//                return RedirectToAction("StaffList");
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> StaffAddEdit(StaffModel staff)
//        {
//            if (!ModelState.IsValid)
//                return View(staff);

//            try
//            {
//                var content = new StringContent(JsonConvert.SerializeObject(staff), Encoding.UTF8, "application/json");

//                if (staff.StaffId == 0)
//                {
//                    var response = await client.PostAsync("Staff", content);
//                    response.EnsureSuccessStatusCode();
//                }
//                else
//                {
//                    var response = await client.PutAsync($"Staff/{staff.StaffId}", content);
//                    response.EnsureSuccessStatusCode();
//                }

//                return RedirectToAction("StaffList");
//            }
//            catch (Exception ex)
//            {
//                TempData["Error"] = "Unable to save staff.";
//                return View("StaffList");
//            }
//        }
//    }
//}
