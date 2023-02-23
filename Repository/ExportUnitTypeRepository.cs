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
    internal class ExportUnitTypeRepository : RepositoryBase<ExportUnitType>, IExportUnitTypeRepository
    {
        public ExportUnitTypeRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateExportUnitType(ExportUnitType exportUnitType)
        {
            exportUnitType.CreatedBy = "Admin";
            exportUnitType.CreatedOn = DateTime.Now;
            exportUnitType.Unit = "Bangalore";
            var result = await Create(exportUnitType);
            
            return result.Id;
        }

        public async Task<string> DeleteExportUnitType(ExportUnitType exportUnitType)
        {
            Delete(exportUnitType);
            string result = $"ExportUnitType details of {exportUnitType.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<ExportUnitType>> GetAllActiveExportUnitTypes()
        {

            var AllActiveExportUnitTypes = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveExportUnitTypes;
        }

        public async Task<IEnumerable<ExportUnitType>> GetAllExportUnitTypes()
        {

            var GetallExportUnitTypes = await FindAll().ToListAsync();

            return GetallExportUnitTypes;
        }

        public async Task<ExportUnitType> GetExportUnitTypeById(int id)
        {
            var ExportUnitTypebyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return ExportUnitTypebyId;
        }

        public async Task<string> UpdateExportUnitType(ExportUnitType exportUnitType)
        {
            exportUnitType.LastModifiedBy = "Admin";
            exportUnitType.LastModifiedOn = DateTime.Now;
            Update(exportUnitType);
            string result = $"ExportUnitType details of {exportUnitType.Id} is updated successfully!";
            return result;
        }
    }
}
