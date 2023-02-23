using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IBasisOfApprovalRepository : IRepositoryBase<BasisOfApproval>
    {
        Task<IEnumerable<BasisOfApproval>> GetAllBasisOfApproval();
        Task<BasisOfApproval> GetBasisOfApprovalById(int id);
        Task<IEnumerable<BasisOfApproval>> GetAllActiveBasisOfApproval();
        Task<int?> CreateBasisOfApproval(BasisOfApproval basisOfApproval);
        Task<string> UpdateBasisOfApproval(BasisOfApproval basisOfApproval);
        Task<string> DeleteBasisOfApproval(BasisOfApproval basisOfApproval);
}
}
