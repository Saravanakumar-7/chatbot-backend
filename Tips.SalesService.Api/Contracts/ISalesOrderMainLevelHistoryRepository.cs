using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISalesOrderMainLevelHistoryRepository : IRepositoryBase<SalesOrderMainLevelHistory>
    {
        //Task<PagedList<SalesOrderMainLevelHistory>> GetAllSalesOrderMainLevelHistory(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<SalesOrderMainLevelHistory> GetSalesOrderMainLevelHistoryBySalesOrderId(int soid);
        Task<SalesOrderMainLevelHistory> CreateSalesOrderMainLevelHistory(SalesOrderMainLevelHistory salesOrderMainLevelHistory);
        Task<string> UpdateSalesOrderMainLevelHistory(SalesOrderMainLevelHistory salesOrderMainLevelHistory);
        Task<List<SOHistoryRevNoListDto>> GetSalesOrderMainLevelHistoryRevNoListBySalesOrderIdAndRevNo(int salesOrderId, int RevNo);
        Task<SalesOrderMainLevelHistory> GetSalesOrderMainLevelHistoryBySalesOrderHistoryIdAndRevNo(int SalesOrderHistoryId, int RevNo);
        void SaveChanges();
    }
}
