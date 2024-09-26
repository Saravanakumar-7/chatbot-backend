using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IQuoteSpecialTermsRepository : IRepositoryBase<QuoteSpecialTerms>
    {
        Task<PagedList<QuoteSpecialTerms>> GetAllQuoteSpecialTerms(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<QuoteSpecialTerms> GetQuoteSpecialTermsById(int id);
        Task<IEnumerable<QuoteSpecialTerms>> GetAllActiveQuoteSpecialTerms();
        Task<long> CreateQuoteSpecialTerms(QuoteSpecialTerms quoteSpecialTerms);
        Task<string> UpdateQuoteSpecialTerms(QuoteSpecialTerms quoteSpecialTerms);
        Task<string> DeleteQuoteSpecialTerms(QuoteSpecialTerms quoteSpecialTerms);
    }
}
