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

        [HttpGet]
        public async Task<IActionResult> PaymentAddEdit(int? id)
        {
            try
            {
                PaymentModel payment = new PaymentModel();

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
                }

                await LoadGuests(payment.GuestId);
                return View(payment);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Unable to load payment form: {ex.Message}";
                return RedirectToAction("PaymentList");
            }
        }


        [HttpPost]
        public async Task<IActionResult> PaymentAddEdit(PaymentModel model)
        {
            // Fetch and set ReservationId based on selected GuestId
            var guestResponse = await client.GetAsync("Reservation");
            if (guestResponse.IsSuccessStatusCode)
            {
                var reservationJson = await guestResponse.Content.ReadAsStringAsync();
                var reservations = JsonConvert.DeserializeObject<List<ReservationModel>>(reservationJson);

                var confirmedReservation = reservations
                    .FirstOrDefault(r => r.GuestId == model.GuestId && r.ReservationStatus == "Confirmed");

                if (confirmedReservation != null)
                    model.ReservationId = confirmedReservation.ReservationId;
                else
                    ModelState.AddModelError("GuestId", "Selected guest does not have a confirmed reservation.");
            }

            if (!ModelState.IsValid)
            {
                await LoadGuests(model.GuestId);
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
                    await LoadGuests(model.GuestId);
                    return View(model);
                }

                TempData["SuccessMessage"] = model.PaymentId == 0 ? "Payment added successfully." : "Payment updated successfully.";
                return RedirectToAction("PaymentList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Unable to save Payment: {ex.Message}";
                await LoadGuests(model.GuestId);
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
            var response = await client.GetStringAsync("Reservation");
            var reservations = JsonConvert.DeserializeObject<List<ReservationModel>>(response);

            // Filter only confirmed
            var confirmedReservations = reservations
                .Where(r => r.ReservationStatus == "Confirmed")
                .Select(r => new { r.ReservationId, r.ReservationStatus })
                .ToList();

            ViewBag.Reservations = new SelectList(confirmedReservations, "ReservationId", "ReservationStatus", selectedReservationId);
        }



        private async Task LoadGuests(int? selectedGuestId = null)
        {
            var reservationResponse = await client.GetAsync("Reservation");
            if (!reservationResponse.IsSuccessStatusCode)
            {
                ViewBag.Guests = new SelectList(new List<GuestModel>(), "GuestId", "FullName");
                return;
            }

            var reservationJson = await reservationResponse.Content.ReadAsStringAsync();
            var reservations = JsonConvert.DeserializeObject<List<ReservationModel>>(reservationJson);

            var confirmedGuestIds = reservations
                .Where(r => r.ReservationStatus == "Confirmed")
                .Select(r => r.GuestId)
                .Distinct()
                .ToList();

            var guestResponse = await client.GetAsync("Guest");
            if (!guestResponse.IsSuccessStatusCode)
            {
                ViewBag.Guests = new SelectList(new List<GuestModel>(), "GuestId", "FullName");
                return;
            }

            var guestJson = await guestResponse.Content.ReadAsStringAsync();
            var guests = JsonConvert.DeserializeObject<List<GuestModel>>(guestJson);

            var filteredGuests = guests
                .Where(g => confirmedGuestIds.Contains(g.GuestId))
                .ToList();

            ViewBag.Guests = new SelectList(filteredGuests, "GuestId", "FullName", selectedGuestId);
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
