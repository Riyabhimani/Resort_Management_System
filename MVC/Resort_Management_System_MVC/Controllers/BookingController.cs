using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Resort_Management_System_MVC.Models;
using System.Text;

namespace Resort_Management_System_MVC.Controllers
{
    public class BookingController : Controller
    {
        private readonly HttpClient client;

        public BookingController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5159/api/");
        }
        public async Task<IActionResult> BookingList()
        {
            var response = await client.GetAsync("Booking");
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<BookingModel>>(json);
            return View(list);
        }

        public async Task<IActionResult> BookingDelete(int id)
        {
            var response = await client.DeleteAsync($"Booking/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Booking Detail deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to delete booking. Status: {response.StatusCode}";
            }

            return RedirectToAction("BookingList");
        }

        public async Task<IActionResult> BookingAddEdit(int? id)
        {
            try
            {
                BookingModel booking = new BookingModel();

                if (id != null)
                {
                    var response = await client.GetAsync($"Booking/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Booking Detail not found.";
                        return RedirectToAction("BookingList");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    booking = JsonConvert.DeserializeObject<BookingModel>(json);
                }

                return View(booking);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to load form.";
                return RedirectToAction("BookingList");
            }
        }

        [HttpPost]
        public async Task<IActionResult> BookingAddEdit(BookingModel booking)
        {
            if (!ModelState.IsValid)
                return View(booking);

            try
            {
                if (booking.BookingId == 0)
                {
                    booking.Created = DateTime.Now;
                }
                else
                {
                    booking.Modified = DateTime.Now;
                }
                var content = new StringContent(JsonConvert.SerializeObject(booking), Encoding.UTF8, "application/json");

                if (booking.BookingId == 0)
                {
                    var response = await client.PostAsync("Booking", content);
                    response.EnsureSuccessStatusCode();
                    TempData["SuccessMessage"] = "Booking detail Added successfully!";
                }
                else
                {
                    var response = await client.PutAsync($"Booking/{booking.BookingId}", content);
                    response.EnsureSuccessStatusCode();
                    TempData["SuccessMessage"] = "Booking detail updated successfully!";
                }

                return RedirectToAction("BookingList");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unable to save booking detail: {ex.Message}";
                return View(booking);
            }
        }

    }
}