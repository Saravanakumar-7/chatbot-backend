using Entities;
using Entities.DTOs;
using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUserRepository: IRepositoryBase<RegistrationForm>
    {
        Task<(LoginResult Result, int UserId, string UserName)> ConfirmUser(ResetPW loginDto);
        Task<string> ResetPassword(int Id, string NewPW, string ConfirmPW);
       
    }
}
