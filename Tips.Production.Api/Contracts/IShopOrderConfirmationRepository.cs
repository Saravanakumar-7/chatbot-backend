using Entities;
using Entities.DTOs;
using Entities.Helper;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Contracts
{
    public interface IShopOrderConfirmationRepository : IRepositoryBase<ShopOrderConfirmation>
    {
        Task<PagedList<ShopOrderConfirmation>> GetAllShopOrderConfirmation(PagingParameter pagingParameter, SearchParamess searchParamess);

        Task<ShopOrderConfirmation> GetShopOrderConfirmationById(int id);
        Task<long> CreateShopOrderConfirmation(ShopOrderConfirmation shopOrderConfirmation);
        Task<string> UpdateShopOrderConfirmation(ShopOrderConfirmation shopOrderConfirmation);
        Task<IEnumerable<ShopOrderConfirmation>> GetAllShopOrderConfirmationByShopOrderNo(string shopOrderNo);
        Task<IEnumerable<ShopOrderConfirmation>> GetOpenDataForOqcByShopOrderNo(string shopOrderNo);
        Task<IEnumerable<ShopOrderItemNoListDto>> GetShopOrderItemNoByFGItemType();
        Task<IEnumerable<ShopOrderItemNoListDto>> GetShopOrderItemNoBySAItemType();
        Task<IEnumerable<ShopOrderItemNoListDto>> GetShopOrderItemNoByKITItemType();
        Task<IEnumerable<ShopOrderDetailsDto>> GetShopOrderDetailsByItemNo(string itemNumber);
        Task<IEnumerable<ShopOrderDetailsDto>> GetShopOrderConformationDetailsByItemNo(string itemNumber);
    }
}
