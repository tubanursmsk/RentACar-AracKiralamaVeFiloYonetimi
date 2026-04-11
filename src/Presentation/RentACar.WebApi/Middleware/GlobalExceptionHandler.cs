using System.Net;
using System.Text.Json;

namespace RentACar.WebApi.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // İstek sorunsuzsa bir sonraki adıma geç
                await _next(context);
            }
            catch (Exception ex)
            {
                // Hata oluşursa yakala ve özel metoda gönder
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Inner exception (iç hata) varsa onun mesajını al, yoksa normal hata mesajını al
            string message = exception.InnerException?.Message ?? exception.Message;

            // E-ticaret projesindeki gibi detaylı loglama
            _logger.LogError(
                exception,
                "Unhandled exception: {Message} | Path: {Path} | IP: {IP} | Agent: {Agent} | Type: {Type}",
                message,
                context.Request.Path,
                context.Connection.RemoteIpAddress?.ToString(),
                context.Request.Headers["User-Agent"].ToString(),
                exception.GetType().ToString()
            );

            // E-ticaret standartı: Hatalar 400 Bad Request olarak döner
            var statusCode = StatusCodes.Status400BadRequest;
            
            // E-ticaret standartındaki anonim nesne (error, code, timestamp)
            var response = new
            {
                error = message, // Doğrudan Entity Framework'ün veya senin fırlattığın net mesaj
                code = statusCode,
                timestamp = DateTime.UtcNow
            };

            var payload = JsonSerializer.Serialize(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            
            await context.Response.WriteAsync(payload);
        }
    }
}