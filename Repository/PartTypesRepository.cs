using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class PartTypesRepository : RepositoryBase<PartTypes>, IPartTypesRepository
    {
        public PartTypesRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreatePartTypes(PartTypes partTypes)
        {
            partTypes.CreatedBy = "Admin";
            partTypes.CreatedOn = DateTime.Now;
            var result = await Create(partTypes);
            partTypes.Unit = "Bangalore";
            return result.Id;
        }

        public async Task<string> DeletePartTypes(PartTypes partTypes)
        {
            Delete(partTypes);
            string result = $"PartTypes details of {partTypes.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<PartTypes>> GetAllActivePartTypes()
        {
            var PartTypeList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return PartTypeList;
        }

        public async Task<IEnumerable<PartTypes>> GetAllPartTypes()
        {
            var PartTypeList = await FindAll().ToListAsync();

            return PartTypeList;
        }

        public async Task<PartTypes> GetPartTypesById(int id)
        {
            var partTypes = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return partTypes;
        }

        public async Task<string> UpdatePartTypes(PartTypes partTypes)
        {
            partTypes.LastModifiedBy = "Admin";
            partTypes.LastModifiedOn = DateTime.Now;
            Update(partTypes);
            string result = $"PartType details of {partTypes.Id} is updated successfully!";
            return result;
        }
    }
}
