using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IReturnInvoiceRepository : IRepositoryBase<ReturnInvoice>
    {
        Task<PagedList<ReturnInvoice>> GetAllReturnInvoice(PagingParameter pagingParameter, SearchParams searchParams);

        Task<long?> CreateReturnInvoice(ReturnInvoice returnInvoice);
        Task<string> UpdateReturnInvoice(ReturnInvoice returnInvoice);
        Task<string> DeleteReturnInvoice(ReturnInvoice returnInvoice);

        Task<string> GetReturnInvoiceByInvoiceNo(string InvoiceNumber);
        Task<PagedList<ReturnInvoiceSPResport>> GetReturnInvoiceSPResport(PagingParameter pagingParameter);
        Task<IEnumerable<ReturnInvoiceSPResport>> ReturnInvoiceSPReportDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<ReturnInvoiceSPResport>> ReturnInvoiceSPReportWithParameter(string InvoiceNumber, string DoNumber, string CustomerName,string CustomerAliasName, string SalesOrderNumber, string Location, string Warehouse, string KPN, string MPN, string IssuedTo);
        Task<ReturnInvoice> GetReturnInvoiceById(int id);
        Task<IEnumerable<ReturnInvoiceNumberListDto>> GetReturnInvoiceNumberList();
    }
}
