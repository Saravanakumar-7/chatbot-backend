using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Repository
{
    public class DeliveryOrderRepository : RepositoryBase<DeliveryOrder>, IDeliveryOrderRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        public DeliveryOrderRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
        }

        public async Task<long> CreateDeliveryOrder(DeliveryOrder deliveryOrder)
        {
            deliveryOrder.CreatedBy = "Admin";
            deliveryOrder.CreatedOn = DateTime.Now;
            deliveryOrder.LastModifiedBy = "Admin";
            deliveryOrder.LastModifiedOn = DateTime.Now;
            var result = await Create(deliveryOrder);
            return result.Id;
        }

        public async Task<string> DeleteDeliveryOrder(DeliveryOrder deliveryOrder)
        {
            Delete(deliveryOrder);
            string result = $"DeliveryOrder details of {deliveryOrder.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<DeliveryOrder>> GetAllActiveDeliveryOrder()
        {
            var deliveryOrderDetails = await FindAll().ToListAsync();
            return deliveryOrderDetails;
        }

        public async Task<PagedList<DeliveryOrder>> GetAllDeliveryOrder(PagingParameter pagingParameter, string DeliveryOrderNumber)
        {
            var deliveryOrderDetails = PagedList<DeliveryOrder>.ToPagedList(FindAll()
                                 .Include(t => t.deliveryOrderItems)
                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return deliveryOrderDetails;
        }

        public async Task<DeliveryOrder> GetDeliveryOrderById(int id, string DeliveryOrderNumber)
        {
            var deliveryOrderDetails = await _tipsWarehouseDbContext.deliveryOrder.Where(x => x.Id == id)
                              .Include(t => t.deliveryOrderItems)
                              .FirstOrDefaultAsync();


            return deliveryOrderDetails;
        }

        public async Task<string> UpdateDeliveryOrder(DeliveryOrder deliveryOrder, string DeliveryOrderNumber)
        {
            deliveryOrder.LastModifiedBy = "Admin";
            deliveryOrder.LastModifiedOn = DateTime.Now;
            Update(deliveryOrder);
            string result = $"DeliveryOrder of Detail {deliveryOrder.Id} is updated successfully!";
            return result;
        }
    }
}
