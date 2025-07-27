using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Resort_Management_System_API.Models;

namespace Resort_Management_System_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        #region Configuration Fields 
        private readonly ResortManagementContext context;
        public UserController(ResortManagementContext context)
        {
            this.context = context;
        }
        #endregion

        #region GetAllUsers
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            if (context == null)
                return StatusCode(500, "Database context is null.");

            var users = context.Users.ToList();
            return Ok(users);
        }
        #endregion

        #region GetUserById 
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        #endregion

        #region DeleteUserById 
        [HttpDelete("{id}")]
        public IActionResult DeleteUserById(int id)
        {
            var user = context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }
            context.Users.Remove(user);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region InsertUser
        [HttpPost]
        public IActionResult InsertUser(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region UpdateUser
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }
            var existingUser = context.Users.Find(id);
            if (existingUser == null)
            {
                return NotFound();
            }
            existingUser.UserName = user.UserName;
            existingUser.Password = user.Password;
            existingUser.Role = user.Role;
            //existingUser.Created = user.Created;
            existingUser.Modified = user.Modified;
            existingUser.IsActive = user.IsActive;

            context.Users.Update(existingUser);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region UserSerialize
        [HttpPost("Serialize")]
        public IActionResult UserSerialize([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid User data.");
            }

            //Serialize object to JSON string
            var jsonOutput = JsonConvert.SerializeObject(user, Formatting.Indented);

            //Log or return the Json
            return Ok(new
            {
                Message = "User data received successfully",
                JsonData = jsonOutput
            });
        }
        #endregion

        #region UserDeserializeJson
        [HttpPost("Deserialize")]
        public IActionResult UserDeserializeJson([FromBody] string json)
        {
            try
            {
                var U = JsonConvert.DeserializeObject<User>(json);
                return Ok(new
                {
                    Message = "JSON deserialized successfully",
                    User = U
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

        #region GetTop10Users
        // Get top 10 users
        [HttpGet("Top10")]
        public async Task<ActionResult<IEnumerable<User>>> GetTop10Users()
        {
            return await context.Users.Take(10).ToListAsync();
        }
        #endregion

        #region SearchUser
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<User>>> SearchUser([FromQuery] string? userName)
        {
            var query = context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(userName))
                query = query.Where(u => u.UserName == userName);

            return await query.ToListAsync();
        }
        #endregion

        #region UsersDropdown
        // Get all Users (for dropdown)
        [HttpGet("dropdown/users")]
        public async Task<ActionResult<IEnumerable<object>>> UsersDropdown()
        {
            return await context.Users
                .Select(u => new { u.UserId, u.UserName })
                .ToListAsync();
        }
        #endregion

    }
}