using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepository : RepositoryBase<RegistrationForm>, IUserRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TipsMasterDbContext _tipsMasterDbContext;
        public UserRepository(TipsMasterDbContext tipsMasterDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsMasterDbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _tipsMasterDbContext = tipsMasterDbContext;
        }
        public async Task<(LoginResult Result, int UserId, string UserName)> ConfirmUser(ResetPW loginDto)
        {
            var userDetail = await _tipsMasterDbContext.RegistrationForms
                .Where(m => m.EmailId == loginDto.UserName || m.UserName == loginDto.UserName)
                .FirstOrDefaultAsync();

            if (userDetail == null)
            {
                return (LoginResult.UserNotFound, 0, null);
            }

            if (userDetail.Password != loginDto.Password)
            {
                return (LoginResult.InvalidPassword, 0, null);
            }
            if (userDetail.Unit != loginDto.UnitName)
            {
                return (LoginResult.InvalidUnit, 0, null);
            }
            if (userDetail.IsActive == false)
            {
                return (LoginResult.InvalidEntry, 0, null);
            }
            return (LoginResult.Success, userDetail.Id, userDetail.UserName);
        }
        public async Task<string> ResetPassword(int Id, string NewPW, string ConfirmPW)
        {
            var user = await _tipsMasterDbContext.RegistrationForms
                .Where(m => m.Id == Id)
                .FirstOrDefaultAsync();

            user.Password = NewPW;
            user.PasswordConfirm = ConfirmPW;
            Update(user);
            return user.Password;
        }
    }
}
