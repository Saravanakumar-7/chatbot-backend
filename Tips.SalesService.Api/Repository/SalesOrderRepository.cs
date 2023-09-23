using AutoMapper.Internal;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Mysqlx.Crud;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using System.Collections.Generic;
using System.Linq;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Entities.Enum;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            var date = DateTime.Now;
            salesOrder.CreatedBy = "Admin";
            salesOrder.CreatedOn = date.Date;
            salesOrder.Unit = "Banglore";
            var version = 1;
            salesOrder.RevisionNumber = (version);
            var result = await Create(salesOrder);
            return result.Id;
        }

        public async Task<int?> GetSONumberAutoIncrementCount(DateTime date)
        {
            var getSONumberAutoIncrementCount = _tipsSalesServiceDbContext.SalesOrders.Where(x => x.CreatedOn == date.Date).Count();

            return getSONumberAutoIncrementCount;
        }
        public async Task<string> DeleteSalesOrder(SalesOrder salesOrder)
        {
            Delete(salesOrder);
            string result = $"SalesOrder details of {salesOrder.Id} is deleted successfully!";
            return result;
        }
        //public async Task<IEnumerable<SalesOrder>> GetAllSalesOrderWithItems(SalesOrderSearchDto salesOrderSearch)
        // {
        //     using (var context = _tipsSalesServiceDbContext)
        //     {
        //         var query = _tipsSalesServiceDbContext.SalesOrders.Include("SalesOrdersItems");

        //         if (salesOrderSearch != null &&
        //             (salesOrderSearch.SalesOrderNumber.Any() ||
        //              salesOrderSearch.ProjectNumber.Any() ||
        //              salesOrderSearch.CustomerName.Any() ||
        //              salesOrderSearch.SOStatus.Any()))
        //         {
        //             query = query.Where(so =>
        //                 (salesOrderSearch.SalesOrderNumber.Any() ? salesOrderSearch.SalesOrderNumber.Contains(so.SalesOrderNumber) : true)
        //                 && (salesOrderSearch.ProjectNumber.Any() ? salesOrderSearch.ProjectNumber.Contains(so.ProjectNumber) : true)
        //                 && (salesOrderSearch.CustomerName.Any() ? salesOrderSearch.CustomerName.Contains(so.CustomerName) : true)
        //                 && (salesOrderSearch.SOStatus.Any() ? salesOrderSearch.SOStatus.Contains(so.SOStatus.ToString()) : true))
        //                 .Include(itm => itm.SalesOrdersItems)
        //                 .ThenInclude(p => p.ScheduleDates)
        //                 .Include(p => p.SalesOrderAdditionalCharges);
        //         }

        //         return query.ToList();
        //     }

        // }
        public async Task<IEnumerable<SalesOrder>> GetAllSalesOrderWithItems(SalesOrderSearchDto salesOrderSearch)
        {
            using (var context = _tipsSalesServiceDbContext)
            {
                var query = _tipsSalesServiceDbContext.SalesOrders.Include("SalesOrdersItems");
                if (salesOrderSearch != null || (salesOrderSearch.SalesOrderNumber.Any())
                    && salesOrderSearch.ProjectNumber.Any() && salesOrderSearch.CustomerName.Any() && salesOrderSearch.SOStatus.Any())

                {
                    query = query.Where
                        (so => (salesOrderSearch.SalesOrderNumber.Any() ? salesOrderSearch.SalesOrderNumber.Contains(so.SalesOrderNumber) : true)
                        && (salesOrderSearch.ProjectNumber.Any() ? salesOrderSearch.ProjectNumber.Contains(so.ProjectNumber) : true)
                        && (salesOrderSearch.CustomerName.Any() ? salesOrderSearch.CustomerName.Contains(so.CustomerName) : true)
                        && (salesOrderSearch.SOStatus.Any() ? salesOrderSearch.SOStatus.Contains(so.SOStatus) : true)
                        )
                        .Include(itm => itm.SalesOrdersItems)
                        .ThenInclude(p => p.ScheduleDates)
                                .Include(p => p.SalesOrderAdditionalCharges);
                }
                return query.ToList();
            }

        }
        public async Task<PagedList<SalesOrder>> GetAllActiveSalesOrder([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var getAllActiveSalesOrder = FindAll()
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue)
                     || inv.SalesOrderNumber.Contains(searchParammes.SearchValue)
                     || inv.ProjectNumber.Contains(searchParammes.SearchValue)
                     || inv.OrderType.Contains(searchParammes.SearchValue)
                     || inv.CustomerName.Contains(searchParammes.SearchValue)
                     || inv.OrderDate.Equals(int.Parse(searchParammes.SearchValue))
                     || inv.ReceivedDate.Equals(int.Parse(searchParammes.SearchValue))
                     || inv.PODate.Equals(int.Parse(searchParammes.SearchValue))
                     || inv.RevisionNumber.Equals(int.Parse(searchParammes.SearchValue))
                     || inv.CustomerId.Equals(int.Parse(searchParammes.SearchValue)))))
                   .Include(t => t.SalesOrdersItems)
                   .ThenInclude(p => p.ScheduleDates)
                                .Include(p => p.SalesOrderAdditionalCharges);

            return PagedList<SalesOrder>.ToPagedList(getAllActiveSalesOrder, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<SalesOrder>> GetAllSalesOrder([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var salesOrderDetails = FindAll().OrderByDescending(x => x.Id)
               .Where(inv => (string.IsNullOrWhiteSpace(searchParammes.SearchValue))
                              // Add this condition to filter by SalesOrderNumber
                              || (string.IsNullOrEmpty(searchParammes.SearchValue))
                              || inv.SalesOrderNumber.Contains(searchParammes.SearchValue)
                              || inv.ProjectNumber.Contains(searchParammes.SearchValue)
                              || inv.CustomerName.Contains(searchParammes.SearchValue)
                              || inv.CustomerId.Contains(searchParammes.SearchValue)
                            )
               .Include(t => t.SalesOrdersItems)
               .ThenInclude(p => p.ScheduleDates)
               .Include(p => p.SalesOrderAdditionalCharges);

            return PagedList<SalesOrder>.ToPagedList(salesOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

            //int searchValueInt;
            //bool isSearchValueInt = int.TryParse(searchParammes.SearchValue, out searchValueInt);

            //var salesOrderDetails = FindAll().OrderByDescending(x => x.Id)
            //    .Where(inv => (string.IsNullOrWhiteSpace(searchParammes.SearchValue)
            //                   || inv.SalesOrderNumber.Contains(searchParammes.SearchValue)
            //                   || inv.ProjectNumber.Contains(searchParammes.SearchValue)
            //                   || inv.OrderType.Contains(searchParammes.SearchValue)
            //                   || inv.CustomerName.Contains(searchParammes.SearchValue)
            //                   || inv.OrderDate.ToString().Contains(searchParammes.SearchValue)
            //                   || inv.ReceivedDate.ToString().Contains(searchParammes.SearchValue)
            //                   || inv.PODate.ToString().Contains(searchParammes.SearchValue)
            //                   || (!isSearchValueInt || inv.RevisionNumber == searchValueInt)
            //                   || inv.CustomerId.Contains(searchParammes.SearchValue))
            //                   // Add this condition to filter by SalesOrderNumber
            //                   && (string.IsNullOrEmpty(searchParammes.SearchValue) || inv.SalesOrderNumber == searchParammes.SearchValue))
            //    .Include(t => t.SalesOrdersItems)
            //    .ThenInclude(p => p.ScheduleDates)
            //    .Include(p => p.SalesOrderAdditionalCharges);

            //return PagedList<SalesOrder>.ToPagedList(salesOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

        }
        public async Task<IEnumerable<SalesOrder>> SearchSalesOrderDate([FromQuery] SearchDateParam searchDateParam)
        {
            var salesOrderDetails = _tipsSalesServiceDbContext.SalesOrders
                             .Where(inv => ((inv.CreatedOn >= searchDateParam.SearchFromDate &&
                                inv.CreatedOn <= searchDateParam.SearchToDate
                                )))
                             .Include(itm => itm.SalesOrdersItems)
                              .ThenInclude(p => p.ScheduleDates)
                                .Include(p => p.SalesOrderAdditionalCharges)
                             .ToList();
            return salesOrderDetails;
        }

        public async Task<IEnumerable<SalesOrder>> SearchSalesOrder([FromQuery] SearchParammes searchParams)
        {
            using (var context = _tipsSalesServiceDbContext)
            {
                var query = _tipsSalesServiceDbContext.SalesOrders.Include("SalesOrdersItems");
                if (!string.IsNullOrEmpty(searchParams.SearchValue))
                {
                    query = query.Where(so => so.SalesOrderNumber.Contains(searchParams.SearchValue)
                || so.CustomerName.Contains(searchParams.SearchValue) ||
                so.OrderDate.ToString().Contains(searchParams.SearchValue) ||
                so.SalesOrdersItems.Any(s => s.ItemNumber.Contains(searchParams.SearchValue) ||
                s.Description.Contains(searchParams.SearchValue)))
                        .Include(itm => itm.SalesOrdersItems)
                        .ThenInclude(p => p.ScheduleDates)
                                .Include(p => p.SalesOrderAdditionalCharges);
                }
                return query.ToList();
            }

        }

        public async Task<IEnumerable<SalesOrderIdNameListDto>> GetAllActiveSalesOrderNameList()
        {
            IEnumerable<SalesOrderIdNameListDto> activeSalesOrderNameList = await _tipsSalesServiceDbContext.SalesOrders
                                .Select(x => new SalesOrderIdNameListDto()
                                {
                                    Id = x.Id,
                                    SalesOrderNumber = x.SalesOrderNumber,
                                })
                              .ToListAsync();

            return activeSalesOrderNameList;
        }
        public async Task<string> GenerateSONumber()
        {
            using var transaction = await _tipsSalesServiceDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var poNumberEntity = await _tipsSalesServiceDbContext.SONumbers.SingleAsync();
                poNumberEntity.CurrentValue += 1;
                _tipsSalesServiceDbContext.Update(poNumberEntity);
                await _tipsSalesServiceDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"SO-{poNumberEntity.CurrentValue:D6}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<IEnumerable<SalesOrderIdNameListDto>> GetAllSalesOrderIdNameList()
        {
            IEnumerable<SalesOrderIdNameListDto> activeSalesOrderNameList = await _tipsSalesServiceDbContext.SalesOrders
                                .Select(x => new SalesOrderIdNameListDto()
                                {
                                    Id = x.Id,
                                    SalesOrderNumber = x.SalesOrderNumber,
                                    PONumber = x.PONumber,
                                })
                              .ToListAsync();

            return activeSalesOrderNameList;
        } 
        public async Task<SalesOrder> GetSalesOrderById(int id)
        {
            var getSalesOrderbyId = await _tipsSalesServiceDbContext.SalesOrders.Where(x => x.Id == id)
                                  .Include(t => t.SalesOrdersItems)
                                  .ThenInclude(p => p.ScheduleDates)
                                   .Include(t => t.SalesOrdersItems)
                                  .ThenInclude(p => p.SoConfirmationDates)
                                    .Include(o => o.SalesOrderAdditionalCharges)

                                 .FirstOrDefaultAsync();

            return getSalesOrderbyId;
        }
        public async Task<SalesOrder> GetSalesOrderDetailsBySONumber(string salesOrderNumber)
        {
            var salesOrderDetails = await _tipsSalesServiceDbContext.SalesOrders
                .Where(x => x.SalesOrderNumber == salesOrderNumber && x.IsShortClosed == false)
                .Include(o => o.SalesOrdersItems)
                .ThenInclude(x => x.ScheduleDates)
                .Include(t => t.SalesOrderAdditionalCharges)
                                
                                .FirstOrDefaultAsync();

            return salesOrderDetails;
        }
        public async Task<IEnumerable<ListofSalesOrderDetails>> GetSalesOrderDetailsByCustomerId(string Customerid)
        {

            IEnumerable<ListofSalesOrderDetails> getSalesorderList = await _tipsSalesServiceDbContext.SalesOrders
                                .Where(b => b.CustomerId == Customerid)
                                .Select(x => new ListofSalesOrderDetails()
                                {
                                    SalesOrderId = x.Id,
                                    SalesOrderNumber = x.SalesOrderNumber,
                                    PONumber = x.PONumber,
                                })
                              .ToListAsync();

            return getSalesorderList;
        }

        public async Task<object> GetSalesOrderTotalBySalesOrderId(int salesOrderId)
        {

            var salesOrderTotal = await _tipsSalesServiceDbContext.SalesOrders
                                .Where(b => b.Id == salesOrderId)
                                .Select(x => x.Total)
                              .FirstOrDefaultAsync();

            return salesOrderTotal;
        }

        public async Task<string> UpdateSalesOrder(SalesOrder salesOrder)
        {
            salesOrder.LastModifiedBy = "Admin";
            salesOrder.LastModifiedOn = DateTime.Now;
            var oldRevisionNumber = _tipsSalesServiceDbContext.SalesOrders
               .Where(x => x.SalesOrderNumber == salesOrder.SalesOrderNumber)
               .OrderByDescending(x => x.Id)
               .Select(x => x.RevisionNumber)
               .FirstOrDefault();

            var increaseVersionNumber = 1;
            var version = oldRevisionNumber + increaseVersionNumber;
            salesOrder.RevisionNumber = (version);
            Update(salesOrder);
            string result = $"SalesOrder of Detail {salesOrder.Id} is updated successfully!";
            return result;
        }

        public async Task<List<ProjectSODetailDto>> GetProjectDetailsByItemNo(string itemNumber)
        {
            var projectNumbers = await _tipsSalesServiceDbContext.SalesOrdersItems
                                .Where(x => x.ItemNumber == itemNumber)
                                .Select(m => m.ProjectNumber).Distinct().ToListAsync();


            var projectSODetails = await _tipsSalesServiceDbContext.SalesOrders
                                .Where(m => projectNumbers.Contains(m.ProjectNumber)
                                && m.SOStatus != OrderStatus.Closed && m.IsShortClosed == false)
                                .Select(s => new ProjectSODetailDto()
                                {
                                    ProjectNumber = s.ProjectNumber,
                                    CustomerName = s.CustomerName,
                                    CustomerId = s.CustomerId
                                }).Distinct().ToListAsync();
            return projectSODetails;
        }

        //    public async Task<List<SalesOrderQtyDto>> GetSalesOrderQtyDetailsByItemNo(string itemNumber ,string projectNo)
        //    {
        //        List<SalesOrderQtyDto> salesOrderQtyDtos = new List<SalesOrderQtyDto>();

        //        OrderStatus[] status = { OrderStatus.Open, OrderStatus.PartiallyClosed };

        //        var SalesOrderNoList = await _tipsSalesServiceDbContext.SalesOrdersItems
        //.Where(x => x.ItemNumber == itemNumber && x.ProjectNumber == projectNo && status.Contains(x.StatusEnum))
        //.Select(x => x.SalesOrderNumber).Distinct().ToListAsync();

        //        foreach (var salesOrderNo in SalesOrderNoList)
        //        {

        //            decimal totalOrderQty = await _tipsSalesServiceDbContext.SalesOrdersItems
        //    .Where(x => x.ItemNumber == itemNumber && x.ProjectNumber == projectNo && x.SalesOrderNumber == salesOrderNo 
        //    && status.Contains(x.StatusEnum)
        //    )
        //    .SumAsync(m => m.OrderQty);

        //            decimal totalBalanceQty = await _tipsSalesServiceDbContext.SalesOrdersItems
        //                .Where(x => x.ItemNumber == itemNumber && x.ProjectNumber == projectNo
        //                && x.SalesOrderNumber == salesOrderNo
        //                && status.Contains(x.StatusEnum)
        //                )
        //                .SumAsync(m => m.BalanceQty);

        //            decimal OpenShopOrderSumQty = await _tipsSalesServiceDbContext.SalesOrdersItems
        //                .Where(x => x.ItemNumber == itemNumber && x.ProjectNumber == projectNo
        //                && x.SalesOrderNumber == salesOrderNo
        //                && status.Contains(x.StatusEnum)
        //                )
        //                .SumAsync(m => m.ShopOrderQty);

        //            decimal OpenSalesOrderQty = totalOrderQty - OpenShopOrderSumQty;

        //            var salesOrderQtyDetail = await _tipsSalesServiceDbContext.SalesOrdersItems
        //                .Where(x => x.ItemNumber == itemNumber && x.ProjectNumber == projectNo && status.Contains(x.StatusEnum)
        //                && x.SalesOrderNumber == salesOrderNo)
        //                .Select(m => new SalesOrderQtyDto()
        //                {
        //                    SalesOrderNo = m.SalesOrderNumber,
        //                    SalesOrderQty = totalOrderQty,
        //                    OpenSalesOrderQty = OpenSalesOrderQty // Set the sum of BalanceQty

        //                }).FirstOrDefaultAsync();
        //            //        decimal totalOrderQty = await _tipsSalesServiceDbContext.SalesOrdersItems
        //            //.Where(x => x.ItemNumber == itemNumber && x.ProjectNumber == projectNo)
        //            //.SumAsync(m => m.OrderQty);

        //            //        var salesOrderQtyDetails = await _tipsSalesServiceDbContext.SalesOrdersItems
        //            //                           .Where(x => x.ItemNumber == itemNumber && x.ProjectNumber == projectNo)
        //            //                           .Select(m => new SalesOrderQtyDto()
        //            //                           {
        //            //                               SalesOrderNo = m.SalesOrderNumber,
        //            //                               SalesOrderQty = m.OrderQty, //we have to change sum of ordered qty
        //            //                               OpenSalesOrderQty = m.BalanceQty //we have to change sum of balanced qty

        //            //                           }).ToListAsync();
        //            salesOrderQtyDtos.Add(salesOrderQtyDetail);
        //        }

        //                 return salesOrderQtyDtos;
        //    }


        public async Task<List<SalesOrderQtyDto>> GetSalesOrderQtyDetailsByItemNo(string itemNumber, string projectNo)
        {
            OrderStatus[] status = { OrderStatus.Open, OrderStatus.PartiallyClosed };
            List<SalesOrderQtyDto> salesOrderQtyDtos = await _tipsSalesServiceDbContext.SalesOrdersItems
              .Where(x => x.ItemNumber == itemNumber
                         && x.ProjectNumber == projectNo
                         && status.Contains(x.StatusEnum))
              .GroupBy(x => x.SalesOrderNumber)
              .Select(g => new SalesOrderQtyDto
              {
                  SalesOrderNo = g.Key,
                  SalesOrderQty = g.Sum(x => x.OrderQty),
                  OpenSalesOrderQty = g.Sum(x => x.OrderQty) - g.Sum(x => x.ShopOrderQty)
              })
              .ToListAsync();

            return salesOrderQtyDtos;
        }

        public async Task<decimal> GetOpenSalesOrderQuantityByItemNumber(string itemNumber)
        {
            return await _tipsSalesServiceDbContext.SalesOrdersItems
            .Where(soi => soi.ItemNumber == itemNumber && soi.StatusEnum != OrderStatus.Closed
                && soi.SalesOrder.IsShortClosed ==false)
            .SumAsync(soi => soi.BalanceQty);
        }
    }
    public class SalesOrderItemRepository : RepositoryBase<SalesOrderItems>, ISalesOrderItemsRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContexts;
        public SalesOrderItemRepository(TipsSalesServiceDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsSalesServiceDbContexts = repositoryContext;
        }
        public async Task<SalesOrderItems> GetSalesOrderItemDetailsById(int soItemId)
        {
            var salesOrderItemDetails = await _tipsSalesServiceDbContexts.SalesOrdersItems
                 .Where(x => x.Id == soItemId)
                          .FirstOrDefaultAsync();

            return salesOrderItemDetails;
        }
        public async Task<IEnumerable<ListOfProjectNoDto>> GetprojectNoByItemNo(string itemNo)
        {
            //OrderStatus[] status = { OrderStatus.Open, OrderStatus.PartiallyClosed };

            var salesOrderNo = await _tipsSalesServiceDbContexts.SalesOrders
                               .Where(x => x.SalesOrderStatus == SalesOrderStatus.BuildToPrint)
                               .Select(x => x.SalesOrderNumber).Distinct().ToListAsync();

            IEnumerable<ListOfProjectNoDto> salesOrderDetails = await _tipsSalesServiceDbContexts.SalesOrdersItems
                              .Where(m => salesOrderNo.Contains(m.SalesOrderNumber) && m.ItemNumber == itemNo)
                               .Select(x => new ListOfProjectNoDto()
                               {
                                   Id = x.Id,
                                   ProjectNumber = x.ProjectNumber

                               })
                               .Distinct().ToListAsync();

            return salesOrderDetails;

            //IEnumerable<ListOfProjectNoDto> getProjectNumberList = await _tipsSalesServiceDbContexts.SalesOrdersItems
            //                     .Where(b => b.ItemNumber == itemNo && status.Contains(b.StatusEnum))
            //                     .Select(x => new ListOfProjectNoDto()
            //                     {
            //                         Id = x.Id,
            //                         ProjectNumber = x.ProjectNumber

            //                     })
            //                   .ToListAsync();

            //return getProjectNumberList;

        }
        //serach by item level
        public async Task<IEnumerable<SalesOrderItems>> SearchSalesOrderItem([FromQuery] SearchParammes searchParams)
        {
            var getSalesOrderItemDetails = await _tipsSalesServiceDbContexts.SalesOrdersItems
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue)
                     || inv.ItemNumber.Contains(searchParams.SearchValue)
                     || inv.ProjectNumber.Contains(searchParams.SearchValue)
                    || inv.SalesOrderNumber.Contains(searchParams.SearchValue)
                    || inv.UOM.Contains(searchParams.SearchValue)
                    || inv.Currency.Contains(searchParams.SearchValue)
                     )))
                      .ToListAsync();
            return getSalesOrderItemDetails;
        } 

        public async Task<List<SalesOrderFGandBalanceQty>> GetAllSalesOrderFGOrTGItemDetails()
        {  
                var salesOrderIdList = await _tipsSalesServiceDbContexts.SalesOrders
                    .Where(so => (so.SalesOrderStatus == SalesOrderStatus.BuildToPrint || so.SalesOrderStatus == SalesOrderStatus.Forecast) &&
                                 (so.SOStatus == OrderStatus.Open || so.SOStatus == OrderStatus.PartiallyClosed) &&
                                 so.IsShortClosed == false &&
                                 so.ConfirmStatus == true && so.ApproveStatus == true).Select(x => x.Id).ToListAsync();

                var openSalesOrderQty = await _tipsSalesServiceDbContexts.SalesOrdersItems
                    .Where(x =>
                                (x.StatusEnum == OrderStatus.Open || x.StatusEnum == OrderStatus.PartiallyClosed) &&
                                x.BalanceQty > 0 && salesOrderIdList.Contains(x.SalesOrderId))
                    .GroupBy(x => new { x.ItemNumber })
                    .Select(group => new SalesOrderFGandBalanceQty
                    {
                        FGItemNumber = group.Key.ItemNumber,
                        Balance_Qty = group.Sum(x => x.BalanceQty)
                    }).ToListAsync();

                return openSalesOrderQty;          

        }
        //update shoporderQty
        public async Task<IEnumerable<SalesOrderItems>> UpdateShopOrderBySalesOrderNoandItemNo(string salesOrderNumber, string itemNumber,string projectNumber)
        {
            OrderStatus[] status = { OrderStatus.Open, OrderStatus.PartiallyClosed };

            var updateShopOrderQty = await _tipsSalesServiceDbContexts.SalesOrdersItems
                 .Where(x => x.ItemNumber == itemNumber && x.ProjectNumber == projectNumber && x.SalesOrderNumber == salesOrderNumber
                 & status.Contains(x.StatusEnum))
                          .ToListAsync();

            return updateShopOrderQty;
        }
        public async Task<IEnumerable<SalesOrderItems>> GetSalesOrderItemDetailsByIdandItemNo(string ItemNumber, int SalesOrderId)
        {
            OrderStatus[] status = { OrderStatus.Open, OrderStatus.PartiallyClosed };

            var getSalesOrderDetailsBySOandItemNo = await _tipsSalesServiceDbContexts.SalesOrdersItems
                 .Where(x => x.ItemNumber == ItemNumber && x.SalesOrderId == SalesOrderId &&
                  status.Contains(x.StatusEnum))
                          .ToListAsync();

            return getSalesOrderDetailsBySOandItemNo;
        }

        public async Task<IEnumerable<GetSalesOrderDetailsDto>> getSalesOrderDetailByProjectNoandItemNo(string ItemNo, string ProjectNo)
        {
            var join = from e in _tipsSalesServiceDbContexts.SalesOrdersItems
                       where e.ItemNumber == ItemNo && e.ProjectNumber == ProjectNo
                       join d in _tipsSalesServiceDbContexts.SalesOrders on e.SalesOrderNumber equals d.SalesOrderNumber
                       where d.SalesOrderStatus == SalesOrderStatus.BuildToPrint
                       select new GetSalesOrderDetailsDto()
                       {
                           Id = e.Id,
                           SalesOrderNumber = e.SalesOrderNumber,
                           OrderQty = e.OrderQty

                       };
            var postdata = join.ToList();

            return postdata;
            //IEnumerable<GetSalesOrderDetailsDto> getSalesorderList = await _tipsSalesServiceDbContexts.SalesOrdersItems
            //                    .Where(b => b.ItemNumber == ItemNo && b.ProjectNumber == ProjectNo)
            //                    .Select(x => new GetSalesOrderDetailsDto()
            //                    {
            //                        Id = x.Id,
            //                        SalesOrderNumber = x.SalesOrderNumber,
            //                        OrderQty = x.OrderQty
            //                    })
            //                  .ToListAsync();

            //return getSalesorderList;
        }

        public async Task<string> UpdateSalesOrderItem(SalesOrderItems salesOrderItems)
        {
            Update(salesOrderItems);

            string result = $"SalesOrderItem of Detail {salesOrderItems.Id} is updated successfully!";
            return result;
        }
        public async Task<SalesOrderItems> GetSOItemDetailById(int soItemId)
        {
            var soItemDetailBySOItemId = await _tipsSalesServiceDbContexts.SalesOrdersItems.Where(x => x.Id == soItemId)

                                .FirstOrDefaultAsync();

            return soItemDetailBySOItemId;
        }
        public async Task<int?> GetSOItemOpenStatusCount(int soId)
        {
            var soItemStatusCount = _tipsSalesServiceDbContexts.SalesOrdersItems
                                        .Where(x => x.SalesOrderId == soId && (x.StatusEnum == OrderStatus.Open || x.StatusEnum == OrderStatus.PartiallyClosed)).Count();

            return soItemStatusCount;
        }
    }


    public class SalesOrderHistoryRepository : RepositoryBase<SalesOrderHistory>, ISalesOrderHistoryRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContexts;
        public SalesOrderHistoryRepository(TipsSalesServiceDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsSalesServiceDbContexts = repositoryContext;
        }

        public async Task<SalesOrderHistory> CreateSalesOrderHistory(SalesOrderHistory salesOrderHistory)
        {
            salesOrderHistory.Unit = "Banglore";
            var result = await Create(salesOrderHistory);
            return result;
        }
    }
}


