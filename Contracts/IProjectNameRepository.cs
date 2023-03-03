using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public interface IProjectNameRepository : IRepositoryBase<ProjectName>
    {
        Task<IEnumerable<ProjectName>> GetAllProjectName();
        Task<ProjectName> GetProjectNameById(int id);
        Task<IEnumerable<ProjectName>> GetAllActiveProjectName();
        Task<int?> CreateProjectName(ProjectName projectName);
        Task<string> UpdateProjectName(ProjectName projectName);
        Task<string> DeleteProjectName(ProjectName projectName);
    }
}