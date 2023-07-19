using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IQuoteTermsRepository : IRepositoryBase<QuoteTerms>
    {
        Task<IEnumerable<QuoteTerms>> GetAllQuoteTerms(SearchParames searchParams);
        Task<QuoteTerms> GetQuoteTermsById(int id);
        Task<IEnumerable<QuoteTerms>> GetAllActiveQuoteTerms(SearchParames searchParams);
        Task<int?> CreateQuoteTerms(QuoteTerms quoteTerms);
        Task<string> UpdateQuoteTerms(QuoteTerms quoteTerms);
        Task<string> DeleteQuoteTerms(QuoteTerms quoteTerms);
    }

}
