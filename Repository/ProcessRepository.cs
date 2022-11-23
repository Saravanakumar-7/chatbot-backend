using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ProcessRepository : RepositoryBase<Process>, IProcessRepository
    {
        public ProcessRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateProcess(Process process)
        {
            process.CreatedBy = "Admin";
            process.CreatedOn = DateTime.Now;
            var result = await Create(process);
            process.Unit = "Bangalore";
            return result.Id;
        }

        public async Task<string> DeleteProcess(Process process)
        {
            Delete(process);
            string result = $"Process details of {process.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Process>> GetAllActiveProcesses()
        {
            var ProcessList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return ProcessList;
        }

        public async Task<IEnumerable<Process>> GetAllProcesses()
        {
            var ProcessList = await FindAll().ToListAsync();

            return ProcessList;
        }

        public async Task<Process> GetProcessById(int id)
        {
            var process = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return process;
        }

        public async Task<string> UpdateProcess(Process process)
        {
            process.LastModifiedBy = "Admin";
            process.LastModifiedOn = DateTime.Now;
            Update(process);
            string result = $"Process details of {process.Id} is updated successfully!";
            return result;
        }
    }
}
