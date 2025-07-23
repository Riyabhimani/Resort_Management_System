using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Resort_Management_System_MVC.Models;

namespace Resort_Management_System_MVC.Controllers
{
    public class PaymentController : Controller
    {
        private readonly HttpClient client;


        public PaymentController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5159/api/Payment");
        }

        public async Task<IActionResult> PaymentList()
        {
            var response = await client.GetAsync("Payment");
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<PaymentModel>>(json);
            return View(list);
        }
        public async Task<IActionResult> PaymentDelete(int id)
        {

            try
            {
                await client.DeleteAsync($"Payment/{id}");
                //TempData["SuccessMessage"] = "Payment deleted successfully.";
                return RedirectToAction("PaymentList");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the Payment: " + ex.Message;
                return RedirectToAction("PaymentList");
            }
        }
    }
}
