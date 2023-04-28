using AutoMapper.Internal;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Mysqlx.Crud;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
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
                        && (salesOrderSearch.SOStatus.Any() ? salesOrderSearch.SOStatus.Contains(so.SOStatus) : true));
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
                   .Include(t => t.SalesOrdersItems);
            return PagedList<SalesOrder>.ToPagedList(getAllActiveSalesOrder, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<SalesOrder>> GetAllSalesOrder([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var salesOrderDetails = FindAll().OrderByDescending(x => x.Id)
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
                   .ThenInclude(p => p.ScheduleDates);


            return PagedList<SalesOrder>.ToPagedList(salesOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

        }
        public async Task<IEnumerable<SalesOrder>> SearchSalesOrderDate([FromQuery] SearchDateParam searchDateParam)
        {
            var salesOrderDetails = _tipsSalesServiceDbContext.SalesOrders
                             .Where(inv => ((inv.CreatedOn >= searchDateParam.SearchFromDate &&
                                inv.CreatedOn<= searchDateParam.SearchToDate
                                )))
                             .Include(itm => itm.SalesOrdersItems)
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
                s.Description.Contains(searchParams.SearchValue)));
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
                                 .FirstOrDefaultAsync();

            return getSalesOrderbyId;
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

        public async Task<List<SalesOrderQtyDto>> GetSalesOrderQtyDetailsByItemNo(string itemNumber,string projectNo)
        {
            var salesOrderQtyDetails = await _tipsSalesServiceDbContext.SalesOrdersItems
                               .Where(x => x.ItemNumber == itemNumber && x.ProjectNumber == projectNo)
                               .Select(m => new SalesOrderQtyDto()
                               {
                                   SalesOrderNo = m.SalesOrderNumber,
                                   SalesOrderQty = m.OrderQty, //we have to change sum of ordered qty
                                   OpenSalesOrderQty = m.BalanceQty //we have to change sum of balanced qty

                               }).ToListAsync();

            return salesOrderQtyDetails;
        }

    }
    public class SalesOrderItemRepository : RepositoryBase<SalesOrderItems>, ISalesOrderItemsRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContexts;
        public SalesOrderItemRepository(TipsSalesServiceDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsSalesServiceDbContexts = repositoryContext;
        }

        public async Task<IEnumerable<ListOfProjectNoDto>> GetprojectNoByItemNo(string itemNo)
        {
            //OrderStatus[] status = { OrderStatus.Open, OrderStatus.PartiallyClosed };

            var salesOrderNo = await _tipsSalesServiceDbContexts.SalesOrders
                               .Where(x => x.SalesOrderStatus == SalesOrderStatus.BuildToPrint)
                               .Select(x => x.SalesOrderNumber).Distinct().ToListAsync();

            IEnumerable<ListOfProjectNoDto> salesOrderDetails = await _tipsSalesServiceDbContexts.SalesOrdersItems
                              .Where(m => salesOrderNo.Contains(m.SalesOrderNumber)&& m.ItemNumber == itemNo)
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


        public async Task<IEnumerable<SalesOrderItems>> GetSalesOrderDetailsByIdandItemNo(string ItemNumber, int SalesOrderId)
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


