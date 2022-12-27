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
            openDeliveryOrder.CreatedBy = "Admin";
            openDeliveryOrder.CreatedOn = DateTime.Now;
            openDeliveryOrder.LastModifiedBy = "Admin";
            openDeliveryOrder.LastModifiedOn = DateTime.Now;

            var result = await Create(openDeliveryOrder);
            openDeliveryOrder.Unit = "Bangalore";
            return result.Id;
        }

            public async Task<string> DeleteOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder)
        {
            Delete(openDeliveryOrder);
            string result = $"OpenDeliveryOrder details of {openDeliveryOrder.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<OpenDeliveryOrder>> GetAllOpenDeliveryOrders(PagingParameter pagingParameter)
        {

            var OpenDeliveryOrderDetails = PagedList<OpenDeliveryOrder>.ToPagedList(FindAll()
                                .Include(x => x.OpenDeliveryOrderParts)
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return OpenDeliveryOrderDetails;

        }

        public async Task<OpenDeliveryOrder> GetOpenDeliveryOrderById(int id)
        {
            var OpenDeliveryOrderDetails = await _tipsWarehouseDbContext.OpenDeliveryOrders.Where(x => x.Id == id)
                               .Include(x => x.OpenDeliveryOrderParts)

                               .FirstOrDefaultAsync();

            return OpenDeliveryOrderDetails;
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
