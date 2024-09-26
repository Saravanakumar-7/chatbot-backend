using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Repository
{
    public class OpenDeliveryOrderPartsRepository : RepositoryBase<OpenDeliveryOrderParts>, IOpenDeliveryOrderPartsRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContexts;
        public OpenDeliveryOrderPartsRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContexts = repositoryContext;
        }

        public async Task<OpenDeliveryOrderParts> GetOpenDelieveryOrderPartDetails(int odoDeliveryOrderPartsId)
        {
            var getBTODeliveryOrderItemDetails = await _tipsWarehouseDbContexts.OpenDeliveryOrderParts
                    .Where(x => x.Id == odoDeliveryOrderPartsId)
                          .FirstOrDefaultAsync();
            return getBTODeliveryOrderItemDetails;
        }

        //public async Task<OpenDeliveryOrderParts> UpdateOpenDelieveryOrderBalanceQty(string itemNumber, string odoNumber, decimal Qty)
        //{
        //    var openDeliveryOrderDetails = await _tipsWarehouseDbContexts.OpenDeliveryOrderParts
        //           .Where(x => x.ItemNumber == itemNumber && x.ODONumber == odoNumber)
        //                 .FirstOrDefaultAsync();
        //    //var Quantity = Convert.ToDecimal(Qty);
        //    openDeliveryOrderDetails.InvoicedQty = openDeliveryOrderDetails.InvoicedQty + Qty;
        //    openDeliveryOrderDetails.BalanceDoQty = openDeliveryOrderDetails.DispatchQty - openDeliveryOrderDetails.InvoicedQty;

        //    Update(openDeliveryOrderDetails);
        //    return openDeliveryOrderDetails;
        //}
    }
}
