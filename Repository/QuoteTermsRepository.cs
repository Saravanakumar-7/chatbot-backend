using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class QuoteTermsRepository : RepositoryBase<QuoteTerms>, IQuoteTermsRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public QuoteTermsRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateQuoteTerms(QuoteTerms quoteTerms)
        {
            quoteTerms.CreatedBy = _createdBy;
            quoteTerms.CreatedOn = DateTime.Now;
            quoteTerms.Unit = _unitname;
            var result = await Create(quoteTerms);

            return result.Id;
        }

        public async Task<string> DeleteQuoteTerms(QuoteTerms quoteTerms)
        {
            Delete(quoteTerms);
            string result = $"QuoteTerms details of {quoteTerms.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<QuoteTerms>> GetAllActiveQuoteTerms([FromQuery] SearchParames searchParams)
        {
            var quoteTermDetails = FindAll()
                                       .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.QuoteTermsName.Contains(searchParams.SearchValue) ||
                                 inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return quoteTermDetails;
        }

        public async Task<IEnumerable<QuoteTerms>> GetAllQuoteTerms([FromQuery] SearchParames searchParams)
        {
            var quoteTermDetails = FindAll().OrderByDescending(x => x.Id)
                                      .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.QuoteTermsName.Contains(searchParams.SearchValue) ||
                                inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return quoteTermDetails;
        }

        public async Task<QuoteTerms> GetQuoteTermsById(int id)
        {
            var QuoteTermsbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return QuoteTermsbyId;
        }

        public async Task<string> UpdateQuoteTerms(QuoteTerms quoteTerms)
        {
            quoteTerms.LastModifiedBy = _createdBy;
            quoteTerms.LastModifiedOn = DateTime.Now;
            Update(quoteTerms);
            string result = $"QuoteTerms details of {quoteTerms.Id} is updated successfully!";
            return result;
        }
    }
}
