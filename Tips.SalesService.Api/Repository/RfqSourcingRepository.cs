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

        public async Task<PagedList<RfqSourcing>> GetAllRfqSourcing(PagingParameter pagingParameter)
        {
            var rfqsourcing = PagedList<RfqSourcing>.ToPagedList(FindAll()
           .Include(t => t.rfqSourcingItems)
           .ThenInclude(x => x.rfqSourcingVendors)
           .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
            return rfqsourcing;

        }

        public async Task<RfqSourcing> GetRfqSourcingById(int id)
        {
            var rfqsourcing = await _tipsSalesServiceDbContext.RfqSourcings.Where(x => x.Id == id)
                               .Include(t => t.rfqSourcingItems)
                               .ThenInclude(x => x.rfqSourcingVendors)
                            .FirstOrDefaultAsync();

            return rfqsourcing;
        }

        public async Task<string> UpdateRfqSourcing(RfqSourcing rfqSourcing)
        {
            rfqSourcing.LastModifiedBy = "Admin";
            rfqSourcing.LastModifiedOn = DateTime.Now;
            Update(rfqSourcing);
            string result = $"rfqSourcing of Detail {rfqSourcing.Id} is updated successfully!";
            return result;
        }
    }

  

    }
