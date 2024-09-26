using Entities.Helper;
using Entities;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Contracts
{
    public interface IMaterialRequestItemsRepository : IRepositoryBase<MaterialRequestItems>
    {
        Task<PagedList<MaterialRequestItems>> GetAllMaterialRequestItem(PagingParameter pagingParameter, SearchParamess searchParammes);
        Task<MaterialRequestItems> GetMaterialRequestItemById(int id);
        Task<int?> CreateMaterialRequestItem(MaterialRequestItems mri);
        Task<string> UpdateMaterialRequestItem(MaterialRequestItems mri);
        Task<string> DeleteMaterialRequestItem(MaterialRequestItems mri);
        public void SaveAsync();
    }
}
