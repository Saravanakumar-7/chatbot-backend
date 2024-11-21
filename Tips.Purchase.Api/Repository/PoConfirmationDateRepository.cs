using Microsoft.EntityFrameworkCore;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Repository
{
    public class PoConfirmationDateRepository : RepositoryBase<PoConfirmationDate>, IPoConfirmationDateRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;
        public PoConfirmationDateRepository(TipsPurchaseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsPurchaseDbContext = repositoryContext;
        }
        public async Task<IEnumerable<long>> CreatePoConfirmationDateList(List<PoConfirmationDate> poConfirmationDates)
        {
            List<long> poConfirmationDateList = new List<long>();

            foreach (var poConfirmationDate in poConfirmationDates)
            {
                var result = await Create(poConfirmationDate);
                poConfirmationDateList.Add(result.Id);
            }

            return poConfirmationDateList;
        }

        public async Task<IEnumerable<PoConfirmationDate>> GetPoConfirmationDateDetailsById(int poItemId)
        {
            var poConfirmationDateDetailsById = await _tipsPurchaseDbContext.PoConfirmationDates.Where(x => x.POItemDetailId == poItemId)
                .ToListAsync();

                return poConfirmationDateDetailsById;
        }
        public async Task<IEnumerable<string>> DeletePoConfirmationDateList(IEnumerable<PoConfirmationDate> poConfirmationDates)
        {
            List<string> poConfirmationDateList = new List<string>();
            foreach (var poConfirmationDate in poConfirmationDates)
            {
                Delete(poConfirmationDate);
                string result = $"PoConfirmationDate details of {poConfirmationDate.Id} is deleted successfully!";
                poConfirmationDateList.Add(result);
            }
            return poConfirmationDateList;
        }
        public async Task<IEnumerable<string>> UpdatePoConfirmationDate(IEnumerable<PoConfirmationDate> poConfirmationDates)
        {
            List<string> poConfirmationDateList = new List<string>();
            foreach (var poConfirmationDate in poConfirmationDates)
            {
                Update(poConfirmationDate);
                string result = $"PoConfirmationDate of Detail {poConfirmationDate.Id} is updated successfully!";
                poConfirmationDateList.Add(result);
            }
            return poConfirmationDateList;
        }

    }
}
