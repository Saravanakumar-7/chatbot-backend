using Entities;
using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IDemoStatusRepository : IRepositoryBase<DemoStatus>
    {
        Task<PagedList<DemoStatus>> GetAllDemoStatus(PagingParameter pagingParameter, SearchParames searchParams);
        Task<DemoStatus> GetDemoStatusById(int id);
        Task<PagedList<DemoStatus>> GetAllActiveDemoStatus(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateDemoStatus(DemoStatus demoStatus);
        Task<string> UpdateDemoStatus(DemoStatus demoStatus);
        Task<string> DeleteDemoStatus(DemoStatus demoStatus);
    }
}
