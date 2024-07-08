using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Tips.SalesService.Api.Contracts
{
    public interface IQuoteRepository : IRepositoryBase<Quote>
    {
        Task<PagedList<Quote>> GetAllQuote(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<Quote> GetQuoteById(int id);
        Task<string> GenerateQuoteNumber();
        Task<IEnumerable<QuoteNoDto>> GetAllQuoteNumberList();
        Task<List<QuoteforKeusDto>> GetAllQuoteforKeus([FromQuery] string? SearchTerm, [FromQuery] int Offset, [FromQuery] int Limit);
        Task<int> GetAllQuoteCountforKeus(string? SearchTerm);
        Task CreateShortClosed(ShortClosedDto shortClosedDto);
        Task<IEnumerable<rfqEnggItemDetailsForQuoteDto>> GetAllRfqEnggDetailsByRfqNo(string rfqNumber);

        Task<int?> GetQuoteNumberAutoIncrementCount(DateTime date);
        Task<IEnumerable<CsItemDetailsForQuoteDto>> GetCsItemDetailsForQuote(string rfqNumber, List<string> latestPriceListName);

        Task<PagedList<Quote>> GetAllActiveQuote(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<long> CreateQuote(Quote quote);       
        Task<Quote> ChangeQuoteVersion(Quote quote);
        Task<string> GenerateQuoteNumberAvision();
         Task<string> UpdateQuote(Quote quote);
        Task<string> DeleteQuote(Quote quote);
        Task<Quote> GetQuoteByQuoteNumber(string quoteNumber);
        Task<IEnumerable<QuoteSPReport>> GetQuoteSPReport(string CustomerName, string CustomerId, string RfqNumber);
        Task<IEnumerable<QuotationSPReport>> GetQuotationSPReportWithParam(string CustomerId);
        Task<IEnumerable<SoSummaryQuotationDto>> GetSoSummaryQuotationSPReportWithParam(string FirstQuotenumber, string SOlatestSalesorder);
        Task<IEnumerable<SoSummaryQuotationDto>> GetSoSummaryQuotationSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<QuotationSPReport>> GetQuotationSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<SoSummaryQuotationDto>> GetSoSummaryQuotationSPReport();
    }
}
