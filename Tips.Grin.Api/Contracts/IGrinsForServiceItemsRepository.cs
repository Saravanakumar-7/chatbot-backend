using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IGrinsForServiceItemsRepository : IRepositoryBase<GrinsForServiceItems>
    {
        Task<PagedList<GrinsForServiceItems>> GrinsForServiceItemsForServiceItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams);
    }
}
