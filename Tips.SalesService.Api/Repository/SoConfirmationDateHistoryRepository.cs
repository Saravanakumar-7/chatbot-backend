using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Repository
{
    public class SoConfirmationDateHistoryRepository : RepositoryBase<SoConfirmationDateHistory>, ISoConfirmationDateHistoryRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public SoConfirmationDateHistoryRepository(TipsSalesServiceDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsSalesServiceDbContext = repositoryContext;
        }
        public async Task<long> CreateSoConfirmationHistory(SoConfirmationDateHistory soConfirmationDateHistory)
        {
            var result = await Create(soConfirmationDateHistory);
            return result.Id;
        }
    }
}
