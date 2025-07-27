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
        public async Task<IActionResult> PaymentDelete(int id)
        {
            var response = await client.DeleteAsync($"Payment/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Payment detail deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to delete payment detail. Status: {response.StatusCode}";
            }

            return RedirectToAction("PaymentList");
        }

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
                        TempData["ErrorMessage"] = "Payment detail not found.";
                        return RedirectToAction("PaymentList");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    payment = JsonConvert.DeserializeObject<PaymentModel>(json);

                    if (payment.ReservationId != null && payment.ReservationId > 0)
                    {
                        await LoadGuests(payment.ReservationId, payment.GuestId);
                    }
                    else
                    {
                        ViewBag.GuestList = new SelectList(new List<GuestDropdownModel>(), "GuestId", "FullName");
                    }
                }
                else
                {
                    ViewBag.GuestList = new SelectList(new List<GuestDropdownModel>(), "GuestId", "FullName");
                }
                return View(payment);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unable to load payment form: {ex.Message}";
                return RedirectToAction("PaymentList");
            }
        }

        // ✅ POST: Save new or updated city
        [HttpPost]
        public async Task<IActionResult> PaymentAddEdit(PaymentModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadReservations(model.ReservationId);
                await LoadGuests(model.ReservationId, model.GuestId);
                return View(model);
            }

            // Set timestamps
            if (model.PaymentId == 0)
                model.Created = DateTime.Now;
            else
                model.Modified = DateTime.Now;

            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response;

                if (model.PaymentId == 0)
                {
                    response = await client.PostAsync("Payment", content);
                }
                else
                {
                    response = await client.PutAsync($"Payment/{model.PaymentId}", content);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"API call failed: {response.StatusCode} - {error}";
                    await LoadReservations(model.ReservationId);
                    await LoadGuests(model.ReservationId, model.GuestId);
                    return View(model);
                }

                return RedirectToAction("PaymentList");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unable to save payment: {ex.Message}";
                await LoadReservations(model.ReservationId);
                await LoadGuests(model.ReservationId, model.GuestId);
                return View(model);
            }
        }

        //private async Task LoadReservations(int? selectedReservationId = null)
        //{
        //    var response = await client.GetStringAsync("Reservation/dropdown/reservations");
        //    if (!string.IsNullOrWhiteSpace(response))
        //    {
        //        var reservations = JsonConvert.DeserializeObject<List<ReservationDropdownModel>>(response);
        //        ViewBag.ReservationList = new SelectList(reservations, "ReservationId", "ReservationStatus", selectedReservationId);
        //    }
        //    else
        //    {
        //        ViewBag.ReservationList = new SelectList(new List<ReservationDropdownModel>(), "ReservationId", "ReservationStatus");
        //    }

        //}
        //private async Task LoadGuests(int reservationId, int? selectedGuestId = null)
        //{
        //    var response = await client.GetStringAsync($"Guest/dropdown/guests/{reservationId}");
        //    var guests = JsonConvert.DeserializeObject<List<GuestDropdownModel>>(response);
        //    ViewBag.GuestList = new SelectList(guests, "GuestId", "FullName", selectedGuestId);
        //}

        private async Task LoadReservations(int? selectedReservationId = null)
        {
            try
            {
                var response = await client.GetStringAsync("Reservation/dropdown/reservations");
                var reservations = JsonConvert.DeserializeObject<List<ReservationDropdownModel>>(response);
                ViewBag.ReservationList = new SelectList(reservations, "ReservationId", "ReservationStatus", selectedReservationId);
            }
            catch
            {
                ViewBag.ReservationList = new SelectList(new List<ReservationDropdownModel>(), "ReservationId", "ReservationStatus");
            }
        }

        private async Task LoadGuests(int reservationId, int? selectedGuestId = null)
        {
            if (reservationId <= 0)
            {
                ViewBag.GuestList = new SelectList(new List<GuestDropdownModel>(), "GuestId", "FullName");
                return;
            }

            var response = await client.GetStringAsync($"Guest/dropdown/guests/{reservationId}");
            var guests = JsonConvert.DeserializeObject<List<GuestDropdownModel>>(response);
            ViewBag.GuestList = new SelectList(guests, "GuestId", "FullName", selectedGuestId);
        }




    }
}


