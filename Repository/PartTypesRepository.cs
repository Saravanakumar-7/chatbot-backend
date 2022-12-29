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
            partTypes.Unit = "Bangalore";
            var result = await Create(partTypes);
            
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
            var AllActivePartTypes = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivePartTypes;
        }

        public async Task<IEnumerable<PartTypes>> GetAllPartTypes()
        {
            var GetallPartTypes = await FindAll().ToListAsync();

            return GetallPartTypes;
        }

        public async Task<PartTypes> GetPartTypesById(int id)
        {
            var PartTypesbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return PartTypesbyId;
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
