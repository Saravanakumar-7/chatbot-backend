using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IQuoteRFQNotesRepository : IRepositoryBase<QuoteRFQNotes>
    {
        Task<PagedList<QuoteRFQNotes>> GetAllQuoteRFQNotes(PagingParameter pagingParameter);
        Task<QuoteRFQNotes> GetQuoteRFQNotesById(int id);
        Task<IEnumerable<QuoteRFQNotes>> GetAllActiveQuoteRFQNotes();
        Task<long> CreateQuoteRFQNotes(QuoteRFQNotes quoteRFQNotes);
        Task<string> UpdateQuoteRFQNotes(QuoteRFQNotes quoteRFQNotes);
        Task<string> DeleteQuoteRFQNotes(QuoteRFQNotes quoteRFQNotes);
    }
}
