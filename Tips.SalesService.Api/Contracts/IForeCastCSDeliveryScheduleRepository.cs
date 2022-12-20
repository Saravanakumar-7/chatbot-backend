using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForeCastCSDeliveryScheduleRepository
    {
        Task<IEnumerable<ForeCastCSDeliverySchedule>> GetAllForeCastCSDeliverySchedule();
        Task<ForeCastCSDeliverySchedule> GetForeCastCSDeliveryScheduleById(int id);
        Task<int?> CreateForeCastCSDeliverySchedule(ForeCastCSDeliverySchedule rfqCSDeliverySchedule);
        Task<string> UpdateForeCastCSDeliverySchedule(ForeCastCSDeliverySchedule rfqCSDeliverySchedule);
        Task<string> DeleteForeCastCSDeliverySchedule(ForeCastCSDeliverySchedule rfqCSDeliverySchedule);
    }
}
