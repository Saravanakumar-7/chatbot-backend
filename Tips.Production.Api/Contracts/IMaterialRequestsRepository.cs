using Entities.Helper;
using Entities;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Contracts
{
    public interface IMaterialRequestsRepository : IRepositoryBase<MaterialRequests>
    {
        Task<PagedList<MaterialRequests>> GetAllMaterialRequest(PagingParameter pagingParameter, SearchParamess searchParammes);
        Task<MaterialRequests> GetMaterialRequestById(int id);
        Task<MaterialRequests> GetMaterialReqByMRNumber(string MRnumber);
        Task<int?> GetMRNumberAutoIncrementCount(DateTime date);
        Task<int?> CreateMaterialRequest(MaterialRequests request);
        Task<string> UpdateMaterialRequest(MaterialRequests request);
        Task<string> DeleteMaterialRequest(MaterialRequests request);
        Task<IEnumerable<MaterialRequestIdNoDto>> GetAllOpenMRIdNoList();
        public void SaveAsync();
    }
}
