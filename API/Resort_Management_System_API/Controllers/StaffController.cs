using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Resort_Management_System_API.Models;

namespace Resort_Management_System_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {

        #region Configuration Fields 
        private readonly ResortManagementContext context;
        public StaffController(ResortManagementContext context)
        {
            this.context = context;
        }
        #endregion

        #region GetAllStaffs
        [HttpGet]
        public IActionResult GetAllStaffs()
        {
            if (context == null)
                return StatusCode(500, "Database context is null.");

            var staffs = context.Staff.ToList();
            return Ok(staffs);
        }
        #endregion

        #region GetStaffById 
        [HttpGet("{id}")]
        public IActionResult GetStaffById(int id)
        {
            var staff = context.Staff.Find(id);
            if (staff == null)
            {
                return NotFound();
            }
            return Ok(staff);
        }
        #endregion

        #region DeleteStaffById 
        [HttpDelete("{id}")]
        public IActionResult DeleteStaffById(int id)
        {
            var staff = context.Staff.Find(id);

            if (staff == null)
            {
                return NotFound();
            }
            context.Staff.Remove(staff);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region InsertStaff
        [HttpPost]
        public IActionResult InsertStaff(Staff staff)
        {
            context.Staff.Add(staff);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region UpdateStaff
        [HttpPut("{id}")]
        public IActionResult UpdateStaff(int id, Staff staff)
        {
            if (id != staff.StaffId)
            {
                return BadRequest();
            }
            var existingStaff = context.Staff.Find(id);
            if (existingStaff == null)
            {
                return NotFound();
            }
            existingStaff.FullName = staff.FullName;
            existingStaff.Role = staff.Role;
            existingStaff.Email = staff.Email;
            existingStaff.ContactNumber = staff.ContactNumber;
            existingStaff.JoiningDate = staff.JoiningDate;
            existingStaff.Salary = staff.Salary;
            existingStaff.IsActive = staff.IsActive;
            existingStaff.Created = staff.Created;
            existingStaff.Modified = DateTime.Now;


            context.Staff.Update(existingStaff);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region StaffSerialize
        [HttpPost("Serialize")]
        public IActionResult StaffSerialize([FromBody] Staff staff)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Staff data.");
            }

            //Serialize object to JSON string
            var jsonOutput = JsonConvert.SerializeObject(staff, Formatting.Indented);

            //Log or return the Json
            return Ok(new
            {
                Message = "Staff data received successfully",
                JsonData = jsonOutput
            });
        }
        #endregion

        #region StaffDeserializeJson
        [HttpPost("Deserialize")]
        public IActionResult StaffDeserializeJson([FromBody] string json)
        {
            try
            {
                var S = JsonConvert.DeserializeObject<Staff>(json);
                return Ok(new
                {
                    Message = "JSON deserialized successfully",
                    Staff = S
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

        #region GetTop10Staff
        // Get top 10 staffs
        [HttpGet("Top10")]
        public async Task<ActionResult<IEnumerable<Staff>>> GetTop10Staff()
        {
            return await context.Staff.Take(10).ToListAsync();
        }
        #endregion

        #region SearchStaff
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Staff>>> SearchStaff([FromQuery] string? fullName, string? role)
        {
            var query = context.Staff.AsQueryable();

            if (!string.IsNullOrEmpty(fullName))
                query = query.Where(s => s.FullName == fullName);

            if (!string.IsNullOrEmpty(role))
                query = query.Where(s => s.Role == role);

            return await query.ToListAsync();
        }
        #endregion

        #region StaffDropdown
        // Get all Staffs (for dropdown)
        [HttpGet("dropdown/staffs")]
        public async Task<ActionResult<IEnumerable<object>>> StaffDropdown()
        {
            return await context.Staff
                .Select(s => new { s.StaffId, s.FullName })
                .ToListAsync();
        }
        #endregion

    }
}
