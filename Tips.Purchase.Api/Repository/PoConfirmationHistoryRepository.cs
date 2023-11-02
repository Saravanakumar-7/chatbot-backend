using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Repository
{
    public class PoConfirmationHistoryRepository : RepositoryBase<PoConfirmationHistory>, IPoConfirmationHistoryRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;
        public PoConfirmationHistoryRepository(TipsPurchaseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsPurchaseDbContext = repositoryContext;
        }
        public async Task<long> CreatePoConfirmationHistory(PoConfirmationHistory poConfirmationHistory)
        {
           // poConfirmationHistory.CreatedBy = "Admin";
            //poConfirmationHistory.CreatedOn = DateTime.Now;
           // poConfirmationHistory.LastModifiedBy = "Admin";
          //  poConfirmationHistory.LastModifiedOn = DateTime.Now;
            var result = await Create(poConfirmationHistory);
            return result.Id;
        }
    }
}
