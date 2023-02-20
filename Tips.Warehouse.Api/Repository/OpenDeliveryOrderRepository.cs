using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Repository;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;



namespace Tips.Warehouse.Api.Repository
{
    public class OpenDeliveryOrderRepository : RepositoryBase<OpenDeliveryOrder>, IOpenDeliveryOrderRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        public OpenDeliveryOrderRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
        }

        public async Task<int?> CreateOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder)
        {
            var date = DateTime.Now;
            openDeliveryOrder.CreatedBy = "Admin";
            openDeliveryOrder.CreatedOn = date.Date;          
          
            openDeliveryOrder.Unit = "Bangalore";
            var result = await Create(openDeliveryOrder);
           
            return result.Id;
        }        

        public async Task<int?> GetODONumberAutoIncrementCount(DateTime date)
        {
            var getOpenDeliveryOrderDetailsByIds = _tipsWarehouseDbContext.OpenDeliveryOrders.Where(x => x.CreatedOn == date.Date).Count();

            return getOpenDeliveryOrderDetailsByIds;
        }

        public async Task<string> DeleteOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder)
        {
            Delete(openDeliveryOrder);
            string result = $"OpenDeliveryOrder details of {openDeliveryOrder.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<OpenDeliveryOrder>> GetAllOpenDeliveryOrders(PagingParameter pagingParameter)
        {

            var getAllOpenDeliveryOrderDetails = PagedList<OpenDeliveryOrder>.ToPagedList(FindAll().OrderByDescending(x => x.Id)
                                .Include(x => x.OpenDeliveryOrderParts)
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return getAllOpenDeliveryOrderDetails;

        }

        public async Task<OpenDeliveryOrder> GetOpenDeliveryOrderById(int id)
        {
            var getOpenDeliveryOrderDetailsById = await _tipsWarehouseDbContext.OpenDeliveryOrders.Where(x => x.Id == id)
                               .Include(x => x.OpenDeliveryOrderParts)

                               .FirstOrDefaultAsync();

            return getOpenDeliveryOrderDetailsById;
        }

        public async Task<string> UpdateOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder)
        {
            openDeliveryOrder.LastModifiedBy = "Admin";
            openDeliveryOrder.LastModifiedOn = DateTime.Now;
            Update(openDeliveryOrder);
            string result = $"openDeliveryOrder of Detail {openDeliveryOrder.Id} is updated successfully!";
            return result;
        }

    }
}
