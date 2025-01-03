using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface ILocationTransferPartNoRepository : IRepositoryBase<LocationTransferPartNo>
    {
        Task<int?> CreateLocationTransferPartNo(LocationTransferPartNo locationTransferPartNo);
        Task<PagedList<LocationTransferPartNo>> GetAllLocationTransferPartNo(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<LocationTransferPartNo> GetLocationTransferPartNoById(int id);
        Task<int> GetLatestLocationTransferPartNoId();
    }
}
