using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Repository
{
    public class POInitialConfirmationDateHistoryRepository : RepositoryBase<POInitialConfirmationDateHistory>, IPOInitialConfirmationDateHistoryRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;
        public POInitialConfirmationDateHistoryRepository(TipsPurchaseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsPurchaseDbContext = repositoryContext;
        }
        public async Task<long> CreatePOInitialConfirmationDate(POInitialConfirmationDateHistory poInitialConfirmationDateHistory)
        {
            var result = await Create(poInitialConfirmationDateHistory);
            return result.Id;
        }
    }
}
