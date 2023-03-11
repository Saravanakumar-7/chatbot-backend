using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Repository
{
    public class MaterialIssueRepository : RepositoryBase<MaterialIssue>, IMaterialIssueRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;
        public MaterialIssueRepository(TipsProductionDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
        }

        public async Task<int> CreateMaterialIssue(MaterialIssue materialIssue)
        {
            materialIssue.CreatedBy = "Admin";
            materialIssue.CreatedOn = DateTime.Now;
            materialIssue.Unit = "Bangalore";
            var result = await Create(materialIssue);
            return result.Id;
        }

        public async Task<string> DeleteMaterialIssue(MaterialIssue materialIssue)
        {
            Delete(materialIssue);
            string result = $"MaterialIssue details of {materialIssue.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<MaterialIssue>> GetAllMaterialIssues(PagingParameter pagingParameter)
        {
            var materialIssueDetails = PagedList<MaterialIssue>.ToPagedList(FindAll()                              
               .OrderByDescending(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return materialIssueDetails;
        }

        public async Task<MaterialIssue> GetMaterialIssueById(int id)
        {
            var materialIssueDetail = await _tipsProductionDbContext.MaterialIssue
                                    .Where(x => x.Id == id)                           
                                    .Include(m=> m.MaterialIssueItems)
                                    .FirstOrDefaultAsync();

            return materialIssueDetail;
        }

        public async Task<MaterialIssue> GetMaterialIssueByShopOrderNo(string shopOrderNo)
        {
            var materialIssueDetail = await _tipsProductionDbContext.MaterialIssue
                                    .Where(x => x.ShopOrderNumber == shopOrderNo)
                                    .Include(m => m.MaterialIssueItems)
                                    .FirstOrDefaultAsync();

            return materialIssueDetail;
        }

        public  async Task<string> UpdateMaterialIssue(MaterialIssue materialIssue)
        {
            materialIssue.LastModifiedBy = "Admin";
            materialIssue.LastModifiedOn = DateTime.Now;
            Update(materialIssue);
            string result = $"MaterialIssue of Detail {materialIssue.Id} is updated successfully!";
            return result;
        }
       
    }
}
