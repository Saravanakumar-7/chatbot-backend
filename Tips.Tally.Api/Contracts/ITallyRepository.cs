using Entities;
using Tips.Tally.Api.Entities.DTOs;

namespace Tips.Tally.Api.Contracts
{
    public interface ITallyRepository : ITallyRepositoryBase<TallyVendorMasterSpReport>
    {
        Task<IEnumerable<TallyVendorMasterSpReport>> GetTallyVendorMasterSpReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<TallyCurrencyMasterSPReport>> GetTallyCurrencyMastertSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<TallyCustomerMasterSpReport>> GetTallyCustomerMastertSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<TallyPurchaseOrderSpReport>> GetTallyPurchaseOrderSpReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<TallyStockItemSPReport>> GetTallyStockItemSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
    }
}
