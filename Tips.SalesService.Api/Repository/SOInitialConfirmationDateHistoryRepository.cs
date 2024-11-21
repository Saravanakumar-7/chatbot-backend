using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Repository
{
    public class SOInitialConfirmationDateHistoryRepository : RepositoryBase<SOInitialConfirmationDateHistory>, ISOInitialConfirmationDateHistoryRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public SOInitialConfirmationDateHistoryRepository(TipsSalesServiceDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsSalesServiceDbContext = repositoryContext;
        }
        public async Task<long> CreateSOInitialConfirmationDate(SOInitialConfirmationDateHistory soInitialConfirmationDateHistory)
        {
                var result = await Create(soInitialConfirmationDateHistory);
                return result.Id;
        }
    }
}
