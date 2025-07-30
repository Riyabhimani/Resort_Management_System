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
    public class RoomController : ControllerBase
    {
        #region Configuration Fields 

        private readonly ResortManagementContext context;
        private readonly IValidator<Room> _validator;

        public RoomController(ResortManagementContext context, IValidator<Room> validator)
        {
            this.context = context;
            _validator = validator;
        }

        #endregion

        #region GetAllRooms
        [HttpGet]
        public IActionResult GetAllRooms()
        {
            if (context == null)
                return StatusCode(500, "Database context is null.");

            var rooms = context.Rooms.ToList();
            return Ok(rooms);
        }
        #endregion

        #region GetRoomById 
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoomById(int id)
        {
            var room = await context.Rooms.FindAsync(id);
            if (room == null)
                return NotFound();

            return room;
        }
        #endregion

        #region DeleteRoomById 
        [HttpDelete("{id}")]
        public IActionResult DeleteRoomById(int id)
        {
            var room = context.Rooms.Find(id);

            if (room == null)
            {
                return NotFound();
            }
            context.Rooms.Remove(room);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region InsertRoom
        [HttpPost]
        public async Task<IActionResult> InsertRoom([FromBody] Room room)
        {
            var validationResult = await _validator.ValidateAsync(room);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            context.Rooms.Add(room);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllRooms), new { id = room.RoomId }, room);
        }
        #endregion

        #region UpdateRoom
        [HttpPut("{id}")]
        public IActionResult UpdateRoom(int id, Room room)
        {
            if (id != room.RoomId)
            {
                return BadRequest();
            }
            var existingRoom = context.Rooms.Find(id);
            if (existingRoom == null)
            {
                return NotFound();
            }
            existingRoom.RoomNumber = room.RoomNumber;
            existingRoom.RoomType = room.RoomType;
            existingRoom.Description = room.Description;
            existingRoom.PricePerDay = room.PricePerDay;
            existingRoom.RoomStatus = room.RoomStatus;
            existingRoom.IsActive = room.IsActive;
            existingRoom.Created = room.Created;
            existingRoom.Modified = DateTime.Now;

            context.Rooms.Update(existingRoom);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region RoomSerialize
        [HttpPost("Serialize")]
        public IActionResult RoomSerialize([FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Room data.");
            }

            //Serialize object to JSON string
            var jsonOutput = JsonConvert.SerializeObject(room, Formatting.Indented);

            //Log or return the Json
            return Ok(new
            {
                Message = "Room data received successfully",
                JsonData = jsonOutput
            });
        }
        #endregion

        #region RoomDeserializeJson
        [HttpPost("Deserialize")]
        public IActionResult RoomDeserializeJson([FromBody] string json)
        {
            try
            {
                var r = JsonConvert.DeserializeObject<Room>(json);
                return Ok(new
                {
                    Message = "JSON deserialized successfully",
                    Room = r
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

        #region GetTop10Rooms
        // Get top 10 cities
        [HttpGet("Top10")]
        public async Task<ActionResult<IEnumerable<Room>>> GetTop10Rooms()
        {
            return await context.Rooms.Take(10).ToListAsync();
        }
        #endregion

        #region SearchRoom
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Room>>> SearchRoom([FromQuery] string? roomType, string? roomNumber, string? roomStatus)
        {
            var query = context.Rooms.AsQueryable();

            if (!string.IsNullOrEmpty(roomType))
                query = query.Where(r => r.RoomType == roomType);

            if (!string.IsNullOrEmpty(roomNumber))
                query = query.Where(r => r.RoomNumber == roomNumber);

            if (!string.IsNullOrEmpty(roomStatus))
                query = query.Where(r => r.RoomStatus == roomStatus);

            return await query.ToListAsync();
        }
        #endregion

        #region RoomsDropdown
        // Get all Rooms (for dropdown)
        [HttpGet("dropdown/rooms")]
        public async Task<ActionResult<IEnumerable<object>>> RoomsDropdown()
        {
            return await context.Rooms
                .Select(r => new { r.RoomId, r.RoomNumber, r.RoomType })
                .ToListAsync();
        }
        #endregion

    }
}
