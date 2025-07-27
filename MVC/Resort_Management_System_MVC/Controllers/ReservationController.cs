using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Resort_Management_System_MVC.Models;

namespace Resort_Management_System_MVC.Controllers
{
    public class ReservationController : Controller
    {
        private readonly HttpClient client;

        public ReservationController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5159/api/Reservation");
        }
        public async Task<IActionResult> ReservationList()
        {
            var response = await client.GetAsync("Reservation");
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<ReservationModel>>(json);
            return View(list);
        }

        public async Task<IActionResult> ReservationDelete(int id)
        {
            var response = await client.DeleteAsync($"Reservation/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Reservation deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to delete Reservation. Status: {response.StatusCode}";
            }

            return RedirectToAction("ReservationList");
        }

        public async Task<IActionResult> ReservationAddEdit(int? id)
        {
            try
            {
                ReservationModel reservation = new ReservationModel();
                await LoadGuests();
                await LoadRooms();

                if (id != null)
                {
                    var response = await client.GetAsync($"Reservation/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "Reservation not found.";
                        return RedirectToAction("ReservationList");
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    reservation = JsonConvert.DeserializeObject<ReservationModel>(json);

                }

                return View(reservation);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unable to load reservation form: {ex.Message}";
                return RedirectToAction("ReservationList");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReservationAddEdit(ReservationModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadGuests(model.GuestId);
                await LoadRooms(model.RoomId);
                return View(model);
            }

            // Set timestamps
            if (model.ReservationId == 0)
                model.Created = DateTime.UtcNow;
            else
                model.Modified = DateTime.UtcNow;

            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response;

                if (model.ReservationId == 0)
                {
                    response = await client.PostAsync("Reservation", content);
                }
                else
                {
                    response = await client.PutAsync($"Reservation/{model.ReservationId}", content);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"API call failed: {response.StatusCode} - {error}";
                    await LoadGuests(model.GuestId);
                    await LoadRooms(model.RoomId);
                    return View(model);
                }

                return RedirectToAction("ReservationList");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Unable to save reservation: {ex.Message}";
                await LoadGuests(model.GuestId);
                await LoadRooms(model.RoomId);
                return View(model);
            }
        }

        private async Task LoadGuests(int? selectedGuestId = null)
        {
            var response = await client.GetStringAsync("Guest/dropdown/guests");
            var guests = JsonConvert.DeserializeObject<List<GuestDropdownModel>>(response);
            ViewBag.GuestList = new SelectList(guests, "GuestId", "FullName", selectedGuestId);
        }

        private async Task LoadRooms(int? selectedRoomId = null)
        {
            var response = await client.GetStringAsync("Room/dropdown/rooms");
            var rooms = JsonConvert.DeserializeObject<List<RoomDropdownModel>>(response);
            ViewBag.RoomList = new SelectList(rooms, "RoomId", "RoomType", selectedRoomId);
        }
    }
}
