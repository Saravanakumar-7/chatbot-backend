using Contracts;
using Entities;
using Entities.Helper;
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
            ApprovalRanges.Version = 1;
            ApprovalRanges.CreatedBy = _createdBy;
            ApprovalRanges.CreatedOn = DateTime.Now;
            ApprovalRanges.Unit = _unitname;
            var result = await Create(ApprovalRanges);

            return result.Id;
        }
        public async Task<ApprovalRanges> GetApprovalRangesById(int id)
        {
            var ApprovalRangesbyId = await FindByCondition(x => x.Id == id).Include(x=>x.Ranges).FirstOrDefaultAsync();
            return ApprovalRangesbyId;
        } 
        public async Task<ApprovalRanges> GetApprovalRangesByProcurementType(string ProcurementType)
        {
            var ApprovalRangesbyProcurementType = await FindByCondition(x => x.ProcurementName == ProcurementType).Include(x => x.Ranges).OrderByDescending(x=>x.Version).FirstOrDefaultAsync();
            return ApprovalRangesbyProcurementType;
        }
        public async Task<List<string>> GetListofProcurementType()
        {
            var ApprovalRangesProcurementType = await FindAll().Select(x=>x.ProcurementName).Distinct().ToListAsync();
            return ApprovalRangesProcurementType;
        }
        public async Task<PagedList<ApprovalRanges>> GetAllApprovalRanges(PagingParameter pagingParameter, SearchParames searchParams)
        {
            var ApprovalRangesDetails = FindAll().OrderByDescending(x => x.Id).Where(inv =>
                    (string.IsNullOrWhiteSpace(searchParams.SearchValue) ||
                    inv.ProcurementName.Contains(searchParams.SearchValue) ||
                    inv.Description.Contains(searchParams.SearchValue))).GroupBy(inv => inv.ProcurementName)
                    .Select(group => group.OrderByDescending(x => x.Version).First());

            return PagedList<ApprovalRanges>.ToPagedList(ApprovalRangesDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task UpdateApprovalRange(ApprovalRanges approvalRanges)
        {
            approvalRanges.LastModifiedBy=_createdBy;
            approvalRanges.LastModifiedOn=DateTime.Now;
            Update(approvalRanges);
        }
        public async Task<ApprovalRanges> CreateNewApprovalRangeVersion(ApprovalRanges approvalRanges)
        {
            approvalRanges.Version += 1;
            approvalRanges.CreatedBy = _createdBy;
            approvalRanges.CreatedOn = DateTime.Now;
            approvalRanges.Unit = _unitname;
            await Create(approvalRanges);

            return approvalRanges;
        }
    }
}
