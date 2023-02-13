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
            var date = DateTime.Now;
        invoice.CreatedBy = "Admin";
        invoice.CreatedOn = date.Date;
        //Guid invoiceNumber = Guid.NewGuid();
        //invoice.InvoiceNo = " IN-" + invoiceNumber.ToString();
        invoice.Unit = "Bangalore";
        var result = await Create(invoice);
        return result.Id;
        }
        public async Task<int?> GetInvoiceNumberAutoIncrementCount(DateTime date)
        {
            var getInvoiceNumberAutoIncrementCount = _tipsWarehouseDbContext.invoices.Where(x => x.CreatedOn == date.Date).Count();

            return getInvoiceNumberAutoIncrementCount;
        }

        public async Task<PagedList<Invoice>> GetAllInvoices(PagingParameter pagingParameter)
        {
            var getAllInvoiceList = PagedList<Invoice>.ToPagedList(FindAll()
                .Include(k => k.InvoiceChildItems)
                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return (getAllInvoiceList);

         }


        

        public async Task<Invoice> GetInvoiceById(int id)
        {
        var getInvoiceListById = await _tipsWarehouseDbContext.invoices
                        .Where(x => x.Id == id)
                        .Include(k => k.InvoiceChildItems)
                         .FirstOrDefaultAsync();
        return getInvoiceListById;
        }

        public async Task<string> UpdateInvoice(Invoice invoice)
        {
        invoice.LastModifiedBy = "Admin";
        invoice.LastModifiedOn = DateTime.Now;
        Update(invoice);
        string result = $"Invoice details of {invoice.Id} is updated successfully!";
        return result;
        }

        public async Task<string> DeleteInvoice(Invoice invoice)
        {
            Delete(invoice);
            string result = $"Invoice details of {invoice.Id} is deleted successfully!";
            return result;
        }

    }

    public class InvoiceChildRepository : RepositoryBase<InvoiceChildItem>, IInvoiceChildRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContexts;

        public InvoiceChildRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
        }
        public async Task<InvoiceChildItem> GetInvoiceChildItemDetails(int invoiceChildId)
        {
            var getInvoiceChildItemDetails = await _tipsWarehouseDbContexts.invoiceChildItems
                    .Where(x => x.Id == invoiceChildId)
                          .FirstOrDefaultAsync();
            return getInvoiceChildItemDetails;
        }
    }
    }
