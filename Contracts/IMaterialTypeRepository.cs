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
        Task<PagedList<MaterialType>> GetAllMaterialType(PagingParameter pagingParameter, SearchParames searchParames);
        Task<MaterialType> GetMaterialTypeById(int id);
        Task<PagedList<MaterialType>> GetAllActiveMaterialType(PagingParameter pagingParameter, SearchParames searchParames);
        Task<int?> CreateMaterialType(MaterialType materialType);
        Task<string> UpdateMaterialType(MaterialType materialType);
        Task<string> DeleteMaterialType(MaterialType materialType);
    }
}
