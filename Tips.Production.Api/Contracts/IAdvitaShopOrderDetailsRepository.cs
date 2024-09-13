using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Contracts
{
    public interface IAdvitaShopOrderDetailsRepository
    {
        Task<int?> CreateAdvitaShopOrderDetails(AdvitaShopOrderDetails advitaShopOrderDetails);
        void SaveAdvitaAsync();
    }

}
