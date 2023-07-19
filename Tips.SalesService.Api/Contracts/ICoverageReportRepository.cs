using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface ICoverageReportRepository : IRepositoryBase<CoverageReport>
    {
        Task<List<SalesOrder>> GetAllForecastSalesOrderDetails();
        Task<List<SalesOrderItems>> GetAllSalesOrderItemDetails(int salesorderId);
        Task<IEnumerable<CoverageReportDto>> GetAllSalesOrderDetails();

    }
}
