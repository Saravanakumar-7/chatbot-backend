using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
  
        public class GLAccountRepository : RepositoryBase<GlAccounts>, IGLAccountsRepository
    {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly String _createdBy;
            private readonly String _unitname;
            public GLAccountRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
            {
                _httpContextAccessor = httpContextAccessor;
                var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
                _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
                _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
            }
            public async Task<int?> CreateGLAccount(GlAccounts glaccount)
            {
            glaccount.CreatedBy = _createdBy;
            glaccount.CreatedOn = DateTime.Now;
            glaccount.Unit = _unitname;
                var result = await Create(glaccount);

                return result.Id;
            }

            public async Task<string> DeleteGLAccount(GlAccounts glaccount)
            {
                Delete(glaccount);
                string result = $"GLAccount details of {glaccount.Id} is deleted successfully!";
                return result;
            }

            public async Task<IEnumerable<GlAccounts>> GetAllActiveGLAccounts([FromQuery] SearchParames searchParams)
            {
                var GLAccountDetails = FindAll()
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.Name.Contains(searchParams.SearchValue) ||
               inv.Code.Contains(searchParams.SearchValue) || inv.Group.Contains(searchParams.SearchValue))) && inv.IsActive==true);
                return GLAccountDetails;
            }
            public async Task<IEnumerable<GlAccounts>> GetAllGLAccounts([FromQuery] SearchParames searchParams)
            {
            var GLAccountDetails = FindAll()
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.Name.Contains(searchParams.SearchValue) ||
            inv.Code.Contains(searchParams.SearchValue) || inv.Group.Contains(searchParams.SearchValue))));
            return GLAccountDetails;
        }

            public async Task<GlAccounts> GetGLAccountById(int id)
            {
                var GLAccountbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
                return GLAccountbyId;
            }

            public async Task<string> UpdateGLAccount(GlAccounts gLAccount)
            {
                 gLAccount.LastModifiedBy = _createdBy;
                 gLAccount.LastModifiedOn = DateTime.Now;
                Update(gLAccount);
                string result = $"GLAccount details of {gLAccount.Id} is updated successfully!";
                return result;
            }
        }

    
}
