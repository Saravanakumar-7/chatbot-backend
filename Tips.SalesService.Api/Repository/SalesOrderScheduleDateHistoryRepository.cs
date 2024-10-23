using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Repository
{
    public class SalesOrderScheduleDateHistoryRepository : RepositoryBase<SalesOrderScheduleDateHistory>, ISalesOrderScheduleDateHistoryRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContexts;

        public SalesOrderScheduleDateHistoryRepository(TipsSalesServiceDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsSalesServiceDbContexts = repositoryContext;
        }

        public async Task<SalesOrderScheduleDateHistory> CreateSalesOrderScheduleDateHistory(SalesOrderScheduleDateHistory salesOrderScheduleDateHistory)
        {
            var result = await Create(salesOrderScheduleDateHistory);
            return result;
        }
        public async Task<string> UpdateSalesOrderScheduleDateHistory(SalesOrderScheduleDateHistory salesOrderScheduleDateHistory)
        {
            Update(salesOrderScheduleDateHistory);
            string result = $"SalesOrderScheduleDateHistory of Detail {salesOrderScheduleDateHistory.Id} is updated successfully!";
            return result;
        }

    }
}
