using Contracts;
using Entities;
using Entities.Helper;
using Tips.Warehouse.Api.Entities; 

namespace Tips.Warehouse.Api.Contracts
{
    public interface IInvoiceChildRepository : IRepositoryBase<InvoiceChildItem>
    {
        Task<InvoiceChildItem> GetInvoiceChildItemDetails(int invoiceChildId);

    }
}


 
