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

namespace Tips.SalesService.Api.Repository
{
    public class RfqCustomerSupportRepository : RepositoryBase<RfqCustomerSupport>, IRfqCustomerSupportRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public RfqCustomerSupportRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<int?> CreateRfqCustomerSupport(RfqCustomerSupport rfqCustomerSupport)
        {
            rfqCustomerSupport.CreatedBy = "Admin";
            rfqCustomerSupport.CreatedOn = DateTime.Now;
            rfqCustomerSupport.Unit = "Bangalore";
           
            var result = await Create(rfqCustomerSupport);
            return result.Id;
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
            rfqCustomerSupport.LastModifiedBy = "Admin";
            rfqCustomerSupport.LastModifiedOn = DateTime.Now;            
            Update(rfqCustomerSupport);
            string result = $"RFQ of Detail {rfqCustomerSupport.Id} is updated successfully!";
            return result;
        }

     
        
    }

    public class RfqCustomerSupportItemsRepository : RepositoryBase<RfqCustomerSupportItems>, IRfqCustomerSupportItemRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;

        public RfqCustomerSupportItemsRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;

        }

        public async Task<string> ActivateRfqCustomerSupportItemById(RfqCustomerSupportItems rfqCustomerSupportItems)
        {
            rfqCustomerSupportItems.LastModifiedBy = "Admin";
            rfqCustomerSupportItems.LastModifiedOn = DateTime.Now;
            Update(rfqCustomerSupportItems);
            string result = $"CustomerSupport details of {rfqCustomerSupportItems.Id} is updated successfully!";
            return result;
        }

        public Task<int?> CreateRfqCustomerSupportItem(RfqCustomerSupportItems rfqCustomerSupportItems)
        {
            throw new NotImplementedException();
        }

        public async Task<string> DeactivateRfqCustomerSupportItemById(RfqCustomerSupportItems rfqCustomerSupportItems)
        {
            rfqCustomerSupportItems.LastModifiedBy = "Admin";
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
            IEnumerable<RfqCustomerSupportItems> getAllActiveRfqCSItemsByRfqNumber = await _tipsSalesServiceDbContext.RfqCustomerSupportItems
             .Where(x => x.RfqNumber == rfqNumber && x.ReleaseStatus == true).OrderByDescending(x => x.Id).ToListAsync();

            return getAllActiveRfqCSItemsByRfqNumber;
        }

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

        public RfqRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;

        }
        public async Task<Rfq> RfqSourcingByRfqNumbers(string RfqNumber)
        {
            var SourcingByRfqNumber = await _tipsSalesServiceDbContext.Rfqs
              .Where(x => x.RfqNumber == RfqNumber)
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
        public async Task<Rfq> RfqEnggByRfqNumbers(string RfqNumber)
        {
            var rfqEnggByRfqNumber = await _tipsSalesServiceDbContext.Rfqs
              .Where(x => x.RfqNumber == RfqNumber)
                        .FirstOrDefaultAsync();
            return rfqEnggByRfqNumber;
        }


        public async Task<Rfq> RfqDetailsByRfqNumbers(string rfqNumber)
        {
            var rfqDetailsByRfqNumber = await _tipsSalesServiceDbContext.Rfqs
              .Where(x => x.RfqNumber == rfqNumber)
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
            rfq.CreatedBy = "Admin";
            rfq.CreatedOn = date.Date;
            var version = 1;
            rfq.RevisionNumber = version;
            //Guid rfqNumber = Guid.NewGuid();
            //rfq.RfqNumber = "RFQ-" + rfqNumber.ToString();
            rfq.Unit = "Bangalore";
            var result = await Create(rfq);
            return result.Id;
        }
        public async Task<int?> GetRfqNumberAutoIncrementCount(DateTime date)
        {
            var getRfqNumberAutoIncrementCount = _tipsSalesServiceDbContext.Rfqs.Where(x => x.CreatedOn == date.Date).Count();

            return getRfqNumberAutoIncrementCount;
        }

        public async Task<string> DeleteRfq(Rfq rfq)
        {
            Delete(rfq);
            string result = $"RFQ details of {rfq.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<RfqNumberListDto>> GetAllActiveRfqNumberList()
        {
            IEnumerable<RfqNumberListDto> getAllActiveRfqNumberList = await _tipsSalesServiceDbContext.Rfqs
                                .Select(x => new RfqNumberListDto()
                                {
                                    Id = x.Id,
                                    RfqNumber = x.RfqNumber,
                                    CustomerName = x.CustomerName
                                })
                              .OrderByDescending(x => x.Id).ToListAsync();

            return getAllActiveRfqNumberList;
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
            //var rfqDetails = FindAll().OrderByDescending(x => x.Id)
            //    .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.RfqNumber.Contains(searchParammes.SearchValue)
            //    || inv.CustomerName.Contains(searchParammes.SearchValue) 
            //    || inv.RevisionNumber.Equals(int.Parse(searchParammes.SearchValue))
            //     )));

            int searchValueInt;
            bool isSearchValueInt = int.TryParse(searchParammes.SearchValue, out searchValueInt);

            var rfqDetails = FindAll()
                .Where(inv =>
                    (string.IsNullOrWhiteSpace(searchParammes.SearchValue) ||
                    inv.RfqNumber.Contains(searchParammes.SearchValue) ||
                    inv.CustomerName.Contains(searchParammes.SearchValue)) &&
                    (!isSearchValueInt || inv.RevisionNumber == searchValueInt) && inv.IsModified == false)
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
            rfq.LastModifiedBy = "Admin";
            rfq.LastModifiedOn = DateTime.Now;
            
            Update(rfq);
            string result = $"RFQ of Detail {rfq.Id} is updated successfully!";
            return result;
        }

        public async Task<IEnumerable<RfqNumberListDto>> GetAllActiveRfqNumberListByCustomerId(string CustomerId)
        {           

            IEnumerable<RfqNumberListDto> getAllActiveRfqNumberList = await _tipsSalesServiceDbContext.Rfqs
                                .Select(x => new RfqNumberListDto()
                                {
                                    Id = x.Id,
                                    RfqNumber = x.RfqNumber,
                                    CustomerName = x.CustomerName,
                                    CustomerId = x.CustomerId

                                })
                               .Where(b => b.CustomerId == CustomerId)                                
                              .ToListAsync();

            return getAllActiveRfqNumberList;


           
        }

        public async Task<Rfq> GetCustomerIdByRfqNumber(string rfqnumber)
        {
            var getCustomerId = await _tipsSalesServiceDbContext.Rfqs
                        .Where(x => x.RfqNumber == rfqnumber)
                                  .FirstOrDefaultAsync();
            return getCustomerId;
        }

        public async Task<Rfq> UpdateRfqRevNo(Rfq rfq)
        {
            //rfq.CreatedBy = "Admin";
            //rfq.CreatedOn = DateTime.Now;
            //rfq.Unit = "Bangalore";
            //var getOldRevisionNumber = _tipsSalesServiceDbContext.Rfqs
            //    .Where(x => x.RfqNumber == rfq.RfqNumber)
            //    .OrderByDescending(x => x.Id)
            //    .Select(x => x.RevisionNumber)
            //    .FirstOrDefault();

            //rfq.RevisionNumber = getOldRevisionNumber;
            //var result = await Create(rfq);
            //return result;

            var getOldRfqDetails = _tipsSalesServiceDbContext.Rfqs
                .Where(x => x.RfqNumber == rfq.RfqNumber && x.IsModified == false)
                .FirstOrDefault();

            if (getOldRfqDetails != null)
            {
                getOldRfqDetails.IsModified = true;
                getOldRfqDetails.LastModifiedBy = "Admin";
                getOldRfqDetails.LastModifiedOn = DateTime.Now;
                Update(getOldRfqDetails);
            }

            rfq.CreatedBy = rfq.CreatedBy;
            rfq.CreatedOn = rfq.CreatedOn;
            rfq.LastModifiedBy = "Admin";
            rfq.LastModifiedOn = DateTime.Now;
            var getOldRevisionNumber = _tipsSalesServiceDbContext.Rfqs
                .Where(x => x.RfqNumber == rfq.RfqNumber)
                .OrderByDescending(x => x.Id)
                .Select(x => x.RevisionNumber)
                .FirstOrDefault();

            rfq.RevisionNumber = (getOldRevisionNumber + 1);
            var result = await Create(rfq);
            return result;

        }
    }
    public class RfqEnggRepository : RepositoryBase<RfqEngg>, IRfqEnggRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public RfqEnggRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<int?> CreateRfqEngg(RfqEngg rfqEngg)
        {
            rfqEngg.CreatedBy = "Admin";
            rfqEngg.CreatedOn = DateTime.Now;
            rfqEngg.Unit = "Bangalore";            
            var result = await Create(rfqEngg);
            return result.Id;
        }

        public async Task<string> DeleteRfqEngg(RfqEngg rfqEngg)
        {
            Delete(rfqEngg);
            string result = $"RFQEngg details of {rfqEngg.Id} is deleted successfully!";
            return result;
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
        public async Task<RfqEngg> GetRfqEnggByRfqNumber(string RfqNumber)
        {
            var getRfqEnggByRfqNumber = await _tipsSalesServiceDbContext.RfqEnggs
                .Include(t => t.RfqEnggItems)                
                .Include(m => m.RfqEnggRiskIdentifications)
              .Where(x => x.RFQNumber == RfqNumber)
                        .FirstOrDefaultAsync();
            return getRfqEnggByRfqNumber;
        }

        public Task<IEnumerable<RfqEnggItem>> GetRfqEnggItemsByRfqNumber(string rfqNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UpdateRfqEngg(RfqEngg rfqEngg)
        {
            rfqEngg.LastModifiedBy = "Admin";
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

        public async Task<string> ActivateRfqEnggItemById(RfqEnggItem rfqEnggItem)
        {
            // rfqEnggItem.LastModifiedBy = "Admin";
            //rfqEnggItem.LastModifiedOn = DateTime.Now;
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


        public async Task<IEnumerable<RfqEnggItem>> GetAllActiveRfqEnggItemByRfqNumber(string rfqNumber)
        {
            int poId = await _tipsSalesServiceDbContext.RfqEnggs
              .Where(s => s.RFQNumber == rfqNumber).Select(x => x.Id).FirstOrDefaultAsync();

            IEnumerable<RfqEnggItem> rfqEnggItems = await _tipsSalesServiceDbContext.RfqEnggItems
                 .Where(x => x.RfqEnggId == poId)
             .Where(x => x.ReleaseStatus == true).ToListAsync();

            return rfqEnggItems;
        }

        public async Task<IEnumerable<RfqEnggItem>> GetRfqEnggRelesedDetailsByRfqNumber(string rfqNumber)
        {
            List<int> poId = await _tipsSalesServiceDbContext.RfqEnggs
              .Where(s => s.RFQNumber == rfqNumber).Select(x => x.Id).Distinct().ToListAsync();

            var rfqEnggRelesedDetails = await _tipsSalesServiceDbContext.RfqEnggItems.Where(x => poId.Contains(x.RfqEnggId)
            && x.ReleaseStatus == true).ToListAsync();
            return rfqEnggRelesedDetails;
        }

        public async Task<IEnumerable<RfqEnggItem>> GetRfqEnggCountByRfqNumber(string rfqNumber)
        {
            List<int> poId = await _tipsSalesServiceDbContext.RfqEnggs
              .Where(s => s.RFQNumber == rfqNumber).Select(x => x.Id).Distinct().ToListAsync();

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

            public RfqLPCostingRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
            {
                _tipsSalesServiceDbContext = tipsSalesServiceDbContext;

            }
            public async Task<int?> CreateRfqLPCosting(RfqLPCosting rfqLPCosting)
            {
                rfqLPCosting.CreatedBy = "Admin";
                rfqLPCosting.CreatedOn = DateTime.Now;
                rfqLPCosting.Unit = "Bangalore";
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
                rfqLPCosting.LastModifiedBy = "Admin";
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

            public RfqCustomGroupRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
            {
                _tipsSalesServiceDbContext = tipsSalesServiceDbContext;

            }

            public async Task<int?> CreateRfqCustomGroup(RfqCustomGroup rfqCustomGroup)
            {
                rfqCustomGroup.CreatedBy = "Admin";
                rfqCustomGroup.CreatedOn = DateTime.Now;
                rfqCustomGroup.LastModifiedBy = "Admin";
                rfqCustomGroup.LastModifiedOn = DateTime.Now;
                rfqCustomGroup.Unit = "Bangalore";
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
                rfqCustomGroup.LastModifiedBy = "Admin";
                rfqCustomGroup.LastModifiedOn = DateTime.Now;
                Update(rfqCustomGroup);
                string result = $"RfqCustomGroup Detail {rfqCustomGroup.Id} is updated successfully!";
                return result;
            }
        }
    public class RfqCustomFieldRepository : RepositoryBase<RfqCustomField>, IRfqCustomFieldRepository
        {
            private TipsSalesServiceDbContext _tipsSalesServiceDbContext;

            public RfqCustomFieldRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
            {
                _tipsSalesServiceDbContext = tipsSalesServiceDbContext;

            }

            public async Task<int?> CreateRfqCustomField(RfqCustomField rfqCustomField)
            {
                rfqCustomField.CreatedBy = "Admin";
                rfqCustomField.CreatedOn = DateTime.Now;
                rfqCustomField.LastModifiedBy = "Admin";
                rfqCustomField.LastModifiedOn = DateTime.Now;
                rfqCustomField.Unit = "Bangalore";
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
                rfqCustomField.LastModifiedBy = "Admin";
                rfqCustomField.LastModifiedOn = DateTime.Now;
                Update(rfqCustomField);
                string result = $"RfqCustomField Detail {rfqCustomField.Id} is updated successfully!";
                return result;
            }
        }
    

    public class RfqLPReleaseRepository : RepositoryBase<ReleaseLp>, IReleaseLpRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;

        public RfqLPReleaseRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }

        public async Task<ReleaseLp> BulkRelease(ReleaseLp releaseLp)
        {
            releaseLp.CreatedBy = "Admin";
            releaseLp.CreatedOn = DateTime.Now;
            releaseLp.Unit = "Bangalore";
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

 
    }
