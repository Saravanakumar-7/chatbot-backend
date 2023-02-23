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
        Task<IEnumerable<PartTypes>> GetAllPartTypes();
        Task<PartTypes> GetPartTypesById(int id);
        Task<IEnumerable<PartTypes>> GetAllActivePartTypes();
        Task<int?> CreatePartTypes(PartTypes partTypes);
        Task<string> UpdatePartTypes(PartTypes partTypes);
        Task<string> DeletePartTypes(PartTypes partTypes);
    }
}
