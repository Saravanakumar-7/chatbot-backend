using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Repository
{
    public class BTODeliveryOrderInventoryHistoryRepository : RepositoryBase<BTODeliveryOrderInventoryHistory>, IBTODeliveryOrderInventoryHistoryRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        public BTODeliveryOrderInventoryHistoryRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
        }


        public async Task<long> CreateBTODeliveryOrderInventoryHistory(BTODeliveryOrderInventoryHistory bTODeliveryOrderInventoryHistory)
        {
            bTODeliveryOrderInventoryHistory.CreatedBy = "Admin";
            bTODeliveryOrderInventoryHistory.CreatedOn = DateTime.Now;
            bTODeliveryOrderInventoryHistory.Unit = "Banglore";
            var result = await Create(bTODeliveryOrderInventoryHistory);
            return result.Id;
        }
    }
}