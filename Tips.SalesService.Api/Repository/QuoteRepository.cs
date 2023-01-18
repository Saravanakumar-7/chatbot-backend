using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Repository
{
    public class QuoteRepository : RepositoryBase<Quote>, IQuoteRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public QuoteRepository(TipsSalesServiceDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsSalesServiceDbContext = repositoryContext;
        }

        public async Task<long> CreateQuote(Quote quote)
        {
            quote.CreatedBy = "Admin";
            quote.CreatedOn = DateTime.Now;
            quote.Unit = "Bangalore";
            var version = 1.0;
            quote.RevisionNumber = Convert.ToDecimal(version);
            var result = await Create(quote);
            return result.Id;
        }
         
        public async Task<string> DeleteQuote(Quote quote)
        {
            Delete(quote);
            string result = $"Quote details of {quote.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Quote>> GetAllActiveQuote()
        {
            var quoteDetails = await FindAll().ToListAsync();
            return quoteDetails;
        }

        public async Task<PagedList<Quote>> GetAllQuote(PagingParameter pagingParameter)
        {
            var quoteDetails = PagedList<Quote>.ToPagedList(FindAll()
                               .Include(t => t.quoteGenerals)
                               .Include(x => x.quoteAdditionalCharges)
                               .Include(m => m.quoteOtherTerms)
                               .Include(i => i.quoteRFQNotes)
                               .Include(i => i.quoteSpecialTerms)

              .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return quoteDetails;
        }

        public async Task<Quote> GetQuoteById(int id)
        {
            var quoteDetails = await _tipsSalesServiceDbContext.quotes.Where(x => x.Id == id)
                               .Include(t => t.quoteGenerals)
                               .Include(x => x.quoteAdditionalCharges)
                               .Include(m => m.quoteOtherTerms)
                               .Include(i => i.quoteRFQNotes)
                               .Include(i => i.quoteSpecialTerms)
                               .FirstOrDefaultAsync();

            return quoteDetails;
        }

        public async Task<string> UpdateQuote(Quote quote)
        {
            quote.LastModifiedBy = "Admin";
            quote.LastModifiedOn = DateTime.Now;
            Update(quote);
            string result = $"Quote of Detail {quote.Id} is updated successfully!";
            return result;
        }
    }
}
