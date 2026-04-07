using System.Net;
using System.Text.Json;
using RentACar.Application.DTOs.Responses;

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
                // Hata oluşursa logla ve yakala
                _logger.LogError(ex, "Sistemde beklenmeyen bir hata oluştu: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Standart ApiResponse formatında hata mesajı döndürüyoruz
            var response = ApiResponse<object>.ErrorResult(
                "Sunucu tarafında beklenmeyen bir hata oluştu.",
                new List<string> { exception.Message } // İstersen canlı ortamda exception.Message'ı gizleyebilirsin
            );

            // Json isimlendirme standartlarını (camelCase) korumak için opsiyon ekliyoruz
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);
            
            return context.Response.WriteAsync(json);
        }
    }
}