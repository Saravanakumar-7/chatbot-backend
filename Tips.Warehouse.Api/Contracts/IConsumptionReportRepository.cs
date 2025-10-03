using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IConsumptionReportRepository : IRepositoryBase<ConsumptionSPReport>
    {
        Task<List<int>> CreateConsumptionReports(List<ConsumptionSPReport> consumptionSPReports);
    }
}
