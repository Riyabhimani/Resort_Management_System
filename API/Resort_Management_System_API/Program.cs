using System.Net;
using Resort_Management_System_API.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddControllers();
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
