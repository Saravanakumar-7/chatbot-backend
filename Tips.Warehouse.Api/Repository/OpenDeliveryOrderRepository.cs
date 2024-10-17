using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Repository;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Tips.Warehouse.Api.Entities.DTOs;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Tips.Warehouse.Api.Repository
{
    public class OpenDeliveryOrderRepository : RepositoryBase<OpenDeliveryOrder>, IOpenDeliveryOrderRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public OpenDeliveryOrderRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder)
        {
            var date = DateTime.Now;
            openDeliveryOrder.CreatedBy = _createdBy;
            openDeliveryOrder.CreatedOn = date;

            openDeliveryOrder.Unit = _unitname;
            var result = await Create(openDeliveryOrder);

            return result.Id;
        }

        public async Task<int?> GetODONumberAutoIncrementCount(DateTime date)
        {
            var getOpenDeliveryOrderDetailsByIds = _tipsWarehouseDbContext.OpenDeliveryOrders.Where(x => x.CreatedOn == date.Date).Count();

            return getOpenDeliveryOrderDetailsByIds;
        }
        public async Task<string> GenerateODONumberAvision()
        {
            using var transaction = await _tipsWarehouseDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var rfqNumberEntity = await _tipsWarehouseDbContext.ODONumbers.SingleAsync();
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

                return $"ASPL|ODO|{currentYear:D2}-{nextYear:D2}|{rfqNumberEntity.CurrentValue:D3}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<string> GenerateODONumber()
        {
            using var transaction = await _tipsWarehouseDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var odoNumberEntity = await _tipsWarehouseDbContext.ODONumbers.SingleAsync();
                odoNumberEntity.CurrentValue += 1;
                _tipsWarehouseDbContext.Update(odoNumberEntity);
                await _tipsWarehouseDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"ODO-{odoNumberEntity.CurrentValue:D6}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task<string> DeleteOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder)
        {
            Delete(openDeliveryOrder);
            string result = $"OpenDeliveryOrder details of {openDeliveryOrder.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<OpenDeliveryOrderSPReport>> OpenDeliveryOrderSPReportWithParam(string OpenDONumber, string CustomerName, string CustomerAliasName, string LeadId, 
            string IssuedTo,string KPN, string MPN, string Warehouse, string Location, string ODOType)
        {

            var result = _tipsWarehouseDbContext
            .Set<OpenDeliveryOrderSPReport>()
            .FromSqlInterpolated($"CALL Open_Delivery_Order_Report_withparameter({OpenDONumber},{CustomerName},{CustomerAliasName},{LeadId},{IssuedTo},{KPN},{MPN},{Warehouse},{Location},{ODOType})")
            .ToList();

            return result;


        }

        public async Task<IEnumerable<OpenDeliveryOrderSPReportForTrans>> OpenDeliveryOrderSPReportWithParamForTrans(string OpenDONumber, string CustomerName,
                                                                                                                 string IssuedTo, string ItemNumber, string MPN, string Warehouse, 
                                                                                                                 string Location, string ODOType, string ProjectNumber)
        {

            var result = _tipsWarehouseDbContext
            .Set<OpenDeliveryOrderSPReportForTrans>()
            .FromSqlInterpolated($"CALL Open_Delivery_Order_Report_withparameter_tras({OpenDONumber},{CustomerName},{IssuedTo},{ItemNumber},{MPN},{Warehouse},{Location},{ODOType},{ProjectNumber})")
            .ToList();

            return result;


        }
        public async Task<IEnumerable<OpenDeliveryOrderSPReportForTrans>> OpenDeliveryOrderSPReportDateForTrans(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsWarehouseDbContext.Set<OpenDeliveryOrderSPReportForTrans>()
                          .FromSqlInterpolated($"CALL Open_Delivery_Order_Report_withparameter_withdate_tras({FromDate},{ToDate})")
                          .ToList();

            return results;
        }
        public async Task<IEnumerable<OpenDeliveryOrderSPReport>> OpenDeliveryOrderSPReportDates(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsWarehouseDbContext.Set<OpenDeliveryOrderSPReport>()
                          .FromSqlInterpolated($"CALL Open_Delivery_Order_Report_withparameter_withdate({FromDate},{ToDate})")
                          .ToList();

            return results;
        }
        public async Task<IEnumerable<ODOMonthlyConsumptionSPReport>> GetODOMonthlyConsumptionSPReportWithParam(string CustomerId)
        {
            var result = _tipsWarehouseDbContext
            .Set<ODOMonthlyConsumptionSPReport>()
            .FromSqlInterpolated($"CALL monthly_consumption_withodonumber_withparameter({CustomerId})")
            .ToList();

            return result;

        }

        public async Task<IEnumerable<ODOMonthlyConsumptionSPReport>> GetODOMonthlyConsumptionSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsWarehouseDbContext.Set<ODOMonthlyConsumptionSPReport>()
                        .FromSqlInterpolated($"CALL monthly_consumption_withodonumber_withdate({FromDate},{ToDate})")
                        .ToList();

            return results;

        }

        public async Task<IEnumerable<OpenDeliveryOrder>> SearchOpenDeliveryOrderDate([FromQuery] SearchsDateParms searchsDateParms)
        {
            var openDeliveryOrderDetails = _tipsWarehouseDbContext.OpenDeliveryOrders
            .Where(inv => ((inv.CreatedOn >= searchsDateParms.SearchFromDate &&
            inv.CreatedOn <= searchsDateParms.SearchToDate
            )))
            .Include(itm => itm.OpenDeliveryOrderParts)
            .ToList();
            return openDeliveryOrderDetails;
        }
        public async Task<IEnumerable<OpenDeliveryOrder>> GetAllOpenDeliveryOrderWithItems(OpenDeliveryOrderSearchDto openDeliveryOrderSearch)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.OpenDeliveryOrders.Include("OpenDeliveryOrderParts");
                if (openDeliveryOrderSearch != null || (openDeliveryOrderSearch.DOType.Any())
                 && openDeliveryOrderSearch.ODONumber.Any() && openDeliveryOrderSearch.CustomerName.Any()
                 && openDeliveryOrderSearch.Description.Any() && openDeliveryOrderSearch.IssuedTo.Any())

                {
                    query = query.Where
                    (po => (openDeliveryOrderSearch.CustomerName.Any() ? openDeliveryOrderSearch.CustomerName.Contains(po.CustomerName) : true)
                   && (openDeliveryOrderSearch.Description.Any() ? openDeliveryOrderSearch.Description.Contains(po.Description) : true)
                   && (openDeliveryOrderSearch.ODONumber.Any() ? openDeliveryOrderSearch.ODONumber.Contains(po.OpenDONumber) : true)
                   && (openDeliveryOrderSearch.DOType.Any() ? openDeliveryOrderSearch.DOType.Contains(po.DOType) : true)
                   && (openDeliveryOrderSearch.IssuedTo.Any() ? openDeliveryOrderSearch.IssuedTo.Contains(po.IssuedTo) : true));
                }
                return query.ToList();
            }
        }

        public async Task<IEnumerable<OpenDeliveryOrder>> SearchOpenDeliveryOrder([FromQuery] SearchParames searchParames)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.OpenDeliveryOrders.Include("OpenDeliveryOrderParts");
                if (!string.IsNullOrEmpty(searchParames.SearchValue))
                {
                    query = query.Where(po => po.OpenDONumber.Contains(searchParames.SearchValue)
                    || po.CustomerName.Contains(searchParames.SearchValue)
                    || po.CustomerAliasName.Contains(searchParames.SearchValue)
                    || po.CustomerId.Contains(searchParames.SearchValue)
                    || po.DOType.Contains(searchParames.SearchValue)
                    || po.IssuedTo.Contains(searchParames.SearchValue)
                    || po.OpenDeliveryOrderParts.Any(s => s.ItemNumber.Contains(searchParames.SearchValue) ||
                    s.ItemDescription.Contains(searchParames.SearchValue)
                    || s.ItemType.ToString().Contains(searchParames.SearchValue)
                    || s.Location.Contains(searchParames.SearchValue)
                    ));
                }
                return query.ToList();
            }
        }
        public async Task<PagedList<OpenDeliveryOrder>> GetAllOpenDeliveryOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {


            var getAllOpenDeliveryOrderDetails = FindAll().OrderByDescending(x => x.Id)
               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.OpenDONumber.Contains(searchParams.SearchValue) ||
                inv.CustomerAliasName.Contains(searchParams.SearchValue) || inv.CustomerName.Contains(searchParams.SearchValue) || inv.CustomerId.Contains(searchParams.SearchValue))))
                .Include(x => x.OpenDeliveryOrderParts);
            return PagedList<OpenDeliveryOrder>.ToPagedList(getAllOpenDeliveryOrderDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

        }

        public async Task<PagedList<OpenDeliveryOrderSPReport>> OpenDeliveryOrderSPReport(PagingParameter pagingParameter)
        {
            var results = _tipsWarehouseDbContext.Set<OpenDeliveryOrderSPReport>()
                        .FromSqlInterpolated($"CALL Open_Delivery_Order_Report")
                        .ToList();

            return PagedList<OpenDeliveryOrderSPReport>.ToPagedList(results.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<OpenDeliveryOrder> GetOpenDeliveryOrderById(int id)
        {
            var getOpenDeliveryOrderDetailsById = await _tipsWarehouseDbContext.OpenDeliveryOrders
                                .Where(x => x.Id == id)
                               .Include(x => x.OpenDeliveryOrderParts).ThenInclude(x => x.QtyDistribution)
                               .FirstOrDefaultAsync();

            return getOpenDeliveryOrderDetailsById;
        }


        public async Task<ODODetailsDto> GetODODetailsByItemNo(string itemNumber)
        {
            string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };
            decimal Stock = _tipsWarehouseDbContext.Inventories
                .Where(x => x.PartNumber == itemNumber && !skipWareHouse.Contains(x.Warehouse))
                .Sum(x => x.Balance_Quantity);

            var projectNumbers = await _tipsWarehouseDbContext.Inventories
                            .Where(x => x.PartNumber == itemNumber && !skipWareHouse.Contains(x.Warehouse))
                            .Select(s => new ODODetailsDto()
                            {
                                ItemNumber = s.ProjectNumber,
                                ItemType = s.PartType,
                                UOM = s.UOM,
                                StockAvailable = Stock,
                            }).Distinct().FirstOrDefaultAsync();

            return projectNumbers;
        }

        public async Task<List<WarehouseDetailsDto>> GetWarehouseODOByItemNo(string itemNumber)
        {
            string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };
            var projectNumbers = await _tipsWarehouseDbContext.Inventories
                                .Where(x => x.PartNumber == itemNumber && !skipWareHouse.Contains(x.Warehouse))
                                .Select(s => new WarehouseDetailsDto()
                                {
                                    WarehouseName = s.Warehouse,

                                }).Distinct().ToListAsync();

            return projectNumbers;
        }

        public async Task<List<LocationDetailsDto>> GetLocationODOByItemNo(string itemNumber, string warehouse)
        {
            string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };
            var projectNumbers = await _tipsWarehouseDbContext.Inventories
                                .Where(x => x.PartNumber == itemNumber && x.Warehouse == warehouse && !skipWareHouse.Contains(x.Warehouse))
                                .Select(s => new LocationDetailsDto()
                                {
                                    LocationName = s.Location,
                                    LocationStock = s.Balance_Quantity,

                                }).Distinct().ToListAsync();

            return projectNumbers;
        }

        public async Task<string> UpdateOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder)
        {
            openDeliveryOrder.LastModifiedBy = _createdBy;
            openDeliveryOrder.LastModifiedOn = DateTime.Now;
            Update(openDeliveryOrder);
            string result = $"openDeliveryOrder of Detail {openDeliveryOrder.Id} is updated successfully!";
            return result;
        }
        public async Task<IEnumerable<OpenDeliveryOrderIdNameList>> GetAllOpenDeliveryOrderIdNameList()
        {
            IEnumerable<OpenDeliveryOrderIdNameList> btoIddNameList = await _tipsWarehouseDbContext.OpenDeliveryOrders
                               .Select(x => new OpenDeliveryOrderIdNameList()
                               {
                                   Id = x.Id,

                                   ODONumber = x.OpenDONumber,
                                   ODOType = x.DOType

                               })
                               .OrderByDescending(x => x.Id)
                             .ToListAsync();

            return btoIddNameList;
        }
        public async Task<IEnumerable<odoLotNumberListDto>> GetODOLotNumberListByODONoAndItemNo(string odoNumber, string itemNumber)
        {
            var odoItemIdList = await _tipsWarehouseDbContext.OpenDeliveryOrderParts
                               .Where(x => x.ODONumber == odoNumber)
                               .Select(b => b.Id).Distinct().ToListAsync();

            var odoLotNumberlist = await _tipsWarehouseDbContext.OpenDeliveryOrderPartsQtyDistribution
                               .Where(x => x.PartNumber == itemNumber && odoItemIdList.Contains(x.OpenDeliveryOrderPartsId))
                               .GroupBy(x => x.LotNumber)
                              .Select(g => new odoLotNumberListDto()
                              {
                                  LotNumber = g.Key

                              })
                            .ToListAsync();

            return odoLotNumberlist;
        }
    }
}
