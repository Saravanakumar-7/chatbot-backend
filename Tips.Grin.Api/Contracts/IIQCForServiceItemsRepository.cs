using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IIQCForServiceItemsRepository : IRepositoryBase<IQCForServiceItems>
    {
        Task<IQCForServiceItems> GetIqcForServiceItemsDetailsbyGrinForServiceItemsNo(string grinsForServiceItemsNumber);
        Task<int?> CreateIQCForServiceItems(IQCForServiceItems iQCForServiceItems);
        Task<PagedList<IQCForServiceItems>> GetAllIQCForServiceItemsDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams);
        Task<IQCForServiceItems> GetIQCForServiceItemsDetailsbyGrinForServiceItemsNo(string grinsForServiceItemsNumber);
        Task<string> UpdateIQCForServiceItems(IQCForServiceItems iQCForServiceItems);
    }
}
