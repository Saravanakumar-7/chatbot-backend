using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISoConfirmationDateRepository : IRepositoryBase<SoConfirmationDate>
    {
        Task<IEnumerable<long>> CreateSoConfirmationDateList(List<SoConfirmationDate> soConfirmationDates);
        Task<IEnumerable<SoConfirmationDate>> GetSoConfirmationDateDetailsById(int soItemId);
        Task<IEnumerable<string>> DeleteSoConfirmationDateList(IEnumerable<SoConfirmationDate> soConfirmationDates);
    }
}
