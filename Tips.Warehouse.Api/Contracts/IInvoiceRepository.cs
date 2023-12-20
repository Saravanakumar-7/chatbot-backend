using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IInvoiceRepository : IRepositoryBase<Invoice>
    {
        Task<PagedList<Invoice>> GetAllInvoices(PagingParameter pagingParameter, SearchParams searchParams);
        Task<int?> GetInvoiceNumberAutoIncrementCount(DateTime date);
        Task<string> GenerateInvoiceNumber();
        Task<long?> CreateInvoice(Invoice invoice);
        Task<string> UpdateInvoice(Invoice invoice);
        Task<string> DeleteInvoice(Invoice invoice);
        Task<string> GenerateInvoiceNumberAvision();
        Task<Invoice> GetInvoiceById(int id);
        Task<IEnumerable<Invoice>> GetAllInvoiceWithItems(InvoiceSearchDto invoiceSearch);
        Task<IEnumerable<Invoice>> SearchInvoice([FromQuery] SearchParames searchParames);
        Task<IEnumerable<Invoice>> SearchInvoiceDate([FromQuery] SearchsDateParms searchsDateParms);
        Task<IEnumerable<InvoiceIdNameList>> GetAllInvoiceIdNameList();
        Task<IEnumerable<InvoiceSPReport>> InvoiceSPReport();
        Task<IEnumerable<InvoiceSPReport>> InvoiceSPReportDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<InvoiceSPReport>> InvoiceSPReportWithParameter(string InvoiceNumber, string DONumber, string LeadId, string CustomerName,
        string CustomerAliasName, string SalesOrderNumber, string Location, string Warehouse, string KPN, string MPN);

    }
}
