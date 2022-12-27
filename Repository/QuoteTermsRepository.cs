using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class QuoteTermsRepository : RepositoryBase<QuoteTerms>, IQuoteTermsRepository
    {
        public QuoteTermsRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateQuoteTerms(QuoteTerms quoteTerms)
        {
            quoteTerms.CreatedBy = "Admin";
            quoteTerms.CreatedOn = DateTime.Now;
            var result = await Create(quoteTerms);
            quoteTerms.Unit = "Bangalore";
            return result.Id;
        }

        public async Task<string> DeleteQuoteTerms(QuoteTerms quoteTerms)
        {
            Delete(quoteTerms);
            string result = $"QuoteTerms details of {quoteTerms.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<QuoteTerms>> GetAllActiveQuoteTerms()
        {
            var quoteTermsList = await FindByCondition(x => x.ActiveStatus == true).ToListAsync();
            return quoteTermsList;
        }

        public async Task<IEnumerable<QuoteTerms>> GetAllQuoteTerms()
        {
            var quoteTermsList = await FindAll().ToListAsync();
            return quoteTermsList;
        }

        public async Task<QuoteTerms> GetQuoteTermsById(int id)
        {
            var quoteTermsList = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return quoteTermsList;
        }

        public async Task<string> UpdateQuoteTerms(QuoteTerms quoteTerms)
        {
            quoteTerms.LastModifiedBy = "Admin";
            quoteTerms.LastModifiedOn = DateTime.Now;
            Update(quoteTerms);
            string result = $"QuoteTerms details of {quoteTerms.Id} is updated successfully!";
            return result;
        }
    }
}
