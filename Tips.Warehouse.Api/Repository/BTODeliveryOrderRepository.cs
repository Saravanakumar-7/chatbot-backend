using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.Identity.Client;
using System.Linq;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Entities.DTOs;
using Entities.Enums;

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

        public async Task<string> GenerateBTONumber()
        {
            using var transaction = await _tipsWarehouseDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var btoNumberEntity = await _tipsWarehouseDbContext.BTONumbers.SingleAsync();
                btoNumberEntity.CurrentValue += 1;
                _tipsWarehouseDbContext.Update(btoNumberEntity);
                await _tipsWarehouseDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"BTO-{btoNumberEntity.CurrentValue:D6}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<IEnumerable<ListOfBtoNumberDetails>> GetBtoNumberListBySalesOrderId(int salesOrderId)
        {

            IEnumerable<ListOfBtoNumberDetails> btoNumberList = await _tipsWarehouseDbContext.bTODeliveryOrder
                                 .Where(x => x.SalesOrderId == salesOrderId)
                                .Select(x => new ListOfBtoNumberDetails()
                                {
                                    CustomerLeadID = x.CustomerId,
                                    BTONumber = x.BTONumber,
                                    BtoDeliveryOrderId = x.Id,
                                    OrderType = x.OrderType,
                                    TotalValue = x.TotalValue

                                })

                              .ToListAsync();

            return btoNumberList;
        }
        public async Task<string> DeleteBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder)
        {
            Delete(bTODeliveryOrder);
            string result = $"BTODeliveryOrder details of {bTODeliveryOrder.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<BTODeliveryOrder>> GetAllBTODeliveryOrderWithItems(BTODeliveryOrderSearchDto bTODeliveryOrderSearch)
        {
            using (var context = _tipsWarehouseDbContext)
            { 
                var query = _tipsWarehouseDbContext.bTODeliveryOrder.Include("bTODeliveryOrderItems");
                if (bTODeliveryOrderSearch != null || (bTODeliveryOrderSearch.SalesOrderNumber.Any())
                 && bTODeliveryOrderSearch.BTONumber.Any() && bTODeliveryOrderSearch.CustomerName.Any() 
                 && bTODeliveryOrderSearch.PONumber.Any() && bTODeliveryOrderSearch.IssuedTo.Any())

                {
                    query = query.Where
                    (po => (bTODeliveryOrderSearch.CustomerName.Any() ? bTODeliveryOrderSearch.CustomerName.Contains(po.CustomerName) : true)
                   && (bTODeliveryOrderSearch.SalesOrderNumber.Any() ? bTODeliveryOrderSearch.SalesOrderNumber.Contains(po.SalesOrderNumber) : true)
                   && (bTODeliveryOrderSearch.PONumber.Any() ? bTODeliveryOrderSearch.PONumber.Contains(po.PONumber) : true)
                   && (bTODeliveryOrderSearch.BTONumber.Any() ? bTODeliveryOrderSearch.BTONumber.Contains(po.BTONumber) : true)
                   && (bTODeliveryOrderSearch.IssuedTo.Any() ? bTODeliveryOrderSearch.IssuedTo.Contains(po.IssuedTo) : true));
                }
                return query.ToList();
            }
        }
        public async Task<IEnumerable<BTODeliveryOrder>> SearchBTODeliveryOrderDate([FromQuery] SearchsDateParms searchsDateParms)
        {
            var btoDeliveryOrderDetails = _tipsWarehouseDbContext.bTODeliveryOrder
            .Where(inv => ((inv.CreatedOn >= searchsDateParms.SearchFromDate &&
            inv.CreatedOn <= searchsDateParms.SearchToDate
            )))
            .Include(itm => itm.bTODeliveryOrderItems)
            .ToList();
            return btoDeliveryOrderDetails;
        }
        public async Task<IEnumerable<BTODeliveryOrder>> SearchBTODeliveryOrder([FromQuery] SearchParames searchParames)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.bTODeliveryOrder.Include("bTODeliveryOrderItems");
                if (!string.IsNullOrEmpty(searchParames.SearchValue))
                {
                    query = query.Where(po => po.SalesOrderNumber.Contains(searchParames.SearchValue)
                    || po.CustomerName.Contains(searchParames.SearchValue)
                    || po.PONumber.Contains(searchParames.SearchValue)
                    || po.IssuedTo.Contains(searchParames.SearchValue)
                    || po.bTODeliveryOrderItems.Any(s => s.FGItemNumber.Contains(searchParames.SearchValue) ||
                    s.BTONumber.Contains(searchParames.SearchValue)
                    || s.Description.Contains(searchParames.SearchValue)));
                }
                return query.ToList();
            }
        }
        public async Task<PagedList<BTODeliveryOrder>> GetAllActiveBTODeliveryOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {


            var getAllActiveBTODetails = FindAll()
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue)
                || inv.BTONumber.Contains(searchParams.SearchValue)
                || inv.PONumber.Contains(searchParams.SearchValue)
                || inv.CustomerName.Contains(searchParams.SearchValue)
                || inv.SalesOrderId.Equals(int.Parse(searchParams.SearchValue)))))
                .Include(t => t.bTODeliveryOrderItems);

            return PagedList<BTODeliveryOrder>.ToPagedList(getAllActiveBTODetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<BTODeliveryOrder>> GetAllBTODeliveryOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {


            var getAllBTODetails = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BTONumber.Contains(searchParams.SearchValue) ||
                 inv.PONumber.Contains(searchParams.SearchValue) || inv.CustomerName.Contains(searchParams.SearchValue)
                 || inv.SalesOrderId.Equals(int.Parse(searchParams.SearchValue)))))
                 .Include(t => t.bTODeliveryOrderItems);

            return PagedList<BTODeliveryOrder>.ToPagedList(getAllBTODetails, pagingParameter.PageNumber, pagingParameter.PageSize);
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
                                    BtoDeliveryOrderId = x.Id,
                                    OrderType = x.OrderType,
                                    TotalValue = x.TotalValue

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
                                .Include(t => t.bTODeliveryOrderItems)
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

        public async Task<IEnumerable<BtoIDNameList>> GetAllBTOIdNameIdNameList()
        {
            IEnumerable<BtoIDNameList> btoIddNameList = await _tipsWarehouseDbContext.bTODeliveryOrder
                               .Select(x => new BtoIDNameList()
                               {
                                   Id = x.Id,
                                   
                                   BTONumber = x.BTONumber,

                                   IssuedTo= x.IssuedTo

                               })
                               .OrderByDescending(x => x.Id)
                             .ToListAsync();

            return btoIddNameList;
        }
    }
    public class BTODeliveryOrderItemRepository : RepositoryBase<BTODeliveryOrderItems>, IBTODeliveryOrderItemsRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContexts;
        public BTODeliveryOrderItemRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContexts = repositoryContext;
        }

        public async Task<List<BTODeliveryOrderItems>> GetOpenDoItemDetailsByItemNoAndDoNo(string itemNumber, string BtoDeliveryNumber)
        {
            var btoDeliveryOrderDetails = await _tipsWarehouseDbContexts.bTODeliveryOrderItems
                    .Where(x => x.FGItemNumber == itemNumber && x.BTONumber == BtoDeliveryNumber && x.DoStatus != Status.Closed)
                          .ToListAsync();
            
            return btoDeliveryOrderDetails;
        }

        public async Task UpdateBtoDelieveryOrderItem(BTODeliveryOrderItems btoDeliveryOrderItem)
        {
            btoDeliveryOrderItem.LastModifiedBy = "Admin";
            btoDeliveryOrderItem.LastModifiedOn = DateTime.Now;
            Update(btoDeliveryOrderItem);
        }

        //public async Task<List<BTODeliveryOrderItems>> UpdateBtoDelieveryOrderBalanceQty(string itemNumber, string BtoDeliveryNumber, decimal Qty)
        //{
        //    var btoDeliveryOrderDetails = await _tipsWarehouseDbContexts.bTODeliveryOrderItems
        //            .Where(x => x.FGItemNumber == itemNumber && x.BTONumber == BtoDeliveryNumber && x.DoStatus != Status.Closed)
        //                  .ToListAsync();
        //    //var Quantity = Convert.ToDecimal(Qty);
        //    btoDeliveryOrderDetails.InvoicedQty = btoDeliveryOrderDetails.InvoicedQty + Qty;
        //    btoDeliveryOrderDetails.BalanceDoQty = btoDeliveryOrderDetails.DispatchQty - btoDeliveryOrderDetails.InvoicedQty;
            
        //    Update(btoDeliveryOrderDetails);
        //    return btoDeliveryOrderDetails;
        //}
       
        
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
            bTODeliveryOrderHistory.Unit = "Banglore";
            var result = await Create(bTODeliveryOrderHistory);
            return result.Id;
        }


        public async Task<PagedList<BTODeliveryOrderHistory>> GetAllBtoHistoryDetails(PagingParameter pagingParameter)
        {
            var bto = await _tipsWarehouseDbContext.ReturnBtoDeliveryOrders
              .FirstOrDefaultAsync();

            var getAllBTODetails = PagedList<BTODeliveryOrderHistory>.ToPagedList(FindAll()
                    .Where(x => x.UniqeId != null)
                    .OrderByDescending(x => x.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

          

            return getAllBTODetails;
        }

        public async Task<BTODeliveryOrderHistory> GetBtoHistoryDetailsById(int id)
        {
            var BtoHistoryDetails = await _tipsWarehouseDbContext.BTODeliveryOrderHistories.Where(x => x.Id == id)
                                .FirstOrDefaultAsync();
            return BtoHistoryDetails;
        }
        public async Task<IEnumerable<BTODeliveryOrderHistory>> GetBtoHistoryDetailsByBtoNo(string btoNumber, string uniqueId)
        {
            var BtoHistoryDetails = await _tipsWarehouseDbContext.BTODeliveryOrderHistories
              .Where(x => x.BTONumber == btoNumber && x.UniqeId == uniqueId)                
                        .ToListAsync();
            return BtoHistoryDetails;
        }
    }
}


