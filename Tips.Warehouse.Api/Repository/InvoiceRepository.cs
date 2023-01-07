using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Contracts;
using Entities;
using Entities.Helper;
using Entities.DTOs;


namespace Tips.Warehouse.Api.Repository
{
    public class InvoiceRepository : RepositoryBase<Invoice>, IInvoiceRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;



    public InvoiceRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
    {
            _tipsWarehouseDbContext = repositoryContext;
    }

    public async Task<long?> CreateInvoice(Invoice invoice)
        {
        
        invoice.CreatedBy = "Admin";
        invoice.CreatedOn = DateTime.Now;
        invoice.Unit = "Bangalore";
        var result = await Create(invoice);
        return result.Id;
        }

        public async Task<PagedList<Invoice>> GetAllInvoice(PagingParameter pagingParameter)
        {
            var allInvoiceList = PagedList<Invoice>.ToPagedList(FindAll()
                .Include(k => k.InvoiceChildItems)
                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return (allInvoiceList);

         }

        


        public async Task<Invoice> GetInvoiceById(int id)
        {
        var invoiceListById = await _tipsWarehouseDbContext.invoices
                        .Where(x => x.Id == id)
                        .Include(k => k.InvoiceChildItems)
                         .FirstOrDefaultAsync();
        return invoiceListById;
        }

        public async Task<string> UpdateInvoice(Invoice invoice)
        {
        invoice.LastModifiedBy = "Admin";
        invoice.LastModifiedOn = DateTime.Now;
        Update(invoice);
        string result = $"LeadTime details of {invoice.Id} is updated successfully!";
        return result;
        }

        public async Task<string> DeleteInvoice(Invoice invoice)
        {
            Delete(invoice);
            string result = $"invoice details of {invoice.Id} is deleted successfully!";
            return result;
        }

    }
}
