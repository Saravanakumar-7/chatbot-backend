using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IPartTypesRepository
    {
        Task<IEnumerable<PartTypes>> GetAllPartTypes(SearchParames searchParams);
        Task<PartTypes> GetPartTypesById(int id);
        Task<IEnumerable<PartTypes>> GetAllActivePartTypes(SearchParames searchParams);
        Task<int?> CreatePartTypes(PartTypes partTypes);
        Task<string> UpdatePartTypes(PartTypes partTypes);
        Task<string> DeletePartTypes(PartTypes partTypes);
    }
}
