using System.Text;
using Microsoft.AspNetCore.Mvc;
using Resort_Management_System_MVC.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Resort_Management_System_MVC.Controllers
{
    public class RoomController : Controller
    {
        private readonly HttpClient client;
        public RoomController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5159/api/");
        }

        public async Task<IActionResult> RoomList()
        {
            var response = await client.GetAsync("Room");
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<RoomModel>>(json);
            return View(list);
        }

        public async Task<IActionResult> RoomDetails(int page = 1, int pageSize = 10)
        {
            var response = await client.GetAsync("Room"); // API call to get all rooms
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to load rooms.";
                return View(new List<RoomModel>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var rooms = JsonConvert.DeserializeObject<List<RoomModel>>(json);

            // Pagination logic
            var totalRooms = rooms.Count;
            var paginatedRooms = rooms.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRooms = totalRooms;

            return View(paginatedRooms);
        }


        public async Task<IActionResult> RoomDelete(int id)
        {
            var response = await client.DeleteAsync($"Room/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Room deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to delete room. Status: {response.StatusCode}";
            }

            return RedirectToAction("RoomList");
        }

        public async Task<IActionResult> RoomAddEdit(int? id)
        {
            try
            {
                RoomModel room = new RoomModel();

                if (id != null)
                {
                    var response = await client.GetAsync($"Room/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Room not found.";
                        return RedirectToAction("RoomList");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    room = JsonConvert.DeserializeObject<RoomModel>(json);
                }

                return View(room);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to load form.";
                return RedirectToAction("Index");
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> RoomAddEdit(RoomModel room)
        //{
        //    if (!ModelState.IsValid)
        //        return View(room);

        //    try
        //    {
        //        var content = new StringContent(JsonConvert.SerializeObject(room), Encoding.UTF8, "application/json");

        //        if (room.RoomId == 0)
        //        {
        //            var response = await client.PostAsync("Room", content);
        //            response.EnsureSuccessStatusCode();
        //        }
        //        else
        //        {
        //            var response = await client.PutAsync($"Room/{room.RoomId}", content);
        //            response.EnsureSuccessStatusCode();
        //        }

        //        return RedirectToAction("RoomList");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Unable to save room.";
        //        return View(room);
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> RoomAddEdit(RoomModel room)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "⚠ Validation failed. Please check inputs.";
                return View(room);
            }

            try
            {
                HttpResponseMessage response;

                // Step 1: Save room details (without image)
                var jsonContent = new StringContent(JsonConvert.SerializeObject(room), Encoding.UTF8, "application/json");

                if (room.RoomId == 0)
                {
                    response = await client.PostAsync("Room", jsonContent);

                    var errorDetails = await response.Content.ReadAsStringAsync(); // 👈 add this line

                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = $"❌ Failed to add room. API Response: {response.StatusCode} - {errorDetails}";
                        return View(room);
                    }

                    var createdJson = await response.Content.ReadAsStringAsync();
                    var createdRoom = JsonConvert.DeserializeObject<RoomModel>(createdJson);
                    room.RoomId = createdRoom.RoomId;
                }

                else
                {
                    response = await client.PutAsync($"Room/{room.RoomId}", jsonContent);
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "❌ Failed to update room.";
                        return View(room);
                    }
                }

                // Step 2: Upload picture if provided
                if (room.RoomImageFile != null && room.RoomImageFile.Length > 0)
                {
                    using var formData = new MultipartFormDataContent();
                    var fileStream = room.RoomImageFile.OpenReadStream();
                    var fileContent = new StreamContent(fileStream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(room.RoomImageFile.ContentType);

                    formData.Add(fileContent, "file", room.RoomImageFile.FileName);

                    var uploadResponse = await client.PostAsync($"Room/{room.RoomId}/upload-picture", formData);
                    if (!uploadResponse.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "⚠ Room saved but image upload failed.";
                    }
                }

                TempData["SuccessMessage"] = room.RoomId == 0 ? "✅ Room added successfully." : "✅ Room updated successfully.";
                return RedirectToAction("RoomList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "❌ Unable to save room: " + ex.Message;
                return View(room);
            }
        }




        //Get Top 10 Rooms
        public async Task<IActionResult> Top10Rooms()
        {
            var response = await client.GetAsync("Room/Top10");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to fetch top 10 room details.";
                return RedirectToAction("RoomList");
            }
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<RoomModel>>(json);

            return View("RoomList", list); // reuse same view
        }


        //Search Rooms
        [HttpGet]
        public async Task<IActionResult> SearchRoom(string? roomType, string? roomNumber, string? roomStatus)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(roomType)) queryParams.Add($"roomType={Uri.EscapeDataString(roomType)}");
            if (!string.IsNullOrEmpty(roomNumber)) queryParams.Add($"roomNumber={Uri.EscapeDataString(roomNumber)}");
            if (!string.IsNullOrEmpty(roomStatus)) queryParams.Add($"roomStatus={Uri.EscapeDataString(roomStatus)}");

            var url = "Room/filter";
            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var rooms = await client.GetFromJsonAsync<List<RoomModel>>(url);

            if (rooms == null || !rooms.Any())
            {
                TempData["ErrorMessage"] = "⚠ No rooms found.";
                return View("RoomList", new List<RoomModel>());
            }

            return View("RoomList", rooms);
        }
    }
}





//using System.Text;
//using Microsoft.AspNetCore.Mvc;
//using Resort_Management_System_MVC.Models;
//using Newtonsoft.Json;

//namespace Resort_Management_System_MVC.Controllers
//{
//    public class RoomController : Controller
//    {
//        private readonly HttpClient client;


//        public RoomController(IHttpClientFactory httpClientFactory)
//        {
//            client = httpClientFactory.CreateClient();
//            client.BaseAddress = new Uri("http://localhost:5159/api/");
//        }

//        public async Task<IActionResult> RoomList()
//        {
//            var response = await client.GetAsync("Room");
//            var json = await response.Content.ReadAsStringAsync();
//            var list = JsonConvert.DeserializeObject<List<RoomModel>>(json);
//            return View(list);
//        }

//        public async Task<IActionResult> RoomDelete(int id)
//        {
//            var response = await client.DeleteAsync($"Room/{id}");

//            if (response.IsSuccessStatusCode)
//            {
//                TempData["SuccessMessage"] = "Room deleted successfully.";
//            }
//            else
//            {
//                TempData["ErrorMessage"] = $"Failed to delete room. Status: {response.StatusCode}";
//            }

//            return RedirectToAction("RoomList");
//        }

//        public async Task<IActionResult> RoomAddEdit(int? id)
//        {
//            try
//            {
//                RoomModel room = new RoomModel();

//                if (id != null)
//                {
//                    var response = await client.GetAsync($"Room/{id}");
//                    if (!response.IsSuccessStatusCode)
//                    {
//                        TempData["ErrorMessage"] = "Room not found.";
//                        return RedirectToAction("RoomList");
//                    }

//                    var json = await response.Content.ReadAsStringAsync();
//                    room = JsonConvert.DeserializeObject<RoomModel>(json);
//                }

//                return View(room);
//            }
//            catch (Exception ex)
//            {
//                TempData["ErrorMessage"] = "Unable to load form.";
//                return RedirectToAction("Index");
//            }
//        }

//        //[HttpPost]
//        //public async Task<IActionResult> RoomAddEdit(RoomModel room)
//        //{
//        //    if (!ModelState.IsValid)
//        //        return View(room);

//        //    try
//        //    {
//        //        var content = new StringContent(JsonConvert.SerializeObject(room), Encoding.UTF8, "application/json");

//        //        if (room.RoomId == 0)
//        //        {
//        //            var response = await client.PostAsync("Room", content);
//        //            response.EnsureSuccessStatusCode();
//        //        }
//        //        else
//        //        {
//        //            var response = await client.PutAsync($"Room/{room.RoomId}", content);
//        //            response.EnsureSuccessStatusCode();
//        //        }

//        //        return RedirectToAction("RoomList");
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        TempData["ErrorMessage"] = "Unable to save room.";
//        //        return View(room);
//        //    }
//        //}


//        [HttpPost]
//        public async Task<IActionResult> RoomAddEdit(RoomModel room)
//        {
//            if (!ModelState.IsValid)
//                return View(room);

//            try
//            {
//                if (room.RoomId == 0)
//                {
//                    // Save room details first (without image)
//                    var content = new StringContent(JsonConvert.SerializeObject(room), Encoding.UTF8, "application/json");
//                    var response = await client.PostAsync("Room", content);
//                    response.EnsureSuccessStatusCode();

//                    var createdJson = await response.Content.ReadAsStringAsync();
//                    var createdRoom = JsonConvert.DeserializeObject<RoomModel>(createdJson);

//                    // Upload image if provided
//                    if (room.RoomImageFile != null)
//                    {
//                        var form = new MultipartFormDataContent();
//                        using var stream = room.RoomImageFile.OpenReadStream();
//                        form.Add(new StreamContent(stream), "file", room.RoomImageFile.FileName);

//                        await client.PostAsync($"Room/{createdRoom.RoomId}/upload-picture", form);
//                    }
//                }
//                else
//                {
//                    // Update room details
//                    var content = new StringContent(JsonConvert.SerializeObject(room), Encoding.UTF8, "application/json");
//                    var response = await client.PutAsync($"Room/{room.RoomId}", content);
//                    response.EnsureSuccessStatusCode();

//                    // Upload new image if selected
//                    if (room.RoomImageFile != null)
//                    {
//                        var form = new MultipartFormDataContent();
//                        using var stream = room.RoomImageFile.OpenReadStream();
//                        form.Add(new StreamContent(stream), "file", room.RoomImageFile.FileName);

//                        await client.PostAsync($"Room/{room.RoomId}/upload-picture", form);
//                    }
//                }

//                return RedirectToAction("RoomList");
//            }
//            catch (Exception ex)
//            {
//                TempData["ErrorMessage"] = "Unable to save room. " + ex.Message;
//                return View(room);
//            }
//        }




//        //Get Top 10 Rooms
//        public async Task<IActionResult> Top10Rooms()
//        {
//            var response = await client.GetAsync("Room/Top10");
//            if (!response.IsSuccessStatusCode)
//            {
//                TempData["ErrorMessage"] = "Unable to fetch top 10 room details.";
//                return RedirectToAction("RoomList");
//            }
//            var json = await response.Content.ReadAsStringAsync();
//            var list = JsonConvert.DeserializeObject<List<RoomModel>>(json);

//            return View("RoomList", list); // reuse same view
//        }


//        //Search Rooms
//        [HttpGet]
//        public async Task<IActionResult> SearchRoom(string? roomType, string? roomNumber, string? roomStatus)
//        {
//            var queryParams = new List<string>();
//            if (!string.IsNullOrEmpty(roomType)) queryParams.Add($"roomType={Uri.EscapeDataString(roomType)}");
//            if (!string.IsNullOrEmpty(roomNumber)) queryParams.Add($"roomNumber={Uri.EscapeDataString(roomNumber)}");
//            if (!string.IsNullOrEmpty(roomStatus)) queryParams.Add($"roomStatus={Uri.EscapeDataString(roomStatus)}");

//            var url = "Room/filter";
//            if (queryParams.Any())
//                url += "?" + string.Join("&", queryParams);

//            var rooms = await client.GetFromJsonAsync<List<RoomModel>>(url);

//            if (rooms == null || !rooms.Any())
//            {
//                TempData["ErrorMessage"] = "⚠ No rooms found.";
//                return View("RoomList", new List<RoomModel>());
//            }

//            return View("RoomList", rooms);
//        }
//    }
//}






//using System.Text;
//using Microsoft.AspNetCore.Mvc;
//using Resort_Management_System_MVC.Models;
//using Newtonsoft.Json;

//namespace Resort_Management_System_MVC.Controllers
//{
//    public class RoomController : Controller
//    {
//        private readonly HttpClient client;


//        public RoomController(IHttpClientFactory httpClientFactory)
//        {
//            client = httpClientFactory.CreateClient();
//            client.BaseAddress = new Uri("http://localhost:5159/api/");
//        }

//        public async Task<IActionResult> RoomList()
//        {
//            var response = await client.GetAsync("Room");
//            var json = await response.Content.ReadAsStringAsync();
//            var list = JsonConvert.DeserializeObject<List<RoomModel>>(json);
//            return View(list);
//        }

//        public async Task<IActionResult> RoomDelete(int id)
//        {
//            var response = await client.DeleteAsync($"Room/{id}");

//            if (response.IsSuccessStatusCode)
//            {
//                TempData["SuccessMessage"] = "Room deleted successfully.";
//            }
//            else
//            {
//                TempData["ErrorMessage"] = $"Failed to delete room. Status: {response.StatusCode}";
//            }

//            return RedirectToAction("RoomList");
//        }

//        public async Task<IActionResult> RoomAddEdit(int? id)
//        {
//            try
//            {
//                RoomModel room = new RoomModel();

//                if (id != null)
//                {
//                    var response = await client.GetAsync($"Room/{id}");
//                    if (!response.IsSuccessStatusCode)
//                    {
//                        TempData["ErrorMessage"] = "Room not found.";
//                        return RedirectToAction("RoomList");
//                    }

//                    var json = await response.Content.ReadAsStringAsync();
//                    room = JsonConvert.DeserializeObject<RoomModel>(json);
//                }

//                return View(room);
//            }
//            catch (Exception ex)
//            {
//                TempData["ErrorMessage"] = "Unable to load form.";
//                return RedirectToAction("Index");
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> RoomAddEdit(RoomModel room)
//        {
//            if (!ModelState.IsValid)
//                return View(room);

//            try
//            {
//                var content = new StringContent(JsonConvert.SerializeObject(room), Encoding.UTF8, "application/json");

//                if (room.RoomId == 0)
//                {
//                    var response = await client.PostAsync("Room", content);
//                    response.EnsureSuccessStatusCode();
//                }
//                else
//                {
//                    var response = await client.PutAsync($"Room/{room.RoomId}", content);
//                    response.EnsureSuccessStatusCode();
//                }

//                return RedirectToAction("RoomList");
//            }
//            catch (Exception ex)
//            {
//                TempData["ErrorMessage"] = "Unable to save room.";
//                return View(room);
//            }
//        }

//        //Get Top 10 Rooms
//        public async Task<IActionResult> Top10Rooms()
//        {
//            var response = await client.GetAsync("Room/Top10");
//            if (!response.IsSuccessStatusCode)
//            {
//                TempData["ErrorMessage"] = "Unable to fetch top 10 room details.";
//                return RedirectToAction("RoomList");
//            }
//            var json = await response.Content.ReadAsStringAsync();
//            var list = JsonConvert.DeserializeObject<List<RoomModel>>(json);

//            return View("RoomList", list); // reuse same view
//        }


//        //Search Rooms
//        [HttpGet]
//        public async Task<IActionResult> SearchRoom(string? roomType, string? roomNumber, string? roomStatus)
//        {
//            var queryParams = new List<string>();
//            if (!string.IsNullOrEmpty(roomType)) queryParams.Add($"roomType={Uri.EscapeDataString(roomType)}");
//            if (!string.IsNullOrEmpty(roomNumber)) queryParams.Add($"roomNumber={Uri.EscapeDataString(roomNumber)}");
//            if (!string.IsNullOrEmpty(roomStatus)) queryParams.Add($"roomStatus={Uri.EscapeDataString(roomStatus)}");

//            var url = "Room/filter";
//            if (queryParams.Any())
//                url += "?" + string.Join("&", queryParams);

//            var rooms = await client.GetFromJsonAsync<List<RoomModel>>(url);

//            if (rooms == null || !rooms.Any())
//            {
//                TempData["ErrorMessage"] = "⚠ No rooms found.";
//                return View("RoomList", new List<RoomModel>());
//            }

//            return View("RoomList", rooms);
//        }
//    }
//}