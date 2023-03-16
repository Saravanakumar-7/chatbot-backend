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
        Task<PagedList<ExportUnitType>> GetAllExportUnitTypes(PagingParameter pagingParameter, SearchParames searchParams);
        Task<ExportUnitType> GetExportUnitTypeById(int id);
        Task<PagedList<ExportUnitType>> GetAllActiveExportUnitTypes(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateExportUnitType(ExportUnitType exportUnitType);
        Task<string> UpdateExportUnitType(ExportUnitType exportUnitType);
        Task<string> DeleteExportUnitType(ExportUnitType exportUnitType);
    }
}
