using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IOrderTypeRepository : IRepositoryBase<OrderType>
    {
        Task<IEnumerable<OrderType>> GetAllOrderType(SearchParames searchParams);
        Task<OrderType> GetOrderTypeById(int id);
        Task<IEnumerable<OrderType>> GetAllActiveOrderType(SearchParames searchParams);
        Task<int?> CreateOrderType(OrderType orderType);
        Task<string> UpdateOrderType(OrderType orderType);
        Task<string> DeleteOrderType(OrderType orderType);
        Task<OrderType> GetDefaultOrderType(int id);
        Task<IEnumerable<OrderType>> GetDefaultOrderTypeValue(int id); 
    }
}
