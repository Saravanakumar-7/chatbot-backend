using Entities.Helper;
using Entities;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

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
        Task<IEnumerable<MaterialRequests>> GetAllMaterialRequestsWithItems(MaterialRequestSearchDto materialRequestSearch);
        Task<IEnumerable<MaterialRequests>> SearchMaterialRequests([FromQuery] SearchParamess searchParammes);
        Task<IEnumerable<MaterialRequests>> SearchMaterialRequestsDate([FromQuery] SearchDateparames searchDatesParams);
        public void SaveAsync();
    }
}
