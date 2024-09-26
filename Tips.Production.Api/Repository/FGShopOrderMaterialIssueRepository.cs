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
    public class FGShopOrderMaterialIssueRepository : RepositoryBase<FGShopOrderMaterialIssue>, IFGShopOrderMaterialIssueRepository
    {
        private AdvitaTipsProductionDbContext _advitaTipsProductionDbContext;
        private TipsProductionDbContext _tipsProductionDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public FGShopOrderMaterialIssueRepository(TipsProductionDbContext repositoryContext, AdvitaTipsProductionDbContext advitaTipsProductionDbContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext, advitaTipsProductionDbContext)
        {
            _advitaTipsProductionDbContext = advitaTipsProductionDbContext;
            _tipsProductionDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<long> CreateFGShopOrderMaterialIssue(FGShopOrderMaterialIssue fGShopOrderMaterialIssue)
        {
            //fGShopOrderMaterialIssue.LastModifiedBy = _createdBy;
            //fGShopOrderMaterialIssue.LastModifiedOn = DateTime.Now;
            fGShopOrderMaterialIssue.CreatedBy = _createdBy;
            fGShopOrderMaterialIssue.CreatedOn = DateTime.Now;
            fGShopOrderMaterialIssue.Unit = _unitname;
            var result = await Create(fGShopOrderMaterialIssue);
            return result.Id;
        }

        public async Task<string> DeleteFGShopOrderMaterialIssue(FGShopOrderMaterialIssue fGShopOrderMaterialIssue)
        {
            Delete(fGShopOrderMaterialIssue);
            string result = $"FGShopOrderMaterialIssue details of {fGShopOrderMaterialIssue.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<FGShopOrderMaterialIssue>> GetAllFGShopOrderMaterialIssues([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            var fgShopOrderDetails = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParamess.SearchValue) || inv.ShopOrderNumber.Contains(searchParamess.SearchValue) ||
                   inv.FGPartNumber.Contains(searchParamess.SearchValue) || inv.ProjectNumber.Contains(searchParamess.SearchValue) || inv.ShopOrderType.Contains(searchParamess.SearchValue)
                   || inv.ShopOrderQty.Equals(int.Parse(searchParamess.SearchValue)))));
            return PagedList<FGShopOrderMaterialIssue>.ToPagedList(fgShopOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }


        public async Task<FGShopOrderMaterialIssue> GetFGShopOrderMaterialIssueById(int id)
        {
            var fGShopOrderMaterialIssueDetailById = await _tipsProductionDbContext.FGShopOrderMaterialIssues.Where(x => x.Id == id)
                                .Include(t => t.FGShopOrderMaterialIssueGeneralList)
                                .FirstOrDefaultAsync();

            return fGShopOrderMaterialIssueDetailById;
        }

        public async Task<string> UpdateFGShopOrderMaterialIssue(FGShopOrderMaterialIssue fGShopOrderMaterialIssue)
        {
            fGShopOrderMaterialIssue.LastModifiedBy = _createdBy;
            fGShopOrderMaterialIssue.LastModifiedOn = DateTime.Now;
            Update(fGShopOrderMaterialIssue);
            string result = $"FGShopOrderMaterialIssue of Detail {fGShopOrderMaterialIssue.Id} is updated successfully!";
            return result;
        }
    }
}
