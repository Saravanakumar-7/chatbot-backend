using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface ICoverageReportRepository : IRepositoryBase<CoverageReport>
    {
        Task<IEnumerable<CoverageReportDto>> GetAllSalesOrderDetails();

    }
}
