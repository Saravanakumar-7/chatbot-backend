using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IBTODeliveryOrderInventoryHistoryRepository : IRepositoryBase<BTODeliveryOrderInventoryHistory>
    {
        Task<long> CreateBTODeliveryOrderInventoryHistory(BTODeliveryOrderInventoryHistory bTODeliveryOrderInventoryHistory);

    }
}
