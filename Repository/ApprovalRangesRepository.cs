using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    internal class ApprovalRangesRepository : RepositoryBase<ApprovalRanges>, IApprovalRangesRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ApprovalRangesRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateApprovalRanges(ApprovalRanges ApprovalRanges)
        {
            ApprovalRanges.CreatedBy = _createdBy;
            ApprovalRanges.CreatedOn = DateTime.Now;
            ApprovalRanges.Unit = _unitname;
            var result = await Create(ApprovalRanges);

            return result.Id;
        }
        public async Task<ApprovalRanges> GetApprovalRangesById(int id)
        {
            var ApprovalRangesbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return ApprovalRangesbyId;
        } 
        public async Task<ApprovalRanges> GetApprovalRangesByProcurementType(string ProcurementType)
        {
            var ApprovalRangesbyProcurementType = await FindByCondition(x => x.ProcurementName == ProcurementType).FirstOrDefaultAsync();
            return ApprovalRangesbyProcurementType;
        }
    }
}
