using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPrAddDeliveryScheduleRepository
    {
        Task<IEnumerable<PrAddDeliverySchedule>> GetAllPrAddDeliverySchedules();
        Task<PrAddDeliverySchedule> GetPrAddDeliveryScheduleById(int id);
        Task<IEnumerable<PrAddDeliverySchedule>> GetAllActivePrAddDeliverySchedules();
        Task<int?> CreatePrAddDeliverySchedule(PrAddDeliverySchedule prAddDeliverySchedule);
        Task<string> UpdatePrAddDeliverySchedule(PrAddDeliverySchedule prAddDeliverySchedule);
        Task<string> DeletePrAddDeliverySchedule(PrAddDeliverySchedule prAddDeliverySchedule);
    }
}
