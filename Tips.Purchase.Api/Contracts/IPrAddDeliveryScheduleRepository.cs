using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPrAddDeliveryScheduleRepository
    {
        Task<IEnumerable<PrAddDeliverySchedule>> GetAllPrAddDeliverySchedule();
        Task<PrAddDeliverySchedule> GetPrAddDeliveryScheduleById(int id);
        Task<IEnumerable<PrAddDeliverySchedule>> GetAllActivePrAddDeliverySchedule();
        Task<int?> CreatePrAddDeliverySchedule(PrAddDeliverySchedule prAddDeliverySchedule);
        Task<string> UpdatePrAddDeliverySchedule(PrAddDeliverySchedule prAddDeliverySchedule);
        Task<string> DeletePrAddDeliverySchedule(PrAddDeliverySchedule prAddDeliverySchedule);
    }
}
