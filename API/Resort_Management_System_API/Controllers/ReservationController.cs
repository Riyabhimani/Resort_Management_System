using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Resort_Management_System_API.Models;

namespace Resort_Management_System_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        #region Configuration Fields 
        private readonly ResortManagementContext context;
        private readonly IValidator<Reservation> _validator;

        public ReservationController(ResortManagementContext context, IValidator<Reservation> validator)
        {
            this.context = context;
            _validator = validator;
        }
        #endregion

        #region GetAllReservations
        [HttpGet]
        public IActionResult GetAllReservations()
        {
            if (context == null)
                return StatusCode(500, "Database context is null.");

            var reservations = context.Reservations.ToList();
            return Ok(reservations);
        }
        #endregion

        #region GetReservationById 
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservationById(int id)
        {
            var reservation = await context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            return reservation;
        }
        #endregion

        #region DeleteReservationById 
        [HttpDelete("{id}")]
        public IActionResult DeleteReservationById(int id)
        {
            var reservation = context.Reservations.Find(id);

            if (reservation == null)
            {
                return NotFound();
            }
            context.Reservations.Remove(reservation);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region InsertReservation
        [HttpPost]
        public async Task<IActionResult> InsertReservation([FromBody] Reservation reservation)
        {
            var validationResult = await _validator.ValidateAsync(reservation);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            context.Reservations.Add(reservation);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllReservations), new { id = reservation.ReservationId }, reservation);
        }
        #endregion

        #region UpdateReservation
        [HttpPut("{id}")]
        public IActionResult UpdateReservation(int id, Reservation reservation)
        {
            if (id != reservation.ReservationId)
            {
                return BadRequest();
            }
            var existingReservation = context.Reservations.Find(id);
            if (existingReservation == null)
            {
                return NotFound();
            }
            existingReservation.GuestId = reservation.GuestId;
            existingReservation.RoomId = reservation.RoomId;
            existingReservation.CheckInDate = reservation.CheckInDate;
            existingReservation.CheckOutDate = reservation.CheckOutDate;
            existingReservation.BookingDate = reservation.BookingDate;
            existingReservation.TotalAmount = reservation.TotalAmount;
            existingReservation.ReservationStatus = reservation.ReservationStatus;
            existingReservation.Created = reservation.Created;
            existingReservation.Modified = DateTime.Now;

            context.Reservations.Update(existingReservation);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region ReservationSerialize
        [HttpPost("Serialize")]
        public IActionResult ReservationSerialize([FromBody] Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Reservation data.");
            }

            //Serialize object to JSON string
            var jsonOutput = JsonConvert.SerializeObject(reservation, Formatting.Indented);

            //Log or return the Json
            return Ok(new
            {
                Message = "Reservation data received successfully",
                JsonData = jsonOutput
            });
        }
        #endregion

        #region ReservationDeserializeJson
        [HttpPost("Deserialize")]
        public IActionResult ReservationDeserializeJson([FromBody] string json)
        {
            try
            {
                var R = JsonConvert.DeserializeObject<Reservation>(json);
                return Ok(new
                {
                    Message = "JSON deserialized successfully",
                    Reservation = R
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

        #region GetTop10Reservations
        // Get top 10 users
        [HttpGet("Top10")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetTop10Reservations()
        {
            return await context.Reservations.Take(10).ToListAsync();
        }
        #endregion

        #region SearchReservation
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Reservation>>> SearchReservation([FromQuery] int? reservationId, int? guestId , int? roomId)
        {
            var query = context.Reservations.AsQueryable();

            if (reservationId.HasValue)
                query = query.Where(r => r.ReservationId == reservationId);

            if (guestId.HasValue)
                query = query.Where(r => r.GuestId == guestId);

            if (roomId.HasValue)
                query = query.Where(r => r.RoomId == roomId);

            return await query.ToListAsync();
        }
        #endregion

        #region ReservationsDropdown
        // Get all Reservations (for dropdown)
        [HttpGet("dropdown/reservations")]
        public async Task<ActionResult<IEnumerable<object>>> ReservationsDropdown()
        {
            return await context.Reservations
                .Select(r => new { r.ReservationId, r.GuestId })
                .ToListAsync();
        }
        #endregion

    }
}
