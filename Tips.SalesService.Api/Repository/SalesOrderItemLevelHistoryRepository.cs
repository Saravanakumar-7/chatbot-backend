using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Repository
{
    public class SalesOrderItemLevelHistoryRepository : RepositoryBase<SalesOrderItemLevelHistory>, ISalesOrderItemLevelHistoryRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContexts;

    public SalesOrderItemLevelHistoryRepository(TipsSalesServiceDbContext repositoryContext) : base(repositoryContext)
    {
        _tipsSalesServiceDbContexts = repositoryContext;
    }

    public async Task<SalesOrderItemLevelHistory> CreateSalesOrderItemLevelHistory(SalesOrderItemLevelHistory salesOrderItemLevelHistory)
    {
        var result = await Create(salesOrderItemLevelHistory);
        return result;
    }
    public async Task<string> UpdateSalesOrderItemLevelHistory(SalesOrderItemLevelHistory salesOrderItemLevelHistory)
    {
        Update(salesOrderItemLevelHistory);
        string result = $"salesOrderItemLevelHistory of Detail {salesOrderItemLevelHistory.Id} is updated successfully!";
        return result;
    }
    public async Task<SalesOrderItemLevelHistory> GetSalesOrderItemLevelHistoryBySalesOrderItemId(int soItemid)
        {
            var getSalesOrderItemHisbyId = await _tipsSalesServiceDbContexts.SalesOrderItemLevelHistories.Where(x => x.SalesOrderItemId == soItemid)

                                 .FirstOrDefaultAsync();

            return getSalesOrderItemHisbyId;
        }

    }
}
