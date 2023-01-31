using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Repository
{
    public class FGShopOrderMaterialIssueRepository : RepositoryBase<FGShopOrderMaterialIssue>, IFGShopOrderMaterialIssueRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;

        public FGShopOrderMaterialIssueRepository(TipsProductionDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
        }

        public async Task<long> CreateFGShopOrderMaterialIssue(FGShopOrderMaterialIssue fGShopOrderMaterialIssue)
        {
            fGShopOrderMaterialIssue.LastModifiedBy = "Admin";
            fGShopOrderMaterialIssue.LastModifiedOn = DateTime.Now;
            fGShopOrderMaterialIssue.CreatedBy = "Admin";
            fGShopOrderMaterialIssue.CreatedOn = DateTime.Now;
            fGShopOrderMaterialIssue.Unit = "Bangalore";
            var result = await Create(fGShopOrderMaterialIssue);
            return result.Id;
        }

        public async Task<string> DeleteFGShopOrderMaterialIssue(FGShopOrderMaterialIssue fGShopOrderMaterialIssue)
        {
            Delete(fGShopOrderMaterialIssue);
            string result = $"FGShopOrderMaterialIssue details of {fGShopOrderMaterialIssue.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<FGShopOrderMaterialIssue>> GetAllFGShopOrderMaterialIssues(PagingParameter pagingParameter)
        {
            var fGShopOrderMaterialIssueDetails = PagedList<FGShopOrderMaterialIssue>.ToPagedList(FindAll()
                               .Include(t => t.FGShopOrderMaterialIssueGeneralList)
              .OrderByDescending(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return fGShopOrderMaterialIssueDetails;
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
            fGShopOrderMaterialIssue.LastModifiedBy = "Admin";
            fGShopOrderMaterialIssue.LastModifiedOn = DateTime.Now;
            Update(fGShopOrderMaterialIssue);
            string result = $"FGShopOrderMaterialIssue of Detail {fGShopOrderMaterialIssue.Id} is updated successfully!";
            return result;
        }
    }
}
