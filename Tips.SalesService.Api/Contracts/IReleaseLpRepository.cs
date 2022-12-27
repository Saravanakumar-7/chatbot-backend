using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IReleaseLpRepository : IRepositoryBase<ReleaseLp>
    {
        Task<ReleaseLp> BulkRelease(ReleaseLp releaseLp);


    }
}