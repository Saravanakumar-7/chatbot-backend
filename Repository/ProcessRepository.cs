using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ProcessRepository : RepositoryBase<Process>, IProcessRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ProcessRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateProcess(Process process)
        {
            process.CreatedBy = _createdBy;
            process.CreatedOn = DateTime.Now;
            process.Unit = _unitname;
            var result = await Create(process);

            return result.Id;
        }

        public async Task<string> DeleteProcess(Process process)
        {
            Delete(process);
            string result = $"Process details of {process.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Process>> GetAllActiveProcesses([FromQuery] SearchParames searchParams)
        {
            var processDetails = FindAll()
                             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ProcessName.Contains(searchParams.SearchValue) ||
                                    inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return processDetails;
        }

        public async Task<IEnumerable<Process>> GetAllProcesses([FromQuery] SearchParames searchParams)
        {
            var processDetails = FindAll()
                            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ProcessName.Contains(searchParams.SearchValue) ||
                                   inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return processDetails;
        }

        public async Task<Process> GetProcessById(int id)
        {
            var ProcessById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return ProcessById;
        }

        public async Task<string> UpdateProcess(Process process)
        {
            process.LastModifiedBy = _createdBy;
            process.LastModifiedOn = DateTime.Now;
            Update(process);
            string result = $"Process details of {process.Id} is updated successfully!";
            return result;
        }
    }
}
