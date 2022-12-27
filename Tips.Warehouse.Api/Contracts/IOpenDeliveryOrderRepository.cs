using Contracts;
using Entities;
using Entities.Helper;
using System.Threading.Tasks;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{   
    public interface IOpenDeliveryOrderRepository : IRepositoryBase<OpenDeliveryOrder>
    {
        Task<PagedList<OpenDeliveryOrder>> GetAllOpenDeliveryOrders(PagingParameter pagingParameter);

        Task<OpenDeliveryOrder> GetOpenDeliveryOrderById(int id);
        Task<int?> CreateOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder);
        Task<string> UpdateOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder);
        Task<string> DeleteOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder);
    }

}
