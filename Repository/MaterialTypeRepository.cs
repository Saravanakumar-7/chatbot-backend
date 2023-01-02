using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class MaterialTypeRepository:RepositoryBase<MaterialType>, IMaterialTypeRepository
    {
        public MaterialTypeRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateMaterialType(MaterialType materialType)
        {
            materialType.CreatedBy = "Admin";
            materialType.CreatedOn = DateTime.Now;
            materialType.Unit = "Bangalore";
            var result = await Create(materialType);
            
            return result.Id;
        }

        public async Task<string> DeleteMaterialType(MaterialType materialType)
        {
            Delete(materialType);
            string result = $"MaterialType details of {materialType.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<MaterialType>> GetAllActiveMaterialType()
        {
            var AllActiveMaterialTypes = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveMaterialTypes;
        }

        public async Task<IEnumerable<MaterialType>> GetAllMaterialType()
        {
            var GetallmaterialTypes = await FindAll().ToListAsync();
            return GetallmaterialTypes;
        }

        public async Task<MaterialType> GetMaterialTypeById(int id)
        {
            var MaterialTypebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return MaterialTypebyId;
        }

        public async Task<string> UpdateMaterialType(MaterialType materialType)
        {
            materialType.LastModifiedBy = "Admin";
            materialType.LastModifiedOn = DateTime.Now;
            Update(materialType);
            string result = $"MaterialType details of {materialType.Id} is updated successfully!";
            return result;
        }
    }
}
