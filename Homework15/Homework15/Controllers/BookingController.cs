using Homework15.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Homework15.Controllers
{
    public class BookingController : Controller
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "bookings.json");

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            List<BookingModel> bookings = await ReadBookingsFromFileAsync();
            bookings.Add(model);

            var txt = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(bookings, txt);
            await System.IO.File.WriteAllTextAsync(_filePath, jsonString);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<BookingModel> bookings = await ReadBookingsFromFileAsync();
            return View(bookings);
        }

        private async Task<List<BookingModel>> ReadBookingsFromFileAsync()
        {
            if (!System.IO.File.Exists(_filePath))
            {
                return new List<BookingModel>();
            }

            string jsonString = await System.IO.File.ReadAllTextAsync(_filePath);

            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return new List<BookingModel>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<BookingModel>>(jsonString) ?? new List<BookingModel>();
            }
            catch
            {
                return new List<BookingModel>();
            }
        }
    }
}
