using Contracts;
using Entities;
using Entities.Helper;
using Entities.Migrations;
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
    public class BasisOfApprovalRepository : RepositoryBase<BasisOfApproval>, IBasisOfApprovalRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public BasisOfApprovalRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateBasisOfApproval(BasisOfApproval basisOfApproval)
        {
            basisOfApproval.CreatedBy = _createdBy;
            basisOfApproval.CreatedOn = DateTime.Now;
            basisOfApproval.Unit = _unitname;
            var result = await Create(basisOfApproval);
            return result.Id;
        }

        public async Task<string> DeleteBasisOfApproval(BasisOfApproval basisOfApproval)
        {
            Delete(basisOfApproval);
            string result = $"Basis Of Approval details of {basisOfApproval.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<BasisOfApproval>> GetAllBasisOfApproval([FromQuery] SearchParames searchParams)
        {
            var basisOfApprovalDetails = FindAll()
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BasisOfApprovalName.Contains(searchParams.SearchValue) ||
          inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return basisOfApprovalDetails;
        }

        public async Task<IEnumerable<BasisOfApproval>> GetAllActiveBasisOfApproval([FromQuery] SearchParames searchParams)
        {
            var basisOfApprovalDetails = FindAll()
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BasisOfApprovalName.Contains(searchParams.SearchValue) ||
          inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return basisOfApprovalDetails;
        }

        public async Task<BasisOfApproval> GetBasisOfApprovalById(int id)
        {
            var BasisOfApprovalbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return BasisOfApprovalbyId;
        }

        public async Task<string> UpdateBasisOfApproval(BasisOfApproval basisOfApproval)
        {
            basisOfApproval.LastModifiedBy = _unitname;
            basisOfApproval.LastModifiedOn = DateTime.Now;
            Update(basisOfApproval);
            string result = $"Basis Of Approval of Detail {basisOfApproval.Id} is updated successfully!";
            return result;
        }
         
    }
}
