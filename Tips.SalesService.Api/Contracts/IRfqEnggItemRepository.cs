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
    public interface IRfqEnggItemRepository : IRepositoryBase<RfqEnggItem>
    {
        Task<PagedList<RfqEnggItem>> GetAllRfqEnggItems(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<RfqEnggItem> GetRfqEnggItemById(int id);
        Task<int?> CreateRfqEnggItem(RfqEnggItem rfqEnggItem);
        Task<string> UpdateRfqEnggItem(RfqEnggItem rfqEnggItem);
        Task<string> DeleteRfqEnggItem(RfqEnggItem rfqEnggItem);
        Task<IEnumerable<RfqEnggItem>> GetAllActiveRfqEnggItemByRfqNumber(string rfqNumber);
        Task<string> ActivateRfqEnggItemById(RfqEnggItem rfqEnggItem);
        Task<string> DeactivateRfqEnggItemById(RfqEnggItem rfqEnggItem);
    }
}
