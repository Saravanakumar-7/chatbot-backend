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
            var date = DateTime.Now;
            quote.CreatedBy = "Admin";
            quote.CreatedOn = date.Date;
            quote.Unit = "Bangalore";
            var version = 1;
            quote.RevisionNumber = Convert.ToDecimal(version);
            var result = await Create(quote);
            return result.Id;
        }

        public async Task<int?> GetQuoteNumberAutoIncrementCount(DateTime date)
        {
            var getQuoteNumberAutoIncrementCount = _tipsSalesServiceDbContext.Quotes.Where(x => x.CreatedOn == date.Date).Count();

            return getQuoteNumberAutoIncrementCount;
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
            var increaseVersionNumber = 1;
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

            var rfqCDetail = await _tipsSalesServiceDbContext.RfqCustomerSupports
               .Where(x => x.RfqNumber == rfqNumber)
               .FirstOrDefaultAsync();


            var leftOuterJoin = from e in _tipsSalesServiceDbContext.RfqCustomerSupportItems
                                where e.RfqNumber == rfqNumber && e.ReleaseStatus == true
                                join d in _tipsSalesServiceDbContext.ItemPriceLists on e.ItemNumber equals d.ItemNumber into dept
                                from ItemPriceList in dept.DefaultIfEmpty()
                                select new CsItemDetailsForQuoteDto
                                {
                                    RFQNumber = e.RfqNumber,
                                    CustomerName = rfqDetail.CustomerName,
                                    CustomerAliasName = rfqDetail.CustomerAliasName,
                                    RoomName=e.RoomName,
                                    CustomerId = rfqDetail.CustomerId,
                                    ItemNumber = e.ItemNumber,
                                    Description = e.Description,
                                    CustomFields = e.CustomFields,
                                    PriceListName = ItemPriceList.PriceListName,
                                    Qty = e.Qty,
                                    UnitPrice = ItemPriceList.LeastCost,
                                    LeastCostPlus = ItemPriceList.LeastCostPlus,
                                    LeastCostminus =ItemPriceList.LeastCostminus,
                                    DiscountMinus = ItemPriceList.DiscountMinus,
                                    DiscountPlus = ItemPriceList.DiscountPlus,
                                    Markup = ItemPriceList.Markup,
                                    CreatedOn = ItemPriceList.CreatedOn,
                                    IsDiscountApplicable = ItemPriceList.IsDiscountApplicable
                                };

            var postdata = leftOuterJoin.ToList(); 

            return postdata;
        }
        


        public async Task<PagedList<Quote>> GetAllQuote(PagingParameter pagingParameter)
        {
            var quoteDetails = PagedList<Quote>.ToPagedList(FindAll()
                               .Include(t => t.QuoteGenerals)
                               .Include(x => x.QuoteAdditionalCharges)
                               .Include(m => m.QuoteOtherTerms)
                               .Include(i => i.QuoteRFQNotes)
                               .Include(i => i.QuoteSpecialTerms)

              .OrderByDescending(x => x.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

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
