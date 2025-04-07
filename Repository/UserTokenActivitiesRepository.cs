using Contracts;
using Entities;
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
                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(token))
                {
                    return;
                }

                var jwtToken = handler.ReadJwtToken(token);
                var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
                var userIdClaim = Convert.ToInt32(jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);

                if (expClaim != null && long.TryParse(expClaim, out long expSeconds))
                {   
                    // Convert Unix timestamp to DateTime
                    DateTime expiryDateTime = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
                    if (DateTime.UtcNow > expiryDateTime)
                    {                       
                        var User = await TipsMasterDbContext.UserTokenActivities.Where(x => x.RegistrationId == userIdClaim).FirstOrDefaultAsync();
                        User.Token = token;
                        User.TokenIsActive = false;
                        User.Validity = expiryDateTime;
                        Update(User);
                        await TipsMasterDbContext.SaveChangesAsync();
                    }
                }            
        }

    }
}