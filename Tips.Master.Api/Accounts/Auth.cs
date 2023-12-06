using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Entities.Helper;
using Entities.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Accounts
{
    public class Auth : IJwtAuth
    {
        private readonly TipsMasterDbContext _tipsMasterDbContext;
        private readonly string _key;
        private readonly IConfiguration _configuration;

        public Auth(TipsMasterDbContext tipsMasterDbContext, IConfiguration configuration)
        {
            _tipsMasterDbContext = tipsMasterDbContext; 
            _configuration = configuration;
        }
        public async Task<(LoginResult Result, string Token, int UserId, string UserName)> GetToken(LoginDto loginDto)
        {
            var userDetail = await _tipsMasterDbContext.RegistrationForms
                .Where(m => m.EmailId == loginDto.UserName)
                .FirstOrDefaultAsync();

            if (userDetail == null)
            {
                return (LoginResult.UserNotFound, null, 0,null);
            }

            if (userDetail.Password != loginDto.Password)
            {
                return (LoginResult.InvalidPassword, null, 0, null);
            }
            if (userDetail.Unit != loginDto.UnitName)
            {
                return (LoginResult.InvalidUnit, null, 0, null);
            }

            var key = _configuration["Jwt:key"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                new Claim(ClaimTypes.Name, userDetail.UserName),
                new Claim(ClaimTypes.Email, userDetail.EmailId),
                new Claim("UnitName", userDetail.Unit), 
                new Claim("UserId", userDetail.Id.ToString()),
                    }),
                Expires = DateTime.UtcNow.AddHours(5),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return (LoginResult.Success, tokenHandler.WriteToken(token), userDetail.Id, userDetail.UserName);
        }
    //    public async Task<(string token, int userId)> GetToken(string userName, string password)

    //    //public async Task<string> GetToken(string userName, string password)
    //    {
            
    //            var userDetail = await _tipsMasterDbContext.RegistrationForms
    //                            .Where(m => m.EmailId == userName && m.Password == password)
    //                            .FirstOrDefaultAsync();          
              
    //                if (userDetail != null)
    //                {
    //                    var key = _configuration["Jwt:key"];
    //                    var tokenHandler = new JwtSecurityTokenHandler();
    //                    var tokenKey = Encoding.ASCII.GetBytes(key);
    //                    var tokenDescriptor = new SecurityTokenDescriptor()
    //                    {
    //                        Subject = new ClaimsIdentity(
    //                            new Claim[]
    //                            {
    //                    new Claim(ClaimTypes.Name, userDetail.UserName),
    //                    new Claim(ClaimTypes.Email, userDetail.EmailId),
    //                    new Claim("UnitName", userDetail.Unit),
    //                    new Claim("UserId", userDetail.Id.ToString()),// Add UserId claim
    //                             }),
    //                        Expires = DateTime.UtcNow.AddHours(1),
    //                        SigningCredentials = new SigningCredentials(
    //                            new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
    //                    }; 
    //                    var token = tokenHandler.CreateToken(tokenDescriptor);
                
    //                   // return tokenHandler.WriteToken(token);
    //            return (tokenHandler.WriteToken(token), userDetail.Id);
    //        }
    //                else
    //                {
    //                return (null, 0);
                         
    //                }  
            
    //    }
         
    }

}


