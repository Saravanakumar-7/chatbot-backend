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
        Task<PagedList<EnggBom>> GetAllEnggBOM(PagingParameter pagingParameter);
        Task<EnggBom> GetEnggBomById(int id);
        Task<EnggBom> GetEnggBomByFgPartNumber(string fgPartNumber);
        Task<IEnumerable<EnggBom>> GetAllActiveEnggBom();
        Task<int?> CreateEnggBom(EnggBom enggBom);
        Task<string> UpdateEnggBom(EnggBom enggBom);
        Task<string> DeleteEnggBom(EnggBom enggBom);
        Task<EnggBom> UpdateEnggBomVersion(EnggBom enggBom);
        Task <IEnumerable<object>> GetAllEnggBomItemNumberVersionList();
        Task<EnggBom> ReleasedEnggBomByItemAndRevisionNumber(string itemNumber,decimal revisionNumber);
    }
}
