using AutoMapper.Internal;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Mysqlx.Crud;
using NPOI.SS.Formula.Functions;
using NuGet.Protocol.Core.Types;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public SalesOrderRepository(TipsSalesServiceDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsSalesServiceDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<long> CreateSalesOrder(SalesOrder salesOrder)
        {
            var date = DateTime.Now;
            salesOrder.CreatedBy = _createdBy;
            salesOrder.CreatedOn = date;
            salesOrder.Unit = _unitname;
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
        //recieveable report 

        // public async Task<IEnumerable<RecievableCustomer>> GetRecievableCustomersWithCustomerID(string CustomerId)
        // {
        //     if (string.IsNullOrWhiteSpace(CustomerId));
        //     {
        //         var results = _tipsSalesServiceDbContext.Set<RecievableCustomer>()
        //.FromSqlInterpolated($"CALL Recievable_Report_forCustomer({CustomerId})")
        //.ToList();

        //         return results;
        //     }
        // }

        public async Task<IEnumerable<RecievableCustomer>> GetRecievableCustomersWithCustomerID(string CustomerId)
        {
            if (string.IsNullOrWhiteSpace(CustomerId))
            {
                throw new ArgumentException("Customer ID cannot be null or empty.");
            }

            try
            {
                var results = await _tipsSalesServiceDbContext.Set<RecievableCustomer>()
                    .FromSqlInterpolated($"CALL Recievable_Report_forCustomer({CustomerId})")
                    .ToListAsync();

                return results;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred while executing the stored procedure: {ex.Message}");
                return Enumerable.Empty<RecievableCustomer>();
            }
        }

        public async Task<IEnumerable<RecievableCustomer>> GetReceivableReportsForMultiCustomerID(string CustomerId)
        {

            try
            {
                var results = await _tipsSalesServiceDbContext.Set<RecievableCustomer>()
                    .FromSqlInterpolated($"CALL Recievable_Report_forMultiCustomer({CustomerId})")
                    .ToListAsync();

                return results;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occurred while executing the stored procedure: {ex.Message}");
                return Enumerable.Empty<RecievableCustomer>();
            }
        }


        public async Task<PagedList<SalesOrderSPReport>> GetSalesOrderSPReport(PagingParameter pagingParameter)
        {
            var results = _tipsSalesServiceDbContext.Set<SalesOrderSPReport>()
                      .FromSqlInterpolated($"CALL Sales_Order_without_parameter_Report")
                      .ToList();

            return PagedList<SalesOrderSPReport>.ToPagedList(results.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize);


        }
        public async Task<IEnumerable<SalesOrderSPReport>> GetSalesOrderSPReportWithParam(string CustomerName, string SalesOrderNumber, string KPN)
        {
            var result = _tipsSalesServiceDbContext
            .Set<SalesOrderSPReport>()
            .FromSqlInterpolated($"CALL SalesOrder_withparameter_Report({CustomerName},{SalesOrderNumber},{KPN})")
            .ToList();

            return result;

        }
        public async Task<List<SalesOrderDashboardSPReport>> GetSalesOrderDashboardSPReportWithParam(string Bucket_Id)
        {
            var result = _tipsSalesServiceDbContext
            .Set<SalesOrderDashboardSPReport>()
            .FromSqlInterpolated($"CALL SalesOrder_Dashboard({Bucket_Id})")
            .ToList();

            return result;

        }
        public async Task<List<TransactionDashboardSPReport>> GetTransactionDashboardSPReportWithParam()
        {
            var result = _tipsSalesServiceDbContext
            .Set<TransactionDashboardSPReport>()
            .FromSqlInterpolated($"CALL Transaction_Dashboard('bucket_Id4')")
            .ToList();

            return result;

        }
        public async Task<List<TransactionDashboardSPReport_bucketId1>> GetTransactionDashboardSPReportWithParam_bucketId1()
        {
            var result = _tipsSalesServiceDbContext
            .Set<TransactionDashboardSPReport_bucketId1>()
            .FromSqlInterpolated($"CALL Transaction_Dashboard('bucket_Id1')")
            .ToList();

            return result;

        }
        public async Task<List<TransactionDashboardSPReport_bucketId2>> GetTransactionDashboardSPReportWithParam_bucketId2()
        {
            var result = _tipsSalesServiceDbContext
            .Set<TransactionDashboardSPReport_bucketId2>()
            .FromSqlInterpolated($"CALL Transaction_Dashboard('bucket_Id2')")
            .ToList();

            return result;

        }
        public async Task<List<TransactionDashboardSPReport_bucketId3>> GetTransactionDashboardSPReportWithParam_bucketId3()
        {
            var result = _tipsSalesServiceDbContext
            .Set<TransactionDashboardSPReport_bucketId3>()
            .FromSqlInterpolated($"CALL Transaction_Dashboard('bucket_Id3')")
            .ToList();

            return result;

        }
        public async Task<List<TransactionDashboardSPReport_bucketId5>> GetTransactionDashboardSPReportWithParam_bucketId5()
        {
            var result = _tipsSalesServiceDbContext
            .Set<TransactionDashboardSPReport_bucketId5>()
            .FromSqlInterpolated($"CALL Transaction_Dashboard('bucket_Id5')")
            .ToList();

            return result;

        }
        
        public async Task<List<FinancialYearDashboardSPReport>> GetFinancialYearDashboardSPReportWithParam(string Bucket_Id)
        {
            var result = _tipsSalesServiceDbContext
            .Set<FinancialYearDashboardSPReport>()
            .FromSqlInterpolated($"CALL Financial_Year_Dashboard({Bucket_Id})")
            .ToList();

            return result;

        }
        public async Task<IEnumerable<SalesOrderSPReport>> GetSalesOrderSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsSalesServiceDbContext.Set<SalesOrderSPReport>()
                        .FromSqlInterpolated($"CALL SalesOrder_withparameter_withdate({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<SOSummarySPReport>> GetSOSummarySPReportWithParam(string CustomerId, string SalesOrderNumber, string KPN)
        {
            var result = _tipsSalesServiceDbContext
            .Set<SOSummarySPReport>()
            .FromSqlInterpolated($"CALL SO_Summary_Report_with_Parameter({CustomerId},{SalesOrderNumber},{KPN})")
            .ToList();

            return result;

        }

        public async Task<IEnumerable<SOSummarySPReport>> GetSOSummarySPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsSalesServiceDbContext.Set<SOSummarySPReport>()
                        .FromSqlInterpolated($"CALL SO_Summary_Report_with_date({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<RfqSalesOrderSPReport>> GetRfqSalesOrderSPReportWithParam(string CustomerName, string SalesOrderNumber, string KPN, string SOStatus, string CustomerId)
        {
            var result = _tipsSalesServiceDbContext
            .Set<RfqSalesOrderSPReport>()
            .FromSqlInterpolated($"CALL RFQ_salesorder_Kpnwise_withparameter_Report({CustomerName},{SalesOrderNumber},{KPN},{SOStatus},{CustomerId})")
            .ToList();

            return result;

        }
        public async Task<IEnumerable<RfqSalesOrderRoomWiseSPReport>> GetRfqSalesOrderRoomWiseSPReportWithParam(string CustomerName, string SalesOrderNumber, string KPN, string CustomerId)
        {
            var result = _tipsSalesServiceDbContext
            .Set<RfqSalesOrderRoomWiseSPReport>()
            .FromSqlInterpolated($"CALL RFQ_salesorder_withRoomwise_withparameter({CustomerName},{SalesOrderNumber},{KPN},{CustomerId})")
            .ToList();

            return result;

        }
        public async Task<IEnumerable<RfqSalesOrderRoomWiseSPReport>> GetRfqSalesOrderRoomWiseSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsSalesServiceDbContext.Set<RfqSalesOrderRoomWiseSPReport>()
                        .FromSqlInterpolated($"CALL RFQ_salesorder_withRoomwise_withdate({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<RfqSalesOrderSPReportForTrans>> GetRfqSalesOrderSPReportWithParamForTrans(string CustomerName, string SalesOrderNumber, string KPN, string SOStatus, string ProjectNumber)
        {
            var result = _tipsSalesServiceDbContext
            .Set<RfqSalesOrderSPReportForTrans>()
            .FromSqlInterpolated($"CALL RFQ_salesorder_withparameter_Report_tras({CustomerName},{SalesOrderNumber},{KPN},{SOStatus},{ProjectNumber})")
            .ToList();

            return result;

        }

        public async Task<IEnumerable<RfqSalesOrderSPReportForTrans>> GetRfqSalesOrderSPReportWithParamForAvision(string CustomerName, string SalesOrderNumber, string KPN, string SOStatus)
        {
            var result = _tipsSalesServiceDbContext
            .Set<RfqSalesOrderSPReportForTrans>()
            .FromSqlInterpolated($"CALL RFQ_salesorder_withparameter_Report({CustomerName},{SalesOrderNumber},{KPN},{SOStatus})")
            .ToList();

            return result;

        }

        public async Task<IEnumerable<RfqSalesOrderSPReport>> GetRfqSalesOrderSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsSalesServiceDbContext.Set<RfqSalesOrderSPReport>()
                        .FromSqlInterpolated($"CALL RFQ_salesorder_Kpnwise_withdate_Report({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<RfqSalesOrderSPReportForTrans>> GetRfqSalesOrderSPReportWithDateForTransAvision(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsSalesServiceDbContext.Set<RfqSalesOrderSPReportForTrans>()
                        .FromSqlInterpolated($"CALL RFQ_salesorder_withdate_Report({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<ForecastSalesOrderSPReport>> GetForecastSalesOrderSPReportWithParam(string CustomerName, string SalesOrderNumber, string KPN, string SOStatus, string CustomerId)
        {
            var result = _tipsSalesServiceDbContext
            .Set<ForecastSalesOrderSPReport>()
            .FromSqlInterpolated($"CALL Forecast_salesorder_with_parameter({CustomerName},{SalesOrderNumber},{KPN},{SOStatus},{CustomerId})")
            .ToList();

            return result;

        }
        public async Task<IEnumerable<ForecastSalesOrderSPReportForTrans>> GetForecastSalesOrderSPReportWithParamForTrans(string CustomerName, string SalesOrderNumber, string KPN, string SOStatus, string ProjectNumber)
        {
            var result = _tipsSalesServiceDbContext
            .Set<ForecastSalesOrderSPReportForTrans>()
            .FromSqlInterpolated($"CALL Forecast_salesorder_with_parameter_tras({CustomerName},{SalesOrderNumber},{KPN},{SOStatus},{ProjectNumber})")
            .ToList();

            return result;

        }
        public async Task<IEnumerable<ForecastSalesOrderSPReportForTrans>> GetForecastSalesOrderSPReportWithParamForAvision(string CustomerName, string SalesOrderNumber, string KPN, string SOStatus)
        {
            var result = _tipsSalesServiceDbContext
            .Set<ForecastSalesOrderSPReportForTrans>()
            .FromSqlInterpolated($"CALL Forecast_salesorder_with_parameter({CustomerName},{SalesOrderNumber},{KPN},{SOStatus})")
            .ToList();

            return result;

        }
        public async Task<IEnumerable<ForecastSalesOrderSPReport>> GetForecastSalesOrderSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsSalesServiceDbContext.Set<ForecastSalesOrderSPReport>()
                        .FromSqlInterpolated($"CALL Forecast_salesorder_with_date({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<ForecastSalesOrderSPReportForTrans>> GetForecastSalesOrderSPReportWithDateForTransAvision(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsSalesServiceDbContext.Set<ForecastSalesOrderSPReportForTrans>()
                        .FromSqlInterpolated($"CALL Forecast_salesorder_with_date({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<CustomerWiseTransactionSPReport>> GetCustomerWiseTransactionSPReportWithParam(string CustomerId)
        {
            var result = _tipsSalesServiceDbContext
            .Set<CustomerWiseTransactionSPReport>()
            .FromSqlInterpolated($"CALL CustomerWise_Transaction_Report_WithParameter({CustomerId})")
            .ToList();

            return result;

        }
        public async Task<IEnumerable<SOMonthlyConsumptionSPReport>> GetSOMonthlyConsumptionSPReportWithParam(string CustomerId)
        {
            var result = _tipsSalesServiceDbContext
            .Set<SOMonthlyConsumptionSPReport>()
            .FromSqlInterpolated($"CALL monthly_consumption_withsalesorderno_withparameter({CustomerId})")
            .ToList();

            return result;

        }
        public async Task<SalesOrderId_SP> GetSalesOrderDetialsById_SP(int id)
        {
            var result = _tipsSalesServiceDbContext
            .Set<SalesOrderId_SP>()
            .FromSqlInterpolated($"CALL Salesorder_GetBy_Id({id})").ToList();

            return result[0];

        }

        public async Task<IEnumerable<SOMonthlyConsumptionSPReport>> GetSOMonthlyConsumptionSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsSalesServiceDbContext.Set<SOMonthlyConsumptionSPReport>()
                        .FromSqlInterpolated($"CALL monthly_consumption_withsalesorderno_withdate({FromDate},{ToDate})")
                        .ToList();

            return results;

        }

        public async Task<IEnumerable<FGSalesOrderSPReport>> GetFGSalesOrderSPReportWithParam(string? SalesOrderNumber, string? ProjectNumber)
        {
            var result = _tipsSalesServiceDbContext
            .Set<FGSalesOrderSPReport>()
            .FromSqlInterpolated($"CALL All_Fgs_salesorder_Report({SalesOrderNumber},{ProjectNumber})")
            .ToList();

            return result;

        }
        public async Task<IEnumerable<FGSalesOrderSPReportWithDate>> GetFGSalesOrderSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsSalesServiceDbContext.Set<FGSalesOrderSPReportWithDate>()
                        .FromSqlInterpolated($"CALL All_Fgs_salesorder_Report_withdate({FromDate},{ToDate})")
                        .ToList();

            return results;

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

        public async Task<PagedList<SalesOrder>> GetAllSalesOrderRfq([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var salesOrderDetails = FindAll().OrderByDescending(x => x.Id)
               .Where(inv => (inv.SalesOrderStatus == SalesOrderStatus.RetailSalesOrder || inv.SalesOrderStatus == SalesOrderStatus.BuildToPrint)
                && (string.IsNullOrWhiteSpace(searchParammes.SearchValue) || string.IsNullOrEmpty(searchParammes.SearchValue)
                || inv.SalesOrderNumber.Contains(searchParammes.SearchValue) || inv.ProjectNumber.Contains(searchParammes.SearchValue)
                || inv.CustomerName.Contains(searchParammes.SearchValue) || inv.CustomerId.Contains(searchParammes.SearchValue)))
               .Include(t => t.SalesOrdersItems).ThenInclude(p => p.ScheduleDates).Include(p => p.SalesOrderAdditionalCharges);

            return PagedList<SalesOrder>.ToPagedList(salesOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<List<string>> GetAllSalesOrderCustomerNames(List<string> SalesOrders)
        {
            var Customers = FindAll().Where(x=>SalesOrders.Contains(x.SalesOrderNumber)).OrderByDescending(x=>x.Id).Select(x=>x.CustomerName).Distinct().ToList();
            return Customers;
        }
        public async Task<PagedList<SalesOrder>> GetAllSalesOrderForecast([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var salesOrderDetails = FindAll().OrderByDescending(x => x.Id)
               .Where(inv => (inv.SalesOrderStatus.Equals(SalesOrderStatus.Forecast))
               && (string.IsNullOrWhiteSpace(searchParammes.SearchValue) || string.IsNullOrEmpty(searchParammes.SearchValue)
               || inv.SalesOrderNumber.Contains(searchParammes.SearchValue) || inv.ProjectNumber.Contains(searchParammes.SearchValue)
               || inv.CustomerName.Contains(searchParammes.SearchValue) || inv.CustomerId.Contains(searchParammes.SearchValue)))
               .Include(t => t.SalesOrdersItems).ThenInclude(p => p.ScheduleDates).Include(p => p.SalesOrderAdditionalCharges);

            return PagedList<SalesOrder>.ToPagedList(salesOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
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
        public async Task<PagedList<SalesOrder>> GetAllSalesOrderforKeus([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
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
        public async Task<List<SalesOrderforKeusDto>> GetAllSalesOrderforKeus([FromQuery] string? SearchTerm, [FromQuery] int Offset, [FromQuery] int Limit)
        {
            var result = await _tipsSalesServiceDbContext
           .Set<SalesOrderforKeusDto>()
           .FromSqlInterpolated($"CALL GetAllSalesDetailsSPforKeus({SearchTerm},{Offset},{Limit})")
           .ToListAsync();

            return result;
        }
        public async Task<int> GetAllSalesOrderCountforKeus(string? SearchTerm)
        {
            var result = await FindAll()
            .Where(inv => string.IsNullOrWhiteSpace(SearchTerm)
            || inv.SalesOrderNumber==SearchTerm
            || inv.ProjectNumber==SearchTerm
            || inv.CustomerName==SearchTerm
            || inv.CustomerId==SearchTerm
            ).CountAsync();

            return result;
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

        public async Task<string> GenerateSONumberForAvision()
        {
            using var transaction = await _tipsSalesServiceDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var poNumberEntity = await _tipsSalesServiceDbContext.SONumbers.SingleAsync();
                poNumberEntity.CurrentValue += 1;
                _tipsSalesServiceDbContext.Update(poNumberEntity);
                await _tipsSalesServiceDbContext.SaveChangesAsync();
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

                return $"ASPL|SO|{currentYear:D2}-{nextYear:D2}|{poNumberEntity.CurrentValue:D3}";
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
        public async Task<IEnumerable<SalesOrderFGItemNumberDto>> GetAllSalesOrderFGItemNoListByProjectNo(string projectNo)
        {
            IEnumerable<SalesOrderFGItemNumberDto> fgItemNumberList = await _tipsSalesServiceDbContext.SalesOrdersItems
                                .Where(x => x.ProjectNumber == projectNo && (x.StatusEnum == OrderStatus.Open || x.StatusEnum == OrderStatus.PartiallyClosed))
                                .Select(x => new SalesOrderFGItemNumberDto()
                                {
                                    FGItemNumber = x.ItemNumber,
                                })
                              .ToListAsync();

            return fgItemNumberList;
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
                                .Where(b => b.CustomerId == Customerid && b.SOStatus != OrderStatus.ShortClosed && b.SOStatus != OrderStatus.Closed)
                                .Select(x => new ListofSalesOrderDetails()
                                {
                                    SalesOrderId = x.Id,
                                    SalesOrderNumber = x.SalesOrderNumber,
                                    PONumber = x.PONumber,
                                })
                              .ToListAsync();

            return getSalesorderList;
        }

        public async Task<IEnumerable<ListofSalesOrderDetails>> GetSalesOrderNoDetailsByCustomerId(string Customerid)
        {
           var soAddSalesOrderIds = await _tipsSalesServiceDbContext.SalesOrderAdditionalCharges
                                .Where(x=> x.SOAdditionalStatus != SoStatus.Closed)
                                .Select(x => x.SalesOrderId) .ToListAsync();

            IEnumerable<ListofSalesOrderDetails> getSalesorderList = await _tipsSalesServiceDbContext.SalesOrders
                               .Where(b => b.CustomerId == Customerid && soAddSalesOrderIds.Contains( b.Id))
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
        public async Task<string> UpdateSalesOrderShortClose(SalesOrder salesOrder)
        {
            Update(salesOrder);
            string result = $"SalesOrder of Detail {salesOrder.Id} is updated successfully!";
            return result;
        }
        public async Task<string> UpdateSalesOrder(SalesOrder salesOrder)
        {
            salesOrder.CreatedBy = salesOrder.CreatedBy;
            salesOrder.CreatedOn = salesOrder.CreatedOn;
            salesOrder.LastModifiedBy = _createdBy;
            salesOrder.LastModifiedOn = DateTime.Now;

            var oldRevisionNumber = _tipsSalesServiceDbContext.SalesOrders
               .Where(x => x.SalesOrderNumber == salesOrder.SalesOrderNumber)
               .OrderByDescending(x => x.Id)
               .FirstOrDefault();

            oldRevisionNumber.LastModifiedBy = _createdBy;
            oldRevisionNumber.LastModifiedOn = DateTime.Now;
            Update(oldRevisionNumber);

            var increaseVersionNumber = 1;
            var version = oldRevisionNumber.RevisionNumber + increaseVersionNumber;
            salesOrder.RevisionNumber = (version);
            Update(salesOrder);
            string result = $"SalesOrder of Detail {salesOrder.Id} is updated successfully!";
            return result;
        }

        public async Task<List<ProjectSOSADetailDto>> GetProjectDetailsBySAItemNo(string fgItemNumbers)
        {
            var projectNumbers = await _tipsSalesServiceDbContext.SalesOrdersItems
                .Where(x => fgItemNumbers.Contains(x.ItemNumber))
                .Select(m => m.ProjectNumber)
                .Distinct().AsNoTracking()
                .ToListAsync();

            var projectSODetails = await _tipsSalesServiceDbContext.SalesOrders
                .Where(m => projectNumbers.Contains(m.ProjectNumber)
                            && m.SOStatus != OrderStatus.Closed && m.IsShortClosed == false).AsNoTracking()
                .Select(s => new ProjectSOSADetailDto()
                {
                    ProjectNumber = s.ProjectNumber,
                    CustomerName = s.CustomerName,
                    CustomerId = s.CustomerId
                }).Distinct().ToListAsync();

            return projectSODetails;
        }
        //public async Task<List<ProjectSODetailDto>> GetProjectDetailsByItemNo(string itemNumber)
        //{
        //    var projectNumbers = await _tipsSalesServiceDbContext.SalesOrdersItems
        //                        .Where(x => x.ItemNumber == itemNumber)
        //                        .Select(m => m.ProjectNumber).Distinct().ToListAsync();

        //        var projectSODetails = await _tipsSalesServiceDbContext.SalesOrders
        //                            .Where(m => projectNumbers.Contains(m.ProjectNumber)
        //                            && m.SOStatus != OrderStatus.Closed && m.IsShortClosed == false)
        //                            .Select(s => new ProjectSODetailDto()
        //                            {
        //                                ProjectNumber = s.ProjectNumber,
        //                                CustomerName = s.CustomerName,
        //                                CustomerId = s.CustomerId
        //                            }).Distinct().ToListAsync();
        //        return projectSODetails;


        //}

        public async Task<List<ProjectSODetailDto>> GetProjectDetailsByItemNo(string itemNumber, string projectType)
        {
            var projectNumbers = await _tipsSalesServiceDbContext.SalesOrdersItems
                                .Where(x => x.ItemNumber == itemNumber)
                                .Select(m => m.ProjectNumber).Distinct().ToListAsync();

            if (projectType == "ForeCast")
            {
                var projectSODetails = await _tipsSalesServiceDbContext.SalesOrders
                                    .Where(m => projectNumbers.Contains(m.ProjectNumber)
                                    && m.SOStatus != OrderStatus.Closed && m.IsShortClosed == false && m.SalesOrderStatus == SalesOrderStatus.Forecast)
                                    .Select(s => new ProjectSODetailDto()
                                    {
                                        ProjectNumber = s.ProjectNumber,
                                        CustomerName = s.CustomerName,
                                        CustomerId = s.CustomerId
                                    }).Distinct().ToListAsync();
                return projectSODetails;
            }
            else
            {
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
        }



        //public async Task<List<SalesOrderQtyDto>> GetSASalesOrderQtyDetailsByItemNo(string itemNumber, string projectNo, decimal BomQty)
        //{
        //    OrderStatus[] status = { OrderStatus.Open, OrderStatus.PartiallyClosed };
        //    List<SalesOrderQtyForSADto> salesOrderQtyDtos = await _tipsSalesServiceDbContext.SalesOrdersItems
        //      .Where(x => x.ItemNumber == itemNumber
        //                 && x.ProjectNumber == projectNo
        //                 && status.Contains(x.StatusEnum))
        //      .GroupBy(x => x.SalesOrderNumber)
        //      .Select(g => new SalesOrderQtyForSADto
        //      {
        //          SalesOrderNo = g.Key,
        //          SalesOrderQty = g.Sum(x => x.OrderQty),
        //          OpenSalesOrderQty = g.Sum(x => x.OrderQty) - g.Sum(x => x.ShopOrderQty),
        //          RequiredQty = OpenSalesOrderQty * BomQty
        //      })
        //      .ToListAsync();

        //    return salesOrderQtyDtos;
        //}

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
        //SA SalesOrderQtyDetailsByitemNo
        public async Task<List<SalesOrderQtyForSADto>> GetSASalesOrderQtyDetailsByItemNo(string fgItemNumber, string projectNo, decimal BomQty)
        {
            OrderStatus[] status = { OrderStatus.Open, OrderStatus.PartiallyClosed };
            //List<SalesOrderQtyForSADto> salesOrderQtyDtos = await _tipsSalesServiceDbContext.SalesOrdersItems
            //  .Where(x => x.ItemNumber == fgItemNumber
            //             && x.ProjectNumber == projectNo
            //             && status.Contains(x.StatusEnum))
            //  .GroupBy(x => x.SalesOrderNumber)
            //  .Select(g => new SalesOrderQtyForSADto
            //  {
            //      SalesOrderNo = g.Key,
            //      ProjectNumber= projectNo,
            //      FgItemNumber = fgItemNumber, 
            //      SalesOrderQty = g.Sum(x => x.OrderQty),
            //      OpenSalesOrderQty = g.Sum(x => x.OrderQty) - g.Sum(x => x.ShopOrderQty),
            //      RequiredQty = (g.Sum(x => x.OrderQty) - g.Sum(x => x.ShopOrderQty)) * BomQty
            //  })
            //  .ToListAsync();
            List<SalesOrderQtyForSADto> salesOrderQtyDtos = await _tipsSalesServiceDbContext.SalesOrdersItems
              .Where(x => x.ItemNumber == fgItemNumber
                         && x.ProjectNumber == projectNo
                         && status.Contains(x.StatusEnum))
              .GroupBy(x => x.SalesOrderNumber)
              .Select(g => new SalesOrderQtyForSADto
              {
                  SalesOrderNo = g.Key,
                  ProjectNumber = projectNo,
                  FgItemNumber = fgItemNumber,
                  SalesOrderQty = g.Sum(x => x.OrderQty),
                  OpenSalesOrderQty = g.Sum(x => x.OrderQty) - g.Sum(x => x.ShopOrderQty),
                  RequiredQty = (g.Sum(x => x.OrderQty)) * BomQty,
                  Description = g.First().Description // Assuming Description is a property of SalesOrdersItems
              })
              .ToListAsync();



            return salesOrderQtyDtos;
        }

        public async Task<decimal> GetOpenSalesOrderQuantityByItemNumber(string itemNumber)
        {
            return await _tipsSalesServiceDbContext.SalesOrdersItems
            .Where(soi => soi.ItemNumber == itemNumber && soi.StatusEnum != OrderStatus.Closed
                && soi.SalesOrder.IsShortClosed == false)
            .SumAsync(soi => soi.BalanceQty);
        }

        public async Task<IEnumerable<SalesOrderQtyDetailsDto>> GetSalesOrderQtySPReportWithParam(string itemNo, decimal bomQty)
        {
            var result = _tipsSalesServiceDbContext
             .Set<SalesOrderQtyDetailsDto>()
             .FromSqlInterpolated($"CALL SalesOrder_QuantityDetails({itemNo},{bomQty})")
             .ToList();

            return result;
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
        public async Task<SalesOrderRetailFGandBalanceQty> GetAllSalesOrderFGOrTGRetailItemDetails(string fGItemNumber)
        {
            var salesOrderIdList = await _tipsSalesServiceDbContexts.SalesOrders
                .Where(so => (so.SalesOrderStatus == SalesOrderStatus.RetailSalesOrder) &&
                             (so.SOStatus == OrderStatus.Open || so.SOStatus == OrderStatus.PartiallyClosed) &&
                             so.IsShortClosed == false// &&
                                                      //so.ConfirmStatus == true && so.ApproveStatus == true
                             ).Select(x => x.Id).ToListAsync();

            var openSalesOrderQty = await _tipsSalesServiceDbContexts.SalesOrdersItems
                .Where(x =>
                            (x.StatusEnum == OrderStatus.Open || x.StatusEnum == OrderStatus.PartiallyClosed) && x.ItemNumber == fGItemNumber &&
                            x.BalanceQty > 0 && salesOrderIdList.Contains(x.SalesOrderId))
                .GroupBy(x => new { x.ItemNumber })
                .Select(group => new SalesOrderRetailFGandBalanceQty
                {
                    Balance_Qty = group.Sum(x => x.BalanceQty)
                }).FirstOrDefaultAsync();

            return openSalesOrderQty;

        }
        public async Task<List<SalesOrderFGandBalanceQty>> GetAllSalesOrderFGOrTGItemDetails()
        {
            var salesOrderIdList = await _tipsSalesServiceDbContexts.SalesOrders
                .Where(so => (so.SalesOrderStatus == SalesOrderStatus.BuildToPrint || so.SalesOrderStatus == SalesOrderStatus.Forecast) &&
                             (so.SOStatus == OrderStatus.Open || so.SOStatus == OrderStatus.PartiallyClosed) &&
                             so.IsShortClosed == false // &&
                                                       //so.ConfirmStatus == true && so.ApproveStatus == true
                             ).Select(x => x.Id).ToListAsync();

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
        public async Task<List<SalesOrderFGandBalanceQtyByProjectNo>> GetAllSalesOrderFGOrTGItemDetailsByProjectNo(string projectNo)
        {
            var salesOrderIdList = await _tipsSalesServiceDbContexts.SalesOrders
                .Where(so => (so.SalesOrderStatus == SalesOrderStatus.BuildToPrint || so.SalesOrderStatus == SalesOrderStatus.Forecast) &&
                             (so.SOStatus == OrderStatus.Open || so.SOStatus == OrderStatus.PartiallyClosed) &&
                             so.IsShortClosed == false && so.ProjectNumber == projectNo // &&
                                                                                        //so.ConfirmStatus == true && so.ApproveStatus == true
                             ).Select(x => x.Id).ToListAsync();

            var openSalesOrderQty = await _tipsSalesServiceDbContexts.SalesOrdersItems
                .Where(x =>
                            (x.StatusEnum == OrderStatus.Open || x.StatusEnum == OrderStatus.PartiallyClosed) &&
                            x.BalanceQty > 0 && salesOrderIdList.Contains(x.SalesOrderId))
                .GroupBy(x => new { x.ItemNumber, x.ProjectNumber })
                .Select(group => new SalesOrderFGandBalanceQtyByProjectNo
                {
                    FGItemNumber = group.Key.ItemNumber,
                    ProjectNumber = group.Key.ProjectNumber,
                    Description = group.First().Description,
                    UOM = group.First().UOM,
                    Balance_Qty = group.Sum(x => x.BalanceQty)
                }).ToListAsync();

            return openSalesOrderQty;

        }
        public async Task<List<SalesOrderFGandBalanceQtyByProjectNo>> GetAllSalesOrderFGOrTGItemDetailsByMultipleProjectNo(List<string?> projectNos)
        {
            var salesOrderIdList = await _tipsSalesServiceDbContexts.SalesOrders
                .Where(so => (so.SalesOrderStatus == SalesOrderStatus.BuildToPrint || so.SalesOrderStatus == SalesOrderStatus.Forecast) &&
                             (so.SOStatus == OrderStatus.Open || so.SOStatus == OrderStatus.PartiallyClosed) &&
                             so.IsShortClosed == false && projectNos.Contains(so.ProjectNumber) // &&
                                                                                        //so.ConfirmStatus == true && so.ApproveStatus == true
                             ).Select(x => x.Id).ToListAsync();

            var openSalesOrderQty = await _tipsSalesServiceDbContexts.SalesOrdersItems
                .Where(x =>
                            (x.StatusEnum == OrderStatus.Open || x.StatusEnum == OrderStatus.PartiallyClosed) &&
                            x.BalanceQty > 0 && salesOrderIdList.Contains(x.SalesOrderId))
                .GroupBy(x => new { x.ItemNumber, x.ProjectNumber })
                .Select(group => new SalesOrderFGandBalanceQtyByProjectNo
                {
                    FGItemNumber = group.Key.ItemNumber,
                    ProjectNumber = group.Key.ProjectNumber,
                    Description = group.First().Description,
                    UOM = group.First().UOM,
                    Balance_Qty = group.Sum(x => x.BalanceQty)
                }).ToListAsync();

            return openSalesOrderQty;

        }
        public async Task<List<SalesOrderFGandBalanceQtyByCustomerName>> GetAllSalesOrderFGOrTGItemDetailsByCustomerId(string customerId)
        {
            var salesOrderIds = await _tipsSalesServiceDbContexts.SalesOrders
                .Where(so => (so.SalesOrderStatus == SalesOrderStatus.BuildToPrint || so.SalesOrderStatus == SalesOrderStatus.Forecast) &&
                             (so.SOStatus == OrderStatus.Open || so.SOStatus == OrderStatus.PartiallyClosed) &&
                             so.IsShortClosed == false && so.CustomerId == customerId // && //so.ConfirmStatus == true && so.ApproveStatus == true
                             ).Select(x => x.Id).ToListAsync();

            var openSalesOrderQty = await _tipsSalesServiceDbContexts.SalesOrdersItems
                .Where(x =>
                            (x.StatusEnum == OrderStatus.Open || x.StatusEnum == OrderStatus.PartiallyClosed) &&
                            x.BalanceQty > 0 && salesOrderIds.Contains(x.SalesOrderId))
                .GroupBy(x => new { x.ProjectNumber })
                .Select(group => new SalesOrderFGandBalanceQtyByCustomerName
                {
                    ProjectNumber = group.Key.ProjectNumber
                }).ToListAsync();

            return openSalesOrderQty;

        }
        public async Task<List<SalesOrderFGandBalanceQtyByProjectNo>> GetAllSalesOrderFGOrTGItemDetailsByProjectNoAndItemNo(string projectNo, string itemNumber)
        {
            var salesOrderIdList = await _tipsSalesServiceDbContexts.SalesOrders
                .Where(so => (so.SalesOrderStatus == SalesOrderStatus.BuildToPrint || so.SalesOrderStatus == SalesOrderStatus.Forecast) &&
                             (so.SOStatus == OrderStatus.Open || so.SOStatus == OrderStatus.PartiallyClosed) &&
                             so.IsShortClosed == false && so.ProjectNumber == projectNo // &&
                                                                                        //so.ConfirmStatus == true && so.ApproveStatus == true
                             ).Select(x => x.Id).ToListAsync();

            var openSalesOrderQty = await _tipsSalesServiceDbContexts.SalesOrdersItems
                .Where(x =>
                            (x.StatusEnum == OrderStatus.Open || x.StatusEnum == OrderStatus.PartiallyClosed) &&
                            x.BalanceQty > 0 && salesOrderIdList.Contains(x.SalesOrderId) && x.ItemNumber == itemNumber)
                .GroupBy(x => new { x.ItemNumber, x.ProjectNumber })
                .Select(group => new SalesOrderFGandBalanceQtyByProjectNo
                {
                    FGItemNumber = group.Key.ItemNumber,
                    ProjectNumber = group.Key.ProjectNumber,
                    Description = group.First().Description,
                    UOM = group.First().UOM,
                    Balance_Qty = group.Sum(x => x.BalanceQty)
                }).ToListAsync();

            return openSalesOrderQty;

        }

        //update shoporderQty
        public async Task<IEnumerable<SalesOrderItems>> UpdateShopOrderBySalesOrderNoandItemNo(string salesOrderNumber, string itemNumber, string projectNumber)
        {
            OrderStatus[] status = { OrderStatus.Open, OrderStatus.PartiallyClosed };

            var updateShopOrderQty = await _tipsSalesServiceDbContexts.SalesOrdersItems
                 .Where(x => x.ItemNumber == itemNumber && x.ProjectNumber == projectNumber && x.SalesOrderNumber == salesOrderNumber
                 && status.Contains(x.StatusEnum))
                          .ToListAsync();

            return updateShopOrderQty;
        }
        public async Task<IEnumerable<SalesOrderItems>> UpdateShopOrderQtyBySalesOrderNoandItemNo(string salesOrderNumber, string itemNumber, string projectNumber)
        {

            var updateShopOrderQty = await _tipsSalesServiceDbContexts.SalesOrdersItems
                 .Where(x => x.ItemNumber == itemNumber && x.ProjectNumber == projectNumber && x.SalesOrderNumber == salesOrderNumber)
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
        public async Task<IEnumerable<SalesOrderItems>> GetSalesOrderItemDetailsForReturnByIdandItemNo(string ItemNumber, int SalesOrderId)
        {

            var getSalesOrderDetailsBySOandItemNo = await _tipsSalesServiceDbContexts.SalesOrdersItems
                 .Where(x => x.ItemNumber == ItemNumber && x.SalesOrderId == SalesOrderId && x.DispatchQty > 0 && x.StatusEnum != OrderStatus.Open)
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
        public async Task<int?> GetSOItemShortCloseCount(int soId)
        {
            var soItemStatusCount = _tipsSalesServiceDbContexts.SalesOrdersItems
                                        .Where(x => x.SalesOrderId == soId && x.StatusEnum == OrderStatus.ShortClosed).Count();

            return soItemStatusCount;
        }
        public async Task<int?> GetSOItemOpenStatusCount(int soId)
        {
            var soItemStatusCount = _tipsSalesServiceDbContexts.SalesOrdersItems
                                        .Where(x => x.SalesOrderId == soId && x.StatusEnum == OrderStatus.Open).Count();

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
        public async Task<List<SalesOrderHistory>> GetSalesOrderHistoryBySONoAndItemNumberifShortCLosed(string SOnumber, string Itemnumber)
        {
            var SOHistory = await FindAll().Where(x => x.Remarks == "Item ShortClosed" && x.SalesOrderNumber == SOnumber && x.ItemNumber == Itemnumber).ToListAsync();
            return SOHistory;
        }
    }
    public class ScheduleDateHistoryRepository : RepositoryBase<ScheduleDateHistory>, IScheduleDateHistoryRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContexts;
        public ScheduleDateHistoryRepository(TipsSalesServiceDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsSalesServiceDbContexts = repositoryContext;
        }

        public async Task<ScheduleDateHistory> CreateScheduleDateHistory(ScheduleDateHistory scheduleDateHistory)
        {
            var result = await Create(scheduleDateHistory);
            return result;
        }
    }
    public class SalesOrderAdditionalChargesHistoryRepository : RepositoryBase<SalesOrderAdditionalChargesHistory>, ISalesOrderAdditionalChargesHistoryRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContexts;
        public SalesOrderAdditionalChargesHistoryRepository(TipsSalesServiceDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsSalesServiceDbContexts = repositoryContext;
        }

        public async Task<SalesOrderAdditionalChargesHistory> CreateSalesOrderAdditionalChargesHistory(SalesOrderAdditionalChargesHistory salesOrderAdditionalChargesHistory)
        {
            var result = await Create(salesOrderAdditionalChargesHistory);
            return result;
        }
    }
}


