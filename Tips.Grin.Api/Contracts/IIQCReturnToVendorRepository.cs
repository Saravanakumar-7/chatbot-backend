using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IIQCReturnToVendorRepository : IRepositoryBase<IQCReturnToVendor>
    {
        Task<int?> CreateIQCReturnToVendor(IQCReturnToVendor iQCReturnToVendor);
        Task<PagedList<IQCReturnToVendor>> GetAllIQCReturnToVendor([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams);
        Task<IQCReturnToVendor> GetIQCReturnToVendorById(int id);
    }
}
