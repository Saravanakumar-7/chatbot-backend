using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Repository;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IReturnInvoiceItemRepository 
    {
        Task<PagedList<ReturnInvoiceItem>> GetAllReturnInvoiceItem(PagingParameter pagingParameter);

        Task<long?> CreateReturnInvoiceItem(ReturnInvoiceItem returnInvoiceItem);
        Task<string> UpdateReturnInvoiceItem(ReturnInvoiceItem returnInvoiceItem);
        Task<string> DeleteReturnInvoiceItem(ReturnInvoiceItem returnInvoiceItem);

        Task<ReturnInvoiceItem> GetReturnInvoiceItemById(int id);
    }
}
