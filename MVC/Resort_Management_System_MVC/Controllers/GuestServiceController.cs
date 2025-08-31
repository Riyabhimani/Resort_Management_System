using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            client.BaseAddress = new Uri("http://localhost:5159/api/");
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
                TempData["SuccessMessage"] = "GuestServices deleted successfully.";
                return RedirectToAction("GuestServiceList");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the Guest Service: " + ex.Message;
                return RedirectToAction("GuestServiceList");
            }
        }


        //[HttpGet]
        //public async Task<IActionResult> GuestServiceAddEdit(int? id)
        //{
        //    try
        //    {
        //        GuestServiceModel guestService = new GuestServiceModel();

        //        if (id != null)
        //        {
        //            var response = await client.GetAsync($"GuestServices/{id}");
        //            if (!response.IsSuccessStatusCode)
        //            {
        //                TempData["ErrorMessage"] = "GuestService not found.";
        //                return RedirectToAction("GuestServiceList");
        //            }

        //            var json = await response.Content.ReadAsStringAsync();
        //            guestService = JsonConvert.DeserializeObject<GuestServiceModel>(json);
        //        }

        //        await LoadGuests(guestService.GuestId);
        //        await LoadServices(guestService.ServiceId);
        //        return View(guestService);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = $"Unable to load GuestService form: {ex.Message}";
        //        return RedirectToAction("GuestServiceList");
        //    }
        //}


        //[HttpPost]
        //public async Task<IActionResult> GuestServiceAddEdit(GuestServiceModel model)
        //{
        //    // Fetch and set ReservationId based on selected GuestId
        //    var guestResponse = await client.GetAsync("Reservation");
        //    if (guestResponse.IsSuccessStatusCode)
        //    {
        //        var reservationJson = await guestResponse.Content.ReadAsStringAsync();
        //        var reservations = JsonConvert.DeserializeObject<List<ReservationModel>>(reservationJson);

        //        var confirmedReservation = reservations
        //            .FirstOrDefault(r => r.GuestId == model.GuestId && r.ReservationStatus == "Confirmed");

        //        if (confirmedReservation != null)
        //            model.ReservationId = confirmedReservation.ReservationId;
        //        else
        //            ModelState.AddModelError("GuestId", "Selected guest does not have a confirmed reservation.");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        await LoadGuests(model.GuestId);
        //        await LoadServices(model.ServiceId);
        //        return View(model);
        //    }

        //    if (model.GuestServiceId == 0)
        //        model.Created = DateTime.UtcNow;
        //    else
        //        model.Modified = DateTime.UtcNow;

        //    try
        //    {
        //        var json = JsonConvert.SerializeObject(model);
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //        HttpResponseMessage response;
        //        if (model.GuestServiceId == 0)
        //            response = await client.PostAsync("GuestServices", content); 
        //        else
        //            response = await client.PutAsync($"GuestServices/{model.GuestServiceId}", content); 

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var error = await response.Content.ReadAsStringAsync();
        //            TempData["ErrorMessage"] = $"API call failed: {response.StatusCode} - {error}";
        //            await LoadGuests(model.GuestId);
        //            await LoadServices(model.ServiceId);
        //            return View(model);
        //        }

        //        TempData["SuccessMessage"] = model.GuestServiceId == 0 ? "GuestService added successfully." : "GuestService updated successfully.";
        //        return RedirectToAction("GuestServiceList");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = $"Unable to save GuestService: {ex.Message}";
        //        await LoadGuests(model.GuestId);
        //        await LoadServices(model.ServiceId);
        //        return View(model);
        //    }
        //}



        //private async Task LoadGuests(int? selectedGuestId = null)
        //{
        //    var reservationResponse = await client.GetAsync("Reservation");
        //    if (!reservationResponse.IsSuccessStatusCode)
        //    {
        //        ViewBag.Guests = new SelectList(new List<GuestModel>(), "GuestId", "FullName");
        //        return;
        //    }

        //    var reservationJson = await reservationResponse.Content.ReadAsStringAsync();
        //    var reservations = JsonConvert.DeserializeObject<List<ReservationModel>>(reservationJson);

        //    var confirmedGuestIds = reservations
        //        .Where(r => r.ReservationStatus == "Confirmed")
        //        .Select(r => r.GuestId)
        //        .Distinct()
        //        .ToList();

        //    var guestResponse = await client.GetAsync("Guest");
        //    if (!guestResponse.IsSuccessStatusCode)
        //    {
        //        ViewBag.Guests = new SelectList(new List<GuestModel>(), "GuestId", "FullName");
        //        return;
        //    }

        //    var guestJson = await guestResponse.Content.ReadAsStringAsync();
        //    var guests = JsonConvert.DeserializeObject<List<GuestModel>>(guestJson);

        //    var filteredGuests = guests
        //        .Where(g => confirmedGuestIds.Contains(g.GuestId))
        //        .ToList();

        //    ViewBag.Guests = new SelectList(filteredGuests, "GuestId", "FullName", selectedGuestId);
        //}



        //private async Task LoadGuests(int? selectedGuestId = null)
        //{
        //    // ✅ Call API that returns only Confirmed guests
        //    var response = await client.GetAsync("GuestServices/dropdown/guestServices?status=Confirmed");

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        ViewBag.Guests = new SelectList(new List<GuestModel>(), "GuestId", "FullName");
        //        return;
        //    }

        //    var json = await response.Content.ReadAsStringAsync();
        //    var guests = JsonConvert.DeserializeObject<List<GuestModel>>(json);

        //    ViewBag.Guests = new SelectList(guests, "GuestId", "FullName", selectedGuestId);
        //}


        private async Task LoadServices(int? selectedServiceId = null)
        {
            var serviceResponse = await client.GetAsync("Service");
            if (!serviceResponse.IsSuccessStatusCode)
            {
                ViewBag.Services = new SelectList(new List<ServiceModel>(), "ServiceId", "ServiceName");
                return;
            }

            var serviceJson = await serviceResponse.Content.ReadAsStringAsync();
            var services = JsonConvert.DeserializeObject<List<ServiceModel>>(serviceJson);

            ViewBag.Services = new SelectList(services, "ServiceId", "ServiceName", selectedServiceId);
        }


        public async Task<JsonResult> GetGuestsByReservation(int reservationId)
        {
            var response = await client.GetStringAsync($"GuestServices/dropdown/guestServices/{reservationId}");
            var guests = JsonConvert.DeserializeObject<List<GuestModel>>(response);
            return Json(guests);
        }


        [HttpGet]
        public async Task<IActionResult> GuestServiceAddEdit(int? id)
        {
            GuestServiceModel guestService = new GuestServiceModel();

            if (id != null)
            {
                var response = await client.GetAsync($"GuestServices/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "GuestService not found.";
                    return RedirectToAction("GuestServiceList");
                }

                var json = await response.Content.ReadAsStringAsync();
                guestService = JsonConvert.DeserializeObject<GuestServiceModel>(json);
            }

            await LoadReservations(guestService.ReservationId);
            await LoadGuests(guestService.ReservationId, guestService.GuestId);
            await LoadServices(guestService.ServiceId);

            return View(guestService);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuestServiceAddEdit(GuestServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                // reload dropdowns if validation fails
                await LoadReservations(model.ReservationId);
                await LoadGuests(model.ReservationId, model.GuestId);
                await LoadServices(model.ServiceId);
                return View(model);
            }

            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            if (model.GuestServiceId == 0) // Add new
            {
                response = await client.PostAsync("GuestServices", content);
            }
            else // Update existing
            {
                response = await client.PutAsync($"GuestServices/{model.GuestServiceId}", content);
            }

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Guest Service saved successfully.";
                return RedirectToAction("GuestServiceList");
            }
            else
            {
                TempData["ErrorMessage"] = "Error while saving Guest Service.";
                // reload dropdowns again in case of error
                await LoadReservations(model.ReservationId);
                await LoadGuests(model.ReservationId, model.GuestId);
                await LoadServices(model.ServiceId);
                return View(model);
            }
        }


        private async Task LoadReservations(int? selectedReservationId = null)
        {
            var response = await client.GetAsync("GuestServices/dropdown/reservations");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Reservations = new SelectList(new List<ReservationModel>(), "ReservationId", "ReservationStatus");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var reservations = JsonConvert.DeserializeObject<List<ReservationModel>>(json);

            ViewBag.Reservations = new SelectList(reservations, "ReservationId", "ReservationStatus", selectedReservationId);
        }

        private async Task LoadGuests(int? reservationId = null, int? selectedGuestId = null)
        {
            if (reservationId == null)
            {
                ViewBag.Guests = new SelectList(new List<GuestModel>(), "GuestId", "FullName");
                return;
            }

            var response = await client.GetAsync($"GuestServices/dropdown/guests/{reservationId}");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Guests = new SelectList(new List<GuestModel>(), "GuestId", "FullName");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var guests = JsonConvert.DeserializeObject<List<GuestModel>>(json);

            ViewBag.Guests = new SelectList(guests, "GuestId", "FullName", selectedGuestId);
        }





    }
}
