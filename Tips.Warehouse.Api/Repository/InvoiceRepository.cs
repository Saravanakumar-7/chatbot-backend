using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Contracts;
using Entities;
using Entities.Helper;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Tips.Warehouse.Api.Entities.DTOs;

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

        public async Task<string> GenerateInvoiceNumber()
        {
            using var transaction = await _tipsWarehouseDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var invoiceNumberEntity = await _tipsWarehouseDbContext.InvoiceNumbers.SingleAsync();
                invoiceNumberEntity.CurrentValue += 1;
                _tipsWarehouseDbContext.Update(invoiceNumberEntity);
                await _tipsWarehouseDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"IN-{invoiceNumberEntity.CurrentValue:D6}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task<PagedList<Invoice>> GetAllInvoices([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {

            var getAllInvoiceList = FindAll().OrderByDescending(x => x.Id)
               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.InvoiceNumber.Contains(searchParams.SearchValue) ||
                inv.CustomerAliasName.Contains(searchParams.SearchValue) || inv.CustomerName.Contains(searchParams.SearchValue)
                || inv.CompanyName.Contains(searchParams.SearchValue))))
                .Include(k => k.invoiceChildItems)
                .Include(p => p.InvoiceAdditionalCharges);

            return PagedList<Invoice>.ToPagedList(getAllInvoiceList, pagingParameter.PageNumber, pagingParameter.PageSize);

        }

        public async Task<IEnumerable<Invoice>> SearchInvoiceDate([FromQuery] SearchsDateParms searchsDateParms)
        {
            var invoiceDetails = _tipsWarehouseDbContext.invoices
            .Where(inv => ((inv.CreatedOn >= searchsDateParms.SearchFromDate &&
            inv.CreatedOn <= searchsDateParms.SearchToDate
            )))
            .Include(itm => itm.invoiceChildItems)
            .Include(ina => ina.InvoiceAdditionalCharges)
            .ToList();
            return invoiceDetails;
        }

        public async Task<IEnumerable<Invoice>> SearchInvoice([FromQuery] SearchParames searchParames)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.invoices.Include("invoiceChildItems");
                if (!string.IsNullOrEmpty(searchParames.SearchValue))
                {
                    query = query.Where(po => po.InvoiceNumber.Contains(searchParames.SearchValue)
                    || po.CustomerName.Contains(searchParames.SearchValue)
                    || po.CompanyName.Contains(searchParames.SearchValue)
                    || po.invoiceChildItems.Any(s => s.FGItemNumber.Contains(searchParames.SearchValue) ||
                    s.DONumber.Contains(searchParames.SearchValue)
                    || s.Description.Contains(searchParames.SearchValue)))
                        .Include(itm => itm.invoiceChildItems)
            .Include(ina => ina.InvoiceAdditionalCharges);
                }
                return query.ToList();
            }
        }
        public async Task<IEnumerable<Invoice>> GetAllInvoiceWithItems(InvoiceSearchDto invoiceSearch)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.invoices.Include("invoiceChildItems");
                if (invoiceSearch != null || (invoiceSearch.InvoiceNumber.Any())
               && invoiceSearch.CustomerName.Any() && invoiceSearch.CompanyName.Any())
                {
                    query = query.Where
                    (po => (invoiceSearch.CustomerName.Any() ? invoiceSearch.CustomerName.Contains(po.CustomerName) : true)
                   && (invoiceSearch.InvoiceNumber.Any() ? invoiceSearch.InvoiceNumber.Contains(po.InvoiceNumber) : true)
                   && (invoiceSearch.CompanyName.Any() ? invoiceSearch.CompanyName.Contains(po.CompanyName) : true))
                    .Include(itm => itm.invoiceChildItems)
            .Include(ina => ina.InvoiceAdditionalCharges);
                }
                return query.ToList();
            }
        }
        public async Task<Invoice> GetInvoiceById(int id)
        {
        var getInvoiceListById = await _tipsWarehouseDbContext.invoices
                        .Where(x => x.Id == id)
                        .Include(o => o.InvoiceAdditionalCharges)
                        .Include(k => k.invoiceChildItems)
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

        public async Task<IEnumerable<InvoiceIdNameList>> GetAllInvoiceIdNameList()
        {
            IEnumerable<InvoiceIdNameList> invoiceIdNameList = await _tipsWarehouseDbContext.invoices
                                .Select(x => new InvoiceIdNameList()
                                {
                                    Id = x.Id,

                                    InvoiceNumber = x.InvoiceNumber,
                                    CustomerName = x.CustomerName

                                })
                                .OrderByDescending(x => x.Id)
                              .ToListAsync();

            return invoiceIdNameList;
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
            var getInvoiceChildItemDetails = await _tipsWarehouseDbContext.invoiceChildItems
                    .Where(x => x.Id == invoiceChildId)
                          .FirstOrDefaultAsync();
            return getInvoiceChildItemDetails;
        }
    }
    }
