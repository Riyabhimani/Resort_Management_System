using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Resort_Management_System_API.Models;

namespace Resort_Management_System_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {

        #region Configuration Fields 
        private readonly ResortManagementContext context;
        public ServiceController(ResortManagementContext context)
        {
            this.context = context;
        }
        #endregion

        #region GetAllServices
        [HttpGet]
        public IActionResult GetAllServices()
        {
            if (context == null)
                return StatusCode(500, "Database context is null.");

            var services = context.Services.ToList();
            return Ok(services);
        }
        #endregion

        #region GetServiceById 
        [HttpGet("{id}")]
        public IActionResult GetServiceById(int id)
        {
            var service = context.Services.Find(id);
            if (service == null)
            {
                return NotFound();
            }
            return Ok(service);
        }
        #endregion

        #region DeleteServiceById 
        [HttpDelete("{id}")]
        public IActionResult DeleteServiceById(int id)
        {
            var service = context.Services.Find(id);

            if (service == null)
            {
                return NotFound();
            }
            context.Services.Remove(service);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region InsertService
        [HttpPost]
        public IActionResult InsertService(Service service)
        {
            context.Services.Add(service);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region UpdateService
        [HttpPut("{id}")]
        public IActionResult UpdateService(int id, Service service)
        {
            if (id != service.ServiceId)
            {
                return BadRequest();
            }
            var existingService = context.Services.Find(id);
            if (existingService == null)
            {
                return NotFound();
            }
            existingService.ServiceName = service.ServiceName;
            existingService.Description = service.Description;
            existingService.ServiceCost = service.ServiceCost;
            existingService.ServiceStartTime = service.ServiceStartTime;
            existingService.ServiceEndTime = service.ServiceEndTime;
            existingService.IsActive = service.IsActive;
            existingService.Created = service.Created;
            existingService.Modified = service.Modified;


            context.Services.Update(existingService);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region ServiceSerialize
        [HttpPost("Serialize")]
        public IActionResult ServiceSerialize([FromBody] Service service)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Service data.");
            }

            //Serialize object to JSON string
            var jsonOutput = JsonConvert.SerializeObject(service, Formatting.Indented);

            //Log or return the Json
            return Ok(new
            {
                Message = "Service data received successfully",
                JsonData = jsonOutput
            });
        }
        #endregion

        #region ServiceDeserializeJson
        [HttpPost("Deserialize")]
        public IActionResult ServiceDeserializeJson([FromBody] string json)
        {
            try
            {
                var s = JsonConvert.DeserializeObject<Service>(json);
                return Ok(new
                {
                    Message = "JSON deserialized successfully",
                    Service = s
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

        #region GetTop10Services
        // Get top 10 users
        [HttpGet("Top10")]
        public async Task<ActionResult<IEnumerable<Service>>> GetTop10Services()
        {
            return await context.Services.Take(10).ToListAsync();
        }
        #endregion

        #region SearchService
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Service>>> SearchService([FromQuery] string? serviceName)
        {
            var query = context.Services.AsQueryable();

            if (!string.IsNullOrEmpty(serviceName))
                query = query.Where(s => s.ServiceName == serviceName);

            return await query.ToListAsync();
        }
        #endregion

        #region ServicesDropdown
        // Get all Services (for dropdown)
        [HttpGet("dropdown/services")]
        public async Task<ActionResult<IEnumerable<object>>> ServicesDropdown()
        {
            return await context.Services
                .Select(s => new { s.ServiceId, s.ServiceName })
                .ToListAsync();
        }
        #endregion

    }
}
