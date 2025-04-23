using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Repository
{
    public class UserTokenActivitiesRepository : RepositoryBase<UserTokenActivities>, IUserTokenActivitiesRepository
    {
        public UserTokenActivitiesRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }
        public async Task<int> ValidateUser(int RegistrationId)
        {
            var User = await TipsMasterDbContext.UserTokenActivities.Where(x => x.RegistrationId == RegistrationId).FirstOrDefaultAsync();
            if (DateTime.UtcNow > User.Validity && User.TokenIsActive == true)
            {
                User.TokenIsActive = false;
                Update(User);
                return 0;
            }
            else if (User.TokenIsActive == true || User.Validity > DateTime.UtcNow) return 1;
            else return 0;
        }
        public async Task CreateUserTokenActivity(UserTokenActivities userTokenActivities)
        {
            await Create(userTokenActivities);
        }
        public async Task UpdateToken(int RegistrationId, string NewToken, DateTime Validity)
        {
            var User = await TipsMasterDbContext.UserTokenActivities.Where(x => x.RegistrationId == RegistrationId).FirstOrDefaultAsync();
            User.TokenIsActive = true;
            User.Token = NewToken;
            User.Validity = Validity;
            Update(User);
        }

        public async Task DisableTokenInvalidTokenUse(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(token))
                {
                    throw new UnauthorizedAccessException("Cannot read token.");
                }

                var jwtToken = handler.ReadJwtToken(token);
                var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                if (string.IsNullOrEmpty(expClaim) || string.IsNullOrEmpty(userIdClaim))
                {
                    throw new UnauthorizedAccessException("Required claims not found.");
                }

                if (!int.TryParse(userIdClaim, out int userId))
                {
                    throw new UnauthorizedAccessException("Invalid UserId claim.");
                }

                var user = await TipsMasterDbContext.UserTokenActivities
                    .FirstOrDefaultAsync(x => x.RegistrationId == userId);

                if (user == null || !user.Token.Equals(token) || user.TokenIsActive == false)
                {
                    throw new UnauthorizedAccessException("Token not valid or already inactive.");
                }

                if (long.TryParse(expClaim, out long expSeconds))
                {
                    DateTime expiryDateTime = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;

                    if (DateTime.UtcNow > expiryDateTime)
                    {
                        user.Token = token;
                        user.TokenIsActive = false;
                        user.Validity = expiryDateTime;
                        Update(user);
                        await TipsMasterDbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
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