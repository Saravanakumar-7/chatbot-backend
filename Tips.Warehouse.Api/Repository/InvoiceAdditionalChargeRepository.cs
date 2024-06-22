using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Repository
{
    public class InvoiceAdditionalChargeRepository : RepositoryBase<InvoiceAdditionalCharges>, IInvoiceAdditionalChargeRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContexts;

        public InvoiceAdditionalChargeRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
        }
        public async Task<InvoiceAdditionalCharges> GetInvoiceAdditionalChargesDetailsById(int? SalesOrderId, int InvoiceAdditionalChargeId)
        {
            var invoiceAdditionalChargeDetails = await _tipsWarehouseDbContext.InvoiceAdditionalCharges
                    .Where(x => x.SalesOrderId == SalesOrderId && x.Id == InvoiceAdditionalChargeId)
                          .FirstOrDefaultAsync();

            return invoiceAdditionalChargeDetails;
        }

        public async Task<string> UpdateInvoiceAdditionalChargesDetails(InvoiceAdditionalCharges InvoiceAdditionalCharge)
        {
            Update(InvoiceAdditionalCharge);
            string result = $"InvoiceAdditionalCharge details of {InvoiceAdditionalCharge.Id} is updated successfully!";
            return result;
        }
    }
}
