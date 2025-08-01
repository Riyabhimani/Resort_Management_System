using System.Net;
using Resort_Management_System_API.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using FluentValidation;
using System.Diagnostics.Metrics;
using System.Reflection;
using Resort_Management_System_MVC.ValidationClass;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register all validators from this assembly
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IValidator<Guest>, GuestValidator>();
builder.Services.AddScoped<IValidator<User>, UserValidator>();
builder.Services.AddScoped<IValidator<GuestService>, GuestServicesValidator>();
builder.Services.AddScoped<IValidator<Payment>, PaymentValidator>();
builder.Services.AddScoped<IValidator<Reservation>, ReservationValidator>();
builder.Services.AddScoped<IValidator<Room>, RoomValidator>();
builder.Services.AddScoped<IValidator<Service>, ServiceValidator>();
builder.Services.AddScoped<IValidator<Staff>, StaffValidator>();
builder.Services.AddScoped<IValidator<Booking>, BookingValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var provider = builder.Services.BuildServiceProvider();
var config = provider.GetRequiredService<IConfiguration>();
builder.Services.AddDbContext<ResortManagementContext>(item => item.UseSqlServer(config.GetConnectionString("dbcs")));


var app = builder.Build();

//app.UseExceptionHandler(error =>
//{
//    error.Run(async context =>
//    {
//        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
//        context.Response.ContentType = "application/json";

//        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
//        if (contextFeature != null)
//        {
//            string message = "Internal server error";
//            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
//            {
//                StatusCode = context.Response.StatusCode,
//                Message = message,
//                Status = "ERROR"
//            }));
//        }
//    });
//});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
