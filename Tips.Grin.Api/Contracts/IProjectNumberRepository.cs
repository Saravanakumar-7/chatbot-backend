using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IProjectNumberRepository
    {
        Task<IEnumerable<ProjectNumbers>> GetAllProjectNumber();
        Task<ProjectNumbers> GetProjectNumberById(int id);
        Task<IEnumerable<ProjectNumbers>> GetAllActiveProjectNumber();
        Task<int?> CreateProjectNumber(ProjectNumbers projectnumber);
        Task<string> UpdateProjectNumber(ProjectNumbers projectnumber);
        Task<string> DeleteProjectNumber(ProjectNumbers projectnumber);
    }
}
