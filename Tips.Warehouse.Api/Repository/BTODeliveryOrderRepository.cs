using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Repository
{
    public class BTODeliveryOrderRepository : RepositoryBase<BTODeliveryOrder>, IBTODeliveryOrderRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        public BTODeliveryOrderRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
        }

        public async Task<long> CreateBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder)
        {
            bTODeliveryOrder.CreatedBy = "Admin";
            bTODeliveryOrder.CreatedOn = DateTime.Now;
            bTODeliveryOrder.LastModifiedBy = "Admin";
            bTODeliveryOrder.LastModifiedOn = DateTime.Now;
            var result = await Create(bTODeliveryOrder);
            return result.Id;
        }

        public async Task<string> DeleteBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder)
        {
            Delete(bTODeliveryOrder);
            string result = $"BTODeliveryOrder details of {bTODeliveryOrder.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<BTODeliveryOrder>> GetAllActiveBTODeliveryOrder()
        {
            var bTODeliveryOrderDetails = await FindAll().ToListAsync();
            return bTODeliveryOrderDetails;
        }

        public async Task<PagedList<BTODeliveryOrder>> GetAllBTODeliveryOrder(PagingParameter pagingParameter, string BTONumber)
        {
            var bTODeliveryOrderDetails = PagedList<BTODeliveryOrder>.ToPagedList(FindAll()
                                 .Include(t => t.bTODeliveryOrderItems)
                                 .ThenInclude(s => s.bTOSerialNumbers)
                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return bTODeliveryOrderDetails;
        }

        public async Task<BTODeliveryOrder> GetBTODeliveryOrderById(int id, string BTONumber)
        {
            var bTODeliveryOrderDetails = await _tipsWarehouseDbContext.bTODeliveryOrder.Where(x => x.Id == id)
                                .Include(t => t.bTODeliveryOrderItems)
                                .ThenInclude(s=>s.bTOSerialNumbers)
                                .FirstOrDefaultAsync();


            return bTODeliveryOrderDetails;
        }

        public async Task<string> UpdateBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder, string BTONumber)
        {
            bTODeliveryOrder.LastModifiedBy = "Admin";
            bTODeliveryOrder.LastModifiedOn = DateTime.Now;
            Update(bTODeliveryOrder);
            string result = $"BTODeliveryOrder of Detail {bTODeliveryOrder.Id} is updated successfully!";
            return result;
        }
    }
}
