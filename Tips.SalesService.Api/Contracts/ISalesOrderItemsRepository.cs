using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISalesOrderItemsRepository : IRepositoryBase<SalesOrderItems>
    { 
        Task<IEnumerable<GetSalesOrderDetailsDto>> getSalesOrderDetailByProjectNoandItemNo(string ItemNo, string ProjectNo);

        Task<IEnumerable<SalesOrderItems>> GetSalesOrderDetailsByIdandItemNo(string ItemNumber, int SalesOrderId);
 
        Task<IEnumerable<ListOfProjectNoDto>> GetprojectNoByItemNo(string itemNo); 
    }
}
