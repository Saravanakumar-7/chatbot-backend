using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Repository
{
    public class SAShopOrderMaterialIssueRepository : RepositoryBase<SAShopOrderMaterialIssue>, ISAShopOrderMaterialIssueRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;

        public SAShopOrderMaterialIssueRepository(TipsProductionDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
        }

        public async Task<long> CreateSAShopOrderMaterialIssue(SAShopOrderMaterialIssue sAShopOrderMaterialIssue)
        {
            sAShopOrderMaterialIssue.LastModifiedBy = "Admin";
            sAShopOrderMaterialIssue.LastModifiedOn = DateTime.Now;
            sAShopOrderMaterialIssue.CreatedBy = "Admin";
            sAShopOrderMaterialIssue.CreatedOn = DateTime.Now;
            sAShopOrderMaterialIssue.Unit = "Bangalore";
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
            sAShopOrderMaterialIssue.LastModifiedBy = "Admin";
            sAShopOrderMaterialIssue.LastModifiedOn = DateTime.Now;
            Update(sAShopOrderMaterialIssue);
            string result = $"SAShopOrderMaterialIssue of Detail {sAShopOrderMaterialIssue.Id} is updated successfully!";
            return result;
        }
    }
}
