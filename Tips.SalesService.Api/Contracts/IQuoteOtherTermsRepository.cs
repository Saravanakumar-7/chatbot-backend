using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IQuoteOtherTermsRepository : IRepositoryBase<QuoteOtherTerms>
    {
        Task<PagedList<QuoteOtherTerms>> GetAllQuoteOtherTerms(PagingParameter pagingParameter);
        Task<QuoteOtherTerms> GetQuoteOtherTermsById(int id);
        Task<IEnumerable<QuoteOtherTerms>> GetAllActiveQuoteOtherTerms();
        Task<long> CreateQuoteOtherTerms(QuoteOtherTerms quoteOtherTerms);
        Task<string> UpdateQuoteOtherTerms(QuoteOtherTerms quoteOtherTerms);
        Task<string> DeleteQuoteOtherTerms(QuoteOtherTerms quoteOtherTerms);

    }
}
