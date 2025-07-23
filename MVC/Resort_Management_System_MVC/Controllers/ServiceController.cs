using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Resort_Management_System_MVC.Models;

namespace Resort_Management_System_MVC.Controllers
{
    public class ServiceController : Controller
    {

        private readonly HttpClient client;


        public ServiceController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5159/api/Service");
        }

        public async Task<IActionResult> ServiceList()
        {
            var response = await client.GetAsync("Service");
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<ServiceModel>>(json);
            return View(list);
        }
        public async Task<IActionResult> ServiceDelete(int id)
        {

            try
            {
                await client.DeleteAsync($"Service/{id}");
                //TempData["SuccessMessage"] = "Service deleted successfully.";
                return RedirectToAction("ServiceList");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the Service: " + ex.Message;
                return RedirectToAction("ServiceList");
            }
        }

    }
}
