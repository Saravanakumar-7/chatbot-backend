using Entities;
using Entities.Helper;
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
            var result = await Create(sAShopOrderMaterialIssue);
            return result.Id;
        }

        public async Task<string> DeleteSAShopOrderMaterialIssue(SAShopOrderMaterialIssue sAShopOrderMaterialIssue)
        {
            Delete(sAShopOrderMaterialIssue);
            string result = $"SAShopOrderMaterialIssue details of {sAShopOrderMaterialIssue.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<SAShopOrderMaterialIssue>> GetAllSAShopOrderMaterialIssue(PagingParameter pagingParameter)
        {
            var sAShopOrderMaterialIssueDetails = PagedList<SAShopOrderMaterialIssue>.ToPagedList(FindAll()
                              .Include(t => t.SAShopOrderMaterialIssueGeneralList)
             .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return sAShopOrderMaterialIssueDetails;
        }

        public async Task<SAShopOrderMaterialIssue> GetSAShopOrderMaterialIssueById(int id)
        {
            var sAShopOrderMaterialIssueDetails = await _tipsProductionDbContext.SAShopOrderMaterialIssues.Where(x => x.Id == id)
                                .Include(t => t.SAShopOrderMaterialIssueGeneralList)
                                .FirstOrDefaultAsync();

            return sAShopOrderMaterialIssueDetails;
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
