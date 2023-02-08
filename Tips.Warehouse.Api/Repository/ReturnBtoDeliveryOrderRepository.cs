using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Repository
{
    public class ReturnBtoDeliveryOrderRepository : RepositoryBase<ReturnBtoDeliveryOrder>, IReturnBtoDeliveryOrderRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        public ReturnBtoDeliveryOrderRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
        }

        public async Task<int?> CreateReturnBtoDeliveryOrder(ReturnBtoDeliveryOrder returnBtoDeliveryOrder)
        {
            returnBtoDeliveryOrder.CreatedBy = "Admin";
            returnBtoDeliveryOrder.CreatedOn = DateTime.Now;
            returnBtoDeliveryOrder.Unit = "Bangalore";
            var result = await Create(returnBtoDeliveryOrder);
            return result.Id;
        }

        public async Task<string> DeleteReturnBtoDeliveryOrder(ReturnBtoDeliveryOrder returnBtoDeliveryOrder)
        {
            Delete(returnBtoDeliveryOrder);
            string result = $"DeleteReturnBtoDeliveryOrder details of {returnBtoDeliveryOrder.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<ReturnBtoDeliveryOrder>> GetAllReturnBtoDeliveryOrders(PagingParameter pagingParameter)
        {
            var getAllReturnBtoDeliveryOrders = PagedList<ReturnBtoDeliveryOrder>.ToPagedList(FindAll()
                                .Include(t => t.ReturnBtoDeliveryOrderItems)
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllReturnBtoDeliveryOrders;
        }

        public async Task<ReturnBtoDeliveryOrder> GetReturnBtoDeliveryOrderById(int id)
        {
            var getReturnBtoDeliveryOrderById = await _tipsWarehouseDbContext.ReturnBtoDeliveryOrders.Where(x => x.Id == id)
                                .Include(t => t.ReturnBtoDeliveryOrderItems)
                                .FirstOrDefaultAsync();


            return getReturnBtoDeliveryOrderById;
        }
        public async Task<int?> GetReturnBtoDeliveryOrderByBtoNo(string BTONumber)
        {
            var getReturnBtoDeliveryOrderByBtoNo =  _tipsWarehouseDbContext.ReturnBtoDeliveryOrders
                    .Where(x => x.BTONumber == BTONumber).Count();
            return getReturnBtoDeliveryOrderByBtoNo;
        }

        public async Task<string> UpdateReturnBtoDeliveryOrder(ReturnBtoDeliveryOrder returnBtoDeliveryOrder)
        {

            returnBtoDeliveryOrder.LastModifiedBy = "Admin";
            returnBtoDeliveryOrder.LastModifiedOn = DateTime.Now;
            Update(returnBtoDeliveryOrder);
            string result = $"returnBtoDeliveryOrder of Detail {returnBtoDeliveryOrder.Id} is updated successfully!";
            return result;
        }
    }
}
