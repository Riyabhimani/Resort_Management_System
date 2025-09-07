using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Resort_Management_System_API.Models;
using System.Globalization;

namespace Resort_Management_System_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ResortManagementContext _context;

        public DashboardController(ResortManagementContext context)
        {
            _context = context;
        }

        // GET: api/dashboard/stats
        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            try
            {
                var today = DateTime.Today;
                var currentMonth = new DateTime(today.Year, today.Month, 1);

                var stats = new DashboardStatsDto
                {
                    TotalUsers = await _context.Users.CountAsync(u => u.IsActive),
                    TotalGuests = await _context.Guests.CountAsync(),
                    TodayReservations = await _context.Reservations.CountAsync(r => r.BookingDate.Date == today),
                    TodayBookings = await _context.Bookings.CountAsync(b => b.BookingDate.Date == today),
                    TodayRevenue = await _context.Payments
                        .Where(p => p.PaymentDate.Date == today && p.PaymentStatus.ToLower() == "completed")
                        .SumAsync(p => p.AmountPaid),
                    AvailableRooms = await _context.Rooms.CountAsync(r => r.RoomStatus.ToLower() == "available" && r.IsActive),

                    // Additional stats for better insights
                    TotalRooms = await _context.Rooms.CountAsync(r => r.IsActive),
                    OccupiedRooms = await _context.Rooms.CountAsync(r => r.RoomStatus.ToLower() == "occupied" && r.IsActive),
                    PendingReservations = await _context.Reservations.CountAsync(r => r.ReservationStatus.ToLower() == "pending"),
                    CompletedReservations = await _context.Reservations.CountAsync(r => r.ReservationStatus.ToLower() == "completed"),
                    MonthlyRevenue = await _context.Payments
                        .Where(p => p.PaymentDate >= currentMonth && p.PaymentStatus.ToLower() == "completed")
                        .SumAsync(p => p.AmountPaid),

                    // Calculate percentage changes (comparing with previous day/month)
                    YesterdayReservations = await _context.Reservations.CountAsync(r => r.BookingDate.Date == today.AddDays(-1)),
                    YesterdayRevenue = await _context.Payments
                        .Where(p => p.PaymentDate.Date == today.AddDays(-1) && p.PaymentStatus.ToLower() == "completed")
                        .SumAsync(p => p.AmountPaid),
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching dashboard statistics", error = ex.Message });
            }
        }

        // GET: api/dashboard/revenue-chart
        [HttpGet("revenue-chart")]
        public async Task<ActionResult<List<MonthlyRevenueDto>>> GetMonthlyRevenue()
        {
            try
            {
                var currentYear = DateTime.Now.Year;
                var monthlyRevenue = new List<MonthlyRevenueDto>();

                for (int month = 1; month <= 12; month++)
                {
                    var startDate = new DateTime(currentYear, month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);

                    var revenue = await _context.Payments
                        .Where(p => p.PaymentDate >= startDate &&
                                   p.PaymentDate <= endDate &&
                                   p.PaymentStatus.ToLower() == "completed")
                        .SumAsync(p => p.AmountPaid);

                    monthlyRevenue.Add(new MonthlyRevenueDto
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                        Revenue = revenue,
                        MonthNumber = month
                    });
                }

                return Ok(monthlyRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching revenue chart data", error = ex.Message });
            }
        }

        // GET: api/dashboard/occupancy
        [HttpGet("occupancy")]
        public async Task<ActionResult<OccupancyDto>> GetRoomOccupancy()
        {
            try
            {
                var totalRooms = await _context.Rooms.CountAsync(r => r.IsActive);
                var occupiedRooms = await _context.Rooms.CountAsync(r => r.RoomStatus.ToLower() == "occupied" && r.IsActive);
                var availableRooms = await _context.Rooms.CountAsync(r => r.RoomStatus.ToLower() == "available" && r.IsActive);
                var maintenanceRooms = await _context.Rooms.CountAsync(r => r.RoomStatus.ToLower() == "maintenance" && r.IsActive);

                var occupancy = new OccupancyDto
                {
                    Labels = new List<string> { "Occupied", "Available", "Maintenance" },
                    Data = new List<int> { occupiedRooms, availableRooms, maintenanceRooms },
                    Total = totalRooms,
                    OccupancyRate = totalRooms > 0 ? Math.Round((double)occupiedRooms / totalRooms * 100, 1) : 0
                };

                return Ok(occupancy);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching occupancy data", error = ex.Message });
            }
        }

        // GET: api/dashboard/recent-reservations
        [HttpGet("recent-reservations")]
        public async Task<ActionResult<List<RecentReservationDto>>> GetRecentReservations()
        {
            try
            {
                var recentReservations = await _context.Reservations
                    .Include(r => r.Guest)
                    .Include(r => r.Room)
                    .OrderByDescending(r => r.BookingDate)
                    .Take(10)
                    .Select(r => new RecentReservationDto
                    {
                        ReservationId = r.ReservationId,
                        GuestName = r.Guest.FullName,
                        RoomNumber = r.Room.RoomNumber,
                        RoomType = r.Room.RoomType,
                        CheckInDate = r.CheckInDate.ToString("MMM dd, yyyy"),
                        CheckOutDate = r.CheckOutDate.ToString("MMM dd, yyyy"),
                        Status = r.ReservationStatus,
                        Amount = r.TotalAmount,
                        BookingDate = r.BookingDate.ToString("MMM dd, yyyy HH:mm")
                    })
                    .ToListAsync();

                return Ok(recentReservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching recent reservations", error = ex.Message });
            }
        }

        // GET: api/dashboard/today-payments
        [HttpGet("today-payments")]
        public async Task<ActionResult<List<TodayPaymentDto>>> GetTodayPayments()
        {
            try
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);
                var todayPayments = await _context.Payments
                    .Include(p => p.Guest)
                    .Where(p => p.PaymentDate >= today && p.PaymentDate < tomorrow)
                    .OrderByDescending(p => p.PaymentDate)
                    .Take(10)
                    .Select(p => new TodayPaymentDto
                    {
                        PaymentId = p.PaymentId,
                        GuestName = p.Guest.FullName,
                        Service = "Room Payment", // You can enhance this by joining with GuestServices
                        PaymentTime = p.PaymentDate.ToString("HH:mm"),
                        Amount = p.AmountPaid,
                        PaymentMethod = p.PaymentMethod,
                        Status = p.PaymentStatus
                    })
                    .ToListAsync();

                return Ok(todayPayments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching today's payments", error = ex.Message });
            }
        }

        // GET: api/dashboard/guest-services
        [HttpGet("guest-services")]
        public async Task<ActionResult<List<GuestServiceStatsDto>>> GetGuestServiceStats()
        {
            try
            {
                var serviceStats = await _context.GuestServices
                    .Include(gs => gs.Service)
                    .GroupBy(gs => gs.Service.ServiceName)
                    .Select(g => new GuestServiceStatsDto
                    {
                        ServiceName = g.Key,
                        TotalRequests = g.Count(),
                        TotalRevenue = g.Sum(gs => gs.Service.ServiceCost * gs.Quantity)
                    })
                    .OrderByDescending(s => s.TotalRequests)
                    .Take(5)
                    .ToListAsync();

                return Ok(serviceStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching guest service statistics", error = ex.Message });
            }
        }

        // GET: api/dashboard/room-type-stats
        [HttpGet("room-type-stats")]
        public async Task<ActionResult<List<RoomTypeStatsDto>>> GetRoomTypeStats()
        {
            try
            {
                var roomStats = await _context.Reservations
                    .Include(r => r.Room)
                    .Where(r => r.ReservationStatus.ToLower() != "cancelled")
                    .GroupBy(r => r.Room.RoomType)
                    .Select(g => new RoomTypeStatsDto
                    {
                        RoomType = g.Key,
                        TotalReservations = g.Count(),
                        TotalRevenue = g.Sum(r => r.TotalAmount),
                        AverageStay = Math.Round(g.Average(r => EF.Functions.DateDiffDay(r.CheckInDate, r.CheckOutDate)), 1)
                    })
                    .OrderByDescending(s => s.TotalReservations)
                    .ToListAsync();

                return Ok(roomStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching room type statistics", error = ex.Message });
            }
        }

        // GET: api/dashboard/weekly-bookings
        [HttpGet("weekly-bookings")]
        public async Task<ActionResult<List<WeeklyBookingDto>>> GetWeeklyBookings()
        {
            try
            {
                var weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var weeklyBookings = new List<WeeklyBookingDto>();

                for (int i = 0; i < 7; i++)
                {
                    var day = weekStart.AddDays(i);
                    var bookingCount = await _context.Bookings.CountAsync(b => b.BookingDate.Date == day);
                    var reservationCount = await _context.Reservations.CountAsync(r => r.BookingDate.Date == day);

                    weeklyBookings.Add(new WeeklyBookingDto
                    {
                        Day = day.ToString("dddd"),
                        Date = day.ToString("MMM dd"),
                        BookingsCount = bookingCount,
                        ReservationsCount = reservationCount
                    });
                }

                return Ok(weeklyBookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching weekly booking data", error = ex.Message });
            }
        }
    }

    // DTO Classes
    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalGuests { get; set; }
        public int TodayReservations { get; set; }
        public int TodayBookings { get; set; }
        public decimal TodayRevenue { get; set; }
        public int AvailableRooms { get; set; }
        public int TotalRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int PendingReservations { get; set; }
        public int CompletedReservations { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int YesterdayReservations { get; set; }
        public decimal YesterdayRevenue { get; set; }
    }

    public class MonthlyRevenueDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int MonthNumber { get; set; }
    }

    public class OccupancyDto
    {
        public List<string> Labels { get; set; } = new();
        public List<int> Data { get; set; } = new();
        public int Total { get; set; }
        public double OccupancyRate { get; set; }
    }

    public class RecentReservationDto
    {
        public int ReservationId { get; set; }
        public string GuestName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public string CheckInDate { get; set; } = string.Empty;
        public string CheckOutDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string BookingDate { get; set; } = string.Empty;
    }

    public class TodayPaymentDto
    {
        public int PaymentId { get; set; }
        public string GuestName { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public string PaymentTime { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class GuestServiceStatsDto
    {
        public string ServiceName { get; set; } = string.Empty;
        public int TotalRequests { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class RoomTypeStatsDto
    {
        public string RoomType { get; set; } = string.Empty;
        public int TotalReservations { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AverageStay { get; set; }
    }

    public class WeeklyBookingDto
    {
        public string Day { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public int BookingsCount { get; set; }
        public int ReservationsCount { get; set; }
    }
}