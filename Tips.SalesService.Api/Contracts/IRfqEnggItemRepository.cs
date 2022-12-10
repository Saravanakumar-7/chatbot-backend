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
    public interface IRfqEnggItemRepository
    {
        Task<IEnumerable<RfqEnggItem>> GetAllRfqEnggItems();
        Task<RfqEnggItem> GetRfqEnggItemById(int id);
        Task<int?> CreateRfqEnggItem(RfqEnggItem rfqEnggItem);
        Task<string> UpdateRfqEnggItem(RfqEnggItem rfqEnggItem);
        Task<string> DeleteRfqEnggItem(RfqEnggItem rfqEnggItem);
    }
}
