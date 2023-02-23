using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IRfqLPCostingItemRepository
    {
        Task<IEnumerable<RfqLPCostingItem>> GetAllRfqLPCostingItems();
        Task<RfqLPCostingItem> GetRfqLPCostingItemById(int id);
        Task<int?> CreateRfqLPCostingItem(RfqLPCostingItem rfqLPCostingItem);
        Task<string> UpdateRfqLPCostingItem(RfqLPCostingItem rfqLPCostingItem);
        Task<string> DeleteRfqLPCostingItem(RfqLPCostingItem rfqLPCostingItem);
    }
}
