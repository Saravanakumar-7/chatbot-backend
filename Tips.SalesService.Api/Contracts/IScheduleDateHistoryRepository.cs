using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IScheduleDateHistoryRepository : IRepositoryBase<ScheduleDateHistory>
    {
        Task<ScheduleDateHistory> CreateScheduleDateHistory(ScheduleDateHistory scheduleDateHistory);

    }
    
}
