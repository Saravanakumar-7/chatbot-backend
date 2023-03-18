using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IQuoteAdditionalChargesRepository : IRepositoryBase<QuoteAdditionalCharges>
    {
        Task<PagedList<QuoteAdditionalCharges>> GetAllQuoteAdditionalCharges(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<QuoteAdditionalCharges> GetQuoteAdditionalChargesById(int id);
        Task<IEnumerable<QuoteAdditionalCharges>> GetAllActiveQuoteAdditionalCharges();
        Task<long> CreateQuoteAdditionalCharges(QuoteAdditionalCharges quoteAdditionalCharges);
        Task<string> UpdateQuoteAdditionalCharges(QuoteAdditionalCharges quoteAdditionalCharges);
        Task<string> DeleteQuoteAdditionalCharges(QuoteAdditionalCharges quoteAdditionalCharges);
    }
}
