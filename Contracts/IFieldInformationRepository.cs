using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IFieldInformationRepository : IRepositoryBase<FieldInformation>
    {
        Task<PagedList<FieldInformation>> GetAllFieldInformation(PagingParameter pagingParameter, SearchParames searchParams);
        Task<IEnumerable<FieldInformation>> GetFieldInformationByIds(List<int> fieldIds);
        Task<PagedList<FieldInformation>> GetAllActiveFieldInformation(PagingParameter pagingParameter,SearchParames searchParams);
        Task<FieldInformation> CreateFieldInformation(FieldInformation fieldInformation);
        Task<string> UpdateFieldInformation(FieldInformation fieldInformation);
        Task<string> DeleteFieldInformation(FieldInformation fieldInformation);
    }
}
