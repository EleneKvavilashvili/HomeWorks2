namespace Homework15.middlewares
{
    public class BookingRestrictionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public BookingRestrictionMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            bool isBookingNotAllowed = _configuration.GetValue<bool>("BookingNotAllowed");

            if (isBookingNotAllowed && context.Request.Method == "POST" && context.Request.Path.Value.Contains("Create"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync("<h2>Booking not allowed, please check again later!!</h2>");
                return;
            }

            await _next(context);
        }
    }
}
