using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Contracts
{
    public interface IMaterialIssueLocationRepository : IRepositoryBase<MaterialIssueLocation>
    {
        Task<int> CreateMaterialIssueLocation(MaterialIssueLocation materialIssueLocation);
    }
}
