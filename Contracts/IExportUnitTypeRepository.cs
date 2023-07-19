using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IExportUnitTypeRepository : IRepositoryBase<ExportUnitType>
    {
        Task<IEnumerable<ExportUnitType>> GetAllExportUnitTypes(SearchParames searchParams);
        Task<ExportUnitType> GetExportUnitTypeById(int id);
        Task<IEnumerable<ExportUnitType>> GetAllActiveExportUnitTypes(SearchParames searchParams);
        Task<int?> CreateExportUnitType(ExportUnitType exportUnitType);
        Task<string> UpdateExportUnitType(ExportUnitType exportUnitType);
        Task<string> DeleteExportUnitType(ExportUnitType exportUnitType);
    }
}
