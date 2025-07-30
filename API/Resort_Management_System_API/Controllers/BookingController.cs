using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Resort_Management_System_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace Resort_Management_System_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {

        #region Configuration Fields 
        private readonly ResortManagementContext context;
        private readonly IValidator<Booking> _validator;

        public BookingController(ResortManagementContext context, IValidator<Booking> validator)
        {
            this.context = context;
            _validator = validator;
        }
        #endregion

        #region GetAllBookings
        [HttpGet]
        public IActionResult GetAllBookings()
        {
            if (context == null)
                return StatusCode(500, "Database context is null.");

            var bookings = context.Bookings.ToList();
            return Ok(bookings);
        }
        #endregion

        #region GetBookingById 
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBookingById(int id)
        {
            var booking = await context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            return booking;
        }
        #endregion

        #region DeleteBookingById 
        [HttpDelete("{id}")]
        public IActionResult DeleteBookingById(int id)
        {
            var booking = context.Bookings.Find(id);

            if (booking == null)
            {
                return NotFound();
            }
            context.Bookings.Remove(booking);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region InsertBooking
        [HttpPost]
        public async Task<IActionResult> InsertBooking([FromBody] Booking booking)
        {
            var validationResult = await _validator.ValidateAsync(booking);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllBookings), new { id = booking.BookingId }, booking);
        }
        #endregion

        #region UpdateBooking
        [HttpPut("{id}")]
        public IActionResult UpdateBooking(int id, Booking booking)
        {
            if (id != booking.BookingId)
            {
                return BadRequest();
            }
            var existingBooking = context.Bookings.Find(id);
            if (existingBooking == null)
            {
                return NotFound();
            }
            existingBooking.FullName = booking.FullName;
            existingBooking.Email = booking.Email;
            existingBooking.ContactNumber = booking.ContactNumber;
            existingBooking.Address = booking.Address;
            existingBooking.Idproof = booking.Idproof;
            existingBooking.NumberOfPersons = booking.NumberOfPersons;
            existingBooking.NumberOfRoom = booking.NumberOfRoom;
            existingBooking.BookingDate = booking.BookingDate;
            existingBooking.RoomType = booking.RoomType;
            existingBooking.AdvancePayment = booking.AdvancePayment;
            existingBooking.Created = booking.Created;
            existingBooking.Modified = DateTime.Now;

            context.Bookings.Update(existingBooking);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region BookingSerialize
        [HttpPost("Serialize")]
        public IActionResult BookingSerialize([FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Booking data.");
            }

            //Serialize object to JSON string
            var jsonOutput = JsonConvert.SerializeObject(booking, Formatting.Indented);

            //Log or return the Json
            return Ok(new
            {
                Message = "Booking data received successfully",
                JsonData = jsonOutput
            });
        }
        #endregion

        #region BookingDeserializeJson
        [HttpPost("Deserialize")]
        public IActionResult BookingDeserializeJson([FromBody] string json)
        {
            try
            {
                var B = JsonConvert.DeserializeObject<Booking>(json);
                return Ok(new
                {
                    Message = "JSON deserialized successfully",
                    Booking = B
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

        #region GetTop10Bookings
        // Get top 10 Bookings
        [HttpGet("Top10")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetTop10Bookings()
        {
            return await context.Bookings.Take(10).ToListAsync();
        }
        #endregion

        #region SearchBooking
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Booking>>> SearchBooking([FromQuery] string? fullName, int? numberOfRoom, string? roomType)
        {
            var query = context.Bookings.AsQueryable();

            if (!string.IsNullOrEmpty(fullName))
                query = query.Where(b => b.FullName == fullName);

            if (numberOfRoom.HasValue)
                query = query.Where(b => b.NumberOfRoom == numberOfRoom);

            if (!string.IsNullOrEmpty(roomType))
                query = query.Where(b => b.RoomType == roomType);

            return await query.ToListAsync();
        }
        #endregion

        #region BookingsDropdown
        // Get all bookings (for dropdown)
        [HttpGet("dropdown/bookings")]
        public async Task<ActionResult<IEnumerable<object>>> BookingsDropdown()
        {
            return await context.Bookings
                .Select(b => new { b.BookingId, b.FullName })
                .ToListAsync();
        }
        #endregion

    }
}