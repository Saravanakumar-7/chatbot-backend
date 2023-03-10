using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.DTOs;
using Entities.Helper;

namespace Contracts
{
    public interface IReleaseProductBomRepository : IRepositoryBase<ProductionBom>
    {
        Task<PagedList<ProductionBom>> GetAllProductionBom(PagingParameter pagingParameter);
        Task<ProductionBom> GetProductionBomById(int id);
        Task<int?> CreateReleaseProductBom(ProductionBom releaseProductBom);
        Task<IEnumerable<object>> GetAllReleaseProductBomItemNumberVersionList();
        Task<IEnumerable<ProductionBom>> GetAllProductionBomVersionListByItemNumber(string itemNumber);
        Task<IEnumerable<ProductionBomRevisionNumber>> GetAllProductionBomFGListByItemNumber(string itemNumber);
        Task<IEnumerable<ProductionBomRevisionNumber>> GetAllProductionBomSAListByItemNumber(string itemNumber);
    }
}
