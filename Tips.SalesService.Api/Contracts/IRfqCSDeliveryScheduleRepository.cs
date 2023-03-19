using Entities.Helper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IRfqCSDeliveryScheduleRepository
    {
        Task<PagedList<RfqCSDeliverySchedule>> GetAllRfqCSDeliverySchedule(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<RfqCSDeliverySchedule> GetRfqCSDeliveryScheduleById(int id);
        Task<int?> CreateRfqCSDeliverySchedule(RfqCSDeliverySchedule rfqCSDeliverySchedule);
        Task<string> UpdateRfqCSDeliverySchedule(RfqCSDeliverySchedule rfqCSDeliverySchedule);
        Task<string> DeleteRfqCSDeliverySchedule(RfqCSDeliverySchedule rfqCSDeliverySchedule);
    }
}
