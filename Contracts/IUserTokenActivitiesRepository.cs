using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUserTokenActivitiesRepository:IRepositoryBase<UserTokenActivities>
    {
        Task CreateUserTokenActivity(UserTokenActivities userTokenActivities);
        Task<int> ValidateUser(int RegistrationId);
        Task UpdateToken(int RegistrationId, string NewToken, DateTime Validity);
        Task DisableTokenInvalidTokenUse(string token);
        Task<UserTokenActivities?> GetUserTokenDetailsByUserId(int UserId);
        Task DeactivateUserTokenByUserId(int userId);
        Task DeactivateAllUsers();
    }
}
