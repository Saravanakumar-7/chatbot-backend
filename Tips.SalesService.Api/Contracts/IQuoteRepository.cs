using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface IQuoteRepository : IRepositoryBase<Quote>
    {
        Task<PagedList<Quote>> GetAllQuote(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<Quote> GetQuoteById(int id);
        Task<string> GenerateQuoteNumber();
         
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
    }
}
