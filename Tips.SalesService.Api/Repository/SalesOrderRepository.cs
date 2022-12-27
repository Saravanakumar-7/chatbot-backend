using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Repository
{
    public class SalesOrderRepository : RepositoryBase<SalesOrder>, ISalesOrderRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public SalesOrderRepository(TipsSalesServiceDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsSalesServiceDbContext = repositoryContext;
        }

        public async Task<long> CreateSalesOrder(SalesOrder salesOrder)
        {

            salesOrder.CreatedBy = "Admin";
            salesOrder.CreatedOn = DateTime.Now;
            salesOrder.LastModifiedBy = "Admin";
            salesOrder.LastModifiedOn = DateTime.Now;
            var result = await Create(salesOrder);
            return result.Id;
        }

        public async Task<string> DeleteSalesOrder(SalesOrder salesOrder)
        {
            Delete(salesOrder);
            string result = $"SalesOrder details of {salesOrder.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<SalesOrder>> GetAllActiveSalesOrder()
        {
            var salesOrderDetails = await FindAll().ToListAsync();
            return salesOrderDetails;
        }

        public async Task<PagedList<SalesOrder>> GetAllSalesOrder(PagingParameter pagingParameter)
        {

            var salesOrderDetails = PagedList<SalesOrder>.ToPagedList(FindAll()
                                .Include(t => t.salesOrdersItems)
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return salesOrderDetails;
        }

        public async Task<SalesOrder> GetSalesOrderById(int id)
        {
            var salesOrderDetails = await _tipsSalesServiceDbContext.salesOrders.Where(x => x.Id == id)
                                  .Include(t => t.salesOrdersItems)
                                 .FirstOrDefaultAsync();

            return salesOrderDetails;
        }

        public async Task<string> UpdateSalesOrder(SalesOrder salesOrder)
        {
            salesOrder.LastModifiedBy = "Admin";
            salesOrder.LastModifiedOn = DateTime.Now;
            Update(salesOrder);
            string result = $"SalesOrder of Detail {salesOrder.Id} is updated successfully!";
            return result;
        }
    }
}
