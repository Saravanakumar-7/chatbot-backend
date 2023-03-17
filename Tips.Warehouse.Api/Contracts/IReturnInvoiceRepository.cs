using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IReturnInvoiceRepository : IRepositoryBase<ReturnInvoice>
    {
        Task<PagedList<ReturnInvoice>> GetAllReturnInvoice(PagingParameter pagingParameter, SearchParams searchParams);

        Task<long?> CreateReturnInvoice(ReturnInvoice returnInvoice);
        Task<string> UpdateReturnInvoice(ReturnInvoice returnInvoice);
        Task<string> DeleteReturnInvoice(ReturnInvoice returnInvoice);

        Task<int?> GetReturnInvoiceByInvoiceNo(string InvoiceNumber);


        Task<ReturnInvoice> GetReturnInvoiceById(int id);
    }
}
