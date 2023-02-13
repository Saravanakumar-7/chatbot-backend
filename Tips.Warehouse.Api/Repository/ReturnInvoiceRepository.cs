using Entities;
using Entities.Helper;
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

        public async Task<PagedList<ReturnInvoice>> GetAllReturnInvoice(PagingParameter pagingParameter)
        {
            var getAllReturnInvoiceList = PagedList<ReturnInvoice>.ToPagedList(FindAll()
               .Include(k => k.ReturnInvoiceItems)
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return (getAllReturnInvoiceList);
        }

        public async Task<int?> GetReturnInvoiceByInvoiceNo(string InvoiceNumber)
        {
            var getReturnInvoiceDetails = _tipsWarehouseDbContext.ReturnInvoices
                    .Where(x => x.InvoiceNumber == InvoiceNumber).Count();
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
