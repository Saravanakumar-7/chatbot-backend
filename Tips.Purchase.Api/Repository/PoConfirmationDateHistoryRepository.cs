using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Repository
{
    public class PoConfirmationDateHistoryRepository : RepositoryBase<PoConfirmationDateHistory>, IPoConfirmationDateHistoryRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;
        public PoConfirmationDateHistoryRepository(TipsPurchaseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsPurchaseDbContext = repositoryContext;
        }
        public async Task<long> CreatePoConfirmationDateHistory(PoConfirmationDateHistory poConfirmationDateHistory)
        {
            poConfirmationDateHistory.CreatedBy = "Admin";
            poConfirmationDateHistory.CreatedOn = DateTime.Now;
            poConfirmationDateHistory.LastModifiedBy = "Admin";
            poConfirmationDateHistory.LastModifiedOn = DateTime.Now;
            var result = await Create(poConfirmationDateHistory);
            return result.Id;
        }
    }
}
