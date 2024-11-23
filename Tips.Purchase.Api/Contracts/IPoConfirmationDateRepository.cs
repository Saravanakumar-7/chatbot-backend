using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPoConfirmationDateRepository : IRepositoryBase<PoConfirmationDate>
    {
        Task<IEnumerable<long>> CreatePoConfirmationDateList(List<PoConfirmationDate> poConfirmationDates);
        Task<IEnumerable<PoConfirmationDate>> GetPoConfirmationDateDetailsById(int poItemId);
        Task<IEnumerable<string>> DeletePoConfirmationDateList(IEnumerable<PoConfirmationDate> poConfirmationDates);
        Task<IEnumerable<string>> UpdatePoConfirmationDate(IEnumerable<PoConfirmationDate> poConfirmationDates);
    }
}
