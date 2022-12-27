using Entities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IDemoStatusRepository : IRepositoryBase<DemoStatus>
    {
        Task<IEnumerable<DemoStatus>> GetAllDemoStatus();
        Task<DemoStatus> GetDemoStatusById(int id);
        Task<IEnumerable<DemoStatus>> GetAllActiveDemoStatus();
        Task<int?> CreateDemoStatus(DemoStatus demoStatus);
        Task<string> UpdateDemoStatus(DemoStatus demoStatus);
        Task<string> DeleteDemoStatus(DemoStatus demoStatus);
    }
}
