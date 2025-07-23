using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Resort_Management_System_MVC.Models;

namespace Resort_Management_System_MVC.Controllers
{
    public class RoomController : Controller
    {
        private readonly HttpClient client;


        public RoomController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5159/api/Room");
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

            try
            {
                await client.DeleteAsync($"Room/{id}");
                //TempData["SuccessMessage"] = "Room deleted successfully.";
                return RedirectToAction("RoomList");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the Room: " + ex.Message;
                return RedirectToAction("RoomList");
            }
        }
    }
}
