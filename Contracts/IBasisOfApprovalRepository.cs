using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IBasisOfApprovalRepository : IRepositoryBase<BasisOfApproval>
    {
        Task<PagedList<BasisOfApproval>> GetAllBasisOfApproval(PagingParameter pagingParameter, SearchParames searchParams);
        Task<BasisOfApproval> GetBasisOfApprovalById(int id);
        Task<PagedList<BasisOfApproval>> GetAllActiveBasisOfApproval(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateBasisOfApproval(BasisOfApproval basisOfApproval);
        Task<string> UpdateBasisOfApproval(BasisOfApproval basisOfApproval);
        Task<string> DeleteBasisOfApproval(BasisOfApproval basisOfApproval);
}
}
