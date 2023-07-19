using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface IMaterialTypeRepository:IRepositoryBase<MaterialType>
    {
        Task<IEnumerable<MaterialType>> GetAllMaterialType(SearchParames searchParams);
        Task<MaterialType> GetMaterialTypeById(int id);
        Task<IEnumerable<MaterialType>> GetAllActiveMaterialType(SearchParames searchParams);
        Task<int?> CreateMaterialType(MaterialType materialType);
        Task<string> UpdateMaterialType(MaterialType materialType);
        Task<string> DeleteMaterialType(MaterialType materialType);
    }
}
