using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            client.BaseAddress = new Uri("http://localhost:5159/api/");
        }

        public async Task<IActionResult> PaymentList()
        {
            var response = await client.GetAsync("Payment");
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<PaymentModel>>(json);
            return View(list);
        }

        [HttpGet("Payment/AddEdit/{id?}")]
        public async Task<IActionResult> PaymentAddEdit(int? id)
        {
            try
            {
                PaymentModel payment = new PaymentModel();
                await LoadReservations();

                if (id != null)
                {
                    var response = await client.GetAsync($"Payment/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Payment not found.";
                        return RedirectToAction("PaymentList");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    payment = JsonConvert.DeserializeObject<PaymentModel>(json);
                    await LoadGuests(payment.ReservationId, payment.GuestId);
                }
                else
                {
                    ViewBag.Guests = new SelectList(new List<GuestModel>(), "GuestId", "FullName");
                }

                return View(payment);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Unable to load payment form: {ex.Message}";
                return RedirectToAction("PaymentList");
            }
        }

        [HttpPost("Payment/AddEdit")]
        public async Task<IActionResult> PaymentAddEdit(PaymentModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadReservations(model.ReservationId);
                await LoadGuests(model.ReservationId, model.GuestId);
                return View(model);
            }

            if (model.PaymentId == 0)
                model.Created = DateTime.UtcNow;
            else
                model.Modified = DateTime.UtcNow;

            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                if (model.PaymentId == 0)
                    response = await client.PostAsync("Payment", content);
                else
                    response = await client.PutAsync($"Payment/{model.PaymentId}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"API call failed: {response.StatusCode} - {error}";
                    await LoadReservations(model.ReservationId);
                    await LoadGuests(model.ReservationId, model.GuestId);
                    return View(model);
                }

                TempData["SuccessMessage"] = model.PaymentId == 0 ? "Payment added successfully." : "Payment updated successfully.";
                return RedirectToAction("PaymentList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Unable to save Payment: {ex.Message}";
                await LoadReservations(model.ReservationId);
                await LoadGuests(model.ReservationId, model.GuestId);
                return View(model);
            }
        }

        //private async Task LoadReservations(int? selectedReservationId = null)
        //{
        //    var response = await client.GetStringAsync("Payment/dropdown/reservations");
        //    var reservations = JsonConvert.DeserializeObject<List<ReservationModel>>(response);
        //    ViewBag.Reservations = new SelectList(reservations, "ReservationId", "ReservationStatus", selectedReservationId);
        //}

        private async Task LoadReservations(int? selectedReservationId = null)
        {
            var response = await client.GetStringAsync("Payment/dropdown/reservations");
            var reservations = JsonConvert.DeserializeObject<List<ReservationDropdownModel>>(response);

            // Optional: remove duplicates (if API side isn't enough)
            var distinctReservations = reservations
                .GroupBy(r => new { r.ReservationId, r.ReservationStatus })
                .Select(g => g.First())
                .ToList();

            ViewBag.Reservations = new SelectList(distinctReservations, "ReservationId", "ReservationStatus", selectedReservationId);
        }


        private async Task LoadGuests(int reservationId, int? selectedGuestId = null)
        {
            var response = await client.GetStringAsync($"Payment/dropdown/guest/{reservationId}");
            var guests = JsonConvert.DeserializeObject<List<GuestModel>>(response);
            ViewBag.Guests = new SelectList(guests, "GuestId", "FullName", selectedGuestId);
        }

        public async Task<JsonResult> GetGuestsByReservation(int reservationId)
        {
            var response = await client.GetStringAsync($"Payment/dropdown/guest/{reservationId}");
            var guests = JsonConvert.DeserializeObject<List<GuestModel>>(response);
            return Json(guests);
        }

        public async Task<IActionResult> PaymentDelete(int id)
        {
            var response = await client.DeleteAsync($"Payment/{id}");

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Payment deleted successfully.";
            else
                TempData["ErrorMessage"] = $"Failed to delete payment. Status: {response.StatusCode}";

            return RedirectToAction("PaymentList");
        }
    }
}
