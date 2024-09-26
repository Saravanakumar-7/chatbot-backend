using Microsoft.EntityFrameworkCore;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Repository
{
    public class PoItemHistoryRepository : RepositoryBase<PoItemHistory>, IPoItemHistoryRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;
        public PoItemHistoryRepository(TipsPurchaseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsPurchaseDbContext = repositoryContext;
        }

        public async Task<PoItemHistory> CreatePoItemHistory(PoItemHistory poItemHistory)
        {
            var result = await Create(poItemHistory);
            return result;
        }
        public async Task<List<PoItemHistory>> GetPoItemHistoryDetailsByPoItemId(string poNumber , string itemNumber)
        {
            var getPODetailsByPONOandItemNo = await _tipsPurchaseDbContext.PoItemHistories
                 .Where(x => x.PONumber == poNumber && x.ItemNumber == itemNumber)
                          .ToListAsync();

            return getPODetailsByPONOandItemNo;
        }
    }
}
