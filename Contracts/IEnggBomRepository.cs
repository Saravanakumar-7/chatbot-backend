using Entities;
using Entities.DTOs;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEnggBomRepository : IRepositoryBase<EnggBom>
    {
        Task<PagedList<EnggBom>> GetAllEnggBOM(PagingParameter pagingParameter, SearchParames searchParams);
        Task<EnggBom> GetEnggBomById(int id);
        Task<EnggBom> GetEnggBomByItemNoAndRevNo(string itemNumber,decimal revisionNumber);
        Task<EnggBom> GetEnggBomByFgPartNumber(string fgPartNumber);
        Task<EnggBom> UpdateEnggBomVersion(EnggBom enggBom);
        Task<IEnumerable<EnggBom>> GetAllActiveEnggBom();
        Task<int?> CreateEnggBom(EnggBom enggBom);
        Task<string> UpdateEnggBom(EnggBom enggBom);
        Task<string> DeleteEnggBom(EnggBom enggBom); 
        Task<IEnumerable<EnggBomFGItemNumber>> GetAllEnggBomFGItemNoListByItemNumber(string itemNumber);
        Task <IEnumerable<object>> GetAllEnggBomItemNumberVersionList();
        Task<EnggBom> ReleasedEnggBomByItemAndRevisionNumber(string itemNumber,decimal revisionNumber);
        Task<IEnumerable<EngineeringBom>> GetAllEnggBomVersionListByItemNumber(string itemNumber);
        Task<IEnumerable<EnggBomItemDto>> GetAllEnggBOMItemNumber();
        Task<List<EnggBomFGItemNumberWithQtyDto>> GetFGBomItemsChildDetails(List<string> itemNumberList);
    }
}
