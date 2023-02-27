using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IReleaseProductBomRepository : IRepositoryBase<ProductionBom>
    {
        Task<int?> CreateReleaseProductBom(ProductionBom releaseProductBom);
        Task<IEnumerable<object>> GetAllReleaseProductBomItemNumberVersionList();
       
    }
}
