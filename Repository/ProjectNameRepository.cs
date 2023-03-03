using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository
{
    public class ProjectNameRepository : RepositoryBase<ProjectName>, IProjectNameRepository
    {
        public ProjectNameRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        { }
        public async Task<int?> CreateProjectName(ProjectName projectName)
        {
            projectName.CreatedBy = "Admin";
            projectName.CreatedOn = DateTime.Now;
            projectName.Unit = "Bangalore";
            var result = await Create(projectName); return result.Id;
        }
        public async Task<string> DeleteProjectName(ProjectName projectName)
        {
            Delete(projectName);
            string result = $"projectName details of {projectName.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<ProjectName>> GetAllActiveProjectName()
        {
            var AllActiveProjectName = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveProjectName;
        }
        public async Task<IEnumerable<ProjectName>> GetAllProjectName()
        {
            var GetallProjectname = await FindAll().ToListAsync(); return GetallProjectname;
        }
        public async Task<ProjectName> GetProjectNameById(int id)
        {
            var ProjectnamebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); return ProjectnamebyId;
        }
        public async Task<string> UpdateProjectName(ProjectName projectName)
        {
            projectName.LastModifiedBy = "Admin";
            projectName.LastModifiedOn = DateTime.Now;
            Update(projectName);
            string result = $"projectName details of {projectName.Id} is updated successfully!";
            return result;
        }
    }
}