using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Repository
{
    public class SAShopOrderMaterialIssueRepository : RepositoryBase<SAShopOrderMaterialIssue>, ISAShopOrderMaterialIssueRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public SAShopOrderMaterialIssueRepository(TipsProductionDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<long> CreateSAShopOrderMaterialIssue(SAShopOrderMaterialIssue sAShopOrderMaterialIssue)
        {
           // sAShopOrderMaterialIssue.LastModifiedBy = _createdBy;
           // sAShopOrderMaterialIssue.LastModifiedOn = DateTime.Now;
            sAShopOrderMaterialIssue.CreatedBy = _createdBy;
            sAShopOrderMaterialIssue.CreatedOn = DateTime.Now;
            sAShopOrderMaterialIssue.Unit = _unitname;
            var result = await Create(sAShopOrderMaterialIssue);
            return result.Id;
        }

        public async Task<string> DeleteSAShopOrderMaterialIssue(SAShopOrderMaterialIssue sAShopOrderMaterialIssue)
        {
            Delete(sAShopOrderMaterialIssue);
            string result = $"SAShopOrderMaterialIssue details of {sAShopOrderMaterialIssue.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<SAShopOrderMaterialIssue>> GetAllSAShopOrderMaterialIssues([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            var saShopOrderMaterialIssueDetails = FindAll().OrderByDescending(x => x.Id)
                           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.SAShopOrderNumber.Contains(searchParams.SearchValue) ||
                              inv.SAShopOrderQty.Equals(int.Parse(searchParams.SearchValue))  || inv.ShopOrderType.Contains(searchParams.SearchValue))));

            return PagedList<SAShopOrderMaterialIssue>.ToPagedList(saShopOrderMaterialIssueDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<SAShopOrderMaterialIssue> GetSAShopOrderMaterialIssueById(int id)
        {
            var sAShopOrderMaterialIssueDetailById = await _tipsProductionDbContext.SAShopOrderMaterialIssues.Where(x => x.Id == id)
                                .Include(t => t.SAShopOrderMaterialIssueGeneralList)
                                .FirstOrDefaultAsync();

            return sAShopOrderMaterialIssueDetailById;
        }

        public async Task<string> UpdateSAShopOrderMaterialIssue(SAShopOrderMaterialIssue sAShopOrderMaterialIssue)
        {
            sAShopOrderMaterialIssue.LastModifiedBy = _createdBy;
            sAShopOrderMaterialIssue.LastModifiedOn = DateTime.Now;
            Update(sAShopOrderMaterialIssue);
            string result = $"SAShopOrderMaterialIssue of Detail {sAShopOrderMaterialIssue.Id} is updated successfully!";
            return result;
        }
    }
}
