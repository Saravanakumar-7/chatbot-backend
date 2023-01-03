using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IProjectNumberRepository
    {
        Task<IEnumerable<ProjectNumber>> GetAllProjectNumber();
        Task<ProjectNumber> GetProjectNumberById(int id);
        Task<IEnumerable<ProjectNumber>> GetAllActiveProjectNumber();
        Task<int?> CreateProjectNumber(ProjectNumber projectnumber);
        Task<string> UpdateProjectNumber(ProjectNumber projectnumber);
        Task<string> DeleteProjectNumber(ProjectNumber projectnumber);
    }
}
