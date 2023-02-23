using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    internal class PackingInstructionRepository : RepositoryBase<PackingInstruction>, IPackingInstructionRepository
    {
        public PackingInstructionRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreatePackingInstruction(PackingInstruction packingInstruction)
        {
            packingInstruction.CreatedBy = "Admin";
            packingInstruction.CreatedOn = DateTime.Now;
            packingInstruction.Unit = "Bangalore";
            var result = await Create(packingInstruction);
            
            return result.Id;
        }

        public async Task<string> DeletePackingInstruction(PackingInstruction packingInstruction)
        {
            Delete(packingInstruction);
            string result = $"AuditFrequency details of {packingInstruction.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<PackingInstruction>> GetAllActivePackingInstruction()
        {

            var AllActivePackingInstructions = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivePackingInstructions;
        }

        public async Task<IEnumerable<PackingInstruction>> GetAllPackingInstruction()
        {

            var GetallPackingInstructions= await FindAll().ToListAsync();

            return GetallPackingInstructions;
        }

        public async Task<PackingInstruction> GetPackingInstructionById(int id)
        {
            var PackingInstructionbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return PackingInstructionbyId;
        }

        public async Task<string> UpdatePackingInstruction(PackingInstruction packingInstruction)
        {
            packingInstruction.LastModifiedBy = "Admin";
            packingInstruction.LastModifiedOn = DateTime.Now;
            Update(packingInstruction);
            string result = $"UpdatePackingInstruction details of {packingInstruction.Id} is updated successfully!";
            return result;
        }
    }
}
