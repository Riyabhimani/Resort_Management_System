using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Resort_Management_System_API.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace Resort_Management_System_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestServicesController : ControllerBase
    {
        #region Configuration Fields 
        private readonly ResortManagementContext context;
        private readonly IValidator<GuestService> _validator;

        public GuestServicesController(ResortManagementContext context, IValidator<GuestService> validator)
        {
            this.context = context;
            _validator = validator;
        }
        #endregion

        #region GetAllGuestServices
        [HttpGet]
        //public IActionResult GetGuestServices()
        //{
        //    if (context == null)
        //        return StatusCode(500, "Database context is null.");

        //    var guestsevices = context.GuestServices.ToList();
        //    return Ok(guestsevices);
        //}

        public async Task<ActionResult> GetGuestServices()
        {
            try
            {
                var guestservices = await context.GuestServices
                    .Include(g => g.Guest)
                    .Include(g => g.Reservation)
                    .Include(g => g.Service)
                    .Select(g => new
                    {
                        g.GuestServiceId,
                        ReservationStatus = g.Reservation.ReservationStatus,
                        ServiceName = g.Service.ServiceName,
                        FullName = g.Guest.FullName,
                        g.Quantity,
                        g.DateRequested,
                        g.Created,
                        g.Modified
                    })
                    .ToListAsync();

                return Ok(guestservices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving guest services: {ex.Message}");
            }
        }

        #endregion

        #region GetGuestServiceById 
        [HttpGet("{id}")]
        public async Task<ActionResult<GuestService>> GetGuestServiceById(int id)
        {
            var guestService = await context.GuestServices.FindAsync(id);
            if (guestService == null)
                return NotFound();

            return guestService;
        }
        #endregion

        #region DeleteGuestServiceById 
        [HttpDelete("{id}")]
        public IActionResult DeleteGuestServiceById(int id)
        {
            var guestservice = context.GuestServices.Find(id);

            if (guestservice == null)
            {
                return NotFound();
            }
            context.GuestServices.Remove(guestservice);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region InsertGuestService
        [HttpPost]
        public async Task<IActionResult> InsertGuestService([FromBody] GuestService guestService)
        {
            var validationResult = await _validator.ValidateAsync(guestService);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            context.GuestServices.Add(guestService);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGuestServices), new { id = guestService.GuestServiceId }, guestService);
        }

        #endregion

        #region UpdateGuestService
        [HttpPut("{id}")]
        public IActionResult UpdateGuestService(int id, GuestService guestservice)
        {
            if (id != guestservice.GuestServiceId)
            {
                return BadRequest();
            }
            var existingGuestServices = context.GuestServices.Find(id);
            if (existingGuestServices == null)
            {
                return NotFound();
            }
            existingGuestServices.ReservationId = guestservice.ReservationId;
            existingGuestServices.ServiceId = guestservice.ServiceId;
            existingGuestServices.Quantity = guestservice.Quantity;
            existingGuestServices.DateRequested = guestservice.DateRequested;
            existingGuestServices.GuestId = guestservice.GuestId;
            existingGuestServices.Created = guestservice.Created;
            existingGuestServices.Modified = DateTime.Now;

            context.GuestServices.Update(existingGuestServices);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region GuestServiceSerialize
        [HttpPost("Serialize")]
        public IActionResult GuestServiceSerialize([FromBody] GuestService guestservice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid GuestService data.");
            }

            //Serialize object to JSON string
            var jsonOutput = JsonConvert.SerializeObject(guestservice, Formatting.Indented);

            //Log or return the Json
            return Ok(new
            {
                Message = "GuestService data received successfully",
                JsonData = jsonOutput
            });
        }
        #endregion

        #region GuestServiceDeserializeJson
        [HttpPost("Deserialize")]
        public IActionResult GuestServiceDeserializeJson([FromBody] string json)
        {
            try
            {
                var gs = JsonConvert.DeserializeObject<GuestService>(json);
                return Ok(new
                {
                    Message = "JSON deserialized successfully",
                    GuestService = gs
                }

                    );
            }
            catch (JsonException ex)
            {
                return (BadRequest(new
                {
                    Message = "Invalid JSON Format",
                    Error = ex.Message
                }));
            }
        }
        #endregion

        // Global Error Handling

        #region GetSuccess
        [HttpGet("success")]
        public IActionResult GetSuccess()
        {
            return Ok(new { Message = "API is working fine" });
        }
        #endregion

        #region GetAllFail
        [HttpGet("fail")]
        public IActionResult GetFailure()
        {
            try
            {
                throw new Exception("This is a test exception.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        #endregion


        [HttpGet("Top10")]
        public async Task<ActionResult> GetTop10GuestServices()
        {
            var guestservices = await context.GuestServices
                .Include(g => g.Guest)
                .Include(g => g.Reservation)
                .Include(g => g.Service)
                .Select(g => new
                {
                    g.GuestServiceId,
                    ReservationStatus = g.Reservation.ReservationStatus,
                    ServiceName = g.Service.ServiceName,
                    FullName = g.Guest.FullName,
                    g.Quantity,
                    g.DateRequested,
                    g.Created,
                    g.Modified
                })
                .Take(10)
                .ToListAsync();

            return Ok(guestservices);
        }

        [HttpGet("filter")]
        public async Task<ActionResult> SearchGuestService([FromQuery] int? guestServiceId, string? reservationStatus, string? fullName, string? service)
        {
            var query = context.GuestServices
                .Include(g => g.Guest)
                .Include(g => g.Reservation)
                .Include(g => g.Service)
                .AsQueryable();

            if (guestServiceId.HasValue)
                query = query.Where(g => g.GuestServiceId == guestServiceId);
            if (!string.IsNullOrEmpty(reservationStatus))
                query = query.Where(u => u.Reservation.ReservationStatus.Contains(reservationStatus));
            if (!string.IsNullOrEmpty(fullName))
                query = query.Where(u => u.Guest.FullName.Contains(fullName));
            if (!string.IsNullOrEmpty(service))
                query = query.Where(u => u.Service.ServiceName.Contains(service));

            var guestservices = await query
                .Select(g => new
                {
                    g.GuestServiceId,
                    ReservationStatus = g.Reservation.ReservationStatus,
                    ServiceName = g.Service.ServiceName,
                    FullName = g.Guest.FullName,
                    g.Quantity,
                    g.DateRequested,
                    g.Created,
                    g.Modified
                })
                .ToListAsync();

            return Ok(guestservices);
        }


        #region GuestServicesDropdown
        // Get all GuestServices (for dropdown)
        //[HttpGet("dropdown/guestServices")]
        //public async Task<ActionResult<IEnumerable<object>>> GuestServicesDropdown()
        //{
        //    return await context.GuestServices
        //        .Select(g => new { g.GuestServiceId, g.GuestId })
        //        .ToListAsync();
        //}

        [HttpGet("dropdown/guestServices")]
        public async Task<ActionResult<IEnumerable<object>>> GuestServicesDropdown([FromQuery] string? status)
        {
            var query = context.Reservations
                .Include(r => r.Guest)
                .Where(r => status == null || r.ReservationStatus == status);

            var result = await query
                .Select(r => new
                {
                    r.Guest.GuestId,
                    r.Guest.FullName
                })
                .Distinct()
                .ToListAsync();

            return Ok(result);
        }
        #endregion

        #region ReservationDropdown
        // ✅ GET: All reservations for dropdown
        [HttpGet("dropdown/reservations")]
        public async Task<ActionResult> ReservationsDropdown()
        {
            var reservations = await context.Reservations
                .Where(r => r.ReservationStatus == "Confirmed")
                .Select(r => new
                {
                    r.ReservationId,
                    r.GuestId,
                    r.ReservationStatus
                })
                .ToListAsync();

            return Ok(reservations);
        }
        #endregion

        #region GET: Guests by ReservationStatus (Cascade Dropdown)
        [HttpGet("dropdown/guests/by-status/")]
        public async Task<IActionResult> GetGuestsByReservationStatus([FromQuery] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest("Reservation status is required (Confirmed or Pending).");
            }

            if (status != "Confirmed")
            {
                return BadRequest("Only 'Confirmed' status is allowed.");
            }

            var guests = await (from g in context.Guests
                                join r in context.Reservations
                                on g.GuestId equals r.GuestId
                                where r.ReservationStatus == status
                                select new
                                {
                                    g.GuestId,
                                    g.FullName,
                                    r.ReservationStatus
                                })
                                .Distinct()
                                .ToListAsync();

            return Ok(guests);
        }
        #endregion

    }
}