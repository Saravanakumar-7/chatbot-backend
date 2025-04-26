using Contracts;

namespace Tips.Master.Api.Extensions
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserTokenActivitiesRepository tokenRepo)
        {
            try
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Replace("Bearer ", "");

                    // This method can handle blacklist checking, expiration, revocation, etc.
                    await tokenRepo.DisableTokenInvalidTokenUse(token);
                }

                await _next(context); // Continue down the pipeline
            }
            catch (UnauthorizedAccessException ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Optional: You can log here if needed
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
            }
        }
    }
}
