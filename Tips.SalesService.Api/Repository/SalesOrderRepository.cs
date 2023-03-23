using System.Linq;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;

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

        public async Task<IEnumerable<SalesOrder>> GetAllActiveSalesOrder()
        {
            var getAllActiveSalesOrder = await FindAll().OrderByDescending(x => x.Id).ToListAsync();
            return getAllActiveSalesOrder;
        }

        public async Task<PagedList<SalesOrder>> GetAllSalesOrder(PagingParameter pagingParameter)
        {

            var getAllSalesOrders = PagedList<SalesOrder>.ToPagedList(FindAll()
                                .Include(t => t.SalesOrdersItems)
               .OrderByDescending(x => x.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllSalesOrders;
        }

        public async Task<IEnumerable<SalesOrder>> SearchSalesOrderItem([FromQuery] SearchParammes searchParams)
        {
            var salesOrderDetails = _tipsSalesServiceDbContext.SalesOrders
                             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue)
                                || inv.SalesOrderNumber.Contains(searchParams.SearchValue)
                                //|| inv.OrderDate.Equals(DateTime.Parse(searchParams.SearchValue))
                                || inv.ProjectNumber.Contains(searchParams.SearchValue)
                                //|| inv.OrderType.Contains(searchParams.SearchValue)
                                || inv.CustomerName.Contains(searchParams.SearchValue)
                                //|| inv.CustomerId.Contains(searchParams.SearchValue)
                                //|| inv.ReceivedDate.Equals(DateTime.Parse(searchParams.SearchValue))
                                || inv.PONumber.Contains(searchParams.SearchValue)
                                //|| inv.PODate.Equals(DateTime.Parse(searchParams.SearchValue))
                                //|| inv.RevisionNumber.Equals(int.Parse(searchParams.SearchValue))
                                
                                )))

                                .Include(itm => itm.SalesOrdersItems).ToList();
                                
            

            var salesOrderItemsDetails = _tipsSalesServiceDbContext.SalesOrders
                                 .Include(x => x.SalesOrdersItems                          
                                .Where(i => ((string.IsNullOrWhiteSpace(searchParams.SearchValue)
                                || i.ItemNumber.Contains(searchParams.SearchValue)
                                || i.SalesOrderNumber.Contains(searchParams.SearchValue)
                                || i.Description.Contains(searchParams.SearchValue)
                                || i.UOM.Contains(searchParams.SearchValue)
                                || i.Currency.Contains(searchParams.SearchValue))))).ToList();

            var salesOrderUnionList = salesOrderDetails.Union(salesOrderItemsDetails).ToList();


            return salesOrderUnionList;
        }


        public async Task<PagedList<SalesOrder>> GetAllSalesOrderWithItems(PagingParameter pagingParameter, List<string> salesOrderNumber, List<string> projectNumber , List<string> customerName)
        {
            var salesOrderDetailList = FindAll()
            .Where(inv => (inv.SalesOrderNumber.Equals(salesOrderNumber) != null ?
            inv.SalesOrderNumber.Equals(salesOrderNumber) : inv.SalesOrderNumber == inv.SalesOrderNumber)
            && (inv.ProjectNumber.Equals(projectNumber) != null ?
            inv.ProjectNumber.Equals(projectNumber) : inv.ProjectNumber == inv.ProjectNumber)
            && (inv.CustomerName.Equals(customerName) != null ?
            inv.CustomerName.Equals(customerName) : inv.CustomerName == inv.CustomerName));
            
            
            return PagedList<SalesOrder>.ToPagedList(salesOrderDetailList, pagingParameter.PageNumber, pagingParameter.PageSize);   
        }


        public async Task<SalesOrder> GetSalesOrderById(int id)
        {
            var getSalesOrderbyId = await _tipsSalesServiceDbContext.SalesOrders.Where(x => x.Id == id)
                                  .Include(t => t.SalesOrdersItems)
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
            OrderStatus[] status = { OrderStatus.Open, OrderStatus.PartiallyClosed };

            IEnumerable<ListOfProjectNoDto> getProjectNumberList = await _tipsSalesServiceDbContexts.SalesOrdersItems
                                 .Where(b => b.ItemNumber == itemNo && status.Contains(b.StatusEnum))
                                 .Select(x => new ListOfProjectNoDto()
                                 {
                                     Id = x.Id,
                                     ProjectNumber = x.ProjectNumber

                                 })
                               .ToListAsync();

            return getProjectNumberList;



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

            IEnumerable<GetSalesOrderDetailsDto> getSalesorderList = await _tipsSalesServiceDbContexts.SalesOrdersItems
                                .Where(b => b.ItemNumber == ItemNo && b.ProjectNumber == ProjectNo)
                                .Select(x => new GetSalesOrderDetailsDto()
                                {
                                    Id = x.Id,
                                    SalesOrderNumber = x.SalesOrderNumber,
                                    OrderQty = x.OrderQty
                                })
                              .ToListAsync();

            return getSalesorderList;
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


