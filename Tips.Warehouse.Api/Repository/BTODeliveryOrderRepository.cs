using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

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
            //Guid btoDeliveryOrderNumber = Guid.NewGuid();
            //bTODeliveryOrder.BTONumber = " BTO-" + btoDeliveryOrderNumber.ToString();
            var result = await Create(bTODeliveryOrder);
            return result.Id;
        }
        public async Task<int?> GetBTONumberAutoIncrementCount(DateTime date)
        {
            var getBTONumberAutoIncrementCount = _tipsWarehouseDbContext.bTODeliveryOrder.Where(x => x.CreatedOn == date.Date).Count();

            return getBTONumberAutoIncrementCount;
        }
        public async Task<string> DeleteBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder)
        {
            Delete(bTODeliveryOrder);
            string result = $"BTODeliveryOrder details of {bTODeliveryOrder.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<BTODeliveryOrder>> GetAllActiveBTODeliveryOrders()
        {
            var getAllActiveBTODetails = await FindAll().OrderByDescending(x => x.Id).ToListAsync();
            return getAllActiveBTODetails;
        }

        public async Task<PagedList<BTODeliveryOrder>> GetAllBTODeliveryOrders(PagingParameter pagingParameter)
        {
            var getAllBTODetails = PagedList<BTODeliveryOrder>.ToPagedList(FindAll()
                                 .Include(t => t.BTODeliveryOrderItems)
                                 //.ThenInclude(s => s.BTOSerialNumbers)
                .OrderByDescending(x => x.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllBTODetails;
        }

        public async Task<IEnumerable<ListofBtoDeliveryOrderDetails>> GetBtoDeliveryOrderNumberList()
        {

            IEnumerable<ListofBtoDeliveryOrderDetails> getBtoDeliveryOrderList = await _tipsWarehouseDbContext.bTODeliveryOrder
                                .Select(x => new ListofBtoDeliveryOrderDetails()
                                {
                                    BtoDeliveryOrderId = x.Id,
                                    BTONumber = x.BTONumber,

                                })
                                .OrderBy(on => on.BtoDeliveryOrderId)
                              .ToListAsync();

            return getBtoDeliveryOrderList;
        }

        public async Task<IEnumerable<ListOfBtoNumberDetails>> GetBtoNumberListByCustomerId(string customerLeadId)
        {

            IEnumerable<ListOfBtoNumberDetails> getBtoNumberList = await _tipsWarehouseDbContext.bTODeliveryOrder
                                .Select(x => new ListOfBtoNumberDetails()
                                {
                                    CustomerLeadID = x.CustomerId,
                                    BTONumber = x.BTONumber,

                                })
                                .Where(x => x.CustomerLeadID == customerLeadId)
                              .ToListAsync();

            return getBtoNumberList;
        }
        public async Task<BTODeliveryOrder> GetBtoDetailsByBtoNo(string BTONumber)
        {
            var getBtoDetailsByBtoNo = await _tipsWarehouseDbContext.bTODeliveryOrder
                    .Where(x => x.BTONumber == BTONumber)
                          .FirstOrDefaultAsync();
            return getBtoDetailsByBtoNo;
        }

        public async Task<BTODeliveryOrder> GetBTODeliveryOrderById(int id)
        {
            var getBTODeliveryOrderDetailsbyId = await _tipsWarehouseDbContext.bTODeliveryOrder.Where(x => x.Id == id)
                                .Include(t => t.BTODeliveryOrderItems)
                                //.ThenInclude(s => s.BTOSerialNumbers)
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
    public class BTODeliveryOrderItemRepository : RepositoryBase<BTODeliveryOrderItems>, IBTODeliveryOrderItemsRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContexts;
        public BTODeliveryOrderItemRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContexts = repositoryContext;
        }
        public async Task<BTODeliveryOrderItems> UpdateBtoDelieveryOrderBalanceQty(string itemNumber, string BtoDeliveryNumber, string Qty)
        {
            var getSalesOrderDetailsBySOandItemNo = await _tipsWarehouseDbContexts.bTODeliveryOrderItems
                    .Where(x => x.FGItemNumber == itemNumber && x.BTONumber == BtoDeliveryNumber)
                          .FirstOrDefaultAsync();
            decimal Quantity = Convert.ToDecimal(Qty);
            getSalesOrderDetailsBySOandItemNo.BalanceDoQty = getSalesOrderDetailsBySOandItemNo.DispatchQty - Quantity;
            getSalesOrderDetailsBySOandItemNo.InvoicedQty += Quantity;
            Update(getSalesOrderDetailsBySOandItemNo);
            return getSalesOrderDetailsBySOandItemNo;
        }
        public async Task<BTODeliveryOrderItems> GetBtoDelieveryOrderItemDetails(int btoDeliveryOrderPartsId)
        {
            var getBTODeliveryOrderItemDetails = await _tipsWarehouseDbContexts.bTODeliveryOrderItems
                    .Where(x => x.Id == btoDeliveryOrderPartsId)
                          .FirstOrDefaultAsync(); 
            return getBTODeliveryOrderItemDetails;
        }
        

    }

    public class BTODeliveryOrderHistoryRepository : RepositoryBase<BTODeliveryOrderHistory>, IBTODeliveryOrderHistoryRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        public BTODeliveryOrderHistoryRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
        }
         

        public async Task<long> CreateBTODeliveryOrderHistory(BTODeliveryOrderHistory bTODeliveryOrderHistory)
        {
             bTODeliveryOrderHistory.CreatedBy = "Admin";
            bTODeliveryOrderHistory.CreatedOn = DateTime.Now; 
            var result = await Create(bTODeliveryOrderHistory);
            return result.Id;
        }

    }
}


