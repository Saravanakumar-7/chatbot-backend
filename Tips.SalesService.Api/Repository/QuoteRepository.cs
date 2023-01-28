using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Org.BouncyCastle.Ocsp;
using System.Linq;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

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

        public async Task<Quote> ChangeQuoteVersion(Quote quote)
        {
            quote.CreatedBy = "Admin";
            quote.CreatedOn = DateTime.Now;            
            quote.Unit = "Bangalore";
            var getIdByRfqNumber = _tipsSalesServiceDbContext.Quotes
                .Where(x => x.RFQNumber == quote.RFQNumber)
                .OrderByDescending(x => x.Id)
                .Select(x => x.Id)
                .FirstOrDefault();
            var getOldRevisionNumber = _tipsSalesServiceDbContext.Quotes
                .Where(x => x.Id == getIdByRfqNumber)
                .Select(x => x.RevisionNumber)
                .FirstOrDefault();
            var increaseVersionNumber = 0.1;
            var convertversionnumber = Convert.ToDecimal(increaseVersionNumber);
            var version = getOldRevisionNumber + convertversionnumber;
            quote.RevisionNumber = Convert.ToDecimal(version);
            var result = await Create(quote);
            return result;
        } 
        //public async Task<Quote> GetVendorMasterById(int id)
        //{
        //    var getVendorMasterbyId = await _tipsSalesServiceDbContext.quotes.Where(x => x.Id == id)
        //                        .Include(x => x.quoteGenerals)
        //                        .Include(x => x.quoteAdditionalCharges)
        //                        .Include(m => m.quoteRFQNotes)
        //                        .Include(v => v.quoteOtherTerms)
        //                        .Include(v => v.quoteSpecialTerms)

        //                        .FirstOrDefaultAsync();

        //    return getVendorMasterbyId;

        //}


        public async Task<string> DeleteQuote(Quote quote)
        {
            Delete(quote);
            string result = $"Quote details of {quote.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Quote>> GetAllActiveQuote()
        {
            var quoteDetails = await FindAll().OrderByDescending(x => x.Id).ToListAsync();
            return quoteDetails;
        }

        public async Task<IEnumerable<CsItemDetailsForQuoteDto>> GetCsItemDetailsForQuote(string rfqNumber)
        {

            var rfqDetail = await _tipsSalesServiceDbContext.Rfqs
                .Where(x => x.RfqNumber == rfqNumber)
                .FirstOrDefaultAsync();

            //var itemPriceLists = await _tipsSalesServiceDbContext.ItemPriceLists
            //    .GroupBy(x => x.ItemNumber).Select(g => g.OrderByDescending(x => x.CreatedOn).First())
            //    .ToListAsync();
             
            var customersAndOrders = _tipsSalesServiceDbContext.RfqCustomerSupportItems
                     .Where(c => c.RfqNumber == rfqNumber)
                     .Join(_tipsSalesServiceDbContext.ItemPriceLists,                     
                     c => c.ItemNumber,
                     o => o.ItemNumber,
                     (c, o) => new { RfqCustomerSupportItems = c, ItemPriceLists = o })
                     .Select(co => new CsItemDetailsForQuoteDto
                     {
                         RFQNumber = co.RfqCustomerSupportItems.RfqNumber,
                         CustomerName = rfqDetail.CustomerName,
                         CustomerId = rfqDetail.CustomerId,
                         ItemNumber = co.RfqCustomerSupportItems.ItemNumber,
                         Description = co.RfqCustomerSupportItems.Description,
                         PriceListName = co.ItemPriceLists.PriceListName,
                         Qty = co.RfqCustomerSupportItems.Qty,
                         UnitPrice = co.ItemPriceLists.LeastCost,
                         LeastCostPlus = co.ItemPriceLists.LeastCostPlus,
                         LeastCostminus = co.ItemPriceLists.LeastCostminus,
                         DiscountMinus = co.ItemPriceLists.DiscountMinus,
                         DiscountPlus = co.ItemPriceLists.DiscountPlus,
                         Markup = co.ItemPriceLists.Markup,
                         CreatedOn = co.ItemPriceLists.CreatedOn,
                         IsDiscountApplicable = co.ItemPriceLists.IsDiscountApplicable
                     }).ToList();

            return customersAndOrders;
        }
        


        public async Task<PagedList<Quote>> GetAllQuote(PagingParameter pagingParameter)
        {
            var quoteDetails = PagedList<Quote>.ToPagedList(FindAll()
                               .Include(t => t.QuoteGenerals)
                               .Include(x => x.QuoteAdditionalCharges)
                               .Include(m => m.QuoteOtherTerms)
                               .Include(i => i.QuoteRFQNotes)
                               .Include(i => i.QuoteSpecialTerms)

              .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return quoteDetails;
        }


        public async Task<Quote> GetQuoteById(int id)
        {
            var quoteDetails = await _tipsSalesServiceDbContext.Quotes.Where(x => x.Id == id)
                               .Include(t => t.QuoteGenerals)
                               .Include(x => x.QuoteAdditionalCharges)
                               .Include(m => m.QuoteOtherTerms)
                               .Include(i => i.QuoteRFQNotes)
                               .Include(i => i.QuoteSpecialTerms)
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
