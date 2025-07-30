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
    public class PaymentController : ControllerBase
    {
        #region Configuration Fields 
        private readonly ResortManagementContext context;
        private readonly IValidator<Payment> _validator;

        public PaymentController(ResortManagementContext context, IValidator<Payment> validator)
        {
            this.context = context;
            _validator = validator;
        }
        #endregion

        #region GetAllPayments
        [HttpGet]
        public IActionResult GetAllPayments()
        {
            if (context == null)
                return StatusCode(500, "Database context is null.");

            var payments = context.Payments.ToList();
            return Ok(payments);
        }
        #endregion

        #region GetPaymentById 
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPaymentById(int id)
        {
            var payment = await context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound();

            return payment;
        }
        #endregion

        #region DeletePaymentById 
        [HttpDelete("{id}")]
        public IActionResult DeletePaymentById(int id)
        {
            var payment = context.Payments.Find(id);

            if (payment == null)
            {
                return NotFound();
            }
            context.Payments.Remove(payment);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region InsertPayment
        [HttpPost]
        public async Task<IActionResult> InsertPayment([FromBody] Payment payment)
        {
            var validationResult = await _validator.ValidateAsync(payment);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            context.Payments.Add(payment);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllPayments), new { id = payment.PaymentId }, payment);
        }

        #endregion

        #region UpdatePayment
        [HttpPut("{id}")]
        public IActionResult UpdatePayment(int id, Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return BadRequest();
            }
            var existingPayment = context.Payments.Find(id);
            if (existingPayment == null)
            {
                return NotFound();
            }
            existingPayment.GuestId = payment.GuestId;
            existingPayment.ReservationId = payment.ReservationId;
            existingPayment.PaymentDate = payment.PaymentDate;
            existingPayment.AmountPaid = payment.AmountPaid;
            existingPayment.PaymentMethod = payment.PaymentMethod;
            existingPayment.PaymentStatus = payment.PaymentStatus;
            existingPayment.Created = payment.Created;
            existingPayment.Modified = DateTime.Now;

            context.Payments.Update(existingPayment);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region PaymentSerialize
        [HttpPost("Serialize")]
        public IActionResult PaymentSerialize([FromBody] Payment payment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Payment data.");
            }

            //Serialize object to JSON string
            var jsonOutput = JsonConvert.SerializeObject(payment, Formatting.Indented);

            //Log or return the Json
            return Ok(new
            {
                Message = "Payment data received successfully",
                JsonData = jsonOutput
            });
        }
        #endregion

        #region PaymentDeserializeJson
        [HttpPost("Deserialize")]
        public IActionResult PaymentDeserializeJson([FromBody] string json)
        {
            try
            {
                var p = JsonConvert.DeserializeObject<Payment>(json);
                return Ok(new
                {
                    Message = "JSON deserialized successfully",
                    Payment = p
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

        #region GetTop10Payments
        // Get top 10 payments
        [HttpGet("Top10")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetTop10Payments()
        {
            return await context.Payments.Take(10).ToListAsync();
        }
        #endregion

        #region SearchPayment
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Payment>>> SearchPayment([FromQuery] int? paymentId, int? guestId, int? reservationId)
        {
            var query = context.Payments.AsQueryable();

            if (paymentId.HasValue)
                query = query.Where(p => p.PaymentId == paymentId);

            if (guestId.HasValue)
                query = query.Where(p => p.GuestId == guestId);

            if (reservationId.HasValue)
                query = query.Where(p => p.ReservationId == reservationId);

            return await query.ToListAsync();
        }
        #endregion

        #region PaymentsDropdown
        // Get all Payments (for dropdown)
        [HttpGet("dropdown/payments")]
        public async Task<ActionResult<IEnumerable<object>>> PaymentsDropdown()
        {
            return await context.Payments
                .Select(p => new { p.PaymentId, p.GuestId })
                .ToListAsync();
        }
        #endregion


        // ✅ GET: All reservations for dropdown
        [HttpGet("dropdown/reservations")]
        public async Task<ActionResult> ReservationsDropdown()
        {
            try
            {
                var reservations = await context.Reservations
                    .Select(r => new { r.ReservationStatus })
                    .Distinct()
                    .ToListAsync();

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving reservations: {ex.Message}");
            }
        }



        #region GET: Guests by ReservationStatus (Cascade Dropdown)
        [HttpGet("dropdown/guests/by-status/")]
        public async Task<IActionResult> GetGuestsByReservationStatus([FromQuery] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest("Reservation status is required (Confirmed or Pending).");
            }

            if (status != "Confirmed" && status != "Pending")
            {
                return BadRequest("Only 'Confirmed' or 'Pending' status is allowed.");
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


