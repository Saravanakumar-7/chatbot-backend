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
            var result = await Create(materialType);
            materialType.Unit = "Bangalore";
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
            var materialTypeList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return materialTypeList;
        }

        public async Task<IEnumerable<MaterialType>> GetAllMaterialType()
        {
            var materialTypeList = await FindAll().ToListAsync();
            return materialTypeList;
        }

        public async Task<MaterialType> GetMaterialTypeById(int id)
        {
            var materialTypeList = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return materialTypeList;
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
