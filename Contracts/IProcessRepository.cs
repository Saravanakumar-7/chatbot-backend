using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IProcessRepository : IRepositoryBase<Process>
    {
        Task<IEnumerable<Process>> GetAllProcesses(SearchParames searchParams);
        Task<Process> GetProcessById(int id);
        Task<IEnumerable<Process>> GetAllActiveProcesses(SearchParames searchParams);
        Task<int?> CreateProcess(Process process);
        Task<string> UpdateProcess(Process process);
        Task<string> DeleteProcess(Process process);
    }
}
