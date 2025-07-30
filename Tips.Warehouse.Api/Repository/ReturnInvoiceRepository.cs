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
    public class ReturnInvoiceRepository : RepositoryBase<ReturnInvoice>, IReturnInvoiceRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ReturnInvoiceRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task<IEnumerable<ReturnInvoiceSPResport>> ReturnInvoiceSPReportWithParameter(string InvoiceNumber, string DoNumber, string CustomerName, string CustomerAliasName, string SalesOrderNumber, string Location, string Warehouse, string KPN, string MPN, string IssuedTo)
        {
            var result = _tipsWarehouseDbContext
            .Set<ReturnInvoiceSPResport>()
            .FromSqlInterpolated($"CALL Return_Invoice_Report_withParameter({InvoiceNumber},{DoNumber},{CustomerName},{CustomerAliasName},{SalesOrderNumber},{Location},{Warehouse},{KPN},{MPN},{IssuedTo})")
            .ToList();

            return result;
        }

        public async Task<IEnumerable<ReturnInvoiceSPResportForTras>> ReturnInvoiceSPReportWithParameterForTrans(string InvoiceNumber, string DoNumber, string CustomerName, 
                                                                                                                string CustomerAliasName, string SalesOrderNumber, 
                                                                                                                string Location, string Warehouse, string KPN, string MPN, 
                                                                                                                string IssuedTo, string ProjectNumber)
        {
            var result = _tipsWarehouseDbContext
            .Set<ReturnInvoiceSPResportForTras>()
            .FromSqlInterpolated($"CALL Return_Invoice_Report_withParameter_tras({InvoiceNumber},{DoNumber},{CustomerName},{CustomerAliasName},{SalesOrderNumber},{Location},{Warehouse},{KPN},{MPN},{IssuedTo},{ProjectNumber})")
            .ToList();

            return result;
        }

        public async Task<IEnumerable<ReturnInvoiceSPResport>> ReturnInvoiceSPReportDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsWarehouseDbContext.Set<ReturnInvoiceSPResport>()
                        .FromSqlInterpolated($"CALL Return_Invoice_Report_withdate({FromDate},{ToDate})")
                        .ToList();

            return results;
        }
        public async Task<IEnumerable<ReturnInvoiceSPResportForTras>> ReturnInvoiceSPReportDateForTras(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsWarehouseDbContext.Set<ReturnInvoiceSPResportForTras>()
                        .FromSqlInterpolated($"CALL Return_Invoice_Report_withdate_tras({FromDate},{ToDate})")
                        .ToList();

            return results;
        }
        public async Task<IEnumerable<ReturnInvoiceSPResportForAvi>> ReturnInvoiceSPReportDateForAvi(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsWarehouseDbContext.Set<ReturnInvoiceSPResportForAvi>()
                        .FromSqlInterpolated($"CALL Return_Invoice_Report_withdate_avi({FromDate},{ToDate})")
                        .ToList();

            return results;
        }
        public async Task<PagedList<ReturnInvoiceSPResport>> GetReturnInvoiceSPResport(PagingParameter pagingParameter)
        {
            var results = _tipsWarehouseDbContext.Set<ReturnInvoiceSPResport>()
                      .FromSqlInterpolated($"CALL Return_Invoice_Report")
                      .ToList();

            return PagedList<ReturnInvoiceSPResport>.ToPagedList(results.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize);


        }
        public async Task<long?> CreateReturnInvoice(ReturnInvoice returnInvoice)
        {
            returnInvoice.CreatedBy = _createdBy;
            returnInvoice.CreatedOn = DateTime.Now;
            returnInvoice.Unit = _unitname;
            var result = await Create(returnInvoice);
            return result.Id;
        }

        public async Task<string> DeleteReturnInvoice(ReturnInvoice returnInvoice)
        {
            Delete(returnInvoice);
            string result = $"returnInvoice details of {returnInvoice.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<ReturnInvoice>> GetAllReturnInvoice([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            var getAllReturnInvoiceList = FindAll()
                .OrderByDescending(x => x.Id)
                .Where(inv =>
                    (string.IsNullOrWhiteSpace(searchParams.SearchValue) ||
                    inv.InvoiceNumber.Contains(searchParams.SearchValue) ||
                    inv.CustomerAliasName.Contains(searchParams.SearchValue) ||
                    inv.CustomerName.Contains(searchParams.SearchValue) ||
                    inv.CustomerId.Contains(searchParams.SearchValue) ||
                    inv.CompanyName.Contains(searchParams.SearchValue) ||
                    inv.ReturnInvoiceItems.Any(item => item.DONumber.Contains(searchParams.SearchValue)))) // Include searching by DoNumber in ReturnInvoiceItems
                .Include(k => k.ReturnInvoiceItems);

            return PagedList<ReturnInvoice>.ToPagedList(getAllReturnInvoiceList, pagingParameter.PageNumber, pagingParameter.PageSize);
        }


        public async Task<string> GetReturnInvoiceByInvoiceNo(string InvoiceNumber)
        {
            //var getReturnInvoiceDetails = _tipsWarehouseDbContext.ReturnInvoices
            //        .Where(x => x.InvoiceNumber == InvoiceNumber).Count();

            var getReturnInvoiceDetails = await _tipsWarehouseDbContext.ReturnInvoices
                .Where(x => x.InvoiceNumber.StartsWith(InvoiceNumber)).OrderByDescending(x => x.Id)
                .Select(x => x.InvoiceNumber).FirstOrDefaultAsync();

            return getReturnInvoiceDetails;
        }




        public async Task<ReturnInvoice> GetReturnInvoiceById(int id)
        {
            var getReturnInvoiceListById = await _tipsWarehouseDbContext.ReturnInvoices
                      .Where(x => x.Id == id)
                      .Include(x=>x.ReturnInvoiceAdditionalCharges)
                      .Include(k => k.ReturnInvoiceItems).ThenInclude(x => x.QtyDistribution)
                       .FirstOrDefaultAsync();
            return getReturnInvoiceListById;
        }

        public async Task<string> UpdateReturnInvoice(ReturnInvoice returnInvoice)
        {
            returnInvoice.LastModifiedBy = _createdBy;
            returnInvoice.LastModifiedOn = DateTime.Now;
            Update(returnInvoice);
            string result = $"returnInvoice details of {returnInvoice.Id} is updated successfully!";
            return result;
        }
        public async Task<IEnumerable<ReturnInvoiceNumberListDto>> GetReturnInvoiceNumberList()
        {

            IEnumerable<ReturnInvoiceNumberListDto> returnInvoiceNumberList = await _tipsWarehouseDbContext.ReturnInvoices
                                .Select(x => new ReturnInvoiceNumberListDto()
                                {
                                    Id = x.Id,
                                    ReturnInvoiceNumber = x.InvoiceNumber

                                })
                              .ToListAsync();

            return returnInvoiceNumberList;
        }
    }
}
