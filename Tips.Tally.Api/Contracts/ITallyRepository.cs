using Entities;
using Tips.Tally.Api.Entities.DTOs;

namespace Tips.Tally.Api.Contracts
{
    public interface ITallyRepository : ITallyRepositoryBase<TallyVendorMasterSpReport>
    {
        Task<IEnumerable<TallyVendorMasterSpReport>> GetTallyVendorMasterSpReportWithDate(DateTime? FromDate, DateTime? ToDate);
    }
}
