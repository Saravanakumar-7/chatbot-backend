using Contracts;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForeCastReleaseLpRepository : IRepositoryBase<ForeCastReleaseLp>
    {
        Task<ForeCastReleaseLp> BulkRelease(ForeCastReleaseLp foreCastReleaseLp);
    }
}
