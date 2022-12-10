using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IQuoteGeneralRepository : IRepositoryBase<QuoteGeneral>
    {
        Task<PagedList<QuoteGeneral>> GetAllQuoteGeneral(PagingParameter pagingParameter);
        Task<QuoteGeneral> GetQuoteGeneralById(int id);
        Task<IEnumerable<QuoteGeneral>> GetAllActiveQuoteGeneral();
        Task<long> CreateQuoteGeneral(QuoteGeneral quoteGeneral);
        Task<string> UpdateQuoteGeneral(QuoteGeneral quoteGeneral);
        Task<string> DeleteQuoteGeneral(QuoteGeneral quoteGeneral);
    }
}
