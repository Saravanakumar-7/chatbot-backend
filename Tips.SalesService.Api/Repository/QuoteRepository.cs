using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Tips.SalesService.Api.Repository
{
    public class QuoteRepository : RepositoryBase<Quote>, IQuoteRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public QuoteRepository(TipsSalesServiceDbContext repositoryContext, HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsSalesServiceDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _config = config;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<long> CreateQuote(Quote quote)
        {
            var date = DateTime.Now;
            quote.CreatedBy = _createdBy;
            quote.CreatedOn = date.Date;
            quote.Unit = _unitname;
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
        public async Task<string> GenerateQuoteNumberAvision()
        {
            using var transaction = await _tipsSalesServiceDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var rfqNumberEntity = await _tipsSalesServiceDbContext.QuoteNumbers.SingleAsync();
                rfqNumberEntity.CurrentValue += 1;
                _tipsSalesServiceDbContext.Update(rfqNumberEntity);
                await _tipsSalesServiceDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                //int currentYear = DateTime.Now.Year % 100; // Get the last two digits of the current year
                //int nextYear = (DateTime.Now.Year + 1) % 100; // Get the last two digits of the next year

                DateTime currentDate = DateTime.Now;
                DateTime financeYearStart;

                if (currentDate.Month >= 4) // Check if the current date is after or equal to April
                {
                    financeYearStart = new DateTime(currentDate.Year, 4, 1);
                }
                else
                {
                    financeYearStart = new DateTime(currentDate.Year - 1, 4, 1);
                }

                int currentYear = financeYearStart.Year % 100; // Get the last two digits of the current finance year
                int nextYear = (financeYearStart.Year + 1) % 100; // Get the last two digits of the next finance year

                return $"ASPL|QTN|{currentYear:D2}-{nextYear:D2}|{rfqNumberEntity.CurrentValue:D3}";
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
            var getIdByRfqNumber = _tipsSalesServiceDbContext.Quotes
                .Where(x => x.RFQNumber == quote.RFQNumber)
                .OrderByDescending(x => x.Id)
                .Select(x => x.Id)
                .FirstOrDefault();
            var getOldRevisionNumber = _tipsSalesServiceDbContext.Quotes
                .Where(x => x.Id == getIdByRfqNumber)                
                .FirstOrDefault();
            getOldRevisionNumber.LastModifiedBy = _createdBy;
            getOldRevisionNumber.LastModifiedOn = DateTime.Now;
            Update(getOldRevisionNumber);
            var increaseVersionNumber = 1;
            var convertversionnumber = Convert.ToDecimal(increaseVersionNumber);
            var version = getOldRevisionNumber.RevisionNumber + convertversionnumber;
            quote.RevisionNumber = Convert.ToDecimal(version);
            quote.CreatedBy = _createdBy;
            quote.CreatedOn = DateTime.Now;
            quote.Unit = _unitname;
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

        public async Task<IEnumerable<CsItemDetailsForQuoteDto>> GetCsItemDetailsForQuote(string rfqNumber, List<string> latestPriceListName)
        {

            var rfqDetail = await _tipsSalesServiceDbContext.Rfqs
                .Where(x => x.RfqNumber == rfqNumber)
                .FirstOrDefaultAsync();

            var rfqCDetail = await _tipsSalesServiceDbContext.RfqCustomerSupports
               .Where(x => x.RfqNumber == rfqNumber)
               .FirstOrDefaultAsync();

            var rfqCsId = _tipsSalesServiceDbContext.RfqCustomerSupports
                .OrderByDescending(x => x.Id)
                .Where(x => x.RfqNumber == rfqNumber)
                .Select(x=>x.Id)
                .FirstOrDefault();

            var rfqItems = _tipsSalesServiceDbContext.RfqCustomerSupportItems
      .Where(e => e.RfqNumber == rfqNumber && e.ReleaseStatus == true && e.RfqCustomerSupportId == rfqCsId)
      .ToList();

            var itemNumbers = rfqItems.Select(e => e.ItemNumber).Distinct().ToList();

            var postdata = new List<CsItemDetailsForQuoteDto>();

            foreach (var itemNumber in itemNumbers)
            {
                var items = rfqItems.Where(e => e.ItemNumber == itemNumber).ToList();

                foreach (var rfqItem in items)
                {
                    ItemPriceList itemPriceList = null;

                    foreach (var priceListName in latestPriceListName)
                    {
                        itemPriceList = _tipsSalesServiceDbContext.ItemPriceLists
                            .Where(d => d.ItemNumber == rfqItem.ItemNumber && d.PriceListName == priceListName)
                            .OrderByDescending(d => d.CreatedOn)
                            .FirstOrDefault();
                        if (itemPriceList != null)
                        {
                            break;
                        }
                    }
                    if (itemPriceList != null)
                    {
                        var itemdetails = await _httpClient.GetAsync(string.Concat(_config["ItemMasterMainAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={rfqItem.ItemNumber}"));

                        var inventoryObjectString = await itemdetails.Content.ReadAsStringAsync();
                        dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                        dynamic itemobject = inventoryObjectData.data;

                        int? Imageid = itemobject.imageUpload;
                        string? imgbyte=null;
                        if(Imageid != null)
                        {
                            var itemimage = await _httpClient.GetAsync(string.Concat(_config["ItemMasterMainAPI"],
                           $"GetDownloadUrlDetailsforItemImage?imageid={Imageid}"));

                            var inventoryObjectStrin = await itemimage.Content.ReadAsStringAsync();
                            var inventoryObjectDat = JsonConvert.DeserializeObject<GetitemImageDetailDto>(inventoryObjectStrin);
                            var imagy = inventoryObjectDat.data;
                            //byte[]? fileBytes = Convert.FromBase64String(imagy.fileByte);  // Use appropriate conversion method

                            //// Convert byte array to string using UTF-8 encoding
                            //string? fileString = Encoding.UTF8.GetString(fileBytes);
                            imgbyte = imagy.downloadUrl;
                        } 
                        var itemDetails = new CsItemDetailsForQuoteDto
                        {
                            LeadId = rfqDetail.LeadId,
                            RFQNumber = rfqItem.RfqNumber,
                            CustomerName = rfqDetail.CustomerName,
                            CustomerAliasName = rfqDetail.CustomerAliasName,
                            RoomName = rfqItem.RoomName,
                            CustomerId = rfqDetail.CustomerId,
                            ItemNumber = rfqItem.ItemNumber,
                            Description = rfqItem.Description,
                            CustomFields = rfqItem.CustomFields,
                            PriceListName = itemPriceList.PriceListName,
                            Qty = rfqItem.Qty,
                            UnitPrice = itemPriceList.LeastCost,
                            LeastCostPlus = itemPriceList.LeastCostPlus,
                            LeastCostminus = itemPriceList.LeastCostminus,
                            DiscountMinus = itemPriceList.DiscountMinus,
                            DiscountPlus = itemPriceList.DiscountPlus,
                            Markup = itemPriceList.Markup,
                            CreatedOn = itemPriceList.CreatedOn,
                            IsDiscountApplicable = itemPriceList.IsDiscountApplicable,
                            ImageURL = imgbyte
                        };

                        postdata.Add(itemDetails);
                   
                }
                }
            }



            //var leftOuterJoin = from e in _tipsSalesServiceDbContext.RfqCustomerSupportItems
            //                    where e.RfqNumber == rfqNumber && e.ReleaseStatus == true
            //                    join d in _tipsSalesServiceDbContext.ItemPriceLists on e.ItemNumber equals d.ItemNumber into dept
            //                    from ItemPriceList in dept.DefaultIfEmpty()
            //                    select new CsItemDetailsForQuoteDto
            //                    {
            //                        RFQNumber = e.RfqNumber,
            //                        CustomerName = rfqDetail.CustomerName,
            //                        CustomerAliasName = rfqDetail.CustomerAliasName,
            //                        RoomName = e.RoomName,
            //                        CustomerId = rfqDetail.CustomerId,
            //                        ItemNumber = e.ItemNumber,
            //                        Description = e.Description,
            //                        CustomFields = e.CustomFields,
            //                        PriceListName = ItemPriceList.PriceListName,
            //                        Qty = e.Qty,
            //                        UnitPrice = ItemPriceList.LeastCost,
            //                        LeastCostPlus = ItemPriceList.LeastCostPlus,
            //                        LeastCostminus = ItemPriceList.LeastCostminus,
            //                        DiscountMinus = ItemPriceList.DiscountMinus,
            //                        DiscountPlus = ItemPriceList.DiscountPlus,
            //                        Markup = ItemPriceList.Markup,
            //                        CreatedOn = ItemPriceList.CreatedOn,
            //                        IsDiscountApplicable = ItemPriceList.IsDiscountApplicable
            //                    };

            //var postdata = leftOuterJoin.ToList();


            return postdata;
        }

        public async Task<IEnumerable<rfqEnggItemDetailsForQuoteDto>> GetAllRfqEnggDetailsByRfqNo(string rfqNumber)
        {
           
            //    var rfqEnggId = await _tipsSalesServiceDbContext.RfqEnggs
            //      .Where(x => x.RFQNumber == rfqNumber)
            //        .OrderByDescending(x => x.RevisionNumber)
            //         .Select(x => x.Id)
            //   .FirstOrDefaultAsync();


            //    var releaseLpDetails = _tipsSalesServiceDbContext.RfqEnggItems
            //        .Where(x=>x.RfqEnggId == rfqEnggId)
            //.GroupJoin(
            //    _tipsSalesServiceDbContext.RfqEnggs.Where(e => e.RFQNumber == rfqNumber && e.Id == rfqEnggId),
            //    e => e.RfqEnggId,
            //    eng => eng.Id,
            //    (e, engGroup) => new { RfqEnggItem = e, RfqEnggs = engGroup })
            //.SelectMany(
            //    x => x.RfqEnggs.DefaultIfEmpty(),
            //    (x, eng) => new { x.RfqEnggItem, RfqEngg = eng })
            //.GroupJoin(
            //    _tipsSalesServiceDbContext.ReleaseLps.Where(r => r.RfqNumber == rfqNumber),
            //    x => new { x.RfqEnggItem.ItemNumber, RfqNumber = x.RfqEngg.RFQNumber },
            //    rel => new { ItemNumber = rel.ItemNo, rel.RfqNumber },
            //    (x, relGroup) => new { x.RfqEnggItem, x.RfqEngg, ReleaseLps = relGroup })
            //.SelectMany(
            //    x => x.ReleaseLps.DefaultIfEmpty(),
            //    (x, rel) => new rfqEnggItemDetailsForQuoteDto
            //    {
            //        RfqNumber = x.RfqEngg.RFQNumber,
            //        CustomerName = x.RfqEngg.CustomerName,
            //        Rev = x.RfqEnggItem.RfqEngg.RevisionNumber,
            //        CustomFields = x.RfqEnggItem.CustomFields,
            //        DateOnLpCreation = rel.DateOnLpCreation,
            //        CustomerItemNumber = x.RfqEnggItem.CustomerItemNumber,
            //        ItemNumber = x.RfqEnggItem.ItemNumber,
            //        Description = x.RfqEnggItem.Description,
            //        CostingBomVersionNo = x.RfqEnggItem.CostingBomVersionNo,
            //        ReleaseStatus = x.RfqEnggItem.ReleaseStatus,
            //        Qty = x.RfqEnggItem.Qty,
            //        UOC = rel.UOC,
            //        LeastCost = rel.LeastCost,
            //        LeastCostPlus = rel.LeastCostPlus,
            //        LeastCostminus = rel.LeastCostminus,
            //        DiscountPlus = rel.DiscountPlus,
            //        DiscountMinus = rel.DiscountMinus,
            //        Markup = rel.Markup,
            //        PriceList = rel.PriceList,
            //        ValidThrough = rel.ValidThrough,
            //        IsDiscountApplicable = rel.IsDiscountApplicable
            //    })
            //.Distinct()
            //.ToList();

            var rfqDetail = await _tipsSalesServiceDbContext.Rfqs
                 .Where(x => x.RfqNumber == rfqNumber)
                 .FirstOrDefaultAsync();

            var rfqEnggDetail = await _tipsSalesServiceDbContext.RfqEnggs
               .Where(x => x.RFQNumber == rfqNumber)
               .FirstOrDefaultAsync();

            var rfqEnggId = _tipsSalesServiceDbContext.RfqEnggs
                .OrderByDescending(x => x.Id)
                .Where(x => x.RFQNumber == rfqNumber)
                .Select(x => x.Id)
                .FirstOrDefault();

            var rfqEnggItems = _tipsSalesServiceDbContext.RfqEnggItems
      .Where(e => e.ReleaseStatus == true && e.RfqEnggId == rfqEnggId)
      .ToList();

            var itemNumbers = rfqEnggItems.Select(e => e.ItemNumber).Distinct().ToList();

            var postdata = new List<rfqEnggItemDetailsForQuoteDto>();

            foreach (var itemNumber in itemNumbers)
            {
                var items = rfqEnggItems.Where(e => e.ItemNumber == itemNumber).ToList();

                foreach (var rfqItem in items)
                {

                    //var itemPriceList = _tipsSalesServiceDbContext.ReleaseLps
                    //    .Where(d => d.RLpItemNo == rfqItem.ItemNumber)
                    //    .OrderByDescending(d => d.CreatedOn)
                    //    .FirstOrDefault();
                    //
                    var itemPriceList = _tipsSalesServiceDbContext.ItemPriceLists
                        .Where(d => d.ItemNumber == rfqItem.ItemNumber)
                        .OrderByDescending(d => d.CreatedOn)
                        .FirstOrDefault();

                    if (itemPriceList != null)
                    {
                        var itemdetails = await _httpClient.GetAsync(string.Concat(_config["ItemMasterMainAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={rfqItem.ItemNumber}"));

                        var inventoryObjectString = await itemdetails.Content.ReadAsStringAsync();
                        dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                        dynamic itemobject = inventoryObjectData.data;

                        int? Imageid = itemobject.imageUpload;
                        string? imgbyte = null;
                        if (Imageid != null)
                        {
                            var itemimage = await _httpClient.GetAsync(string.Concat(_config["ItemMasterMainAPI"],
                           $"GetDownloadUrlDetailsforItemImage?imageid={Imageid}"));

                            var inventoryObjectStrin = await itemimage.Content.ReadAsStringAsync();
                            var inventoryObjectDat = JsonConvert.DeserializeObject<GetitemImageDetailDto>(inventoryObjectStrin);
                            var imagy = inventoryObjectDat.data;                            
                            imgbyte = imagy.downloadUrl;
                        }
                        var itemDetails = new rfqEnggItemDetailsForQuoteDto
                        {
                            RfqNumber = rfqDetail.RfqNumber,
                            LeadId = rfqDetail.LeadId,
                            CustomerName = rfqDetail.CustomerName,
                            Rev = rfqDetail.RevisionNumber,
                            CustomFields = rfqItem.CustomFields,
                            CustomerItemNumber = rfqItem.CustomerItemNumber,
                            ItemNumber = rfqItem.ItemNumber,
                            Description = rfqItem.Description,
                            CostingBomVersionNo = rfqItem.CostingBomVersionNo,
                            ReleaseStatus = rfqItem.ReleaseStatus,
                            Qty = rfqItem.Qty,
                            UOC = itemPriceList.UOC,
                            LeastCost = itemPriceList.LeastCost,
                            LeastCostPlus = itemPriceList.LeastCostPlus,
                            LeastCostminus = itemPriceList.LeastCostminus,
                            DiscountPlus = itemPriceList.DiscountPlus,
                            DiscountMinus = itemPriceList.DiscountMinus,
                            Markup = itemPriceList.Markup,
                            PriceListName = itemPriceList.PriceListName,
                            ValidThrough = itemPriceList.ValidThrough,
                            IsDiscountApplicable = itemPriceList.IsDiscountApplicable,
                            ImageURL = imgbyte
                        };

                        postdata.Add(itemDetails);
                    }
                }
            }
            return postdata;
        }

        public async Task<PagedList<Quote>> GetAllQuote([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {

            var quoteDetails = FindAll().OrderByDescending(x => x.Id)
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.QuoteNumber.Contains(searchParammes.SearchValue)
            || inv.RFQNumber.Contains(searchParammes.SearchValue)
            || inv.CustomerId.Contains(searchParammes.SearchValue)
            || inv.LeadId.Contains(searchParammes.SearchValue)
            || inv.RevisionNumber.Equals(int.Parse(searchParammes.SearchValue))
            || inv.TotalAmount.Equals(int.Parse(searchParammes.SearchValue))
            || inv.CustomerName.Contains(searchParammes.SearchValue)
            || inv.CustomerId.Contains(searchParammes.SearchValue)
            )))
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
        public async Task<Quote> GetQuoteByQuoteNumber(string quoteNumber)
        {
            var quoteDetails = await _tipsSalesServiceDbContext.Quotes.Where(x => x.QuoteNumber == quoteNumber)
                               
                               .FirstOrDefaultAsync();

            return quoteDetails;
        }

        public async Task<string> UpdateQuote(Quote quote)
        {
            //quote.CreatedBy = quote.CreatedBy;
            //quote.CreatedOn = quote.CreatedOn;
            quote.LastModifiedBy = _createdBy;
            quote.LastModifiedOn = DateTime.Now;
            Update(quote);
            string result = $"Quote of Detail {quote.Id} is updated successfully!";
            return result;
        }
        //To allow short closed
        public async Task CreateShortClosed(ShortClosedDto shortClosedDto)
        {
            var listofquote = await _tipsSalesServiceDbContext.Quotes.Where(x => x.QuoteNumber == shortClosedDto.QuoteNumber)
            .OrderByDescending(x => x.RevisionNumber).Select(inv => inv.Id).ToArrayAsync();
            foreach (var id in listofquote)
            {
                var quoteDetails = await _tipsSalesServiceDbContext.Quotes.Where(x => x.Id == id).FirstOrDefaultAsync();
                quoteDetails.IsShortClosed = true;
                quoteDetails.ShortClosedRemarks = shortClosedDto.ShortClosedRemarks;
                Update(quoteDetails);
                SaveAsync();
            }
        }
    }
}
