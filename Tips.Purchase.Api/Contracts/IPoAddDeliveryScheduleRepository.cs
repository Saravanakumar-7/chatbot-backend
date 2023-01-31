using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPoAddDeliveryScheduleRepository
    {
        Task<IEnumerable<PoAddDeliverySchedule>> GetAllPoAddDeliverySchedules();
        Task<PoAddDeliverySchedule> GetPoAddDeliveryScheduleById(int id);
        Task<IEnumerable<PoAddDeliverySchedule>> GetAllActivePoAddDeliverySchedules();
        Task<int?> CreatePoAddDeliverySchedule(PoAddDeliverySchedule poAddDeliverySchedule);
        Task<string> UpdatePoAddDeliverySchedule(PoAddDeliverySchedule poAddDeliverySchedule);
        Task<string> DeletePoAddDeliverySchedule(PoAddDeliverySchedule poAddDeliverySchedule);
    }
}
