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
        Task<IEnumerable<ProcurementType>> GetAllProcurementType();
        Task<ProcurementType> GetProcurementTypeById(int id);
        Task<IEnumerable<ProcurementType>> GetAllActiveProcurementType();
        Task<int?> CreateProcurementType(ProcurementType ProcurementType);
        Task<string> UpdateProcurementType(ProcurementType ProcurementType);
        Task<string> DeleteProcurementType(ProcurementType ProcurementType);
    }
}
