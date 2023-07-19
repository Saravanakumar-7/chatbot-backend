using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IProcurementTypeRepository
    {
        Task<IEnumerable<ProcurementType>> GetAllProcurementType(SearchParames searchParams);
        Task<ProcurementType> GetProcurementTypeById(int id);
        Task<IEnumerable<ProcurementType>> GetAllActiveProcurementType(SearchParames searchParams);
        Task<int?> CreateProcurementType(ProcurementType ProcurementType);
        Task<string> UpdateProcurementType(ProcurementType ProcurementType);
        Task<string> DeleteProcurementType(ProcurementType ProcurementType);
    }
}
