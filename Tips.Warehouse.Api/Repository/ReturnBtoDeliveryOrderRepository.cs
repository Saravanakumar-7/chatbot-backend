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
    public class ReturnBtoDeliveryOrderRepository : RepositoryBase<ReturnBtoDeliveryOrder>, IReturnBtoDeliveryOrderRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ReturnBtoDeliveryOrderRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
            var _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateReturnBtoDeliveryOrder(ReturnBtoDeliveryOrder returnBtoDeliveryOrder)
        {

            returnBtoDeliveryOrder.CreatedBy = _createdBy;
            returnBtoDeliveryOrder.CreatedOn = DateTime.Now;
            returnBtoDeliveryOrder.Unit = _unitname;
            var result = await Create(returnBtoDeliveryOrder);
            return result.Id;
        }
        public async Task<IEnumerable<ReturnDOSPReport>> ReturnDeliveryOrderSPReportDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsWarehouseDbContext.Set<ReturnDOSPReport>()
                     .FromSqlInterpolated($"CALL Return_DeliveryOrder_Report_withDate({FromDate},{ToDate})")
                     .ToList();

            return results;
        }
        public async Task<IEnumerable<ReturnDOSPReport>> ReturnDOSPReportWithParam(string? DoNumber, string? CustomerName, string? CustomerAliasName, string? LeadId, string? SalesOrderNumber, string? ProductType, string? TypeOfSolution, string? Warehouse, string? Location, string? KPN, string? MPN)
        {
            var result = _tipsWarehouseDbContext
            .Set<ReturnDOSPReport>()
            .FromSqlInterpolated($"CALL Return_DeliveryOrder_Report_withparameter({DoNumber},{CustomerName},{CustomerAliasName},{LeadId},{SalesOrderNumber},{ProductType},{TypeOfSolution},{Warehouse},{Location},{KPN},{MPN})")
            .ToList();

            return result;
        }
        public async Task<PagedList<ReturnDOSPReport>> ReturnDeliveryOrderSPReport(PagingParameter pagingParameter)
        {

            var results = _tipsWarehouseDbContext.Set<ReturnDOSPReport>()
                     .FromSqlInterpolated($"CALL Return_DeliveryOrder_Report")
                     .ToList();

            return PagedList<ReturnDOSPReport>.ToPagedList(results.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize);

        }
        public async Task<PagedList<ReturnBtoDeliveryOrder>> GetAllReturnBtoDeliveryOrderDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {


            var getAllReturnBTODetails = FindAll().OrderByDescending(x => x.Id)
               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ReturnBTONumber.Contains(searchParams.SearchValue) ||
                inv.CustomerAliasName.Contains(searchParams.SearchValue) || inv.CustomerName.Contains(searchParams.SearchValue) || inv.PONumber.Contains(searchParams.SearchValue))))
                .Include(t => t.ReturnBtoDeliveryOrderItems);
            return PagedList<ReturnBtoDeliveryOrder>.ToPagedList(getAllReturnBTODetails, pagingParameter.PageNumber, pagingParameter.PageSize);

        }

        public async Task<string> DeleteReturnBtoDeliveryOrder(ReturnBtoDeliveryOrder returnBtoDeliveryOrder)
        {
            Delete(returnBtoDeliveryOrder);
            string result = $"DeleteReturnBtoDeliveryOrder details of {returnBtoDeliveryOrder.Id} is deleted successfully!";
            return result;
        }

        public async Task<ReturnBtoDeliveryOrder> GetReturnBtoDeliveryOrderById(int id)
        {
            var getReturnBtoDeliveryOrderById = await _tipsWarehouseDbContext.ReturnBtoDeliveryOrders.Where(x => x.Id == id)
                                .Include(t => t.ReturnBtoDeliveryOrderItems).ThenInclude(x => x.QtyDistribution)
                                .FirstOrDefaultAsync();


            return getReturnBtoDeliveryOrderById;
        }
        public async Task<int?> GetReturnBtoDeliveryOrderByBtoNo(string BTONumber)
        {
            var getReturnBtoDeliveryOrderByBtoNo = _tipsWarehouseDbContext.ReturnBtoDeliveryOrders
                    .Where(x => x.BTONumber == BTONumber).Count();
            return getReturnBtoDeliveryOrderByBtoNo;
        }

        public async Task<string> UpdateReturnBtoDeliveryOrder(ReturnBtoDeliveryOrder returnBtoDeliveryOrder)
        {

            returnBtoDeliveryOrder.LastModifiedBy = _createdBy;
            returnBtoDeliveryOrder.LastModifiedOn = DateTime.Now;
            Update(returnBtoDeliveryOrder);
            string result = $"returnBtoDeliveryOrder of Detail {returnBtoDeliveryOrder.Id} is updated successfully!";
            return result;
        }
    }
}
