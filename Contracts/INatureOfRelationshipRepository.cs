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
        Task<IEnumerable<NatureOfRelationship>> GetAllNatureOfRelationships(SearchParames searchParams);
        Task<NatureOfRelationship> GetNatureOfRelationshipById(int id);
        Task<IEnumerable<NatureOfRelationship>> GetAllActiveNatureOfRelationships(SearchParames searchParams);
        Task<int?> CreateNatureOfRelationship(NatureOfRelationship natureOfRelationship);
        Task<string> UpdateNatureOfRelationship(NatureOfRelationship natureOfRelationship);
        Task<string> DeleteNatureOfRelationship(NatureOfRelationship natureOfRelationship);
    }
}
