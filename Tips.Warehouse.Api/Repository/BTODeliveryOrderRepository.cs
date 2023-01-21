using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
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
            var date = DateTime.Now;
            bTODeliveryOrder.CreatedBy = "Admin";
            bTODeliveryOrder.CreatedOn = date.Date;
            bTODeliveryOrder.Unit = "Bangalore";
            Guid btoDeliveryOrderNumber = Guid.NewGuid();
            bTODeliveryOrder.BTONumber = " BTO-" + btoDeliveryOrderNumber.ToString();
            var result = await Create(bTODeliveryOrder);
            return result.Id;
        }
        public async Task<int?> GetBTONumberAutoIncrementCount(DateTime date)
        {
            var getBTODeliveryOrderDetailsByIds = _tipsWarehouseDbContext.bTODeliveryOrder.Where(x => x.CreatedOn == date.Date).Count();

            return getBTODeliveryOrderDetailsByIds;
        }
        public async Task<string> DeleteBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder)
        {
            Delete(bTODeliveryOrder);
            string result = $"BTODeliveryOrder details of {bTODeliveryOrder.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<BTODeliveryOrder>> GetAllActiveBTODeliveryOrders()
        {
            var getAllActiveBTODetails = await FindAll().ToListAsync();
            return getAllActiveBTODetails;
        }

        public async Task<PagedList<BTODeliveryOrder>> GetAllBTODeliveryOrders(PagingParameter pagingParameter)
        {
            var getAllBTODetails = PagedList<BTODeliveryOrder>.ToPagedList(FindAll()
                                 .Include(t => t.BTODeliveryOrderItems)
                                 .ThenInclude(s => s.BTOSerialNumbers)
                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllBTODetails;
        }

        

        public async Task<BTODeliveryOrder> GetBTODeliveryOrderById(int id)
        {
            var getBTODeliveryOrderDetailsbyId = await _tipsWarehouseDbContext.bTODeliveryOrder.Where(x => x.Id == id)
                                .Include(t => t.BTODeliveryOrderItems)
                                .ThenInclude(s => s.BTOSerialNumbers)
                                .FirstOrDefaultAsync();


            return getBTODeliveryOrderDetailsbyId;
        }

    

        public async Task<string> UpdateBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder)
        {
            bTODeliveryOrder.LastModifiedBy = "Admin";
            bTODeliveryOrder.LastModifiedOn = DateTime.Now;
            Update(bTODeliveryOrder);
            string result = $"BTODeliveryOrder of Detail {bTODeliveryOrder.Id} is updated successfully!";
            return result;
        }

       
    }
        }
 
