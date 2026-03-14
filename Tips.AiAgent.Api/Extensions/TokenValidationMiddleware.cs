using System.IdentityModel.Tokens.Jwt;

namespace Tips.AiAgent.Api.Extensions
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Substring("Bearer " .Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                
                if (handler.CanReadToken(token))
                {
                    // Basic validation, add database checks if necessary
                    var jwtToken = handler.ReadJwtToken(token);
                }
            }

            await _next(context);
        }
    }
}
