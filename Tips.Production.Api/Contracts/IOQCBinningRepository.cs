using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Contracts
{
    public interface IOQCBinningRepository
    {
        Task<int> CreateOQCBinning(OQCBinning oQCBinning);
        Task<OQCBinning> GetOQCBinningById(int id);
        Task<PagedList<OQCBinning>> GetAllOQCBinning(PagingParameter pagingParameter, SearchParamess searchParamess);
        Task<decimal?> GetOqcBinningShopOrderQty(string Itemnumber, string ShopOrderNumber);
        void SaveAsync();
    }
}
