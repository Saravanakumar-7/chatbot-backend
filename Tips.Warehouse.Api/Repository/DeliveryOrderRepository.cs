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
            deliveryOrder.Unit = "Bangalore";
            var result = await Create(deliveryOrder);
            return result.Id;
        }

        public async Task<string> DeleteDeliveryOrder(DeliveryOrder deliveryOrder)
        {
            Delete(deliveryOrder);
            string result = $"DeliveryOrder details of {deliveryOrder.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<DeliveryOrder>> GetAllActiveDeliveryOrders()
        {
            var getAllActiveDeliveryOrderDetails = await FindAll().ToListAsync();
            return getAllActiveDeliveryOrderDetails;
        }

        public async Task<PagedList<DeliveryOrder>> GetAllDeliveryOrders(PagingParameter pagingParameter)
        {
            var getAllDeliveryOrderDetails = PagedList<DeliveryOrder>.ToPagedList(FindAll()
                                 .Include(t => t.DeliveryOrderItems)
                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllDeliveryOrderDetails;
        }

        public async Task<DeliveryOrder> GetDeliveryOrderById(int id)
        {
            var getDeliveryOrderDetailsbyId = await _tipsWarehouseDbContext.deliveryOrder.Where(x => x.Id == id)
                              .Include(t => t.DeliveryOrderItems)
                              .FirstOrDefaultAsync();


            return getDeliveryOrderDetailsbyId;
        }

        public async Task<string> UpdateDeliveryOrder(DeliveryOrder deliveryOrder)
        {
            deliveryOrder.LastModifiedBy = "Admin";
            deliveryOrder.LastModifiedOn = DateTime.Now;
            Update(deliveryOrder);
            string result = $"DeliveryOrder of Detail {deliveryOrder.Id} is updated successfully!";
            return result;
        }
    }
}
