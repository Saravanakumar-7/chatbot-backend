using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISalesAdditionalChargesRepository : IRepositoryBase<SalesOrderAdditionalCharges>
    {
        Task<SalesOrderAdditionalCharges> GetSalesAdditionalChargesById(int SalesOrderId, int salesAdditionalChargeId);
        Task<string> UpdateSalesAdditionalCharges(SalesOrderAdditionalCharges salesAdditionalCharges);
    }
}
