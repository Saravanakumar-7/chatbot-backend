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
            salesOrder.Unit = "Banglore";
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
            var getAllActiveSalesOrder = await FindAll().ToListAsync();
            return getAllActiveSalesOrder;
        }

        public async Task<PagedList<SalesOrder>> GetAllSalesOrder(PagingParameter pagingParameter)
        {

            var getAllSalesOrders = PagedList<SalesOrder>.ToPagedList(FindAll()
                                .Include(t => t.SalesOrdersItems)
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllSalesOrders;
        }

        public async Task<SalesOrder> GetSalesOrderById(int id)
        {
            var getSalesOrderbyId = await _tipsSalesServiceDbContext.SalesOrders.Where(x => x.Id == id)
                                  .Include(t => t.SalesOrdersItems)
                                 .FirstOrDefaultAsync();

            return getSalesOrderbyId;
        }

        public async Task<string> UpdateSalesOrder(SalesOrder salesOrder)
        {
            salesOrder.LastModifiedBy = "Admin";
            salesOrder.LastModifiedOn = DateTime.Now;
            Update(salesOrder);
            string result = $"SalesOrder of Detail {salesOrder.Id} is updated successfully!";
            return result;
        }

        //public Task<string> UpdateSOBasedOnCreatingDO()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<string> UpdateSOBasedOnCreatingShopOrder()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
