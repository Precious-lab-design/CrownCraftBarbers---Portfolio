using CrownCraftBarbers.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CrownCraftBarbers.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendBookingConfirmationAsync(
            Booking booking)
        {
            if (booking.Service == null ||
                booking.Barber == null)
            {
                return;
            }

            var startDateTime =
                booking.AppointmentDate.Date
                    .Add(booking.AppointmentTime);

            var bookingReference =
                $"CC-{booking.Id:D5}";

            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Booking Confirmation</title>
</head>

<body style='margin:0; padding:0; background:#f5f0e8; font-family:Arial,sans-serif;'>

    <div style='max-width:650px; margin:30px auto; background:#ffffff;'>

        <div style='background:#151515; padding:35px; text-align:center;'>
            <h1 style='color:#c9a45c; margin:0;'>
                CROWN &amp; CRAFT
            </h1>

            <p style='color:#f5f0e8; margin:8px 0 0;'>
                BARBERS
            </p>
        </div>

        <div style='padding:35px;'>

            <h2 style='color:#181818;'>
                Booking Confirmed
            </h2>

            <p>
                Hi {booking.CustomerName},
            </p>

            <p>
                Thank you for booking with Crown &amp; Craft Barbers.
                Your appointment has been confirmed.
            </p>

            <div style='background:#f5f0e8; padding:25px; margin:25px 0;'>

                <h3 style='margin-top:0; color:#181818;'>
                    Your Appointment
                </h3>

                <p>
                    <strong>Booking Reference:</strong>
                    {bookingReference}
                </p>

                <p>
                    <strong>Service:</strong>
                    {booking.Service.Name}
                </p>

                <p>
                    <strong>Barber:</strong>
                    {booking.Barber.Name}
                </p>

                <p>
                    <strong>Date:</strong>
                    {startDateTime:dddd, dd MMMM yyyy}
                </p>

                <p>
                    <strong>Time:</strong>
                    {startDateTime:HH:mm}
                </p>

                <p>
                    <strong>Duration:</strong>
                    {booking.Service.DurationMinutes} minutes
                </p>

                <p>
                    <strong>Total:</strong>
                    R{booking.Service.Price:0}
                </p>

            </div>

            <h3 style='color:#181818;'>
                Location
            </h3>

            <p>
                18 Market Street<br>
                Welkom<br>
                Free State<br>
                South Africa
            </p>

            <p>
                Phone: +27 10 555 0188<br>
                Email: hello@crowncraft.co.za
            </p>

            {(string.IsNullOrWhiteSpace(booking.Notes)
                ? ""
                : $"<h3 style='color:#181818;'>Your Notes</h3><p>{booking.Notes}</p>")}

            <p style='margin-top:30px;'>
                We look forward to seeing you.
            </p>

            <p>
                <strong>
                    Crown &amp; Craft Barbers
                </strong><br>
                Sharp cuts. Clean craft. Your style.
            </p>

        </div>

        <div style='background:#151515; padding:20px; text-align:center;'>
            <p style='color:#a7a29a; margin:0; font-size:13px;'>
                © 2026 Crown &amp; Craft Barbers
            </p>
        </div>

    </div>

</body>
</html>";

            var message = new MimeMessage();

            var fromEmail =
                _configuration["Brevo:FromEmail"];

            var fromName =
                _configuration["Brevo:FromName"]
                ?? "Crown & Craft Barbers";

            var username =
                _configuration["Brevo:Username"];

            var password =
                _configuration["Brevo:Password"];

            if (string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new InvalidOperationException(
                    "Brevo FromEmail is not configured.");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidOperationException(
                    "Brevo Username is not configured.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Brevo SMTP password is not configured.");
            }

            message.From.Add(
                new MailboxAddress(
                    fromName,
                    fromEmail));

            message.To.Add(
                new MailboxAddress(
                    booking.CustomerName,
                    booking.CustomerEmail));

            message.Subject =
                $"Booking Confirmed - {bookingReference}";

            message.Body =
                new BodyBuilder
                {
                    HtmlBody = html
                }.ToMessageBody();

            using var smtp = new SmtpClient();

            var smtpHost = _configuration["Brevo:SmtpHost"];

            if (string.IsNullOrWhiteSpace(smtpHost))
            {
                throw new InvalidOperationException(
                    "Brevo SMTP host is not configured.");
            }

            var smtpPort = int.Parse(
                _configuration["Brevo:SmtpPort"] ?? "587");

            await smtp.ConnectAsync(
                smtpHost,
                smtpPort,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                username,
                password);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}