using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Repository
{
    public class ReturnOpenDeliveryOrderRepository : RepositoryBase<ReturnOpenDeliveryOrder>, IReturnOpenDeliveryOrderRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ReturnOpenDeliveryOrderRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateReturnOpenDeliveryOrder(ReturnOpenDeliveryOrder returnOpenDeliveryOrder)
        {
            returnOpenDeliveryOrder.CreatedBy = _createdBy;
            returnOpenDeliveryOrder.CreatedOn = DateTime.Now;
            var result = await Create(returnOpenDeliveryOrder);
            return result.Id;
        }

        public async Task<string> DeleteReturnOpenDeliveryOrder(ReturnOpenDeliveryOrder returnOpenDeliveryOrder)
        {
            Delete(returnOpenDeliveryOrder);
            string result = $"ReturnOpenDeliveryOrder details of {returnOpenDeliveryOrder.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<ReturnOpenDeliveryOrder>> GetAllReturnOpenDeliveryOrderDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            var returODODetails = FindAll().OrderByDescending(x => x.Id)
               .Where(odo => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || odo.CustomerId.Contains(searchParams.SearchValue) ||
                odo.CustomerAliasName.Contains(searchParams.SearchValue) || odo.CustomerName.Contains(searchParams.SearchValue)
                || odo.ODONumber.Contains(searchParams.SearchValue)
                || odo.Description.Contains(searchParams.SearchValue))));

            return PagedList<ReturnOpenDeliveryOrder>.ToPagedList(returODODetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<int?> GetReturnOpenDeliveryOrderByODONo(string odoNumber)
        {
            var getReturnBtoDeliveryOrderByBtoNo = _tipsWarehouseDbContext.ReturnOpenDeliveryOrders
                     .Where(x => x.ODONumber == odoNumber).Count();
            return getReturnBtoDeliveryOrderByBtoNo;
        }

        public async Task<ReturnOpenDeliveryOrder> GetReturnOpenDeliveryOrderById(int id)
        {
            var returODODetailsById = await _tipsWarehouseDbContext.ReturnOpenDeliveryOrders.Where(x => x.Id == id)
                                .Include(t => t.ReturnOpenDeliveryOrderParts).ThenInclude(t => t.QtyDistribution)
                                .FirstOrDefaultAsync();


            return returODODetailsById;
        }
        public async Task<IEnumerable<ReturnOpenDeliveryOrderSPResport>> ReturnOpenDeliveryOrderSPReportDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsWarehouseDbContext.Set<ReturnOpenDeliveryOrderSPResport>()
                        .FromSqlInterpolated($"CALL Return_Open_DeliveryOrder_Report_withdate({FromDate},{ToDate})")
                        .ToList();

            return results;
        }
        public async Task<IEnumerable<ReturnOpenDeliveryOrderSPResportForTras>> ReturnOpenDeliveryOrderSPReportDateForTras(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsWarehouseDbContext.Set<ReturnOpenDeliveryOrderSPResportForTras>()
                        .FromSqlInterpolated($"CALL Return_Open_DeliveryOrder_Report_withdate_tras({FromDate},{ToDate})")
                        .ToList();

            return results;
        }
        public async Task<IEnumerable<ReturnOpenDeliveryOrderSPResportForAvi>> ReturnOpenDeliveryOrderSPReportDateForAvi(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsWarehouseDbContext.Set<ReturnOpenDeliveryOrderSPResportForAvi>()
                        .FromSqlInterpolated($"CALL Return_Open_DeliveryOrder_Report_withdate_avi({FromDate},{ToDate})")
                        .ToList();

            return results;
        }

        public async Task<IEnumerable<ReturnOpenDeliveryOrderSPResport>> ReturnOpenDeliveryOrderSPReportWithParam(string? ODONumber, string? CustomerName, string? CustomerAliasName, string? LeadId, string? IssuedTo, string? Location, string? Warehouse, string? KPN, string? MPN, string? ODOType)
        {
            var result = _tipsWarehouseDbContext
            .Set<ReturnOpenDeliveryOrderSPResport>()
            .FromSqlInterpolated($"CALL Return_Open_DeliveryOrder_Report_withparameters({ODONumber},{CustomerName},{CustomerAliasName},{LeadId},{IssuedTo},{KPN},{MPN},{Warehouse},{Location},{ODOType})")
            .ToList();

            return result;

        }

        public async Task<IEnumerable<ReturnOpenDeliveryOrderSPResportForTras>> ReturnOpenDeliveryOrderSPReportWithParamForTrans(string? ODONumber, string? CustomerName, string? CustomerAliasName, string? LeadId, string? IssuedTo, string? Location, string? Warehouse, string? KPN, string? MPN, string? ODOType, string? ProjectNumber)
        {
            var result = _tipsWarehouseDbContext
            .Set<ReturnOpenDeliveryOrderSPResportForTras>()
            .FromSqlInterpolated($"CALL Return_Open_DeliveryOrder_Report_withparameters_tras({ODONumber},{CustomerName},{CustomerAliasName},{LeadId},{IssuedTo},{KPN},{MPN},{Warehouse},{Location},{ODOType},{ProjectNumber})")
            .ToList();

            return result;

        }
        public async Task<IEnumerable<ReturnOpenDeliveryOrderSPResportForAvi>> ReturnOpenDeliveryOrderSPReportWithParamForAvi(string? ODONumber, string? CustomerName, string? CustomerAliasName, string? LeadId, string? IssuedTo, string? Location, string? Warehouse, string? KPN, string? MPN, string? ODOType, string? ProjectNumber)
        {
            var result = _tipsWarehouseDbContext
            .Set<ReturnOpenDeliveryOrderSPResportForAvi>()
            .FromSqlInterpolated($"CALL Return_Open_DeliveryOrder_Report_withparameters_avi({ODONumber},{CustomerName},{CustomerAliasName},{LeadId},{IssuedTo},{KPN},{MPN},{Warehouse},{Location},{ODOType},{ProjectNumber})")
            .ToList();

            return result;

        }
        public async Task<PagedList<ReturnOpenDeliveryOrderSPResport>> GetReturnOpenDeliveryOrderSPResport(PagingParameter pagingParameter)
        {
            var results = _tipsWarehouseDbContext.Set<ReturnOpenDeliveryOrderSPResport>()
                      .FromSqlInterpolated($"CALL Return_Open_DeliveryOrder_Report")
                      .ToList();

            return PagedList<ReturnOpenDeliveryOrderSPResport>.ToPagedList(results.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize);


        }
        public async Task<string> UpdateReturnOpenDeliveryOrder(ReturnOpenDeliveryOrder returnOpenDeliveryOrder)
        {
            returnOpenDeliveryOrder.LastModifiedBy = _createdBy;
            returnOpenDeliveryOrder.LastModifiedOn = DateTime.Now;
            Update(returnOpenDeliveryOrder);
            string result = $"ReturnOpenDeliveryOrder of Detail {returnOpenDeliveryOrder.Id} is updated successfully!";
            return result;
        }
        public async Task<IEnumerable<ReturnODONumberListDto>> GetReturnOpenDeliveryOrderNumberList()
        {

            IEnumerable<ReturnODONumberListDto> returnOpenDeliveryOrderNoList = await _tipsWarehouseDbContext.ReturnOpenDeliveryOrders
                                .Select(x => new ReturnODONumberListDto()
                                {
                                    ReturnODONumber = x.ODONumber

                                })
                              .ToListAsync();

            return returnOpenDeliveryOrderNoList;
        }
    }
}
