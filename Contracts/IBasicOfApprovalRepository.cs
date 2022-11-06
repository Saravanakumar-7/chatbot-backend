using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IBasicOfApprovalRepository : IRepositoryBase<BasicOfApproval>
    {
        Task<IEnumerable<BasicOfApproval>> GetAlBasicOfApproval();
        Task<BasicOfApproval> GetBasicOfApprovalById(int id);
        Task<IEnumerable<BasicOfApproval>> GetAllActiveBasicOfApproval();
        Task<int?> CreateBasicOfApproval(BasicOfApproval basicOfApproval);
        Task<string> UpdateBasicOfApproval(BasicOfApproval basicOfApproval);
        Task<string> DeleteBasicOfApproval(BasicOfApproval basicOfApproval);
}
}
