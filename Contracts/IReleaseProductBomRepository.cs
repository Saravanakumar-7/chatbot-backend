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
        Task<PagedList<ProductionBom>> GetAllProductionBom(PagingParameter pagingParameter,SearchParames searchParams);
        Task<ProductionBom> GetProductionBomById(int id);
        Task<int?> CreateReleaseProductBom(ProductionBom releaseProductBom);
        //Task<int> GetLatestProBomCountByItemNumber(string itemNumber);
        Task<Dictionary<string, decimal>> GetSAsAndLatestVersion();
        Task<List<ProductionBom>?> GetLatestProBomCountByItemNumber(string itemNumber);
        //Task<int> GetLatestProBomCountByItemNumber(string itemNumber); 
        Task<decimal> GetLatestProductionBomByItemNumber(string itemNumber);
        Task<IEnumerable<ProductionBom>> GetLatestProBomByItemNumber(string itemNumber); 
        Task<IEnumerable<object>> GetAllReleaseProductBomItemNumberVersionList(); 
        Task<IEnumerable<ProductionBom>> GetAllProductionBomVersionListByItemNumber(string itemNumber); 
        Task<IEnumerable<ProductionBomRevisionNumber>> GetAllProductionBomFGListByItemNumber(string itemNumber);
        Task<ProductionBomRevisionNumberAndQty> GetAllProductionBomSAListByItemNumber(string itemNumber); 
        Task<EnggBom> GetProductionBomByItemAndBomVersionNo(string itemNumber, decimal bomVersionNo);
        Task<Dictionary<string, decimal>> GetFGsAndLatestVersion();
        Task<Dictionary<string, decimal>> GetSAsAndLatestVersionbyItemNo(string itemNumber);
        Task<IEnumerable<ReleaseProductionBomSPReport>> GetBOMReleaseSPReportWithParamForTrans(string? ItemNumber);
        Task<IEnumerable<ProductionBomKitRevNoDto>> GetProductionBomReleasedKitNoAndLatestVersion();

    }
}
