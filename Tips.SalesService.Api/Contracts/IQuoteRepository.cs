using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IQuoteRepository : IRepositoryBase<Quote>
    {
        Task<PagedList<Quote>> GetAllQuote(PagingParameter pagingParameter);
        Task<Quote> GetQuoteById(int id);
        Task<IEnumerable<Quote>> GetAllActiveQuote();
        Task<long> CreateQuote(Quote quote);
        Task<string> UpdateQuote(Quote quote);
        Task<string> DeleteQuote(Quote quote);
    }
}
