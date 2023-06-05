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
using Microsoft.AspNetCore.Mvc;

namespace Tips.SalesService.Api.Repository
{
    public class RfqSourcingRepository : RepositoryBase<RfqSourcing>, IRfqSourcingRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;

        public RfqSourcingRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;

        }

        public async Task<int?> CreateRfqSourcing(RfqSourcing rfqSourcing)
        {
            rfqSourcing.CreatedBy = "Admin";
            rfqSourcing.CreatedOn = DateTime.Now;
            rfqSourcing.Unit = "Bangalore";            
            var result = await Create(rfqSourcing);    
            
            return result.Id;
        }

        public async Task<string> DeleteRfqSourcing(RfqSourcing rfqSourcing)
        {
            Delete(rfqSourcing);
            string result = $"rfqSourcing details of {rfqSourcing.Id} is deleted successfully!";   
             return result;
            
        }

        public async Task<PagedList<RfqSourcing>> GetAllRfqSourcing([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {

            var getAllRfqSourcing = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue)
              || inv.RFQNumber.Contains(searchParammes.SearchValue) 
              || inv.CustomerName.Contains(searchParammes.SearchValue))))
                 .Include(t => t.RfqSourcingItems)
                 .ThenInclude(x => x.RfqSourcingVendors);

            return PagedList<RfqSourcing>.ToPagedList(getAllRfqSourcing, pagingParameter.PageNumber, pagingParameter.PageSize);

        }

        public async Task<RfqSourcing> GetRfqSourcingById(int id)

        {
            var rfqSourcingById = await _tipsSalesServiceDbContext.RfqSourcings.Where(x => x.Id == id)
                               .Include(t => t.RfqSourcingItems)
                               .ThenInclude(x => x.RfqSourcingVendors)
                            .FirstOrDefaultAsync();

            return rfqSourcingById;
        }

        public async Task<RfqSourcing> GetRfqSourcingDetailsByRfqNo(string rfqNo)
        {
            var rfqSourcingByRfqNo = await _tipsSalesServiceDbContext.RfqSourcings.Where(x => x.RFQNumber == rfqNo)
                              .Include(t => t.RfqSourcingItems)
                              .ThenInclude(x => x.RfqSourcingVendors)
                           .FirstOrDefaultAsync();

            return rfqSourcingByRfqNo;
        }

        public async Task<string> UpdateRfqSourcing(RfqSourcing rfqSourcing)
        {
            rfqSourcing.LastModifiedBy = "Admin";
            rfqSourcing.LastModifiedOn = DateTime.Now;
            Update(rfqSourcing);
            string result =$"rfqSourcing of Detail {rfqSourcing.Id} is updated successfully!";
            return result;
        }
    }

}
