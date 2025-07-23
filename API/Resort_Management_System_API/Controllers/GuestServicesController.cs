using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Resort_Management_System_API.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Resort_Management_System_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestServicesController : ControllerBase
    {
        #region Configuration Fields 
        private readonly ResortManagementContext context;
        public GuestServicesController(ResortManagementContext context)
        {
            this.context = context;
        }
        #endregion

        #region GetAllGuestServices
        [HttpGet]
        public IActionResult GetGuestServices()
        {
            if (context == null)
                return StatusCode(500, "Database context is null.");

            var guestsevices = context.GuestServices.ToList();
            return Ok(guestsevices);
        }
        #endregion

        #region GetGuestServiceById 
        [HttpGet("{id}")]
        public IActionResult GetGuestServiceById(int id)
        {
            var guestservice = context.GuestServices.Find(id);
            if (guestservice == null)
            {
                return NotFound();
            }
            return Ok(guestservice);
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
        public IActionResult InsertGuestService(GuestService guestservice)
        {
            context.GuestServices.Add(guestservice);
            context.SaveChanges();
            return NoContent();
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
            existingGuestServices.Modified = guestservice.Modified;

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

        #region GetTop10GuestServices
        // Get top 10 services
        [HttpGet("Top10")]
        public async Task<ActionResult<IEnumerable<GuestService>>> GetTop10GuestServices()
        {
            return await context.GuestServices.Take(10).ToListAsync();
        }
        #endregion

        #region SearchGuestService
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<GuestService>>> SearchGuestService([FromQuery] int? guestServiceId , int? reservationId, int? guestId, int? serviceId)
        {
            var query = context.GuestServices.AsQueryable();

            if (guestServiceId.HasValue)
                query = query.Where(g => g.GuestServiceId == guestServiceId);

            if (reservationId.HasValue)
                query = query.Where(g => g.ReservationId == reservationId);

            if (guestId.HasValue)
                query = query.Where(g => g.GuestId == guestId);

            if (serviceId.HasValue)
                query = query.Where(g => g.ServiceId == serviceId);

            return await query.ToListAsync();
        }
        #endregion

        #region GuestServicesDropdown
        // Get all GuestSerbices (for dropdown)
        [HttpGet("dropdown/guestServices")]
        public async Task<ActionResult<IEnumerable<object>>> GuestServicesDropdown()
        {
            return await context.GuestServices
                .Select(g => new { g.GuestServiceId, g.GuestId })
                .ToListAsync();
        }
        #endregion

    }
}
