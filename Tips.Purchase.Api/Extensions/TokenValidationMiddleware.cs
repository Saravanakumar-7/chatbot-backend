using Tips.Purchase.Api.Contracts;

namespace Tips.Purchase.Api.Extensions
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ITokenValidationService tokenService)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Replace("Bearer ", "");

                var isValid = await tokenService.IsTokenValid(token);
                if (!isValid)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized: Token is invalid or expired.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
