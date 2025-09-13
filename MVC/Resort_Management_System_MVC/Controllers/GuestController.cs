//using Microsoft.AspNetCore.Mvc;
//using Resort_Management_System_MVC.Models;
//using Newtonsoft.Json;
//using System.Reflection.Metadata.Ecma335;
//using System.Text;

//namespace Resort_Management_System_MVC.Controllers
//{
//    public class GuestController : Controller
//    {
//        private readonly HttpClient client;


//        public GuestController(IHttpClientFactory httpClientFactory)
//        {
//            client = httpClientFactory.CreateClient();
//            client.BaseAddress = new Uri("http://localhost:5159/api/");
//        }

//        public async Task<IActionResult> GuestList()
//        {
//            var response = await client.GetAsync("Guest");
//            var json = await response.Content.ReadAsStringAsync();
//            var list = JsonConvert.DeserializeObject<List<GuestModel>>(json);
//            return View(list);
//        }
//        public async Task<IActionResult> GuestDelete(int id)
//        {
//            var response = await client.DeleteAsync($"Guest/{id}");

//            if (response.IsSuccessStatusCode)
//            {
//                TempData["SuccessMessage"] = "Guest deleted successfully!";
//            }
//            else
//            {
//                TempData["ErrorMessage"] = $"Failed to delete guest. Status: {response.StatusCode}";
//            }

//            return RedirectToAction("GuestList");
//        }


//        public async Task<IActionResult> GuestAddEdit(int? id)
//        {
//            try
//            {
//                GuestModel guest = new GuestModel();

//                if (id != null)
//                {
//                    var response = await client.GetAsync($"Guest/{id}");
//                    if (!response.IsSuccessStatusCode)
//                    {
//                        TempData["ErrorMessage"] = "Guest not found.";
//                        return RedirectToAction("GuestList");
//                    }

//                    var json = await response.Content.ReadAsStringAsync();
//                    guest = JsonConvert.DeserializeObject<GuestModel>(json);
//                }

//                return View(guest);
//            }
//            catch (Exception ex)
//            {
//                TempData["Error"] = "Unable to load form.";
//                return RedirectToAction("GuestList");
//            }
//        }

//        //[HttpPost]
//        //public async Task<IActionResult> GuestAddEdit(GuestModel guest)
//        //{
//        //    if (!ModelState.IsValid)
//        //        return View("GuestList");

//        //    try
//        //    {
//        //        var content = new StringContent(JsonConvert.SerializeObject(guest), Encoding.UTF8, "application/json");

//        //        if (guest.GuestId == 0)
//        //        {
//        //            var response = await client.PostAsync("Guest", content);
//        //            response.EnsureSuccessStatusCode();
//        //        }
//        //        else
//        //        {
//        //            var response = await client.PutAsync($"Guest/{guest.GuestId}", content);
//        //            response.EnsureSuccessStatusCode();
//        //        }

//        //        return RedirectToAction("GuestList");
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        TempData["Error"] = "Unable to save guest.";
//        //        return View("GuestList");
//        //    }
//        //}

//        [HttpPost]
//        public async Task<IActionResult> GuestAddEdit(GuestModel guest)
//        {
//            if (!ModelState.IsValid)
//                return View(guest);

//            try
//            {
//                var content = new StringContent(JsonConvert.SerializeObject(guest), Encoding.UTF8, "application/json");

//                if (guest.GuestId == 0)
//                {
//                    // ADD
//                    var response = await client.PostAsync("Guest", content);
//                    response.EnsureSuccessStatusCode();

//                    // ✅ SweetAlert Add Message
//                    TempData["SuccessMessage"] = "Guest added successfully!";
//                }
//                else
//                {
//                    // UPDATE
//                    var response = await client.PutAsync($"Guest/{guest.GuestId}", content);
//                    response.EnsureSuccessStatusCode();

//                    // ✅ SweetAlert Edit Message
//                    TempData["SuccessMessage"] = "Guest updated successfully!";
//                }

//                return RedirectToAction("GuestList");
//            }
//            catch (Exception ex)
//            {
//                TempData["ErrorMessage"] = "Unable to save guest.";
//                return View(guest);
//            }
//        }

//        //Get Top 10 Guest
//        public async Task<IActionResult> Top10Guests()
//        {
//            var response = await client.GetAsync("Guest/Top10"); 
//            if (!response.IsSuccessStatusCode)
//            {
//                TempData["ErrorMessage"] = "Unable to fetch top 10 guests.";
//                return RedirectToAction("GuestList");
//            }
//            var json = await response.Content.ReadAsStringAsync();
//            var list = JsonConvert.DeserializeObject<List<GuestModel>>(json);

//            return View("GuestList", list); // reuse same view
//        }

//        //Search Guest
//        public async Task<IActionResult> SearchGuest(string fullName)
//        {
//            var response = await client.GetAsync($"Guest/filter?fullName={fullName}");
//            if (!response.IsSuccessStatusCode)
//            {
//                TempData["ErrorMessage"] = "Guest search failed.";
//                return RedirectToAction("GuestList");
//            }

//            var json = await response.Content.ReadAsStringAsync();
//            var list = JsonConvert.DeserializeObject<List<GuestModel>>(json);
//            return View("GuestList", list); // reuse same view
//        }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using Resort_Management_System_MVC.Models;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Resort_Management_System_MVC.Controllers
{
    public class GuestController : Controller
    {
        private readonly HttpClient client;


        public GuestController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5159/api/");
        }

        public async Task<IActionResult> GuestList()
        {
            var response = await client.GetAsync("Guest");
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<GuestModel>>(json);
            return View(list);
        }
        public async Task<IActionResult> GuestDelete(int id)
        {
            var response = await client.DeleteAsync($"Guest/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Guest deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to delete guest. Status: {response.StatusCode}";
            }

            return RedirectToAction("GuestList");
        }


        public async Task<IActionResult> GuestAddEdit(int? id)
        {
            try
            {
                GuestModel guest = new GuestModel();

                if (id != null)
                {
                    var response = await client.GetAsync($"Guest/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Guest not found.";
                        return RedirectToAction("GuestList");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    guest = JsonConvert.DeserializeObject<GuestModel>(json);
                }

                return View(guest);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to load form.";
                return RedirectToAction("GuestList");
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> GuestAddEdit(GuestModel guest)
        //{
        //    if (!ModelState.IsValid)
        //        return View("GuestList");

        //    try
        //    {
        //        var content = new StringContent(JsonConvert.SerializeObject(guest), Encoding.UTF8, "application/json");

        //        if (guest.GuestId == 0)
        //        {
        //            var response = await client.PostAsync("Guest", content);
        //            response.EnsureSuccessStatusCode();
        //        }
        //        else
        //        {
        //            var response = await client.PutAsync($"Guest/{guest.GuestId}", content);
        //            response.EnsureSuccessStatusCode();
        //        }

        //        return RedirectToAction("GuestList");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = "Unable to save guest.";
        //        return View("GuestList");
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> GuestAddEdit(GuestModel guest)
        {
            if (!ModelState.IsValid)
                return View(guest);

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(guest), Encoding.UTF8, "application/json");

                if (guest.GuestId == 0)
                {
                    // ADD
                    var response = await client.PostAsync("Guest", content);
                    response.EnsureSuccessStatusCode();

                    // ✅ SweetAlert Add Message
                    TempData["SuccessMessage"] = "Guest added successfully!";
                }
                else
                {
                    // UPDATE
                    var response = await client.PutAsync($"Guest/{guest.GuestId}", content);
                    response.EnsureSuccessStatusCode();

                    // ✅ SweetAlert Edit Message
                    TempData["SuccessMessage"] = "Guest updated successfully!";
                }

                return RedirectToAction("GuestList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to save guest.";
                return View(guest);
            }
        }

        //Get Top 10 Guest
        public async Task<IActionResult> Top10Guests()
        {
            var response = await client.GetAsync("Guest/Top10");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to fetch top 10 guests.";
                return RedirectToAction("GuestList");
            }
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<GuestModel>>(json);

            return View("GuestList", list); // reuse same view
        }

        //Search Guest
        public async Task<IActionResult> SearchGuest(string fullName)
        {
            var response = await client.GetAsync($"Guest/filter?fullName={fullName}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Guest search failed.";
                return RedirectToAction("GuestList");
            }

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<GuestModel>>(json);
            return View("GuestList", list); // reuse same view
        }

        [HttpPost]
        public async Task<IActionResult> BulkDeleteGuests([FromBody] List<int> guestIds)
        {
            if (guestIds == null || !guestIds.Any())
                return Json(new { success = false, message = "No guests selected." });

            var content = new StringContent(JsonConvert.SerializeObject(guestIds), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("Guest/BulkDelete", content);

            if (!response.IsSuccessStatusCode)
                return Json(new { success = false, message = "Failed to delete guests." });

            return Json(new { success = true, message = "Selected guests deleted successfully." });
        }




    }
}