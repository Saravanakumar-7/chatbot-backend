using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IReleaseLpRepository : IRepositoryBase<ReleaseLp>
    {
        Task<ReleaseLp> BulkRelease(ReleaseLp releaseLp);

        Task<ReleaseLp> GetRfqReleaseLpByRfqNumber(string RfqNumber);
    }
}