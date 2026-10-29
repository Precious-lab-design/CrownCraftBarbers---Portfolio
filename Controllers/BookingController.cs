using System.Text;
using CrownCraftBarbers.Data;
using CrownCraftBarbers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrownCraftBarbers.Services;

namespace CrownCraftBarbers.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public BookingController(ApplicationDbContext context,
            EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: /Booking
        [HttpGet]
        public async Task<IActionResult> Index(int? serviceId)
        {
            var services = await _context.Services
                .ToListAsync();

            var barbers = await _context.Barbers
                .ToListAsync();

            ViewBag.Services = services;
            ViewBag.Barbers = barbers;
            ViewBag.SelectedServiceId = serviceId;

            return View();
        }

        // POST: /Booking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Services = await _context.Services.ToListAsync();
                ViewBag.Barbers = await _context.Barbers.ToListAsync();

                return View(booking);
            }

            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == booking.ServiceId);

            var barber = await _context.Barbers
                .FirstOrDefaultAsync(b => b.Id == booking.BarberId);

            if (service == null || barber == null)
            {
                ModelState.AddModelError(
                    "",
                    "Please select a valid service and barber.");

                ViewBag.Services = await _context.Services.ToListAsync();
                ViewBag.Barbers = await _context.Barbers.ToListAsync();

                return View(booking);
            }

            var appointmentDateTime =
                booking.AppointmentDate.Date
                .Add(booking.AppointmentTime);

            if (appointmentDateTime < DateTime.Now)
            {
                ModelState.AddModelError(
                    "AppointmentDate",
                    "Crown & Craft Barbers is closed on Sundays. Please select another date.");


                ViewBag.Services = await _context.Services.ToListAsync();
                ViewBag.Barbers = await _context.Barbers.ToListAsync();

                return View(booking);
            }

            var appointmentEnd =
                appointmentDateTime.AddMinutes(
                    service.DurationMinutes);

            var existingBookings = await _context.Bookings
                .Include(b => b.Service)
                .Where(b =>
                    b.BarberId == booking.BarberId &&
                    b.AppointmentDate.Date ==
                    booking.AppointmentDate.Date)
                .ToListAsync();

            foreach (var existingBooking in existingBookings)
            {
                if (existingBooking.Service == null)
                    continue;

                var existingStart =
                    existingBooking.AppointmentDate.Date
                    .Add(existingBooking.AppointmentTime);

                var existingEnd =
                    existingStart.AddMinutes(
                        existingBooking.Service.DurationMinutes);

                if (appointmentDateTime < existingEnd &&
                    appointmentEnd > existingStart)
                {
                    ModelState.AddModelError(
                        "AppointmentTime",
                        "This barber is already booked during that time. Please choose another time.");

                    ViewBag.Services =
                        await _context.Services.ToListAsync();

                    ViewBag.Barbers =
                        await _context.Barbers.ToListAsync();

                    return View(booking);
                }
            }

            booking.CreatedAt = DateTime.UtcNow;

            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync();

            booking.Service = service;
            booking.Barber = barber;

            try
            {
                await _emailService.SendBookingConfirmationAsync(booking);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Booking email failed for booking {booking.Id}: {ex.Message}");
            }

            return RedirectToAction(
                nameof(Confirmation),
                new { id = booking.Id });
        }

        // GET: /Booking/Confirmation/5
        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Service)
                .Include(b => b.Barber)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: /Booking/GoogleCalendar/5
        [HttpGet]
        public async Task<IActionResult> GoogleCalendar(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Service)
                .Include(b => b.Barber)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null ||
                booking.Service == null ||
                booking.Barber == null)
            {
                return NotFound();
            }

            var startDateTime =
                booking.AppointmentDate.Date
                .Add(booking.AppointmentTime);

            var endDateTime =
                startDateTime.AddMinutes(
                    booking.Service.DurationMinutes);

            var title =
                $"Crown & Craft - {booking.Service.Name}";

            var details =
                $"Barber: {booking.Barber.Name}\\n" +
                $"Customer: {booking.CustomerName}\\n" +
                $"Booking reference: CC-{booking.Id:D5}\\n" +
                $"Phone: {booking.CustomerPhone}\\n" +
                $"Email: {booking.CustomerEmail}";

            if (!string.IsNullOrWhiteSpace(booking.Notes))
            {
                details +=
                    $"\\nNotes: {booking.Notes}";
            }

            var location =
                "18 Market Street, Welkom, Free State, South Africa";

            var googleCalendarUrl =
                "https://calendar.google.com/calendar/render?action=TEMPLATE" +
                $"&text={Uri.EscapeDataString(title)}" +
                $"&dates={startDateTime.ToUniversalTime():yyyyMMdd'T'HHmmss'Z'}/{endDateTime.ToUniversalTime():yyyyMMdd'T'HHmmss'Z'}" +
                $"&details={Uri.EscapeDataString(details)}" +
                $"&location={Uri.EscapeDataString(location)}";

            return Redirect(googleCalendarUrl);
        }

        // GET: /Booking/CalendarFile/5
        [HttpGet]
        public async Task<IActionResult> CalendarFile(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Service)
                .Include(b => b.Barber)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null ||
                booking.Service == null ||
                booking.Barber == null)
            {
                return NotFound();
            }

            var startDateTime =
                booking.AppointmentDate.Date
                .Add(booking.AppointmentTime);

            var endDateTime =
                startDateTime.AddMinutes(
                    booking.Service.DurationMinutes);

            var location =
                "18 Market Street, Welkom, Free State, South Africa";

            var description =
                $"Barber: {booking.Barber.Name}\\n" +
                $"Customer: {booking.CustomerName}\\n" +
                $"Booking reference: CC-{booking.Id:D5}\\n" +
                $"Phone: {booking.CustomerPhone}\\n" +
                $"Email: {booking.CustomerEmail}";

            if (!string.IsNullOrWhiteSpace(booking.Notes))
            {
                description +=
                    $"\\nNotes: {booking.Notes}";
            }

            var calendarContent = new StringBuilder();

            calendarContent.AppendLine("BEGIN:VCALENDAR");
            calendarContent.AppendLine("VERSION:2.0");
            calendarContent.AppendLine("PRODID:-//Crown & Craft Barbers//Booking//EN");
            calendarContent.AppendLine("CALSCALE:GREGORIAN");
            calendarContent.AppendLine("METHOD:PUBLISH");

            calendarContent.AppendLine("BEGIN:VEVENT");

            calendarContent.AppendLine(
                $"UID:crowncraft-{booking.Id}@crowncraft.co.za");

            calendarContent.AppendLine(
                $"DTSTAMP:{DateTime.UtcNow:yyyyMMdd'T'HHmmss'Z'}");

            calendarContent.AppendLine(
                $"DTSTART:{startDateTime.ToUniversalTime():yyyyMMdd'T'HHmmss'Z'}");

            calendarContent.AppendLine(
                $"DTEND:{endDateTime.ToUniversalTime():yyyyMMdd'T'HHmmss'Z'}");

            calendarContent.AppendLine(
                $"SUMMARY:Crown & Craft - {EscapeIcs(booking.Service.Name)}");

            calendarContent.AppendLine(
                $"LOCATION:{EscapeIcs(location)}");

            calendarContent.AppendLine(
                $"DESCRIPTION:{EscapeIcs(description)}");

            calendarContent.AppendLine("END:VEVENT");
            calendarContent.AppendLine("END:VCALENDAR");

            var bytes =
                Encoding.UTF8.GetBytes(
                    calendarContent.ToString());

            return File(
                bytes,
                "text/calendar",
                $"CrownCraft-Booking-{booking.Id}.ics");
        }

        private static string EscapeIcs(string value)
        {
            return value
                .Replace("\\", "\\\\")
                .Replace(";", "\\;")
                .Replace(",", "\\,")
                .Replace("\r\n", "\\n")
                .Replace("\n", "\\n");
        }
    }
}