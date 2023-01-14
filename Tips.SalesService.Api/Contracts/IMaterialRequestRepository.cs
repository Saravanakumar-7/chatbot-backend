using Tips.SalesService.Api.Entities;
using Entities.Helper;
using Entities;
using Entities.DTOs;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface IMaterialRequestRepository : IRepositoryBase<MaterialRequest>
    {
        Task<PagedList<MaterialRequest>> GetAllMaterialRequest(PagingParameter pagingParameter);
        Task<MaterialRequest> GetMaterialRequestById(int id);
        Task<MaterialRequest> GetMaterialReqByMRNumber(string MRnumber);
        Task<int?> CreateMaterialRequest(MaterialRequest request);
        Task<string> UpdateMaterialRequest(MaterialRequest request);
        Task<string> DeleteMaterialRequest(MaterialRequest request);
        Task<IEnumerable<MaterialRequestIdNoDto>> GetAllOpenMRIdNoList();
        public void SaveAsync();
    }
}