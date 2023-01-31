 using Tips.Production.Api.Entities;
namespace Tips.Production.Api.Contracts
{
    public interface IShopOrderConfirmationRepository : IRepositoryBase<ShopOrderConfirmation>
    {
        Task<IEnumerable<ShopOrderConfirmation>> GetAllShopOrderConfirmations();
        Task<ShopOrderConfirmation> GetShopOrderConfirmationById(int id);
        Task<long> CreateShopOrderConfirmation(ShopOrderConfirmation shopOrderConfirmation);
        Task<string> UpdateShopOrderConfirmation(ShopOrderConfirmation shopOrderConfirmation);
        Task<IEnumerable<ShopOrderConfirmation>> GetAllShopOrderConfirmationByShopOrderNo(string shopOrderNo);
        Task<IEnumerable<ShopOrderConfirmation>> GetOpenDataForOqcByShopOrderNo(string shopOrderNo);
        

    }
}
