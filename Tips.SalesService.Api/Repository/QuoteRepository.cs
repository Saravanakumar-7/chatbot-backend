using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<string> GenerateQuoteNumber()
        {
            using var transaction = await _tipsSalesServiceDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var quoteNumberEntity = await _tipsSalesServiceDbContext.QuoteNumbers.SingleAsync();
                quoteNumberEntity.CurrentValue += 1;
                _tipsSalesServiceDbContext.Update(quoteNumberEntity);
                await _tipsSalesServiceDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"Q-{quoteNumberEntity.CurrentValue:D6}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
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

        public async Task<PagedList<Quote>> GetAllActiveQuote([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var activeQuote = FindAll()
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.QuoteNumber.Contains(searchParammes.SearchValue)
            || inv.RFQNumber.Contains(searchParammes.SearchValue)
            || inv.RevisionNumber.Equals(int.Parse(searchParammes.SearchValue))
            || inv.TotalAmount.Equals(int.Parse(searchParammes.SearchValue))
            || inv.CustomerName.Contains(searchParammes.SearchValue))))
                                .Include(t => t.QuoteGenerals)
                                .Include(x => x.QuoteAdditionalCharges)
                                .Include(m => m.QuoteOtherTerms)
                                .Include(i => i.QuoteRFQNotes)
                                .Include(i => i.QuoteSpecialTerms);
            return PagedList<Quote>.ToPagedList(activeQuote, pagingParameter.PageNumber, pagingParameter.PageSize);
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

        public async Task<IEnumerable<rfqEnggItemDetailsForQuoteDto>> GetAllRfqEnggDetailsByRfqNo(string rfqNumber)
        {

            //var releaseLpDetails = from e in _tipsSalesServiceDbContext.RfqEnggItems
            //                       where e.RfqEngg.RFQNumber == rfqNumber
            //                       join d in _tipsSalesServiceDbContext.ReleaseLps on e.ItemNumber equals d.ItemNo
            //                       where d.RfqNumber == rfqNumber    
            //                       select new rfqEnggItemDetailsForQuoteDto
            //                    {
            //                        RfqNumber = d.RfqNumber,
            //                        CustomerName = d.CustomerName,
            //                        Rev = e.RfqEngg.RevisionNumber,
            //                        CustomFields = e.CustomFields,
            //                        DateOnLpCreation = d.DateOnLpCreation,
            //                        CustomerItemNumber = e.CustomerItemNumber,
            //                        ItemNumber = e.ItemNumber, 
            //                        Description = e.Description,
            //                        CostingBomVersionNo = e.CostingBomVersionNo,
            //                        ReleaseStatus = e.ReleaseStatus,
            //                        Qty = e.Qty,
            //                        UOC = d.UOC,
            //                        LeastCost = d.LeastCost,
            //                        LeastCostPlus = d.LeastCostPlus,
            //                        LeastCostminus = d.LeastCostminus,
            //                        DiscountPlus = d.DiscountPlus,
            //                        DiscountMinus = d.DiscountMinus,
            //                        Markup = d.Markup,
            //                        PriceList = d.PriceList,
            //                        ValidThrough = d.ValidThrough,
            //                        IsDiscountApplicable = d.IsDiscountApplicable
            //                    };

            //var releaseLpList = releaseLpDetails.Distinct().ToList();
            var releaseLpDetails = _tipsSalesServiceDbContext.RfqEnggItems
        .GroupJoin(
            _tipsSalesServiceDbContext.RfqEnggs.Where(e => e.RFQNumber == rfqNumber),
            e => e.RfqEnggId,
            eng => eng.Id,
            (e, engGroup) => new { RfqEnggItem = e, RfqEnggs = engGroup })
        .SelectMany(
            x => x.RfqEnggs.DefaultIfEmpty(),
            (x, eng) => new { x.RfqEnggItem, RfqEngg = eng })
        .GroupJoin(
            _tipsSalesServiceDbContext.ReleaseLps.Where(r => r.RfqNumber == rfqNumber),
            x => new { x.RfqEnggItem.ItemNumber, RfqNumber = x.RfqEngg.RFQNumber },
            rel => new { ItemNumber = rel.ItemNo, rel.RfqNumber },
            (x, relGroup) => new { x.RfqEnggItem, x.RfqEngg, ReleaseLps = relGroup })
        .SelectMany(
            x => x.ReleaseLps.DefaultIfEmpty(),
            (x, rel) => new rfqEnggItemDetailsForQuoteDto
            {
                RfqNumber = x.RfqEngg.RFQNumber,
                CustomerName = x.RfqEngg.CustomerName,
                Rev = x.RfqEnggItem.RfqEngg.RevisionNumber,
                CustomFields = x.RfqEnggItem.CustomFields,
                DateOnLpCreation = rel.DateOnLpCreation,
                CustomerItemNumber = x.RfqEnggItem.CustomerItemNumber,
                ItemNumber = x.RfqEnggItem.ItemNumber,
                Description = x.RfqEnggItem.Description,
                CostingBomVersionNo = x.RfqEnggItem.CostingBomVersionNo,
                ReleaseStatus = x.RfqEnggItem.ReleaseStatus,
                Qty = x.RfqEnggItem.Qty,
                UOC = rel.UOC,
                LeastCost = rel.LeastCost,
                LeastCostPlus = rel.LeastCostPlus,
                LeastCostminus = rel.LeastCostminus,
                DiscountPlus = rel.DiscountPlus,
                DiscountMinus = rel.DiscountMinus,
                Markup = rel.Markup,
                PriceList = rel.PriceList,
                ValidThrough = rel.ValidThrough,
                IsDiscountApplicable = rel.IsDiscountApplicable
            })
        .Distinct()
        .ToList();




            return releaseLpDetails;
        }

        public async Task<PagedList<Quote>> GetAllQuote([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {

            var quoteDetails = FindAll().OrderByDescending(x => x.Id)
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.QuoteNumber.Contains(searchParammes.SearchValue)
            || inv.RFQNumber.Contains(searchParammes.SearchValue)
            || inv.RevisionNumber.Equals(int.Parse(searchParammes.SearchValue))
            || inv.TotalAmount.Equals(int.Parse(searchParammes.SearchValue))
            || inv.CustomerName.Contains(searchParammes.SearchValue))))
                                .Include(t => t.QuoteGenerals)
                                .Include(x => x.QuoteAdditionalCharges)
                                .Include(m => m.QuoteOtherTerms)
                                .Include(i => i.QuoteRFQNotes)
                                .Include(i => i.QuoteSpecialTerms);


            return PagedList<Quote>.ToPagedList(quoteDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
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
