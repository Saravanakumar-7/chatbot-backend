using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IHeadCountingRepository
    {
        Task<IEnumerable<HeadCounting>> GetAllHeadCount();
        Task<HeadCounting> GetHeadCountById(int id);
        Task<IEnumerable<HeadCounting>> GetAllActiveHeadCount();
        Task<int?> CreateHeadCount(HeadCounting headCounting);
        Task<string> UpdateHeadCount(HeadCounting headCounting);
        Task<string> DeleteHeadCount(HeadCounting headCounting);

    }
}
