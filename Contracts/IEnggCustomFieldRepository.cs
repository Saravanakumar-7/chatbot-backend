using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface IEnggCustomFieldRepository:IRepositoryBase<EnggCustomField>
    {
        Task<PagedList<EnggCustomField>> GetAllEnggCustomFields(PagingParameter pagingParameter, SearchParames searchParams);
        Task<EnggCustomField> GetEnggCustomFieldById(int id);
        Task<PagedList<EnggCustomField>> GetAllActiveEnggCustomFields(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateEnggCustomField(EnggCustomField enggcustomFields);
        Task<string> UpdateEnggCustomField(EnggCustomField enggcustomFields);
        Task<string> DeleteEnggCustomField(EnggCustomField enggcustomFields);
        Task<IEnumerable<EnggCustomField>> GetEnggCustomFieldByBomGroup(string BomgroupName);
    }
}
