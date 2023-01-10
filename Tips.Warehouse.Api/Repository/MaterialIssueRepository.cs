using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Repository
{
    public class MaterialIssueRepository : RepositoryBase<MaterialIssue>, IMaterialIssueRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        public MaterialIssueRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
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
            string result = $"NaterialIssue details of {materialIssue.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<MaterialIssue>> GetAllMaterialIssue(PagingParameter pagingParameter)
        {
            var getAllMaterialIssue = PagedList<MaterialIssue>.ToPagedList(FindAll()                              
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllMaterialIssue;
        }

        public async Task<MaterialIssue> GetMaterialIssueById(int id)
        {
            var getMaterialIssueById = await _tipsWarehouseDbContext.MaterialIssue.Where(x => x.Id == id)                           

                              .FirstOrDefaultAsync();

            return getMaterialIssueById;
        }

        public  async Task<string> UpdateMaterialIssue(MaterialIssue materialIssue)
        {
            materialIssue.LastModifiedBy = "Admin";
            materialIssue.LastModifiedOn = DateTime.Now;
            Update(materialIssue);
            string result = $"materialIssue of Detail {materialIssue.Id} is updated successfully!";
            return result;
        }
       
    }
}
