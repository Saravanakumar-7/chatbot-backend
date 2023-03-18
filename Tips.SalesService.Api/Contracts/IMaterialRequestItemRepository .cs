using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IMaterialRequestItemRepository : IRepositoryBase<MaterialRequestItem>
    {
        Task<PagedList<MaterialRequestItem>> GetAllMaterialRequestItem(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<MaterialRequestItem> GetMaterialRequestItemById(int id);
        Task<int?> CreateMaterialRequestItem(MaterialRequestItem mri);
        Task<string> UpdateMaterialRequestItem(MaterialRequestItem mri);
        Task<string> DeleteMaterialRequestItem(MaterialRequestItem mri);
        public void SaveAsync();
    }
}