using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Repository
{
    public class ReturnInvoiceRepository : RepositoryBase<ReturnInvoice>, IReturnInvoiceRepository
    {
        public ReturnInvoiceRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
        }

        public async Task<long?> CreateReturnInvoice(ReturnInvoice returnInvoice)
        {
            returnInvoice.CreatedBy = "Admin";
            returnInvoice.CreatedOn = DateTime.Now;
            returnInvoice.Unit = "Bangalore";
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
            var getAllReturnInvoiceList = FindAll().OrderByDescending(x => x.Id)
               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.InvoiceNumber.Contains(searchParams.SearchValue) ||
                inv.CustomerAliasName.Contains(searchParams.SearchValue) || inv.CustomerName.Contains(searchParams.SearchValue)
                || inv.CompanyName.Contains(searchParams.SearchValue))))
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
                      .Include(k => k.ReturnInvoiceItems)
                       .FirstOrDefaultAsync();
            return getReturnInvoiceListById;
        }

        public async Task<string> UpdateReturnInvoice(ReturnInvoice returnInvoice)
        {
            returnInvoice.LastModifiedBy = "Admin";
            returnInvoice.LastModifiedOn = DateTime.Now;
            Update(returnInvoice);
            string result = $"returnInvoice details of {returnInvoice.Id} is updated successfully!";
            return result;
        }
    }
}
