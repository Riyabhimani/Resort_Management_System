using System.Text;
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
            var response = await client.DeleteAsync($"Service/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Guest deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to delete Service. Status: {response.StatusCode}";
            }

            return RedirectToAction("ServiceList");
        }

        public async Task<IActionResult> ServiceAddEdit(int? id)
        {
            try
            {
                ServiceModel guest = new ServiceModel();

                if (id != null)
                {
                    var response = await client.GetAsync($"Service/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "Service not found.";
                        return RedirectToAction("ServiceList");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    guest = JsonConvert.DeserializeObject<ServiceModel>(json);
                }

                return View(guest);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to load form.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ServiceAddEdit(ServiceModel service)
        {
            if (!ModelState.IsValid)
                return View("ServiceList");

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(service), Encoding.UTF8, "application/json");

                if (service.ServiceId == 0)
                {
                    var response = await client.PostAsync("Service", content);
                    response.EnsureSuccessStatusCode();
                }
                else
                {
                    var response = await client.PutAsync($"Service/{service.ServiceId}", content);
                    response.EnsureSuccessStatusCode();
                }

                return RedirectToAction("ServiceList");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to save service.";
                return View("ServiceList");
            }
        }

        //Get Top 10 Service
        public async Task<IActionResult> Top10Services()
        {
            var response = await client.GetAsync("Service/Top10");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to fetch top 10 service details.";
                return RedirectToAction("ServiceList");
            }
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<ServiceModel>>(json);

            return View("ServiceList", list); // reuse same view
        }

        //Search Service
        public async Task<IActionResult> SearchService(string ServiceName)
        {
            var response = await client.GetAsync($"Service/filter?ServiceName={ServiceName}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Service search failed.";
                return RedirectToAction("ServiceList");
            }

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<ServiceModel>>(json);
            return View("ServiceList", list); // reuse same view
        }
    }
}
