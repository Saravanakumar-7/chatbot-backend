using Contracts;
using Entities;
using Entities.Helper;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IInvoiceRepository : IRepositoryBase<Invoice>
    {
        Task<PagedList<Invoice>> GetAllInvoices(PagingParameter pagingParameter, SearchParams searchParams);
        Task<int?> GetInvoiceNumberAutoIncrementCount(DateTime date);

        Task<long?> CreateInvoice(Invoice invoice);
        Task<string> UpdateInvoice(Invoice invoice);
        Task<string> DeleteInvoice(Invoice invoice);

        Task<Invoice> GetInvoiceById(int id);

        Task<IEnumerable<InvoiceIdNameList>> GetAllInvoiceIdNameList();


    }
}
