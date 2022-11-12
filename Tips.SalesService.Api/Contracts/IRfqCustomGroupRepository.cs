using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IRfqCustomGroupRepository
    {
        Task<IEnumerable<RfqCustomGroup>> GetAllRfqCustomGroup();
        Task<RfqCustomGroup> GetRfqCustomGroupById(int id);
        Task<int?> CreateRfqCustomGroup(RfqCustomGroup rfqCustomGroup);
        Task<string> UpdateRfqCustomGroup(RfqCustomGroup rfqCustomGroup);
        Task<string> DeleteRfqCustomGroup(RfqCustomGroup rfqCustomGroup);
    }
}
