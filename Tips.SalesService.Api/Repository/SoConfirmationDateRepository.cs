using Microsoft.EntityFrameworkCore;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Repository
{
    public class SoConfirmationDateRepository : RepositoryBase<SoConfirmationDate>, ISoConfirmationDateRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public SoConfirmationDateRepository(TipsSalesServiceDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsSalesServiceDbContext = repositoryContext;
        }
        public async Task<IEnumerable<long>> CreateSoConfirmationDateList(List<SoConfirmationDate> soConfirmationDates)
        {
            List<long> soConfirmationDateList = new List<long>();

            foreach (var soConfirmationDate in soConfirmationDates)
            {
                var result = await Create(soConfirmationDate);
                soConfirmationDateList.Add(result.Id);
            }

            return soConfirmationDateList;
        }

        public async Task<IEnumerable<SoConfirmationDate>> GetSoConfirmationDateDetailsById(int soItemId)
        {
            var soConfirmationDateDetailsById = await _tipsSalesServiceDbContext.SoConfirmationDates.Where(x => x.SalesOrderItemsId == soItemId)
                .ToListAsync();

            return soConfirmationDateDetailsById;
        }
        public async Task<IEnumerable<string>> DeleteSoConfirmationDateList(IEnumerable<SoConfirmationDate> soConfirmationDates)
        {
            List<string> soConfirmationDateList = new List<string>();
            foreach (var soConfirmationDate in soConfirmationDates)
            {
                Delete(soConfirmationDate);
                string result = $"SoConfirmationDate details of {soConfirmationDate.Id} is deleted successfully!";
                soConfirmationDateList.Add(result);
            }
            return soConfirmationDateList;
        }
    }
}
