using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Resort_Management_System_MVC.Models;

namespace Resort_Management_System_MVC.Controllers
{
    public class GuestServiceController : Controller
    {
        private readonly HttpClient client;


        public GuestServiceController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5159/api/GuestServices");
        }

        public async Task<IActionResult> GuestServiceList()
        {
            var response = await client.GetAsync("GuestServices");
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<GuestServiceModel>>(json);
            return View(list);
        }
        public async Task<IActionResult> GuestServiceDelete(int id)
        {

            try
            {
                await client.DeleteAsync($"GuestServices/{id}");
                //TempData["SuccessMessage"] = "GuestServices deleted successfully.";
                return RedirectToAction("GuestServiceList");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the Guest Service: " + ex.Message;
                return RedirectToAction("GuestServiceList");
            }
        }
    }
}
