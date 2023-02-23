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
    public class ProcurementTypeRepository: RepositoryBase<ProcurementType>, IProcurementTypeRepository
    {
        public ProcurementTypeRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateProcurementType(ProcurementType ProcurementType)
        {
            ProcurementType.CreatedBy = "Admin";
            ProcurementType.CreatedOn = DateTime.Now;
            ProcurementType.Unit = "Bangalore";
            var result = await Create(ProcurementType);
            
            return result.Id;
        }

        public async Task<string> DeleteProcurementType(ProcurementType ProcurementType)
        {
            Delete(ProcurementType);
            string result = $"ProcurementType details of {ProcurementType.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<ProcurementType>> GetAllActiveProcurementType()
        {
            var AllActiveProcurementTypes = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveProcurementTypes;
        }

        public async Task<IEnumerable<ProcurementType>> GetAllProcurementType()
        {
            var GetallProcurementTypes = await FindAll().ToListAsync();
            return GetallProcurementTypes;
        }

        public async Task<ProcurementType> GetProcurementTypeById(int id)
        {
            var ProcurementTypebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return ProcurementTypebyId;
        }

        public async Task<string> UpdateProcurementType(ProcurementType ProcurementType)
        {
            ProcurementType.LastModifiedBy = "Admin";
            ProcurementType.LastModifiedOn = DateTime.Now;
            Update(ProcurementType);
            string result = $"ProcurementType details of {ProcurementType.Id} is updated successfully!";
            return result;
        }
    }
}
