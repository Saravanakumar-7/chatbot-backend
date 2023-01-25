using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Repository
{
    public class ReturnDeliveryOrderRepository : RepositoryBase<ReturnDeliveryOrder>, IReturnDeliveryOrderRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        public ReturnDeliveryOrderRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
        }

        public async Task<int?> CreateReturnDeliveryOrder(ReturnDeliveryOrder returnDeliveryOrder)
        {
            returnDeliveryOrder.CreatedBy = "Admin";
            returnDeliveryOrder.CreatedOn = DateTime.Now;
            returnDeliveryOrder.Unit = "Bangalore";
            var result = await Create(returnDeliveryOrder);
            return result.Id;
        }

        public async Task<string> DeleteReturnDeliveryOrder(ReturnDeliveryOrder returnDeliveryOrder)
        {
            Delete(returnDeliveryOrder);
            string result = $"DeleteReturnDeliveryOrder details of {returnDeliveryOrder.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<ReturnDeliveryOrder>> GetAllReturnDeliveryOrders(PagingParameter pagingParameter)
        {
            var getAllReturnDeliveryOrderDetails = PagedList<ReturnDeliveryOrder>.ToPagedList(FindAll()
                                .Include(t => t.ReturnDeliveryOrderItems)
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllReturnDeliveryOrderDetails;
        }

        public async Task<ReturnDeliveryOrder> GetReturnDeliveryOrderById(int id)
        {
            var getReturnDeliveryOrderDetailsbyId = await _tipsWarehouseDbContext.ReturnDeliveryOrders.Where(x => x.Id == id)
                                .Include(t => t.ReturnDeliveryOrderItems)
                                .FirstOrDefaultAsync();


            return getReturnDeliveryOrderDetailsbyId;
        }

        public async Task<string> UpdateReturnDeliveryOrder(ReturnDeliveryOrder returnDeliveryOrder)
        {

            returnDeliveryOrder.LastModifiedBy = "Admin";
            returnDeliveryOrder.LastModifiedOn = DateTime.Now;
            Update(returnDeliveryOrder);
            string result = $"returnDeliveryOrder of Detail {returnDeliveryOrder.Id} is updated successfully!";
            return result;
        }
    }
}
