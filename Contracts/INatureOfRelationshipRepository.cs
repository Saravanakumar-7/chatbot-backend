using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface INatureOfRelationshipRepository : IRepositoryBase<NatureOfRelationship>

    {
        Task<PagedList<NatureOfRelationship>> GetAllNatureOfRelationships(PagingParameter pagingParameter, SearchParames searchParames);
        Task<NatureOfRelationship> GetNatureOfRelationshipById(int id);
        Task<PagedList<NatureOfRelationship>> GetAllActiveNatureOfRelationships(PagingParameter pagingParameter, SearchParames searchParames);
        Task<int?> CreateNatureOfRelationship(NatureOfRelationship natureOfRelationship);
        Task<string> UpdateNatureOfRelationship(NatureOfRelationship natureOfRelationship);
        Task<string> DeleteNatureOfRelationship(NatureOfRelationship natureOfRelationship);
    }
}
