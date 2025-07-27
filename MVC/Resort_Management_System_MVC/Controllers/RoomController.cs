using System.Text;
using Microsoft.AspNetCore.Mvc;
using Resort_Management_System_MVC.Models;
using Newtonsoft.Json;

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

        [HttpPost]
        public async Task<IActionResult> RoomAddEdit(RoomModel room)
        {
            if (!ModelState.IsValid)
                return View(room);

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(room), Encoding.UTF8, "application/json");

                if (room.RoomId == 0)
                {
                    var response = await client.PostAsync("Room", content);
                    response.EnsureSuccessStatusCode();
                }
                else
                {
                    var response = await client.PutAsync($"Room/{room.RoomId}", content);
                    response.EnsureSuccessStatusCode();
                }

                return RedirectToAction("RoomList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to save room.";
                return View(room);
            }
        }



    }
}