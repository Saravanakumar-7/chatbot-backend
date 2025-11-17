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
        Task<string> UpdateInvoiceFromReturnInvoice(Invoice invoice);
        Task<string> DeleteInvoice(Invoice invoice);
        Task<string> GenerateInvoiceNumberAvision();
        Task<Invoice> GetInvoiceById(int id);
        Task<IEnumerable<Invoice>> GetAllInvoiceWithItems(InvoiceSearchDto invoiceSearch);
        Task<IEnumerable<Invoice>> SearchInvoice([FromQuery] SearchParames searchParames);
        Task<IEnumerable<Invoice>> SearchInvoiceDate([FromQuery] SearchsDateParms searchsDateParms);
        Task<IEnumerable<InvoiceIdNameList>> GetAllInvoiceIdNameList();
        Task<PagedList<InvoiceSPReport>> InvoiceSPReport(PagingParameter pagingParameter);
        Task<IEnumerable<InvoiceSPReport>> InvoiceSPReportDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<InvoiceSPReportForTrans>> InvoiceSPReportDateForTrans(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<InvoiceSPReportForAvi>> InvoiceSPReportDateForAvi(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<SalesInvoiceSPReport>> SalesInvoiceSPReportDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<SalesInvoiceSPReport>> SalesInvoiceSPReportWithParameter(string? InvoiceNumber, string? CustomerId, string? CustomerName, string? FGItemNumber);
        Task<IEnumerable<InvoiceSPReport>> InvoiceSPReportWithParameter(string InvoiceNumber, string DONumber, string LeadId, string CustomerName,
                                                   string CustomerAliasName, string SalesOrderNumber, string Location, string Warehouse, string KPN, string MPN, string IssuedTo);
        Task<IEnumerable<InvoiceSPReportForTrans>> InvoiceSPReportWithParameterForTrans(string? InvoiceNumber, string? DONumber, string? CustomerId, string? CustomerName,
                                                                                                        string? SalesOrderNumber, string? Location,
                                                                                                       string? Warehouse, string? KPN, string? MPN, string? IssuedTo, string? ProjectNumber);
         Task<IEnumerable<InvoiceSpReportWithoutWorkOrderNoForTras>> GetInvoiceSpReportWithoutWorkOrderNoWithParamForTras(string? InvoiceNumber, string? DONumber, string? CustomerId, string? CustomerName,
                                                                                                    string? SalesOrderNumber, string? Location,
                                                                                                       string? Warehouse, string? KPN, string? MPN, string? IssuedTo, string? ProjectNumber);
        Task<IEnumerable<InvoiceSpReportWithoutWorkOrderNoForTras>> GetInvoiceSpReportWithoutWorkOrderNoWithInvoiceDateForTras(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<InvoiceAdditionalChargesSpReport>> InvoiceAdditionalChargesSPReportWithParameterForTrans(string? InvoiceNumber, string? SalesOrderNumber);
        Task<IEnumerable<InvoiceAdditionalChargesSpReport>> InvoiceAdditionalChargesSPReportWithDateForTrans(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<InvoiceSPReportForAvi>> InvoiceSPReportWithParameterForAvi(string? InvoiceNumber, string? DONumber, string? CustomerId, string? CustomerName,
                                                                                                string? SalesOrderNumber, string? Location,
                                                                                               string? Warehouse, string? KPN, string? MPN, string? IssuedTo, string? ProjectNumber);
        Task<Invoice> GetInvoiceByInvoiceNumber(string InvoiceNumber);
        Task<Invoice> GetInvoiceByIdExceptClosed(int id);
        //Task<IEnumerable<InvoiceConceptionDto>> GetInvoiceDetialsbyDate(DateTime? FromDate, DateTime? ToDate);
        Task<List<InvoiceBTODetailsDto>> GetInvoiceBTODetailsByDate(DateTime? FromDate, DateTime? ToDate);
        Task<List<InvoiceBTODetailsDto>> GetTGInvoiceBTODetailsByDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<SerialNoDetailDto>> GetSerialNoDetailByFGItemNoAndBTONo(string itemNumber, string doNumber);
    }
}
