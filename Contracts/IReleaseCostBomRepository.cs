using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IReleaseCostBomRepository : IRepositoryBase<ReleaseCostBom>
    {
        Task<int?> CreateReleaseCostBom(ReleaseCostBom releaseCostBom);
    }
}
