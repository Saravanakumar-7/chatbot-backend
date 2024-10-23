using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

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
        public async Task<SalesOrderItemLevelHistory> GetSalesOrderItemLevelHistoryBySalesOrderItemIdAndRevNo(int soItemid, int? revNo)
        {
            var getSalesOrderItemHisbyId = await _tipsSalesServiceDbContexts.SalesOrderItemLevelHistories
                                            .Where(x => x.SalesOrderItemId == soItemid
                                            && x.RevisionNumber == revNo)

                                   .FirstOrDefaultAsync();

            return getSalesOrderItemHisbyId;
        }

        public async Task<int> GetSalesOrderItemLevelHistoryIdBySalesOrderItemIdAndRevNo(int soItemid, int? revNo)
        {
            var getSalesOrderItemHisbyId = await _tipsSalesServiceDbContexts.SalesOrderItemLevelHistories
                                                .Where(x => x.SalesOrderItemId == soItemid
                                                    && x.RevisionNumber == revNo)
                                                  .Select(x => x.Id)

                                   .FirstOrDefaultAsync();

            return getSalesOrderItemHisbyId;
        }

        public async Task<SalesOrderItemLevelHistory> GetShortCloseSalesOrderItemLevelHistoryBySalesOrderItemIdAndRevNo(int soItemid, int? revNo)
        {
            var getSalesOrderItemHisbyId = await _tipsSalesServiceDbContexts.SalesOrderItemLevelHistories
                                                            .Where(x => x.SalesOrderItemId == soItemid && x.RevisionNumber == revNo)

                                                .FirstOrDefaultAsync();

            return getSalesOrderItemHisbyId;
        }

        public async Task<int> GetShortCloseSalesOrderItemLevelHistoryIdBySalesOrderItemIdAndRevNo(int soItemid, int? revNo)
        {
            var salesOrderItemHisbyId = await _tipsSalesServiceDbContexts.SalesOrderItemLevelHistories
                                            .Where(x => x.SalesOrderItemId == soItemid && x.RevisionNumber == revNo)
                                            .Select(s => s.Id)

                                 .FirstOrDefaultAsync();

            return salesOrderItemHisbyId;
        }

    }
}
