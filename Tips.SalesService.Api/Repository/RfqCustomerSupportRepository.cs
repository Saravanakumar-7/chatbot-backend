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
         

        public async Task<PagedList<RfqCustomerSupport>> GetAllRfqCustomerSupport(PagingParameter pagingParameter)
       {
           var rfqCustomerSupport = PagedList<RfqCustomerSupport>.ToPagedList(FindAll()
           .Include(t => t.rfqCustomerSupportItems)
           .ThenInclude(u=>u.rfqCSDeliverySchedule)
           .Include(x=>x.rfqCustomerSupportNotes)
           .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return rfqCustomerSupport;
        } 


        public async Task<RfqCustomerSupport> GetRfqCustomerSupportById(int id)
        {
            var rfqCustomerSupport = await _tipsSalesServiceDbContext.rfqCustomerSupports.Where(x => x.Id == id)
                              .Include(t => t.rfqCustomerSupportItems)
                              .ThenInclude(n=>n.rfqCSDeliverySchedule)
                           .Include(m=>m.rfqCustomerSupportNotes)
                           .FirstOrDefaultAsync();

            return rfqCustomerSupport; 
        }          

        //public async Task<string> ActivateRfqCustomerSupportItemById(RfqCustomerSupportItems rfqCustomerSupportItems)
        //{
        //    rfqCustomerSupportItems.LastModifiedBy = "Admin";
        //    rfqCustomerSupportItems.LastModifiedOn = DateTime.Now;
        //    Update(rfqCustomerSupportItems);
        //    string result = $"CostCenter details of {rfqCustomerSupportItems.Id} is updated successfully!";
        //    return result;
        //}

        public async Task<RfqCustomerSupport> RfqCustomerSupportByRfqNumber(string RfqNumber)
        {
            var csByRfqNumber = await _tipsSalesServiceDbContext.rfqCustomerSupports
                .Include(t => t.rfqCustomerSupportItems)
                .ThenInclude(n => n.rfqCSDeliverySchedule)
                .Include(m => m.rfqCustomerSupportNotes)
              .Where(x => x.RfqNumber == RfqNumber)
                        .FirstOrDefaultAsync();
            return csByRfqNumber;
        }

        public async Task<string> UpdateRfqCustomerSupport(RfqCustomerSupport rfqCustomerSupport)
        {
            rfqCustomerSupport.LastModifiedBy = "Admin";
            rfqCustomerSupport.LastModifiedOn = DateTime.Now;
            Update(rfqCustomerSupport);
            string result = $"RFQ of Detail {rfqCustomerSupport.Id} is updated successfully!";
            return result;
        }

     
        //public async Task<string> ActivateRfqCustomerSupportItemById(RfqCustomerSupportItems rfqCustomerSupportItems)
        //{
        //       rfqCustomerSupportItems.LastModifiedBy = "Admin";
        //       rfqCustomerSupportItems.LastModifiedOn = DateTime.Now;
        //       Update(rfqCustomerSupportItems);
        //       string result = $"CostCenter details of {rfqCustomerSupportItems.Id} is updated successfully!";
        //       return result;
        //}
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
            string result = $"CostCenter details of {rfqCustomerSupportItems.Id} is updated successfully!";
            return result;
        }

        public Task<int?> CreateRfqCustomerSupportItem(RfqCustomerSupportItems rfqCustomerSupportItems)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteRfqCustomerSupportItem(RfqCustomerSupportItems rfqCustomerSupportItems)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RfqCustomerSupportItems>> GetAllActiveRfqCustomerSupportItemsByRfqNumber(string rfqNumber)
        {
            IEnumerable<RfqCustomerSupportItems> csByRfqNumber = await _tipsSalesServiceDbContext.rfqCustomerSupportItems
             .Where(x => x.RfqNumber == rfqNumber && x.ReleaseStatus == true).ToListAsync();

            return csByRfqNumber;
        }

        public Task<IEnumerable<RfqCustomerSupportItems>> GetAllRfqCustomerSupportItem()
        {
            throw new NotImplementedException();
        }

        public async Task<RfqCustomerSupportItems> GetRfqCustomerSupportItemById(int id)
        {
            var getActiveStatus = await _tipsSalesServiceDbContext.rfqCustomerSupportItems.Where(x => x.Id == id && x.ReleaseStatus == false).FirstOrDefaultAsync();
            return getActiveStatus;
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
        public async Task<int?> CreateRfq(Rfq rfq)
        {
            rfq.CreatedBy = "Admin";
            rfq.CreatedOn = DateTime.Now;
            rfq.Unit = "Bangalore";
            var result = await Create(rfq);
            return result.Id;
        }

        public async Task<string> DeleteRfq(Rfq rfq)
        {
            Delete(rfq);
            string result = $"RFQ details of {rfq.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<RfqNumberListDto>> GetAllActiveRfqNumberList()
        {
            IEnumerable<RfqNumberListDto> rfqlistDetails = await _tipsSalesServiceDbContext.rfqs
                                .Select(x => new RfqNumberListDto()
                                {
                                    Id = x.Id,
                                    RfqNumber = x.RfqNumber,
                                    CustomerName = x.CustomerName
                                })
                              .ToListAsync();

            return rfqlistDetails;
        }

        public async Task<PagedList<Rfq>> GetAllRfq(PagingParameter pagingParameter)
        {
            var rfq = PagedList<Rfq>.ToPagedList(FindAll()
           .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return rfq;
        }

        public async Task<Rfq> GetRfqById(int id)
        {
            var rfq = await _tipsSalesServiceDbContext.rfqs.Where(x => x.Id == id)
                          .FirstOrDefaultAsync();

            return rfq;
        }

        //public async Task<Rfq> RfqCustomerSupportByRfqNumber(int RfqNumber)
        //{
        //    var csByRfqNumber = await _tipsSalesServiceDbContext.rfqCustomerSupports
        //        .Where(x => x.RfqNumber == RfqNumber)
        //                  .FirstOrDefaultAsync();
        //    return csByRfqNumber;
        //}

        public async Task<string> UpdateRfq(Rfq rfq)
        {
            rfq.LastModifiedBy = "Admin";
            rfq.LastModifiedOn = DateTime.Now;
            Update(rfq);
            string result = $"RFQ of Detail {rfq.Id} is updated successfully!";
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


        public async Task<PagedList<RfqEngg>> GetAllRfqEngg(PagingParameter pagingParameter)
        {
            var rfqengg = PagedList<RfqEngg>.ToPagedList(FindAll()
            .Include(t => t.rfqEnggItems)            
            .Include(x => x.rfqEnggRiskIdentifications)
            .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return rfqengg;
        }


        public async Task<RfqEngg> GetRfqEnggById(int id)
        {
            var rfqengg = await _tipsSalesServiceDbContext.RfqEnggs.Where(x => x.Id == id)
                              .Include(t => t.rfqEnggItems)                              
                           .Include(m => m.rfqEnggRiskIdentifications)
                           .FirstOrDefaultAsync();

            return rfqengg;
        }       
        public async Task<RfqEngg> RfqEnggByRfqNumber(string RfqNumber)
        {
            var EnggByRfqNumber = await _tipsSalesServiceDbContext.RfqEnggs
                .Include(t => t.rfqEnggItems)                
                .Include(m => m.rfqEnggRiskIdentifications)
              .Where(x => x.RFQNumber == RfqNumber)
                        .FirstOrDefaultAsync();
            return EnggByRfqNumber;
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
    // RfqLPCosting
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
        public async Task<PagedList<RfqLPCosting>> GetAllRfqLPCosting(PagingParameter pagingParameter)
        {
            var rfqLPCosting = PagedList<RfqLPCosting>.ToPagedList(FindAll()
                .Include (x => x.rfqLPCostingItems)
                .ThenInclude(u => u.rfqLPCostingProcesses)
                 .Include(x => x.rfqLPCostingItems)
                 .ThenInclude(v => v.rfqLPCostingNREConsumables)
                 .Include(x => x.rfqLPCostingItems)
                 .ThenInclude(w => w.rfqLPCostingOtherCharges)
           .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return rfqLPCosting;
        }

        public async Task<RfqLPCosting> GetRfqLPCostingById(int id)
        {
            var rfqLPCosting = await _tipsSalesServiceDbContext.RfqLPCostings.Where(x => x.Id == id)
                 .Include(x => x.rfqLPCostingItems)
                 .ThenInclude(u => u.rfqLPCostingProcesses)
                 .Include(x => x.rfqLPCostingItems)
                 .ThenInclude(v => v.rfqLPCostingNREConsumables)
                 .Include(x => x.rfqLPCostingItems)
                 .ThenInclude(w => w.rfqLPCostingOtherCharges)
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
        public async Task<RfqLPCosting> RfqLPCostingByRfqNumber(string RfqNumber)
        {
            var LpCostingByRfqNumber = await _tipsSalesServiceDbContext.RfqLPCostings
                .Include(t => t.rfqLPCostingItems)
              .Where(x => x.RfqNumber == RfqNumber)
                        .FirstOrDefaultAsync();
            return LpCostingByRfqNumber;
        }    

    }
}
