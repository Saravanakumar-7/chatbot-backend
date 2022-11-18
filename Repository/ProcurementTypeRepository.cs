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
            var result = await Create(ProcurementType);
            ProcurementType.Unit = "Bangalore";
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
            var ProcurementTypeList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return ProcurementTypeList;
        }

        public async Task<IEnumerable<ProcurementType>> GetAllProcurementType()
        {
            var ProcurementTypeList = await FindAll().ToListAsync();
            return ProcurementTypeList;
        }

        public async Task<ProcurementType> GetProcurementTypeById(int id)
        {
            var ProcurementTypeList = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return ProcurementTypeList;
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
