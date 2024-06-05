using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities; 
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;
using Org.BouncyCastle.Ocsp;
using Tips.SalesService.Api.Entities.DTOs;
using System.Collections.Immutable;
using Entities.DTOs;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Macs;
using Tips.SalesService.Api.Entities.Dto;
using AutoMapper;
using System.Security.Claims;

namespace Tips.SalesService.Api.Repository
{
    public class RfqCustomerSupportRepository : RepositoryBase<RfqCustomerSupport>, IRfqCustomerSupportRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public RfqCustomerSupportRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateRfqCustomerSupport(RfqCustomerSupport rfqCustomerSupport)
        {
            rfqCustomerSupport.CreatedBy = _createdBy;
            rfqCustomerSupport.CreatedOn = DateTime.Now;
            rfqCustomerSupport.Unit = _unitname;
            //rfqCustomerSupport.RevisionNumber = 1;
             var result = await Create(rfqCustomerSupport);
            return result.Id;
        }
        public async Task<RfqCustomerSupport> GetRfqCustomerSupportDetailsbyrfqnumber(string rfqno)
        {
            var getRfqCSById = await _tipsSalesServiceDbContext.RfqCustomerSupports.Where(x => x.RfqNumber == rfqno).OrderByDescending(x => x.RevisionNumber)
                           .FirstOrDefaultAsync();
            getRfqCSById.RfqCustomerSupportItems = null;
            getRfqCSById.RfqCustomerSupportNotes = null;
            return getRfqCSById;
        }
        public async Task<string> UpdateRfqCSRev(string rfqnumber, int rfqrev)
        {
            var csdetails = await _tipsSalesServiceDbContext.RfqCustomerSupports.Where(x => x.RfqNumber == rfqnumber).OrderByDescending(x => x.Id)
           .Include(x => x.RfqCustomerSupportItems).ThenInclude(x => x.RfqCSDeliverySchedule).Include(x => x.RfqCustomerSupportNotes).FirstOrDefaultAsync();
            if (csdetails != null)
            {
                RfqCustomerSupport rfqCustomerSupport = csdetails;
                rfqCustomerSupport.RevisionNumber = rfqrev;
                if (rfqCustomerSupport.RfqCustomerSupportItems != null)
                {
                    foreach (var eachitem in rfqCustomerSupport.RfqCustomerSupportItems)
                    {
                        eachitem.RfqCustomerSupportId = rfqCustomerSupport.Id;
                        if (eachitem.RfqCSDeliverySchedule != null)
                        {
                            foreach (var eachDS in eachitem.RfqCSDeliverySchedule)
                            {
                                eachDS.RfqCustomerSupportItemsId = eachitem.Id;
                                eachDS.Id = 0;
                                _tipsSalesServiceDbContext.RfqCSDeliverySchedules.Add(eachDS);
                            }
                        }
                        eachitem.Id = 0;
                        _tipsSalesServiceDbContext.RfqCustomerSupportItems.Add(eachitem);
                    }
                }
                if (rfqCustomerSupport.RfqCustomerSupportNotes != null)
                {
                    foreach (var eachnote in rfqCustomerSupport.RfqCustomerSupportNotes)
                    {
                        eachnote.RfqCustomerSupportId = rfqCustomerSupport.Id;
                        eachnote.Id = 0;
                        _tipsSalesServiceDbContext.RfqCustomerSupportNotes.Add(eachnote);
                    }
                }
                rfqCustomerSupport.Id = 0;
                await Create(rfqCustomerSupport);
                await _tipsSalesServiceDbContext.SaveChangesAsync();
                string result = $"RFQCS RevNo. of Detail is updated successfully!";
                return result;
            }
            else
            {
                return "RFQCS is not Present";
            }
        }

        public async Task<string> DeleteRfqCustomerSupport(RfqCustomerSupport rfqCustomerSupport)
        {
            Delete(rfqCustomerSupport);
            string result = $"RFQ details of {rfqCustomerSupport.Id} is deleted successfully!";
            return result;
        }


        public async Task<PagedList<RfqCustomerSupport>> GetAllRfqCustomerSupport([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {

            var rfqCustomerSupportDetails = FindAll().OrderByDescending(x => x.Id)
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue)
            || inv.RfqNumber.Contains(searchParammes.SearchValue)
            || inv.CustomerRfqNumber.Contains(searchParammes.SearchValue)
            || inv.CustomerName.Contains(searchParammes.SearchValue))))
                                .Include(t => t.RfqCustomerSupportItems)
            .ThenInclude(u => u.RfqCSDeliverySchedule)
            .Include(x => x.RfqCustomerSupportNotes);


            return PagedList<RfqCustomerSupport>.ToPagedList(rfqCustomerSupportDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<RfqCustomerSupport> GetRfqCustomerSupportById(int id)
        {
            var getRfqCSById = await _tipsSalesServiceDbContext.RfqCustomerSupports.Where(x => x.Id == id)
                              .Include(t => t.RfqCustomerSupportItems)
                              .ThenInclude(n=>n.RfqCSDeliverySchedule)
                           .Include(m=>m.RfqCustomerSupportNotes)
                           .FirstOrDefaultAsync();

            return getRfqCSById; 
        }

        public async Task<RfqCustomerSupport> GetRfqCustomerSupportDetailsById(int id)
        {
            var getRfqCSById = await _tipsSalesServiceDbContext.RfqCustomerSupports.Where(x => x.Id == id) 
                           .FirstOrDefaultAsync();

            return getRfqCSById;
        }
        public async Task<RfqCustomerSupport> GetRfqCsByRfqNoAndRevNo(string rfqNumber, decimal revisionNumber)
        {
            var rfqCsByRfqNoAndRevNo = await _tipsSalesServiceDbContext.RfqCustomerSupports.Where(x => x.RfqNumber == rfqNumber
                               && x.RevisionNumber == revisionNumber)
                .Include(x=>x.RfqCustomerSupportItems)
                .ThenInclude(x => x.RfqCSDeliverySchedule)
                .Include(x => x.RfqCustomerSupportNotes)
                           .FirstOrDefaultAsync();

            return rfqCsByRfqNoAndRevNo;
        }

        public async Task<RfqCustomerSupport> GetRfqCsLatestRevNoByRfqnumber(string rfqNumber)
        {
            var rfqCsLatestRevNoByRfqNo = await _tipsSalesServiceDbContext.RfqCustomerSupports.Where(x => x.RfqNumber == rfqNumber)
                            .OrderByDescending(x => x.Id)
                            .Include(x => x.RfqCustomerSupportItems)
                            .ThenInclude(x => x.RfqCSDeliverySchedule)
                            .Include(x => x.RfqCustomerSupportItems)
                            //.ThenInclude(x => x.Upload)
                            .Include(x => x.RfqCustomerSupportNotes)

                           .FirstOrDefaultAsync(); 

            return rfqCsLatestRevNoByRfqNo;
        }
        public async Task<RfqCustomerSupport> UpdateRfqcsRevNo(RfqCustomerSupport rfqCustomerSupport)
        {

            rfqCustomerSupport.CreatedBy = _createdBy; ;
            rfqCustomerSupport.CreatedOn = DateTime.Now;
           // rfqCustomerSupport.LastModifiedBy = _createdBy;
            //rfqCustomerSupport.LastModifiedOn = DateTime.Now;
            //var getOldRevisionNumber = _tipsSalesServiceDbContext.RfqCustomerSupports
            //    .Where(x => x.RfqNumber == rfqCustomerSupport.RfqNumber)
            //    .OrderByDescending(x => x.Id)
            //    .Select(x => x.RevisionNumber)
            //    .FirstOrDefault();

            //rfqCustomerSupport.RevisionNumber = (getOldRevisionNumber + 1);
            var result = await Create(rfqCustomerSupport);
            return result;

        }

        public async Task<RfqCustomerSupport> GetRfqCustomerSupportByRfqNumber(string RfqNumber)
        {
            var getRfqCSByRfqNumber = await _tipsSalesServiceDbContext.RfqCustomerSupports
                .Where(x => x.RfqNumber == RfqNumber)
                .Include(t => t.RfqCustomerSupportItems)
                .ThenInclude(n => n.RfqCSDeliverySchedule)
                .Include(m => m.RfqCustomerSupportNotes)              
                        .FirstOrDefaultAsync();

            return getRfqCSByRfqNumber;
        }

        public async Task<string> UpdateRfqCustomerSupport(RfqCustomerSupport rfqCustomerSupport)
        {
            rfqCustomerSupport.LastModifiedBy = _createdBy;
            rfqCustomerSupport.LastModifiedOn = DateTime.Now;            
            Update(rfqCustomerSupport);
            string result = $"RFQ of Detail {rfqCustomerSupport.Id} is updated successfully!";
            return result;
        }

        //add


    }

    public class RfqCustomerSupportItemsRepository : RepositoryBase<RfqCustomerSupportItems>, IRfqCustomerSupportItemRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public RfqCustomerSupportItemsRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<IEnumerable<string>> GetRfqCsandForecastCsDetailListByItemNumber(string itemNumber)
        {
            var rfqNumber = _tipsSalesServiceDbContext.RfqCustomerSupportItems
    .Where(r => r.ItemNumber == itemNumber)
    .Select(r => r.RfqNumber)
    .Distinct()
    .ToList();

            var forecastNumber = _tipsSalesServiceDbContext.foreCastCustomerSupportItems
                .Where(r => r.ItemNumber == itemNumber)
                .Select(r => r.ForecastNumber)
                .Distinct()
                .ToList();

            // Null checks to handle empty lists
            if (rfqNumber == null)
                rfqNumber = new List<string>();

            if (forecastNumber == null)
                forecastNumber = new List<string>();

            var rfqAndForecastNumbers = rfqNumber.Union(forecastNumber).ToList(); 

            return rfqAndForecastNumbers;

        }

        //get list of cs and forecast number list
        //charan
        public async Task<IEnumerable<string>> GetRfqCsandForecastCsProjectNumberList()
        {
            var rfqNumber = _tipsSalesServiceDbContext.RfqCustomerSupportItems
    .Select(r => r.RfqNumber)
    .Distinct()
    .ToList();

            var forecastNumber = _tipsSalesServiceDbContext.foreCastCustomerSupportItems
                .Select(r => r.ForecastNumber)
                .Distinct()
                .ToList();

            // Null checks to handle empty lists
            if (rfqNumber == null)
                rfqNumber = new List<string>();

            if (forecastNumber == null)
                forecastNumber = new List<string>();

            var rfqAndForecastNumbers = rfqNumber.Union(forecastNumber).ToList();

            return rfqAndForecastNumbers;

        }

        //getrfqandforecast number list
        public async Task<IEnumerable<string>> GetRfqEnggandForecastCsProjectList()
        {
            var rfqEnggId = _tipsSalesServiceDbContext.RfqEnggItems
                            .Select(r => r.RfqEnggId)
                             .Distinct()
                             .ToList();

            var rfqNumber = _tipsSalesServiceDbContext.RfqEnggs
                        .Where(x => rfqEnggId.Contains(x.Id))
                        .Select(r => r.RFQNumber)
                        .Distinct()
                        .ToList();

            var forecastNumber = _tipsSalesServiceDbContext.foreCastCustomerSupportItems
                .Select(r => r.ForecastNumber)
                .Distinct()
                .ToList();

            // Null checks to handle empty lists
            if (rfqNumber == null)
                rfqNumber = new List<string>();

            if (forecastNumber == null)
                forecastNumber = new List<string>();

            var rfqAndForecastNumbers = rfqNumber.Union(forecastNumber).ToList(); 

            return rfqAndForecastNumbers;

        }


        public async Task<IEnumerable<string>> GetRfqEnggandForecastCsDetailListByItemNumber(string itemNumber)
        {
            var rfqEnggId = _tipsSalesServiceDbContext.RfqEnggItems
                        .Where(r => r.ItemNumber == itemNumber)
                            .Select(r => r.RfqEnggId)
                             .Distinct()
                             .ToList();

            var rfqNumber = _tipsSalesServiceDbContext.RfqEnggs
                        .Where(x => rfqEnggId.Contains(x.Id))
                        .Select(r => r.RFQNumber)
                        .Distinct()
                        .ToList();

            var forecastNumber = _tipsSalesServiceDbContext.foreCastCustomerSupportItems
                .Where(r => r.ItemNumber == itemNumber)
                .Select(r => r.ForecastNumber)
                .Distinct()
                .ToList();

            // Null checks to handle empty lists
            if (rfqNumber == null)
                rfqNumber = new List<string>();

            if (forecastNumber == null)
                forecastNumber = new List<string>();

            var rfqAndForecastNumbers = rfqNumber.Union(forecastNumber).ToList();
            //var rfqNumber = _tipsSalesServiceDbContext.RfqCustomerSupportItems
            //                 .Where(r => r.ItemNumber == itemNumber)
            //                 .Select(r=> r.RfqNumber)
            //                 .Distinct()
            //                 .ToList();

            //var forecastNumber = _tipsSalesServiceDbContext.foreCastCustomerSupportItems
            //                        .Where(r => r.ItemNumber == itemNumber)
            //                        .Select(r => r.ForecastNumber)
            //                        .Distinct()
            //                        .ToList();
            //var rfqAndForecastNumbers = rfqNumber.Concat(forecastNumber);

            return rfqAndForecastNumbers;

        }

        //    //rfq engg 
        //    public async Task<IEnumerable<string>> GetRfqEnggandForecastEnggDetailListByItemNumber(string itemNumber)
        //    {
        //        var rfqNumber = _tipsSalesServiceDbContext.RfqEnggs 
        //            join _tipsSalesServiceDbContext.rfq
        //.Where(r => r.ItemNumber == itemNumber)
        //.Select(r => r.RfqNumber)
        //.Distinct()
        //.ToList();

        //        var forecastNumber = _tipsSalesServiceDbContext.foreCastCustomerSupportItems
        //            .Where(r => r.ItemNumber == itemNumber)
        //            .Select(r => r.ForecastNumber)
        //            .Distinct()
        //            .ToList();

        //        // Null checks to handle empty lists
        //        if (rfqNumber == null)
        //            rfqNumber = new List<string>();

        //        if (forecastNumber == null)
        //            forecastNumber = new List<string>();

        //        var rfqAndForecastNumbers = rfqNumber.Union(forecastNumber).ToList();

        //        return rfqAndForecastNumbers;

        //    }


        public async Task<string> ActivateRfqCustomerSupportItemById(RfqCustomerSupportItems rfqCustomerSupportItems)
        {
            rfqCustomerSupportItems.LastModifiedBy = _createdBy;
            rfqCustomerSupportItems.LastModifiedOn = DateTime.Now;
            Update(rfqCustomerSupportItems);
            string result = $"CustomerSupport details of {rfqCustomerSupportItems.Id} is updated successfully!";
            return result;
        }

        public Task<int?> CreateRfqCustomerSupportItem(RfqCustomerSupportItems rfqCustomerSupportItems)
        {
            throw new NotImplementedException();
        }

        public async Task<List<int>> RfqCsReleasedItemList(string rfqNumber)
        {
            var latestrfqCsId = await _tipsSalesServiceDbContext.RfqCustomerSupports
            .Where(x => x.RfqNumber == rfqNumber)
            .OrderByDescending(x => x.RevisionNumber)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

            var releaseItemList = await _tipsSalesServiceDbContext.RfqCustomerSupportItems
              .Where(x => x.RfqNumber == rfqNumber && x.ReleaseStatus == true && x.RfqCustomerSupportId == latestrfqCsId).Select(x => x.Id)
              .ToListAsync();

            return releaseItemList;
        }
        public async Task<string> DeactivateRfqCustomerSupportItemById(RfqCustomerSupportItems rfqCustomerSupportItems)
        {
            rfqCustomerSupportItems.LastModifiedBy = _createdBy;
            rfqCustomerSupportItems.LastModifiedOn = DateTime.Now;
            Update(rfqCustomerSupportItems);
            string result = $"CostCenter details of {rfqCustomerSupportItems.Id} is updated successfully!";
            return result;
        }

        public Task<string> DeleteRfqCustomerSupportItem(RfqCustomerSupportItems rfqCustomerSupportItems)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RfqCustomerSupportItems>> GetAllActiveRfqCustomerSupportItemsByRfqNumber(string rfqNumber)
        {

            var rfqCsId = await _tipsSalesServiceDbContext.RfqCustomerSupports
     .Where(x => x.RfqNumber == rfqNumber)
     .OrderByDescending(x => x.RevisionNumber)
     .Select(x => x.Id)
     .FirstOrDefaultAsync();

            IEnumerable<RfqCustomerSupportItems> rfqCsItems = await _tipsSalesServiceDbContext.RfqCustomerSupportItems
             .Where(x => x.RfqNumber == rfqNumber && x.ReleaseStatus == true && 
             x.RfqCustomerSupportId == rfqCsId).ToListAsync();

            return rfqCsItems;
        }
        //add
          

        public Task<IEnumerable<RfqCustomerSupportItems>> GetAllRfqCustomerSupportItem()
        {
            throw new NotImplementedException();
        }

        public async Task<RfqCustomerSupportItems> GetRfqCustomerSupportItemById(int id)
        {
            var getRfqCSItemId = await _tipsSalesServiceDbContext.RfqCustomerSupportItems.Where(x => x.Id == id).FirstOrDefaultAsync();
            return getRfqCSItemId;
        }
        //new
        public async Task<IEnumerable<RfqCustomerSupportItems>> GetRfqCustomerSupportItemByRfqNumber(string rfqNumber)
        {
            var getRfqCSItemRfqnumber = await _tipsSalesServiceDbContext.RfqCustomerSupportItems.Where(x => x.RfqNumber == rfqNumber).ToListAsync();
            return getRfqCSItemRfqnumber;
        }
        public async Task<IEnumerable<RfqCustomerSupportItems>> GetRfqCustomerSupportItemByRfqNumber(string rfqNumber,decimal revNumber)
        {
            var csId = await _tipsSalesServiceDbContext.RfqCustomerSupports
                .Where(x => x.RfqNumber == rfqNumber && x.RevisionNumber == revNumber)
                .Select(x => x.Id).FirstOrDefaultAsync();


            var getRfqCSItemRfqnumber = await _tipsSalesServiceDbContext.RfqCustomerSupportItems
                .Where(x => x.RfqNumber == rfqNumber && x.RfqCustomerSupportId == csId).ToListAsync();
            return getRfqCSItemRfqnumber;
        }

        public async Task<bool> IsFullyReleasedRfqEngg(string rfqNumber, decimal revNumber)
        {

            var enggId = await _tipsSalesServiceDbContext.RfqEnggs
              .Where(s => s.RFQNumber == rfqNumber && s.RevisionNumber == revNumber)
              .Select(x => x.Id).FirstOrDefaultAsync();

            if (enggId == 0)
            {
                return false;
            }

            var isFullyReleased = await _tipsSalesServiceDbContext.RfqEnggItems
                .Where(x =>x.RfqEnggId == enggId)
                .AllAsync(x => x.ReleaseStatus == true); 

            return isFullyReleased;
        }
        public async Task<bool> IsNotYetReleasedRfqEngg(string rfqNumber, decimal revNumber)
        {

            var enggId = await _tipsSalesServiceDbContext.RfqEnggs
               .Where(s => s.RFQNumber == rfqNumber && s.RevisionNumber == revNumber).Select(x => x.Id).FirstOrDefaultAsync();

            var isNotYetReleased = await _tipsSalesServiceDbContext.RfqEnggItems
                .Where(x => x.RfqEnggId == enggId)
                .AllAsync(x => x.ReleaseStatus == false);

            return isNotYetReleased;
        }

        public async Task<bool> IsFullyReleasedRfqCs(string rfqNumber, decimal revNumber)
        {

            var CsId = await _tipsSalesServiceDbContext.RfqCustomerSupports
             .Where(s => s.RfqNumber == rfqNumber && s.RevisionNumber== revNumber).Select(x => x.Id).FirstOrDefaultAsync();
            if (CsId == 0)
            {
                return false;
            }
            var isFullyReleased = await _tipsSalesServiceDbContext.RfqCustomerSupportItems
                .Where(x => x.RfqCustomerSupportId == CsId)
                .AllAsync(x => x.ReleaseStatus == true);

            return isFullyReleased;
        }
        public async Task<bool> IsNotYetReleasedRfqCs(string rfqNumber, decimal revNumber)
        {

            var CsId = await _tipsSalesServiceDbContext.RfqCustomerSupports
             .Where(s => s.RfqNumber == rfqNumber && s.RevisionNumber == revNumber).Select(x => x.Id).FirstOrDefaultAsync();

            var isNotYetReleased = await _tipsSalesServiceDbContext.RfqCustomerSupportItems
                .Where(x => x.RfqCustomerSupportId == CsId)
                .AllAsync(x => x.ReleaseStatus == false);

            return isNotYetReleased;
        }


        public async Task<IEnumerable<RfqCustomerSupportItems>> GetRfqCustomerSupportRelesedDetailsByRfqNumber(string rfqNumber)
        {
            var rfqCsRelesedDetails = await _tipsSalesServiceDbContext.RfqCustomerSupportItems.Where(x => x.RfqNumber == rfqNumber
            && x.ReleaseStatus == true).ToListAsync();
            return rfqCsRelesedDetails;
        }


        public Task<string> UpdateRfqCustomerSupportItem(RfqCustomerSupportItems rfqCustomerSupportItems)
        {
            throw new NotImplementedException();
        }
    }

    public class RfqRepository : RepositoryBase<Rfq>, IRfqRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public RfqRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<Rfq> RfqDetailsById(int rfqId)
        {
            var rfqDetailsByRfqNumber = await _tipsSalesServiceDbContext.Rfqs
              .Where(x => x.Id == rfqId).FirstOrDefaultAsync();
            return rfqDetailsByRfqNumber;
        }
        public async Task<IEnumerable<RfqSPReport>> GetRfqSPReport()
        {
            var result = _tipsSalesServiceDbContext
            .Set<RfqSPReport>()
            .FromSqlInterpolated($"CALL RFQ_Report")
            .ToList();

            return result;

        }
        public async Task<Rfq> RfqSourcingByRfqNumbers(string RfqNumber)
        {
            var SourcingByRfqNumber = await _tipsSalesServiceDbContext.Rfqs
              .Where(x => x.RfqNumber == RfqNumber).OrderByDescending(x => x.RevisionNumber)
                        .FirstOrDefaultAsync();
            return SourcingByRfqNumber;
        }
        public async Task<Rfq> RfqCsByRfqNumbers(string RfqNumber)
        {
            var rfqCsByRfqNumber = await _tipsSalesServiceDbContext.Rfqs
              .Where(x => x.RfqNumber == RfqNumber)
                        .FirstOrDefaultAsync();
            return rfqCsByRfqNumber;
        }
        public async Task<IEnumerable<Rfq>> GetRevNumberByRfqNumberList(string rfqnumber)
        {
            IEnumerable<RevNumberByRfqNumberListDto> revNoListbyRfqNumber = await _tipsSalesServiceDbContext.Rfqs
            .Where(x => x.RfqNumber == rfqnumber).Select(x => new RevNumberByRfqNumberListDto()
            {
                RevisionNumber = x.RevisionNumber,
            }).ToListAsync();

            return (IEnumerable<Rfq>)revNoListbyRfqNumber;
        }

        public async Task<Rfq> RfqEnggByRfqNumbers(string RfqNumber)
        {
            var rfqEnggByRfqNumber = await _tipsSalesServiceDbContext.Rfqs
              .Where(x => x.RfqNumber == RfqNumber)
                        .FirstOrDefaultAsync();
            return rfqEnggByRfqNumber;
        }

        public async Task<string> GenerateRFQNumber()
        {
            using var transaction = await _tipsSalesServiceDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var rfqNumberEntity = await _tipsSalesServiceDbContext.RFQNos.SingleAsync();
                rfqNumberEntity.CurrentValue += 1;
                _tipsSalesServiceDbContext.Update(rfqNumberEntity);
                await _tipsSalesServiceDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"RFQ-{rfqNumberEntity.CurrentValue:D6}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        //for Avision Rfq Format
        public async Task<string> GenerateRFQNumberAvision()
        {
            using var transaction = await _tipsSalesServiceDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var rfqNumberEntity = await _tipsSalesServiceDbContext.RFQNos.SingleAsync();
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

                return $"ASPL|RFQ|{currentYear:D2}-{nextYear:D2}|{rfqNumberEntity.CurrentValue:D4}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }


        //public async Task<string> GenerateRFQNumberAvision()
        //{
        //    using var transaction = await _tipsSalesServiceDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

        //    try
        //    {
        //        var rfqNumberEntity = await _tipsSalesServiceDbContext.RFQNos.SingleAsync();
        //        rfqNumberEntity.CurrentValue += 1;
        //        _tipsSalesServiceDbContext.Update(rfqNumberEntity);
        //        await _tipsSalesServiceDbContext.SaveChangesAsync();
        //        await transaction.CommitAsync();
        //        return $"ASPL|RFQ|-{rfqNumberEntity.CurrentValue:D6}";
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        throw ex;
        //    }
        //}
        public async Task<string> GenerateRFQNumberForTransccon()
        {
            using var transaction = await _tipsSalesServiceDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var rfqNumberEntity = await _tipsSalesServiceDbContext.RFQNos.SingleAsync();
                rfqNumberEntity.CurrentValue += 1;
                _tipsSalesServiceDbContext.Update(rfqNumberEntity);
                await _tipsSalesServiceDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"TISPL-{rfqNumberEntity.CurrentValue:D4}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        //public async Task<string> GenerateRFQTrascconNumber()
        //{
        //    using var transaction = await _tipsSalesServiceDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

        //    try
        //    {
        //        var rfqNumberEntity = await _tipsSalesServiceDbContext.RFQNos.SingleAsync();
        //        rfqNumberEntity.CurrentValue += 1;
        //        _tipsSalesServiceDbContext.Update(rfqNumberEntity);
        //        await _tipsSalesServiceDbContext.SaveChangesAsync();
        //        await transaction.CommitAsync();
        //        return $"TISPL-{rfqNumberEntity.CurrentValue:D6}";
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        throw ex;
        //    }
        //}


        public async Task<Rfq> RfqDetailsByRfqNumbers(string rfqNumber)
        {
            var rfqDetailsByRfqNumber = await _tipsSalesServiceDbContext.Rfqs
              .Where(x => x.RfqNumber == rfqNumber).OrderByDescending(x=>x.RevisionNumber)
                        .FirstOrDefaultAsync();
            return rfqDetailsByRfqNumber;
        }
      

        public async Task<Rfq> RfqLpCostingReleaseByRfqNumbers(string RfqNumber)
        {
            var lpcostingByRfqNumber = await _tipsSalesServiceDbContext.Rfqs
              .Where(x => x.RfqNumber == RfqNumber)
                        .FirstOrDefaultAsync();
            return lpcostingByRfqNumber;
        }
        public async Task<Rfq> RfqLpcostingByRfqNumbers(string RfqNumber)
        {
            var lpcostingReleaseByRfqNumber = await _tipsSalesServiceDbContext.Rfqs
              .Where(x => x.RfqNumber == RfqNumber)
                        .FirstOrDefaultAsync();
            return lpcostingReleaseByRfqNumber;
        }
        public async Task<int?> CreateRfq(Rfq rfq)
        {
            var date = DateTime.Now;
            rfq.CreatedBy = _createdBy;
            rfq.CreatedOn = date.Date;
            var version = 1;
            rfq.RevisionNumber = version;            
            rfq.Unit = _unitname;
            var result = await Create(rfq);
            return result.Id;
        }
        public async Task<int?> GetRfqNumberAutoIncrementCount(DateTime date)
        {
            var getRfqNumberAutoIncrementCount = _tipsSalesServiceDbContext.Rfqs.Where(x => x.CreatedOn == date.Date).Count();

            return getRfqNumberAutoIncrementCount;
        }
        public async Task<string> GetRfqNumberAutoIncrementNumber()
        {
            var rfqNumbers = await _tipsSalesServiceDbContext.Rfqs
       .Select(x => x.RfqNumber)
       .ToListAsync();

            int maxNumber = 0;
            foreach (var rfqNumber in rfqNumbers)
            {
                if (int.TryParse(rfqNumber.Split('-').Last(), out int num))
                {
                    if (num > maxNumber)
                    {
                        maxNumber = num;
                    }
                }
            }

            string newRfqNumber = $"TISPL-{maxNumber + 1}";
            return newRfqNumber;
        }
        public async Task<string> DeleteRfq(Rfq rfq)
        {
            Delete(rfq);
            string result = $"RFQ details of {rfq.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<RfqNumberListDto>> GetAllActiveRfqNumberList()
        {
            IEnumerable<RfqNumberListDto> latestRfqs = await _tipsSalesServiceDbContext.Rfqs
                .Where(x => x.RevisionNumber == _tipsSalesServiceDbContext.Rfqs.Where(r => r.RfqNumber == x.RfqNumber)
                .Max(r => r.RevisionNumber)).Select(x => new RfqNumberListDto
                {
                    Id = x.Id,
                    RfqNumber = x.RfqNumber,
                    SalesPerson = x.SalesPerson,
                    CustomerName = x.CustomerName,
                    RevisionNumber = x.RevisionNumber
                }).OrderByDescending(x => x.Id).ToListAsync();

            return latestRfqs;            
        }

        public async Task<IEnumerable<RfqNumberListDto>> GetAllRfqNumberList()
        {
            IEnumerable<RfqNumberListDto> rfqNumberList = await _tipsSalesServiceDbContext.Rfqs
                                .Select(x => new RfqNumberListDto()
                                {
                                    Id = x.Id,
                                    RfqNumber = x.RfqNumber,
                                    SalesPerson = x.SalesPerson,
                                    CustomerName = x.CustomerName
                                })
                              .OrderByDescending(x => x.Id).ToListAsync();

            return rfqNumberList;
        }

        public async Task<Rfq> GetRfqDeatailsByRfqNoAndRevNo(string rfqNumber, int revisionNumber)
        {
            var rfqDetail = await _tipsSalesServiceDbContext.Rfqs
                .Where(x => x.RfqNumber == rfqNumber && x.RevisionNumber == revisionNumber)
                .FirstOrDefaultAsync();

            return rfqDetail;
        }

        public async Task<PagedList<Rfq>> GetAllRfq([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            int searchValueInt;
            bool isSearchValueInt = int.TryParse(searchParammes.SearchValue, out searchValueInt);
            var rfqDetails = FindAll().Where(inv =>
                   ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.RfqNumber.Contains(searchParammes.SearchValue) ||
                    inv.LeadId.Contains(searchParammes.SearchValue) || inv.CustomerId.Contains(searchParammes.SearchValue) ||
                    inv.CustomerName.Contains(searchParammes.SearchValue)) && 
                    (!isSearchValueInt || inv.RevisionNumber == searchValueInt) && inv.IsModified == false)
                    &&(inv.RevisionNumber== _tipsSalesServiceDbContext.Rfqs.Where(r => r.RfqNumber == inv.RfqNumber).Max(r => r.RevisionNumber)))
                .OrderByDescending(x => x.Id);
                    
            return PagedList<Rfq>.ToPagedList(rfqDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
                
        public async Task<Rfq> GetRfqById(int id)
        {
            var getRfqById = await _tipsSalesServiceDbContext.Rfqs.Where(x => x.Id == id)
                          .FirstOrDefaultAsync();

            return getRfqById;
        }

        public async Task<string> UpdateRfq(Rfq rfq)
        {
            rfq.LastModifiedBy = _createdBy;
            rfq.LastModifiedOn = DateTime.Now;

            Update(rfq);
            string result = $"RFQ of Detail {rfq.Id} is updated successfully!";
            return result;
        }

        //public async Task<IEnumerable<RfqNumberListDto>> GetAllActiveRfqNumberListByCustomerId(string CustomerId)
        //{           

        //IEnumerable<RfqNumberListDto> getAllActiveRfqNumberList = await _tipsSalesServiceDbContext.Rfqs
        //                    .Where(b => b.CustomerId == CustomerId)
        //                    .OrderByDescending(r => r.Id)
        //                    .Select(x => new RfqNumberListDto()
        //                    {
        //                        Id = x.Id,
        //                        RfqNumber = x.RfqNumber,
        //                        CustomerName = x.CustomerName,
        //                        CustomerId = x.CustomerId

        //                    })     
        //                  .Distinct().ToListAsync();

        //return getAllActiveRfqNumberList;
        ////}
        //public async Task<IEnumerable<RfqNumberListDto>> GetAllActiveRfqNumberListByCustomerId(string customerId)
        //{
        //    //var latestRfqs = await _tipsSalesServiceDbContext.Rfqs
        //    //    .Where(r => r.CustomerId == customerId && r.IsModified == false)
        //    //    .ToListAsync();

        //    //var getAllActiveRfqNumberList = latestRfqs
        //    //    .GroupBy(r => r.RfqNumber)
        //    //    .SelectMany(group => group.Where(r => r.RevisionNumber == group.Max(g => g.RevisionNumber)))
        //    //    .Select(x => new RfqNumberListDto
        //    //    {
        //    //        Id = x.Id,
        //    //        RfqNumber = x.RfqNumber,
        //    //        SalesPerson = x.SalesPerson,
        //    //        CustomerName = x.CustomerName,
        //    //        CustomerId = x.CustomerId,
        //    //        RevisionNumber = x.RevisionNumber
        //    //    });
        //    var latestRfqs = await _tipsSalesServiceDbContext.Rfqs.GroupBy(r => r.RfqNumber)
        //        .SelectMany(group => group.Where(r => r.RevisionNumber == group.Max(g => g.RevisionNumber)))
        //       //.Where(r => r.CustomerId == customerId && r.IsModified == false)
        //       .ToListAsync();

        //    var getAllActiveRfqNumberList = latestRfqs.Where(r => r.CustomerId == customerId && r.IsModified == false)
        //        //.GroupBy(r => r.RfqNumber)
        //        //.SelectMany(group => group.Where(r => r.RevisionNumber == group.Max(g => g.RevisionNumber)))
        //        .Select(x => new RfqNumberListDto
        //        {
        //            Id = x.Id,
        //            RfqNumber = x.RfqNumber,
        //            SalesPerson = x.SalesPerson,
        //            CustomerName = x.CustomerName,
        //            CustomerId = x.CustomerId,
        //            RevisionNumber = x.RevisionNumber
        //        });

        //    return getAllActiveRfqNumberList;
        //}

        public async Task<IEnumerable<RfqNumberListDto>> GetAllActiveRfqNumberListByCustomerId(string customerId)
        {
            // Step 1: Retrieve the latest revision for each RfqNumber
            var maxRevisions = await _tipsSalesServiceDbContext.Rfqs
                .GroupBy(r => r.RfqNumber)
                .Select(g => new
                {
                    RfqNumber = g.Key,
                    MaxRevisionNumber = g.Max(r => r.RevisionNumber)
                })
                .ToListAsync();

            // Step 2: Retrieve all RFQs
            var allRfqs = await _tipsSalesServiceDbContext.Rfqs.ToListAsync();

            // Step 3: Filter the RFQs based on the latest revisions
            var latestRfqs = allRfqs
                .Where(r => maxRevisions.Any(mr => mr.RfqNumber == r.RfqNumber && mr.MaxRevisionNumber == r.RevisionNumber))
                .ToList();

            // Step 4: Apply additional filters and project to DTO
            var getAllActiveRfqNumberList = latestRfqs
                .Where(r => r.CustomerId == customerId && r.IsModified == false)
                .Select(x => new RfqNumberListDto
                {
                    Id = x.Id,
                    RfqNumber = x.RfqNumber,
                    SalesPerson = x.SalesPerson,
                    CustomerName = x.CustomerName,
                    CustomerId = x.CustomerId,
                    RevisionNumber = x.RevisionNumber
                })
                .ToList();

            return getAllActiveRfqNumberList;
        }



        public async Task<Rfq> GetCustomerIdByRfqNumber(string rfqnumber)
        {
            var getCustomerId = await _tipsSalesServiceDbContext.Rfqs
                        .Where(x => x.RfqNumber == rfqnumber)
                                  .FirstOrDefaultAsync();
            return getCustomerId;
        }

        //public async Task<Rfq> UpdateRfqRevNo(Rfq rfq)
        //{
        //    //rfq.CreatedBy = "Admin";
        //    //rfq.CreatedOn = DateTime.Now;
        //    //rfq.Unit = "Bangalore";
        //    //var getOldRevisionNumber = _tipsSalesServiceDbContext.Rfqs
        //    //    .Where(x => x.RfqNumber == rfq.RfqNumber)
        //    //    .OrderByDescending(x => x.Id)
        //    //    .Select(x => x.RevisionNumber)
        //    //    .FirstOrDefault();

        //    //rfq.RevisionNumber = getOldRevisionNumber;
        //    //var result = await Create(rfq);
        //    //return result;

        //    var getOldRfqDetails = _tipsSalesServiceDbContext.Rfqs
        //        .Where(x => x.RfqNumber == rfq.RfqNumber && x.IsModified == false)
        //        .FirstOrDefault();

        //    if (getOldRfqDetails != null)
        //    {
        //        getOldRfqDetails.IsModified = true;
        //        getOldRfqDetails.LastModifiedBy = "Admin";
        //        getOldRfqDetails.LastModifiedOn = DateTime.Now;
        //        Update(getOldRfqDetails);
        //    }

        //    rfq.CreatedBy = rfq.CreatedBy;
        //    rfq.CreatedOn = rfq.CreatedOn;
        //    rfq.LastModifiedBy = "Admin";
        //    rfq.LastModifiedOn = DateTime.Now;
        //    var getOldRevisionNumber = _tipsSalesServiceDbContext.Rfqs
        //        .Where(x => x.RfqNumber == rfq.RfqNumber)
        //        .OrderByDescending(x => x.Id)
        //        .Select(x => x.RevisionNumber)
        //        .FirstOrDefault();

        //    rfq.RevisionNumber = (getOldRevisionNumber + 1);
        //    var result = await Create(rfq);

        //    //updating the connected tables
        //    var updatecs = await _tipsSalesServiceDbContext.RfqCustomerSupports
        //       .Where(x => x.RfqNumber == rfq.RfqNumber && x.RevisionNumber == getOldRevisionNumber)
        //       .Include(x => x.RfqCustomerSupportItems).ThenInclude(x => x.RfqCSDeliverySchedule)
        //       .Include(x => x.RfqCustomerSupportNotes)
        //   .ToListAsync();

        //    if (updatecs != null)
        //    {
        //        var updatedcsdetails = _mapper.Map<List<RfqCustomerSupport>>(updatecs);
        //        foreach (var detail in updatedcsdetails)
        //        {
        //            detail.RevisionNumber = rfq.RevisionNumber;
        //            if (detail.RfqCustomerSupportItems != null)
        //            {
        //                foreach (var itemdetail in detail.RfqCustomerSupportItems)
        //                {
        //                    itemdetail.RfqCustomerSupportId = detail.Id;
        //                    if (itemdetail.RfqCSDeliverySchedule != null)
        //                    {
        //                        foreach (var itemdelydetail in itemdetail.RfqCSDeliverySchedule)
        //                        {
        //                            itemdelydetail.RfqCustomerSupportItemsId = itemdetail.Id;
        //                            itemdelydetail.Id = 0;
        //                            _tipsSalesServiceDbContext.RfqCSDeliverySchedules.Add(itemdelydetail);
        //                        }
        //                    }
        //                    itemdetail.Id = 0;
        //                    _tipsSalesServiceDbContext.RfqCustomerSupportItems.Add(itemdetail);
        //                }
        //            }
        //            if (detail.RfqCustomerSupportNotes != null)
        //            {
        //                foreach (var notedetail in detail.RfqCustomerSupportNotes)
        //                {
        //                    notedetail.RfqCustomerSupportId = detail.Id;
        //                    notedetail.Id = 0;
        //                    _tipsSalesServiceDbContext.RfqCustomerSupportNotes.Add(notedetail);
        //                }
        //            }
        //            detail.Id = 0;
        //            _tipsSalesServiceDbContext.RfqCustomerSupports.Add(detail);
        //        }
        //        await _tipsSalesServiceDbContext.SaveChangesAsync();
        //    }
        //    return result;

        //}
        public async Task<Rfq> UpdateRfqRevNo(Rfq rfq, string serverKey)
        {
            var getOldRfqDetails = _tipsSalesServiceDbContext.Rfqs.Where(x => x.RfqNumber == rfq.RfqNumber && x.IsModified == false)
                .FirstOrDefault();

            if (getOldRfqDetails != null)
            {
                getOldRfqDetails.IsModified = true;
                getOldRfqDetails.LastModifiedBy = _createdBy;
                getOldRfqDetails.LastModifiedOn = DateTime.Now;
                Update(getOldRfqDetails);
            }

            rfq.CreatedBy = rfq.CreatedBy;
            rfq.CreatedOn = rfq.CreatedOn;
           // rfq.LastModifiedBy = _createdBy;
           // rfq.LastModifiedOn = DateTime.Now;
            var getOldRevisionNumber = _tipsSalesServiceDbContext.Rfqs.Where(x => x.RfqNumber == rfq.RfqNumber).OrderByDescending(x => x.Id)
                .Select(x => x.RevisionNumber).FirstOrDefault();

            rfq.RevisionNumber = (getOldRevisionNumber + 1);
            var result = await Create(rfq);

            //updating the connected tables
            var updatecs = await _tipsSalesServiceDbContext.RfqCustomerSupports
               .Where(x => x.RfqNumber == rfq.RfqNumber && x.RevisionNumber == getOldRevisionNumber)
               .Include(x => x.RfqCustomerSupportItems).ThenInclude(x => x.RfqCSDeliverySchedule)
               .Include(x => x.RfqCustomerSupportNotes)
           .ToListAsync();

            if (updatecs != null)
            {
                var updatedcsdetails = _mapper.Map<List<RfqCustomerSupport>>(updatecs);
                foreach (var detail in updatedcsdetails)
                {
                    detail.RevisionNumber = rfq.RevisionNumber;
                    detail.CustomerName = rfq.CustomerName;
                    detail.CustomerRfqNumber = rfq.CustomerRfqNumber;
                    detail.CustomerAliasName = rfq.CustomerAliasName;
                    detail.RequestReceivedate = rfq.RequestReceivedate;
                    detail.TypeOfSolution = rfq.TypeOfSolution;
                    detail.ProductType = rfq.ProductType;
                    if (detail.RfqCustomerSupportItems != null)
                    {
                        foreach (var itemdetail in detail.RfqCustomerSupportItems)
                        {
                            itemdetail.RfqCustomerSupportId = detail.Id;
                            if (itemdetail.RfqCSDeliverySchedule != null)
                            {
                                foreach (var itemdelydetail in itemdetail.RfqCSDeliverySchedule)
                                {
                                    itemdelydetail.RfqCustomerSupportItemsId = itemdetail.Id;
                                    itemdelydetail.Id = 0;
                                    _tipsSalesServiceDbContext.RfqCSDeliverySchedules.Add(itemdelydetail);
                                }
                            }
                            itemdetail.Id = 0;
                            _tipsSalesServiceDbContext.RfqCustomerSupportItems.Add(itemdetail);
                        }
                    }
                    if (detail.RfqCustomerSupportNotes != null)
                    {
                        foreach (var notedetail in detail.RfqCustomerSupportNotes)
                        {
                            notedetail.RfqCustomerSupportId = detail.Id;
                            notedetail.Id = 0;
                            _tipsSalesServiceDbContext.RfqCustomerSupportNotes.Add(notedetail);
                        }
                    }
                    detail.Id = 0;
                    _tipsSalesServiceDbContext.RfqCustomerSupports.Add(detail);
                }
                await _tipsSalesServiceDbContext.SaveChangesAsync();
            }
            if (!serverKey.Equals("keus"))
            {
                var updateengg = await _tipsSalesServiceDbContext.RfqEnggs
                   .Where(x => x.RFQNumber == rfq.RfqNumber && x.RevisionNumber == getOldRevisionNumber)
                   .Include(x => x.RfqEnggItems).Include(x => x.RfqEnggRiskIdentifications).ToListAsync();
                if (updateengg != null)
                {
                    var updatedenggdetails = _mapper.Map<List<RfqEngg>>(updateengg);
                    foreach (var detail in updatedenggdetails)
                    {
                        detail.RevisionNumber = rfq.RevisionNumber;
                        detail.CustomerName = rfq.CustomerName;
                        detail.CustomerRfqNumber = rfq.CustomerRfqNumber;
                        detail.CustomerAliasName = rfq.CustomerAliasName;
                        detail.RequestReceiveDate = rfq.RequestReceivedate;
                        if (detail.RfqEnggItems != null)
                        {
                            foreach (var itemdetail in detail.RfqEnggItems)
                            {
                                itemdetail.RfqEnggId = detail.Id;
                                itemdetail.Id = 0;
                                _tipsSalesServiceDbContext.RfqEnggItems.Add(itemdetail);
                            }
                        }
                        if (detail.RfqEnggRiskIdentifications != null)
                        {
                            foreach (var riskdetail in detail.RfqEnggRiskIdentifications)
                            {
                                riskdetail.RfqEnggId = detail.Id;
                                riskdetail.Id = 0;
                                _tipsSalesServiceDbContext.RfqEnggRiskIdentifications.Add(riskdetail);
                            }
                        }
                        detail.Id = 0;
                        _tipsSalesServiceDbContext.RfqEnggs.Add(detail);
                    }
                    await _tipsSalesServiceDbContext.SaveChangesAsync();
                }
            }
            return result;

        }
        public async Task<IEnumerable<LatestRfqNumberListDto>> GetAllActiveLatestRfqNumbers()
        {

            var getAllActiveRfqNumberList = _tipsSalesServiceDbContext.Rfqs
                 .GroupBy(r => r.RfqNumber)
             .AsEnumerable() // Switch to client-side evaluation
             .SelectMany(group => group
             .Where(r => r.RfqNumber == group.Max(g => g.RfqNumber)))
              .Select(x => new LatestRfqNumberListDto
              {
                  RfqNumber = x.RfqNumber,
                  SalesPerson = x.SalesPerson,
                  RevisionNumber = x.RevisionNumber
              });
            return getAllActiveRfqNumberList;

        }
    }
    public class RfqEnggRepository : RepositoryBase<RfqEngg>, IRfqEnggRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public RfqEnggRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateRfqEngg(RfqEngg rfqEngg)
        {
            rfqEngg.CreatedBy = _createdBy;
            rfqEngg.CreatedOn = DateTime.Now;
            rfqEngg.Unit = _unitname;            
            var result = await Create(rfqEngg);
            return result.Id;
        }
        public async Task<string> UpdateRfqEnggRev(string rfqnumber, int rfqrev, Rfq newrfq)
        {
            var enggdetails = await _tipsSalesServiceDbContext.RfqEnggs.Where(x => x.RFQNumber == rfqnumber)
           .Include(x => x.RfqEnggItems).Include(x => x.RfqEnggRiskIdentifications).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            if (enggdetails != null)
            {
                RfqEngg rfqEngg = enggdetails;
                rfqEngg.CustomerName = newrfq.CustomerName;
                rfqEngg.CustomerAliasName = newrfq.CustomerAliasName;
                rfqEngg.CustomerRfqNumber = newrfq.CustomerRfqNumber;
                rfqEngg.RequestReceiveDate = newrfq.RequestReceivedate;
                rfqEngg.QuoteExpectDate = newrfq.QuoteExpectdate;
                rfqEngg.RevisionNumber = rfqrev;
                foreach (var eachitem in rfqEngg.RfqEnggItems)
                {
                    eachitem.RfqEnggId = rfqEngg.Id;
                    eachitem.Id = 0;
                    _tipsSalesServiceDbContext.RfqEnggItems.Add(eachitem);
                }
                foreach (var eachitem in rfqEngg.RfqEnggRiskIdentifications)
                {
                    eachitem.RfqEnggId = rfqEngg.Id;
                    eachitem.Id = 0;
                    _tipsSalesServiceDbContext.RfqEnggRiskIdentifications.Add(eachitem);
                }
                rfqEngg.Id = 0;
                await Create(rfqEngg);
                await _tipsSalesServiceDbContext.SaveChangesAsync();
                string result = $"RFQEngg RevNo. of Detail is updated successfully!";
                return result;
            }
            else
            {
                return "RFQEngg is not Present";
            }

        }
        public async Task<string> DeleteRfqEngg(RfqEngg rfqEngg)
        {
            Delete(rfqEngg);
            string result = $"RFQEngg details of {rfqEngg.Id} is deleted successfully!";
            return result;
        }

        public async Task<string> UpdateRfqEnggRev(string rfqnumber, int rfqrev)
        {
            var enggdetails = await _tipsSalesServiceDbContext.RfqEnggs.Where(x => x.RFQNumber == rfqnumber)
           .Include(x => x.RfqEnggItems).Include(x => x.RfqEnggRiskIdentifications).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            if (enggdetails != null)
            {
                RfqEngg rfqEngg = enggdetails;
                rfqEngg.RevisionNumber = rfqrev;
                foreach (var eachitem in rfqEngg.RfqEnggItems)
                {
                    eachitem.RfqEnggId = rfqEngg.Id;
                    eachitem.Id = 0;
                    _tipsSalesServiceDbContext.RfqEnggItems.Add(eachitem);
                }
                rfqEngg.Id = 0;
                await Create(rfqEngg);
                await _tipsSalesServiceDbContext.SaveChangesAsync();
                string result = $"RFQEngg RevNo. of Detail is updated successfully!";
                return result;
            }
            else
            {
                return "RFQEngg is not Present";
            }

        }
        public async Task<PagedList<RfqEngg>> GetAllRfqEngg([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {

            var getAllRfqEngg = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.RFQNumber.Contains(searchParammes.SearchValue)
                || inv.CustomerName.Contains(searchParammes.SearchValue)
                || inv.CustomerRfqNumber.Contains(searchParammes.SearchValue)
                || inv.RevisionNumber.Equals(int.Parse(searchParammes.SearchValue)))))
                .Include(t => t.RfqEnggItems)
            .Include(x => x.RfqEnggRiskIdentifications);

            return PagedList<RfqEngg>.ToPagedList(getAllRfqEngg, pagingParameter.PageNumber, pagingParameter.PageSize);
        }


        public async Task<RfqEngg> GetRfqEnggById(int id)
        {
            var getRfqEnggById = await _tipsSalesServiceDbContext.RfqEnggs.Where(x => x.Id == id)
                              .Include(t => t.RfqEnggItems)
                           .Include(m => m.RfqEnggRiskIdentifications)
                           .FirstOrDefaultAsync();

            return getRfqEnggById;
        }

        public async Task<RfqEngg> GetRfqEnggByRfqNoAndRevNo(string rfqNumber, decimal revisionNumber)
        {
            var rfqEnggByRfqNoAndRevNo = await _tipsSalesServiceDbContext.RfqEnggs.Where(x => x.RFQNumber == rfqNumber
                               && x.RevisionNumber == revisionNumber)
                              .Include(t => t.RfqEnggItems)
                           .Include(m => m.RfqEnggRiskIdentifications)
                           .FirstOrDefaultAsync();

            return rfqEnggByRfqNoAndRevNo;
        }

        public async Task<RfqEngg> GetRfqEnggLatestRevNoByRfqnumber(string rfqNumber)
        {
            var rfqEnggLatestRevNoByRfqNo =await _tipsSalesServiceDbContext.RfqEnggs.Where(x => x.RFQNumber == rfqNumber)
                            .OrderByDescending(x => x.Id)
                              .Include(t => t.RfqEnggItems)
                           .Include(m => m.RfqEnggRiskIdentifications)
                           .FirstOrDefaultAsync();

            return rfqEnggLatestRevNoByRfqNo;
        }

        public async Task<RfqEngg> UpdateRfqEnggRevNo(RfqEngg rfqEngg)
        {

            rfqEngg.CreatedBy = rfqEngg.CreatedBy;
            rfqEngg.CreatedOn = rfqEngg.CreatedOn;
            rfqEngg.LastModifiedBy = _createdBy;
            rfqEngg.LastModifiedOn = DateTime.Now;
            var getOldRevisionNumber = _tipsSalesServiceDbContext.RfqEnggs
                .Where(x => x.RFQNumber == rfqEngg.RFQNumber)
                .OrderByDescending(x => x.Id)
                .Select(x => x.RevisionNumber)
                .FirstOrDefault();

            rfqEngg.RevisionNumber = (getOldRevisionNumber + 1);
            rfqEngg.Id = 0;
            var result = await Create(rfqEngg);
            return result;

        }

        public async Task<RfqEngg> GetRfqEnggByRfqNumber(string RfqNumber)
        {           
            var getRfqEnggByRfqNumber = await _tipsSalesServiceDbContext.RfqEnggs
                .Where(x => (x.RFQNumber == RfqNumber)&&(x.RevisionNumber==_tipsSalesServiceDbContext.RfqEnggs.Where(r=>r.RFQNumber==x.RFQNumber)
                .Max(r=>r.RevisionNumber)))
                .Include(t => t.RfqEnggItems)                
                .Include(m => m.RfqEnggRiskIdentifications)              
                        .FirstOrDefaultAsync();
            return getRfqEnggByRfqNumber;
        }
        public async Task<RfqEngg> GetRfqEnggByRfqNumberRevNo(string RfqNumber)
        {
            var revNo = await _tipsSalesServiceDbContext.RfqEnggs
    .Where(x => x.RFQNumber == RfqNumber)
    .OrderByDescending(x => x.RevisionNumber)
    .Select(x => x.RevisionNumber)
    .FirstOrDefaultAsync();


            var getRfqEnggByRfqNumber = await _tipsSalesServiceDbContext.RfqEnggs
                .Where(x => x.RFQNumber == RfqNumber && x.RevisionNumber == revNo)
                .Include(t => t.RfqEnggItems)
                .FirstOrDefaultAsync();
            return getRfqEnggByRfqNumber;
        }
        public Task<IEnumerable<RfqEnggItem>> GetRfqEnggItemsByRfqNumber(string rfqNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UpdateRfqEngg(RfqEngg rfqEngg)
        {
            rfqEngg.LastModifiedBy = _createdBy;
            rfqEngg.LastModifiedOn = DateTime.Now;
            Update(rfqEngg);
            string result = $"RFQEngg of Detail {rfqEngg.Id} is updated successfully!";
            return result;
        }
    }

    public class RfqEnggItemRepository : RepositoryBase<RfqEnggItem>, IRfqEnggItemRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public RfqEnggItemRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }
        public async Task<List<RfqEnggItem>> GetRfqEnggItemsbyRfqId(int RfqId)
        {
            var rfqnoandrev = await _tipsSalesServiceDbContext.Rfqs.Where(x => x.Id == RfqId).FirstOrDefaultAsync();
            decimal rfqrev = rfqnoandrev.RevisionNumber;
            var rfqEngg = await _tipsSalesServiceDbContext.RfqEnggs
                .Where(x => x.RFQNumber == rfqnoandrev.RfqNumber && x.RevisionNumber == rfqrev).OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefaultAsync();
            var rfqEnggItems = await _tipsSalesServiceDbContext.RfqEnggItems.Where(x => x.RfqEnggId == rfqEngg && x.CostingBomVersionNo!=null).OrderByDescending(x => x.Id).ToListAsync();
            return rfqEnggItems;

        }

        public async Task<RfqEnggItem> GetRfqEnggItemByItemNumber(string itemNumber)
        {
            var rfqEnggItems = await _tipsSalesServiceDbContext.RfqEnggItems.Where(x => x.ItemNumber == itemNumber).FirstOrDefaultAsync();
            return rfqEnggItems;
        }
        public async Task<string> UpdateRfqEnggItemLandedandMOQ(RfqEnggItem rfqEnggItem)
        {
            Update(rfqEnggItem);
            await _tipsSalesServiceDbContext.SaveChangesAsync();
            return "RfqEnggItem LandedPrice And MOQ Cost is Update";
        }
        public async Task<string> ActivateRfqEnggItemById(RfqEnggItem rfqEnggItem)
        { 
            Update(rfqEnggItem);
            string result = $"CostCenter details of {rfqEnggItem.Id} is updated successfully!";
            return result;
        }

        public Task<int?> CreateRfqEnggItem(RfqEnggItem rfqEnggItem)
        {
            throw new NotImplementedException();
        }

        public async Task<string> DeactivateRfqEnggItemById(RfqEnggItem rfqEnggItem)
        {
            // rfqEnggItem.LastModifiedBy = "Admin";
            // rfqEnggItem.LastModifiedOn = DateTime.Now;
            Update(rfqEnggItem);
            string result = $"CostCenter details of {rfqEnggItem.Id} is updated successfully!";
            return result;
        }

        public Task<string> DeleteRfqEnggItem(RfqEnggItem rfqEnggItem)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<RfqEnggItem>> RfqEnggReleasedItemList(string rfqNumber)
        {
            var latestrfqEnggId = await _tipsSalesServiceDbContext.RfqEnggs
            .Where(x => x.RFQNumber == rfqNumber)
            .OrderByDescending(x => x.RevisionNumber)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

            var releaseItemList = await _tipsSalesServiceDbContext.RfqEnggItems
              .Where(x => x.ReleaseStatus == true && x.RfqEnggId == latestrfqEnggId)
              .ToListAsync();

            return releaseItemList;
        }

        public async Task<IEnumerable<RfqEnggItem>> GetAllActiveRfqEnggItemByRfqNumber(string rfqNumber)
        {
            int poId = await _tipsSalesServiceDbContext.RfqEnggs
              .Where(s => s.RFQNumber == rfqNumber).Select(x => x.Id).FirstOrDefaultAsync();

            IEnumerable<RfqEnggItem> rfqEnggItems = await _tipsSalesServiceDbContext.RfqEnggItems
                 .Where(x => x.RfqEnggId == poId)
             .Where(x => x.ReleaseStatus == true).ToListAsync();

            return rfqEnggItems;
        }

        //public async Task<IEnumerable<RfqEnggItem>> GetRfqEnggRelesedDetailsByRfqNumber(string rfqNumber)
        //{
        //        List<int> poId = await _tipsSalesServiceDbContext.RfqEnggs
        //      .Where(s => s.RFQNumber == rfqNumber).Select(x => x.Id).Distinct().ToListAsync();

        //    var rfqEnggRelesedDetails = await _tipsSalesServiceDbContext.RfqEnggItems.Where(x => poId.Contains(x.RfqEnggId)
        //    && x.ReleaseStatus == true).ToListAsync();
        //    return rfqEnggRelesedDetails;
        //}
        public async Task<IEnumerable<RfqEnggItem>> GetRfqEnggRelesedDetailsByRfqNumber(string rfqNumber)
        {
            var rfqnoandrev = await _tipsSalesServiceDbContext.Rfqs.Where(x => x.RfqNumber == rfqNumber)
                .OrderByDescending(x => x.RevisionNumber).FirstOrDefaultAsync();
            decimal rfqrev = rfqnoandrev.RevisionNumber;
            var rfqEngg = await _tipsSalesServiceDbContext.RfqEnggs
                .Where(x => x.RFQNumber == rfqnoandrev.RfqNumber && x.RevisionNumber == rfqrev).Select(x => x.Id).FirstOrDefaultAsync();
            var rfqEnggItems = await _tipsSalesServiceDbContext.RfqEnggItems.Where(x => x.RfqEnggId == rfqEngg).OrderByDescending(x => x.Id).ToListAsync();
            return rfqEnggItems;
        }
        public async Task<IEnumerable<RfqEnggItem>> GetRfqEnggCountByRfqNumber(string rfqNumber)
        {
            List<int> poId = await _tipsSalesServiceDbContext.RfqEnggs
              .Where(s => s.RFQNumber == rfqNumber).Select(x => x.Id).Distinct().ToListAsync();

            var rfqEnggCount = await _tipsSalesServiceDbContext.RfqEnggItems.Where(x => poId.Contains(x.RfqEnggId)
            ).ToListAsync();
            return rfqEnggCount;
        }
        public async Task<IEnumerable<RfqEnggItem>> GetRfqEnggCountByRfqNumber(string rfqNumber, decimal revNo)
        {
            List<int> poId = await _tipsSalesServiceDbContext.RfqEnggs
              .Where(s => s.RFQNumber == rfqNumber && s.RevisionNumber == revNo).Select(x => x.Id).Distinct().ToListAsync();

            var rfqEnggCount = await _tipsSalesServiceDbContext.RfqEnggItems.Where(x => poId.Contains(x.RfqEnggId)
            ).ToListAsync();
            return rfqEnggCount;
        }
        public async Task<PagedList<RfqEnggItem>> GetAllRfqEnggItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var getAllRfqEnggItems = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.CustomerItemNumber.Contains(searchParammes.SearchValue)
                || inv.Description.Contains(searchParammes.SearchValue))));

            return PagedList<RfqEnggItem>.ToPagedList(getAllRfqEnggItems, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<RfqEnggItem> GetRfqEnggItemById(int id)
        {
            var rfqEnggItems = await _tipsSalesServiceDbContext.RfqEnggItems.Where(x => x.Id == id).FirstOrDefaultAsync();
            return rfqEnggItems;
        }

        public Task<string> UpdateRfqEnggItem(RfqEnggItem rfqEnggItem)
        {
            throw new NotImplementedException();
        }
    }
        
    public class RfqLPCostingRepository : RepositoryBase<RfqLPCosting>, IRfqLPCostingRepository
        {
            private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public RfqLPCostingRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsSalesServiceDbContext)
            {
                _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
            public async Task<int?> CreateRfqLPCosting(RfqLPCosting rfqLPCosting)
            {
                rfqLPCosting.CreatedBy = _createdBy;
                rfqLPCosting.CreatedOn = DateTime.Now;
                rfqLPCosting.Unit = _unitname;
                var result = await Create(rfqLPCosting);
                return result.Id;
            }

            public async Task<string> DeleteRfqLPCosting(RfqLPCosting rfqLPCosting)
            {
                Delete(rfqLPCosting);
                string result = $"RFQLPCosting details of {rfqLPCosting.Id} is deleted successfully!";
                return result;
            }
        public async Task<PagedList<RfqLPCosting>> GetAllRfqLPCosting([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {

            var rfqLPCosting = FindAll().OrderByDescending(x => x.Id)
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.RfqNumber.Contains(searchParammes.SearchValue)
           || inv.CustomerName.Contains(searchParammes.SearchValue))))
               .Include(x => x.RfqLPCostingItems)
            .ThenInclude(u => u.RfqLPCostingProcesses)
             .Include(x => x.RfqLPCostingItems)
             .ThenInclude(v => v.RfqLPCostingNREConsumables)
             .Include(x => x.RfqLPCostingItems)
             .ThenInclude(w => w.RfqLPCostingOtherCharges);

            return PagedList<RfqLPCosting>.ToPagedList(rfqLPCosting, pagingParameter.PageNumber, pagingParameter.PageSize);

        }

        public async Task<RfqLPCosting> GetRfqLPCostingById(int id)
            {
                var rfqLPCosting = await _tipsSalesServiceDbContext.RfqLPCostings.Where(x => x.Id == id)
                     .Include(x => x.RfqLPCostingItems)
                     .ThenInclude(u => u.RfqLPCostingProcesses)
                     .Include(x => x.RfqLPCostingItems)
                     .ThenInclude(v => v.RfqLPCostingNREConsumables)
                     .Include(x => x.RfqLPCostingItems)
                     .ThenInclude(w => w.RfqLPCostingOtherCharges)
                              .FirstOrDefaultAsync();

                return rfqLPCosting;
            }


            public async Task<string> UpdateRfqLPCosting(RfqLPCosting rfqLPCosting)
            {
                rfqLPCosting.LastModifiedBy = _createdBy;
                rfqLPCosting.LastModifiedOn = DateTime.Now;
                Update(rfqLPCosting);
                string result = $"RFQ of Detail {rfqLPCosting.Id} is updated successfully!";
                return result;
            }
            public async Task<RfqLPCosting> GetRfqLPCostingByRfqNumber(string RfqNumber)
            {
                var LpCostingByRfqNumber = await _tipsSalesServiceDbContext.RfqLPCostings
                    .Include(t => t.RfqLPCostingItems)
                    .ThenInclude(c=>c.RfqLPCostingProcesses)
                    .Include(t => t.RfqLPCostingItems)
                    .ThenInclude(v=>v.RfqLPCostingNREConsumables)
                    .Include(t => t.RfqLPCostingItems)
                    .ThenInclude(b=>b.RfqLPCostingOtherCharges)
                  .Where(x => x.RfqNumber == RfqNumber)
                            .FirstOrDefaultAsync();
                return LpCostingByRfqNumber;
            }

        }
    public class RfqCustomGroupRepository : RepositoryBase<RfqCustomGroup>, IRfqCustomGroupRepository
        {
            private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public RfqCustomGroupRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsSalesServiceDbContext)
            {
                _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateRfqCustomGroup(RfqCustomGroup rfqCustomGroup)
            {
                rfqCustomGroup.CreatedBy = _createdBy;
                rfqCustomGroup.CreatedOn = DateTime.Now;
               // rfqCustomGroup.LastModifiedBy = "Admin";
                //rfqCustomGroup.LastModifiedOn = DateTime.Now;
                rfqCustomGroup.Unit = _unitname;
                var result = await Create(rfqCustomGroup);
                return result.Id;
            }
        public async Task<IEnumerable<ListOfCustomGroupDto>> GetAllCustomGroupList()
        {
            IEnumerable<ListOfCustomGroupDto> getAllCustomGroupList = await _tipsSalesServiceDbContext.RfqCustomGroups
                                .Select(c => new ListOfCustomGroupDto()
                                {
                                    Id = c.Id,
                                    CustomGroupName = c.CustomGroupName,

                                })
                                .OrderByDescending(c => c.Id)
                              .ToListAsync();

            return getAllCustomGroupList;
        }
        public async Task<string> DeleteRfqCustomGroup(RfqCustomGroup rfqCustomGroup)
            {
                Delete(rfqCustomGroup);
                string result = $"RfqCustomGroup details of {rfqCustomGroup.Id} is deleted successfully!";
                return result;
            }

        public async Task<PagedList<RfqCustomGroup>> GetAllRfqCustomGroup([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var RfqCustomGroups = FindAll().OrderByDescending(x => x.Id)
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.CustomGroupName.Contains(searchParammes.SearchValue))));

            return PagedList<RfqCustomGroup>.ToPagedList(RfqCustomGroups, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<RfqCustomGroup> GetRfqCustomGroupById(int id)
            {
                var getRfqCustomGroupById = await _tipsSalesServiceDbContext.RfqCustomGroups.Where(x => x.Id == id).FirstOrDefaultAsync();
                return getRfqCustomGroupById;
            }

            public async Task<string> UpdateRfqCustomGroup(RfqCustomGroup rfqCustomGroup)
            {
                rfqCustomGroup.LastModifiedBy = _createdBy;
                rfqCustomGroup.LastModifiedOn = DateTime.Now;
                Update(rfqCustomGroup);
                string result = $"RfqCustomGroup Detail {rfqCustomGroup.Id} is updated successfully!";
                return result;
            }
        }
    public class RfqCustomFieldRepository : RepositoryBase<RfqCustomField>, IRfqCustomFieldRepository
        {
            private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public RfqCustomFieldRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsSalesServiceDbContext)
            {
                _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateRfqCustomField(RfqCustomField rfqCustomField)
            {
                rfqCustomField.CreatedBy = _createdBy;
                rfqCustomField.CreatedOn = DateTime.Now;
               // rfqCustomField.LastModifiedBy = "Admin";
               // rfqCustomField.LastModifiedOn = DateTime.Now;
                rfqCustomField.Unit = _unitname;
                var result = await Create(rfqCustomField);
                return result.Id;
            }

            public async Task<string> DeleteRfqCustomField(RfqCustomField rfqCustomField)
            {
                Delete(rfqCustomField);
                string result = $"RfqCustomField details of {rfqCustomField.Id} is deleted successfully!";
                return result;
            }
        public async Task<IEnumerable<RfqCustomField>> GetRfqCustomFieldByCustomGroup(string CustomGroup)
        {
            var getRfqCustomFieldByCustomGroupp = await FindByCondition(x => x.CustomGroupName == CustomGroup).ToListAsync();

            return getRfqCustomFieldByCustomGroupp;
        }

        public async Task<PagedList<RfqCustomField>> GetAllRfqCustomField([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            var rfqCustomFields = FindAll().OrderByDescending(x => x.Id)
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.CustomGroupName.Contains(searchParammes.SearchValue))));

            return PagedList<RfqCustomField>.ToPagedList(rfqCustomFields, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<RfqCustomField> GetRfqCustomFieldById(int id)
            {
                var getRfqCustomFieldById = await _tipsSalesServiceDbContext.RfqCustomFields.Where(x => x.Id == id).FirstOrDefaultAsync();
                return getRfqCustomFieldById;
            }

            public async Task<string> UpdateRfqCustomField(RfqCustomField rfqCustomField)
            {
                rfqCustomField.LastModifiedBy = _createdBy;
                rfqCustomField.LastModifiedOn = DateTime.Now;
                Update(rfqCustomField);
                string result = $"RfqCustomField Detail {rfqCustomField.Id} is updated successfully!";
                return result;
            }
        }
    

    public class RfqLPReleaseRepository : RepositoryBase<ReleaseLp>, IReleaseLpRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public RfqLPReleaseRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<ReleaseLp> BulkRelease(ReleaseLp releaseLp)
        {
            releaseLp.CreatedBy = _createdBy;
            releaseLp.CreatedOn = DateTime.Now;
            releaseLp.Unit = _unitname;
            var result = await Create(releaseLp);
            return result;
        }
        public async Task<IEnumerable<ReleaseLp>> GetRfqReleaseLpByRfqNumber(string RfqNumber)
        {
            var getRfqReleaseLpByRfqNumber = await _tipsSalesServiceDbContext.ReleaseLps
              .Where(x => x.RfqNumber == RfqNumber )
                        .ToListAsync();
            return getRfqReleaseLpByRfqNumber;
        }

    }

    public class UploadDocumentRepository : RepositoryBase<DocumentUpload>, IDocumentUploadRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public UploadDocumentRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task<int?> CreateUploadDocument(DocumentUpload documentUpload)
        {
            documentUpload.CreatedBy = _createdBy;
            documentUpload.CreatedOn = DateTime.Now;

            var result = await Create(documentUpload);
            return result.Id;
        }
        public async Task<List<DocumentUploadDto>> GetDownloadUrlDetails(string FileIds)
        {
            List<DocumentUploadDto> fileUploads = new List<DocumentUploadDto>();
            if (FileIds != null)
            {
                string[]? ids = FileIds.Split(',');

                for (int i = 0; i < ids.Count(); i++)
                {
                    DocumentUploadDto? getDownloadDetails = await _tipsSalesServiceDbContext.DocumentUploads
                                .Where(b => b.Id == Convert.ToInt32(ids[i]))
                                .Select(x => new DocumentUploadDto()
                                {
                                    Id = x.Id,
                                    FileName = x.FileName,
                                    FileExtension = x.FileExtension,
                                    FilePath = x.FilePath,
                                    //FileByte = x.FileByte
                                }).FirstOrDefaultAsync();
                    if (getDownloadDetails != null)
                        fileUploads.Add(getDownloadDetails);
                }
            }
            return fileUploads;
        }
    }
}
