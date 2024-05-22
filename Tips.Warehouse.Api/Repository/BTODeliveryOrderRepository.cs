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
using System.Security.Claims;

namespace Tips.Warehouse.Api.Repository
{
    public class BTODeliveryOrderRepository : RepositoryBase<BTODeliveryOrder>, IBTODeliveryOrderRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public BTODeliveryOrderRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<long> CreateBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder)
        {
            var date = DateTime.Now;
            bTODeliveryOrder.CreatedBy = _createdBy;
            bTODeliveryOrder.CreatedOn = date.Date;
            bTODeliveryOrder.Unit = _unitname;
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
        public async Task<string> GenerateBTONumberAvision()
        {
            using var transaction = await _tipsWarehouseDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var rfqNumberEntity = await _tipsWarehouseDbContext.BTONumbers.SingleAsync();
                rfqNumberEntity.CurrentValue += 1;
                _tipsWarehouseDbContext.Update(rfqNumberEntity);
                await _tipsWarehouseDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                //int currentYear = DateTime.Now.Year % 100; // Get the last two digits of the current year
                //int nextYear = (DateTime.Now.Year + 1) % 100; // Get the last two digits of the next year

                DateTime currentDate = DateTime.Now;
                DateTime financeYearStart;

                if (currentDate.Month >= 4) // Check if the current date is after or equal to April
                {
                    financeYearStart = new DateTime(currentDate.Year, 4, 1);
                }
                else
                {
                    financeYearStart = new DateTime(currentDate.Year - 1, 4, 1);
                }

                int currentYear = financeYearStart.Year % 100; // Get the last two digits of the current finance year
                int nextYear = (financeYearStart.Year + 1) % 100; // Get the last two digits of the next finance year

                return $"ASPL|DO|{currentYear:D2}-{nextYear:D2}|{rfqNumberEntity.CurrentValue:D3}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
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
        public async Task<IEnumerable<DailyDOReport>> GetDailyDeliveryOrderReports()
        {

            var result = _tipsWarehouseDbContext
            .Set<DailyDOReport>()
            .FromSqlInterpolated($"CALL Daily_report_Delivery_Order()")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<DailyDOReport>> GetDailyDeliveryOrderReports(string LeadId, string SONumber, string DOnumber, string DispatchKPN)
        {
            if (string.IsNullOrWhiteSpace(LeadId)
     || string.IsNullOrWhiteSpace(SONumber)
     || string.IsNullOrWhiteSpace(DOnumber)
     || string.IsNullOrWhiteSpace(DispatchKPN)) ;

            var result = _tipsWarehouseDbContext
            .Set<DailyDOReport>()
            .FromSqlInterpolated($"CALL Daily_report_Delivery_Order_withparameter({LeadId},{SONumber},{DOnumber},{DispatchKPN})")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<DeliveryOrderSPReport>> GetDeliveryOrderSPReportsWithParam(string DONumber, string CustomerName, string CustomerAliasName,
                                                                                                    string CustomerID, string SalesOrderNumber, string ProductType,
                                                                                                    string Warehouse, string Location, string KPN, string MPN, string ProjectNumber)
        {
            var result = _tipsWarehouseDbContext
            .Set<DeliveryOrderSPReport>()
            .FromSqlInterpolated($"CALL Delivery_Order_Report_withparameter({DONumber},{CustomerName},{CustomerAliasName},{CustomerID},{SalesOrderNumber},{ProductType},{Warehouse},{Location},{KPN},{MPN},{ProjectNumber})")
            .ToList();

            return result;
        }
        public async Task<PagedList<DeliveryOrderSPReport>> DeliveryOrderSPReport(PagingParameter pagingParameter)
        {
            var results = _tipsWarehouseDbContext.Set<DeliveryOrderSPReport>()
                        .FromSqlInterpolated($"CALL Delivery_Order_Report")
                        .ToList();

            return PagedList<DeliveryOrderSPReport>.ToPagedList(results.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize);

        }
        public async Task<IEnumerable<DeliveryOrderSPReport>> DeliveryOrderSPReportdate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsWarehouseDbContext.Set<DeliveryOrderSPReport>()
                         .FromSqlInterpolated($"CALL Delivery_Order_Report_withparameter_withdate({FromDate},{ToDate})")
                         .ToList();

            return results;
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
            int searchValueInt;
            bool isSearchValueInt = int.TryParse(searchParams.SearchValue, out searchValueInt);
            var getAllBTODetails = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => (string.IsNullOrWhiteSpace(searchParams.SearchValue)) || (string.IsNullOrEmpty(searchParams.SearchValue)
                || inv.BTONumber.Contains(searchParams.SearchValue) || inv.PONumber.Contains(searchParams.SearchValue)
                || inv.CustomerName.Contains(searchParams.SearchValue) || inv.CustomerId.Contains(searchParams.SearchValue)
                || inv.SalesOrderNumber.Contains(searchParams.SearchValue) || inv.CustomerAliasName.Contains(searchParams.SearchValue)
                || (isSearchValueInt && inv.SalesOrderId.Equals(searchValueInt)) || inv.TypeOfSolution.Contains(searchParams.SearchValue)
                || inv.OrderType.Contains(searchParams.SearchValue)))
                 .Include(t => t.bTODeliveryOrderItems);

            return PagedList<BTODeliveryOrder>.ToPagedList(getAllBTODetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        //public async Task<PagedList<BTODeliveryOrder>> GetAllBTODeliveryOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        //{
        //    var getAllBTODetails = FindAll().OrderByDescending(x => x.Id)
        //        .Where(inv => (string.IsNullOrWhiteSpace(searchParams.SearchValue))
        //        || (string.IsNullOrEmpty(searchParams.SearchValue)
        //        || inv.BTONumber.Contains(searchParams.SearchValue)
        //        || inv.PONumber.Contains(searchParams.SearchValue)
        //        || inv.CustomerName.Contains(searchParams.SearchValue)
        //        || inv.CustomerId.Equals(searchParams.SearchValue)
        //         ))
        //         .Include(t => t.bTODeliveryOrderItems);

        //    return PagedList<BTODeliveryOrder>.ToPagedList(getAllBTODetails, pagingParameter.PageNumber, pagingParameter.PageSize);

        //    //var getAllBTODetails = FindAll().OrderByDescending(x => x.Id)
        //    //    .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BTONumber.Contains(searchParams.SearchValue) ||
        //    //     inv.PONumber.Contains(searchParams.SearchValue) || inv.CustomerName.Contains(searchParams.SearchValue)
        //    //     || inv.SalesOrderId.Equals(int.Parse(searchParams.SearchValue)))))
        //    //     .Include(t => t.bTODeliveryOrderItems);

        //    //return PagedList<BTODeliveryOrder>.ToPagedList(getAllBTODetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}
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
                    .Where(x => x.BTONumber == BTONumber).Include(x=>x.bTODeliveryOrderItems)
                          .FirstOrDefaultAsync();
            return getBtoDetailsByBtoNo;
        }

        public async Task<BTODeliveryOrder> GetBTODeliveryOrderById(int id)
        {
            var getBTODeliveryOrderDetailsbyId = await _tipsWarehouseDbContext.bTODeliveryOrder.Where(x => x.Id == id)
                                .Include(t => t.bTODeliveryOrderItems).ThenInclude(x => x.QtyDistribution)
                                //.ThenInclude(s => s.BTOSerialNumbers)
                                .FirstOrDefaultAsync();


            return getBTODeliveryOrderDetailsbyId;
        }



        public async Task<string> UpdateBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder)
        {
            bTODeliveryOrder.LastModifiedBy = _createdBy;
            bTODeliveryOrder.LastModifiedOn = DateTime.Now;
            Update(bTODeliveryOrder);
            string result = $"BTODeliveryOrder of Detail {bTODeliveryOrder.Id} is updated successfully!";
            return result;
        }
        public async Task<string> UpdateBTODeliveryOrderFromReturnDO(BTODeliveryOrder bTODeliveryOrder)
        {
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

                                   IssuedTo = x.IssuedTo

                               })
                               .OrderByDescending(x => x.Id)
                             .ToListAsync();

            return btoIddNameList;
        }
        public async Task<SalesOrderNoandIdDto> GetAllSalesOrderNoAndIdByBTONo(string btoNumber)
        {
             var  btoIddNameList = await _tipsWarehouseDbContext.bTODeliveryOrder
                                .Where(x=>x.BTONumber == btoNumber)
                               .Select(x => new SalesOrderNoandIdDto()
                               {

                                   SalesOrderId = x.SalesOrderId,
                                   SalesOrderNumber = x.SalesOrderNumber

                               })
                             .FirstOrDefaultAsync();

            return btoIddNameList;
        }
        public async Task<BTODeliveryOrder> GetBTODeliveryOrderByIdExcludingClosed(int id)
        {
            var getBTODeliveryOrderDetailsbyId = await _tipsWarehouseDbContext.bTODeliveryOrder.Where(x => x.Id == id)
                                .Include(t => t.bTODeliveryOrderItems.Where(x => x.DoStatus != Status.Closed)).ThenInclude(x => x.QtyDistribution)
                                //.ThenInclude(s => s.BTOSerialNumbers)
                                .FirstOrDefaultAsync();


            return getBTODeliveryOrderDetailsbyId;
        }
        public async Task<IEnumerable<ListOfBtoNumberDetails>> GetBtoNumberListByCustomerIdExcludingClosed(string customerLeadId)
        {

            IEnumerable<ListOfBtoNumberDetails> getBtoNumberList = await _tipsWarehouseDbContext.bTODeliveryOrder.Where(x=>x.DoStatus!=Status.Closed && x.CustomerId == customerLeadId)
                                .Select(x => new ListOfBtoNumberDetails()
                                {
                                    CustomerLeadID = x.CustomerId,
                                    BTONumber = x.BTONumber,
                                    BtoDeliveryOrderId = x.Id,
                                    OrderType = x.OrderType,
                                    TotalValue = x.TotalValue

                                })
                                //.Where(x => x.CustomerLeadID == customerLeadId)
                              .ToListAsync();

            return getBtoNumberList;
        }
    }


    public class BTODeliveryOrderItemRepository : RepositoryBase<BTODeliveryOrderItems>, IBTODeliveryOrderItemsRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContexts;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public BTODeliveryOrderItemRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsWarehouseDbContexts = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<List<BTODeliveryOrderItems>> GetOpenDoItemDetailsByItemNoAndDoNo(string itemNumber, string BtoDeliveryNumber)
        {
            var btoDeliveryOrderDetails = await _tipsWarehouseDbContexts.bTODeliveryOrderItems
                    .Where(x => x.FGItemNumber == itemNumber && x.BTONumber == BtoDeliveryNumber && x.DoStatus != Status.Closed)
                          .ToListAsync();

            return btoDeliveryOrderDetails;
        }
        public async Task<int?> GetBTODeliveryOrderItemsPartiallyClosedAndOpenStatusCount(string btoNumber)
        {
            var bTODeliveryOrderItemsPartiallyClosedAndOpenStatusCount = _tipsWarehouseDbContext.bTODeliveryOrderItems.Where(x => x.BTONumber == btoNumber
                                                            &&(x.DoStatus == Status.PartiallyClosed || x.DoStatus == Status.Open)).Count();

            return bTODeliveryOrderItemsPartiallyClosedAndOpenStatusCount;
        }
        public async Task UpdateBtoDelieveryOrderItem(BTODeliveryOrderItems btoDeliveryOrderItem)
        {
            btoDeliveryOrderItem.LastModifiedBy = _createdBy;
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
                    .Include(x => x.QtyDistribution)
                    .FirstOrDefaultAsync();
            return getBTODeliveryOrderItemDetails;
        }


    }

    public class BTODeliveryOrderHistoryRepository : RepositoryBase<BTODeliveryOrderHistory>, IBTODeliveryOrderHistoryRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public BTODeliveryOrderHistoryRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }


        public async Task<long> CreateBTODeliveryOrderHistory(BTODeliveryOrderHistory bTODeliveryOrderHistory)
        {
            bTODeliveryOrderHistory.CreatedBy = _createdBy;
            bTODeliveryOrderHistory.CreatedOn = DateTime.Now;
            bTODeliveryOrderHistory.Unit = _unitname;
            var result = await Create(bTODeliveryOrderHistory);
            return result.Id;
        }

        public async Task<PagedList<BTODeliveryOrderHistory>> GetAllReturnDeliveryOrder([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParams)
        {
            var getAllBTODetails = _tipsWarehouseDbContext.BTODeliveryOrderHistories.OrderByDescending(x => x.Id)
                    .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue))
                    || (string.IsNullOrEmpty(searchParams.SearchValue)
                    || inv.BTONumber.Contains(searchParams.SearchValue)
                    || inv.PONumber.Contains(searchParams.SearchValue)
                    || inv.CustomerId.Contains(searchParams.SearchValue)
                    || inv.CustomerName.Contains(searchParams.SearchValue))) && (inv.UniqeId != null));


            return PagedList<BTODeliveryOrderHistory>.ToPagedList(getAllBTODetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<BTODeliveryOrderHistory>> GetAllBtoHistoryDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParams)
        {
            var getAllBTODetails = FindAll().OrderByDescending(x => x.Id)
                    .Where(inv => (string.IsNullOrWhiteSpace(searchParams.SearchValue))
                    || (string.IsNullOrEmpty(searchParams.SearchValue)
                    || inv.BTONumber.Contains(searchParams.SearchValue)
                    || inv.PONumber.Contains(searchParams.SearchValue)
                    || inv.CustomerName.Contains(searchParams.SearchValue)

                     ));


            return PagedList<BTODeliveryOrderHistory>.ToPagedList(getAllBTODetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        //public async Task<PagedList<BTODeliveryOrderHistory>> GetAllBtoHistoryDetails(PagingParameter pagingParameter)
        //{
        //    var bto = await _tipsWarehouseDbContext.ReturnBtoDeliveryOrders
        //      .FirstOrDefaultAsync();

        //    var getAllBTODetails = PagedList<BTODeliveryOrderHistory>.ToPagedList(FindAll()
        //            .Where(x => x.UniqeId != null)
        //            .OrderByDescending(x => x.Id), pagingParameter.PageNumber, pagingParameter.PageSize);



        //    return getAllBTODetails;
        //}

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
        public async Task<string> GetBTONumberCount(string btoNumber)
        {
            var btoHistoryNumber = await _tipsWarehouseDbContext.BTODeliveryOrderHistories
                .Where(x => x.BTONumber.StartsWith(btoNumber) && x.UniqeId != null).OrderByDescending(x => x.Id)
                .Select(x => x.BTONumber).FirstOrDefaultAsync();

            return btoHistoryNumber;
        }
    }
}


