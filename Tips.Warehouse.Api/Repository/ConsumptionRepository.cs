using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.Identity.Client;
using System.Linq;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Entities.DTOs;
using Entities.Enums;
using System.Security.Claims;

namespace Tips.Warehouse.Api.Repository
{ 
    public class ConsumptionReportRepository : RepositoryBase<ConsumptionSPReport>, IConsumptionReportRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ConsumptionReportRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        //public async Task<long> CreateConsumptionReport(ConsumptionSPReport consumptionSPReport)
        //{
        //    var date = DateTime.Now;
        //    consumptionSPReport.CreatedBy = _createdBy;
        //    consumptionSPReport.CreatedOn = date;
        //    consumptionSPReport.Unit = _unitname;
        //    var result = await Create(consumptionSPReport);
        //    return result.Id;
        //}
        public async Task<List<long>> CreateConsumptionReports(List<ConsumptionSPReport> consumptionSPReports)
        {
            var date = DateTime.Now;
            var resultIds = new List<long>();

            foreach (var report in consumptionSPReports)
            {
                report.CreatedBy = _createdBy;
                report.CreatedOn = date;
                report.Unit = _unitname;

                var result = await Create(report);
                resultIds.Add(result.Id);
            }

            return resultIds;
        }

    }
}
