using Microsoft.EntityFrameworkCore;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Repository
{
    public class SalesAdditionalChargesRepository : RepositoryBase<SalesOrderAdditionalCharges>, ISalesAdditionalChargesRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContexts;
        public SalesAdditionalChargesRepository(TipsSalesServiceDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsSalesServiceDbContexts = repositoryContext;
        }

        public async Task<SalesOrderAdditionalCharges> GetSalesAdditionalChargesById(int SalesOrderId,int salesAdditionalChargeId)
        {
           
                var salesAdditionalChargesDetailsById = await _tipsSalesServiceDbContexts.SalesOrderAdditionalCharges
                     .Where(x => x.SalesOrderId == SalesOrderId && x.Id == salesAdditionalChargeId)
                              .FirstOrDefaultAsync();

                return salesAdditionalChargesDetailsById;
            
        }

        public async Task<string> UpdateSalesAdditionalCharges(SalesOrderAdditionalCharges salesAdditionalCharges)
        {
            Update(salesAdditionalCharges);
            string result = $"SalesAdditionalCharges of Detail {salesAdditionalCharges.Id} is updated successfully!";
            return result;
        }

    }
}
