using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Repository
{
    public class SOAdditionalChargesHistoryRepository : RepositoryBase<SOAdditionalChargesHistory>, ISOAdditionalChargesHistoryRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContexts;

        public SOAdditionalChargesHistoryRepository(TipsSalesServiceDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsSalesServiceDbContexts = repositoryContext;
        }

        public async Task<SOAdditionalChargesHistory> CreateSOAdditionalChargesHistory(SOAdditionalChargesHistory soAdditionalChargesHistory)
        {
            var result = await Create(soAdditionalChargesHistory);
            return result;
        }
        public async Task<string> UpdateSOAdditionalChargesHistory(SOAdditionalChargesHistory soAdditionalChargesHistory)
        {
            Update(soAdditionalChargesHistory);
            string result = $"soAdditionalChargesHistory of Detail {soAdditionalChargesHistory.Id} is updated successfully!";
            return result;
        }

    }
}
