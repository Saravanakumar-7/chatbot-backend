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

namespace Tips.SalesService.Api.Repository
{
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
            var result = await Create(rfq);
            return result.Id;
        }

        public async Task<string> DeleteRfq(Rfq rfq)
        {
            Delete(rfq);
            string result = $"RFQ details of {rfq.Id} is deleted successfully!";
            return result; 
        }

        public async Task<PagedList<Rfq>> GetAllRfq(PagingParameter pagingParameter)             
        {
            var rfq = PagedList<Rfq>.ToPagedList(FindAll()
           .Include(t => t.rfqCustomerSupports)
           .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);           
            return rfq;
             
        }
         
        public async Task<Rfq> GetRfqById(int id)
        {
            var rfq = await _tipsSalesServiceDbContext.rfqs.Where(x => x.Id == id)
                               .Include(t => t.rfqCustomerSupports)
                               //.Include(m=>m.rfqNotes)
                            .FirstOrDefaultAsync();

            return rfq;
            //var rfqById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            //return rfqById;
        }

        public async Task<string> UpdateRfq(Rfq rfq)
        {
            rfq.LastModifiedBy = "Admin";
            rfq.LastModifiedOn = DateTime.Now;
            Update(rfq);
            string result = $"RFQ of Detail {rfq.Id} is updated successfully!";
            return result;
            //rfq.LastModifiedBy = "Admin";
            //rfq.LastModifiedOn = DateTime.Now;
            //Update(rfq);
            //string result = $"RFQ Detail {rfq.Id} is updated successfully!";
            //return result;
        }
        
    }
}
