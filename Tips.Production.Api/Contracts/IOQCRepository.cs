using Entities.Helper;
using Entities;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Production.Api.Contracts
{
    public interface IOQCRepository
    {
        Task<PagedList<OQC>> GetAllOQC(PagingParameter pagingParameter, SearchParamess searchParamess);
        Task<OQC> GetOQCById(int id);
        Task<int?> CreateOQC(OQC oqc);
        Task<string> UpdateOQC(OQC oqc);
        Task<string> DeleteOQC(OQC oqc);
        void SaveAsync();
        Task<IEnumerable<ShopOrderConfirmationItemNoListDto>> GetShopOrderConfirmationItemNoByFGItemType();
        Task<IEnumerable<ShopOrderConfirmationItemNoListDto>> GetShopOrderConfirmationItemNoBySAItemType();
        Task<IEnumerable<ShopOrderConfirmationDetailsDto>> GetShopOrderConfirmationDetailsByItemNo(string itemNumber);
        Task<IEnumerable<OQC>> SearchOQC([FromQuery] SearchParamess searchParames);
        Task<IEnumerable<OQC>> SearchOQCDate([FromQuery] SearchDateparames searchsDateParms);
        Task<IEnumerable<OQC>> GetAllOQCWithItems(OQCSearchDto oQCSearch);
        Task<IEnumerable<OQCIdNameList>> GetAllOQCIdNameList();
        Task<List<OQCStock>?> GetOQCAcceptedQty(string Itemnumber);
    }
}
