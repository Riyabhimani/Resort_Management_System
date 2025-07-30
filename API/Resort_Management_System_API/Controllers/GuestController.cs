//using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Resort_Management_System_API.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.Diagnostics.Metrics;

namespace Resort_Management_System_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : ControllerBase
    {

        #region Configuration Fields 
        //private readonly ResortManagementContext context;
        //public GuestController(ResortManagementContext context)
        //{
        //    this.context = context;
        //}

        private readonly ResortManagementContext context;
        private readonly IValidator<Guest> _validator;

        public GuestController(ResortManagementContext context, IValidator<Guest> validator)
        {
            this.context = context;
            _validator = validator;
        }

        #endregion

        #region GetAllGuests
        [HttpGet]
        public IActionResult GetGuests()
        {
            if (context == null)
                return StatusCode(500, "Database context is null.");

            var guests = context.Guests.ToList();
            return Ok(guests);
        }
        #endregion

        #region GetGuestById 
        [HttpGet("{id}")]
        //public IActionResult GetGuestById(int id)
        //{
        //    var guest = context.Guests.Find(id);
        //    if (guest == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(guest);

        public async Task<ActionResult<Guest>> GetGuestById(int id)
        {
            var guest = await context.Guests.FindAsync(id);
            if (guest == null)
                return NotFound();

            return guest;
        }


        #endregion

        #region DeleteGuestById 
        [HttpDelete("{id}")]
        public IActionResult DeleteGuestById(int id)
        {
            var guest = context.Guests.Find(id);

            if (guest == null)
            {
                return NotFound();
            }
            context.Guests.Remove(guest);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region InsertGuest 
        [HttpPost]
        //public IActionResult InsertGuest(Guest guest)
        //{
        //    context.Guests.Add(guest);
        //    context.SaveChanges();
        //    return NoContent();
        //}

        public async Task<IActionResult> InsertGuest([FromBody] Guest guest)
        {
            var validationResult = await _validator.ValidateAsync(guest);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            context.Guests.Add(guest);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGuests), new { id = guest.GuestId}, guest);
        }

        #endregion

        #region UpdateGuest
        [HttpPut("{id}")]
        public IActionResult UpdateGuest(int id, Guest guest)
        {
            if (id != guest.GuestId)
            {
                return BadRequest();
            }
            var existingGuest = context.Guests.Find(id);
            if (existingGuest == null)
            {
                return NotFound();
            }
            existingGuest.FullName = guest.FullName;
            existingGuest.Email = guest.Email;
            existingGuest.ContactNumber = guest.ContactNumber;
            existingGuest.Address = guest.Address;
            existingGuest.Idproof = guest.Idproof;
            existingGuest.Created = guest.Created;
            existingGuest.Modified = DateTime.Now;

            context.Guests.Update(existingGuest);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region GuestSerialize
        [HttpPost("Serialize")]
        public IActionResult GuestSerialize([FromBody] Guest guest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Guest data.");
            }

            //Serialize object to JSON string
            var jsonOutput = JsonConvert.SerializeObject(guest, Formatting.Indented);

            //Log or return the Json
            return Ok(new
            {
                Message = "Guest data received successfully",
                JsonData = jsonOutput
            });
        }
        #endregion

        #region GuestDeserializeJson
        [HttpPost("Deserialize")]
        public IActionResult GuestDeserializeJson([FromBody] string json)
        {
            try
            {
                var g = JsonConvert.DeserializeObject<Guest>(json);
                return Ok(new
                {
                    Message = "JSON deserialized successfully",
                    Guest = g
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

        #region GetTop10Guests
        // Get top 10 Guests
        [HttpGet("Top10")]
        public async Task<ActionResult<IEnumerable<Guest>>> GetTop10Guests()
        {
            return await context.Guests.Take(10).ToListAsync();
        }
        #endregion

        #region SearchGuest
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Guest>>> SearchGuest([FromQuery] string? fullName)
        {
            var query = context.Guests.AsQueryable();

            if (!string.IsNullOrEmpty(fullName))
                query = query.Where(f => f.FullName == fullName);

            return await query.ToListAsync();
        }
        #endregion

        #region GuestsDropdown
        // Get all guests (for dropdown)
        [HttpGet("dropdown/guests")]
        public async Task<ActionResult<IEnumerable<object>>> GuestsDropdown()
        {
            return await context.Guests
                .Select(g => new { g.GuestId, g.FullName })
                .ToListAsync();
        }
        #endregion

        [HttpGet("by-reservation-status/{status}")]
        public async Task<IActionResult> GetGuestsByReservationStatus(string status)
        {
            var guests = await (from r in context.Reservations
                                join g in context.Guests on r.GuestId equals g.GuestId
                                where r.ReservationStatus == status
                                select new
                                {
                                    g.GuestId,
                                    g.FullName
                                }).Distinct().ToListAsync();

            return Ok(guests);
        }

    }
}




