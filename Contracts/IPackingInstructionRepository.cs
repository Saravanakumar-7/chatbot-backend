using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IPackingInstructionRepository : IRepositoryBase<PackingInstruction>
    {
        Task<IEnumerable<PackingInstruction>> GetAllPackingInstruction();
        Task<PackingInstruction> GetPackingInstructionById(int id);
        Task<IEnumerable<PackingInstruction>> GetAllActivePackingInstruction();
        Task<int?> CreatePackingInstruction(PackingInstruction packingInstruction);
        Task<string> UpdatePackingInstruction(PackingInstruction packingInstruction);
        Task<string> DeletePackingInstruction(PackingInstruction packingInstruction);
    }
}
