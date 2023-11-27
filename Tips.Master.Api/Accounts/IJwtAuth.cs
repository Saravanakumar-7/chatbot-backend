using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Accounts
{
    public interface IJwtAuth 
    {
        //Task<string> GetToken(string userName, string password);
        //Task<(string token, int userId)> GetToken(string userName, string password);
        Task<(LoginResult Result, string Token, int UserId, string UserName)> GetToken(LoginDto loginDto);


    }
}
