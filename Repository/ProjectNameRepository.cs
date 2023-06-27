using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace Repository
{
    public class ProjectNameRepository : RepositoryBase<ProjectName>, IProjectNameRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ProjectNameRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int?> CreateProjectName(ProjectName projectName)
        {
            projectName.CreatedBy = _createdBy;
            projectName.CreatedOn = DateTime.Now;
            projectName.Unit = _unitname;
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
            projectName.LastModifiedBy = _createdBy;
            projectName.LastModifiedOn = DateTime.Now;
            Update(projectName);
            string result = $"projectName details of {projectName.Id} is updated successfully!";
            return result;
        }
    }
}