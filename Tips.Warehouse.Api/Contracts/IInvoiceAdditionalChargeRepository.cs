using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IInvoiceAdditionalChargeRepository : IRepositoryBase<InvoiceAdditionalCharges>
    {
        Task<InvoiceAdditionalCharges> GetInvoiceAdditionalChargesDetailsById(int? SalesOrderId, int InvoiceAdditionalChargeId);
        Task<string> UpdateInvoiceAdditionalChargesDetails(InvoiceAdditionalCharges InvoiceAdditionalCharge);

    }
}
