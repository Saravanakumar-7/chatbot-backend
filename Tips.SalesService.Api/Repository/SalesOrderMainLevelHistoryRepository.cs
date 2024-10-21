using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Repository
{
    public class SalesOrderMainLevelHistoryRepository : RepositoryBase<SalesOrderMainLevelHistory>, ISalesOrderMainLevelHistoryRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContexts;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;

        public SalesOrderMainLevelHistoryRepository(TipsSalesServiceDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsSalesServiceDbContexts = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<SalesOrderMainLevelHistory> CreateSalesOrderMainLevelHistory(SalesOrderMainLevelHistory salesOrderMainLevelHistory)
        {
            salesOrderMainLevelHistory.CreatedBy = _createdBy;
            salesOrderMainLevelHistory.CreatedOn = DateTime.Now;
            salesOrderMainLevelHistory.Unit = _unitname; 
            var result = await Create(salesOrderMainLevelHistory);
            return result;
        }
        public async Task<string> UpdateSalesOrderMainLevelHistory(SalesOrderMainLevelHistory salesOrderMainLevelHistory)
        {
            Update(salesOrderMainLevelHistory);
            string result = $"salesOrderMainLevelHistory of Detail {salesOrderMainLevelHistory.Id} is updated successfully!";
            return result;
        }
        public async Task<SalesOrderMainLevelHistory> GetSalesOrderMainLevelHistoryBySalesOrderId(int soid)
        {
            var salesOrderHistorybyId = await _tipsSalesServiceDbContexts.SalesOrderMainLevelHistories.Where(x => x.SalesOrderId == soid)
                                  .Include(t => t.SalesOrderItemLevelHistory)
                                  .ThenInclude(p => p.SalesOrderScheduleDateHistory)
                                    .Include(o => o.SOAdditionalChargesHistory)

                                 .FirstOrDefaultAsync();

            return salesOrderHistorybyId;
        }
        public async Task<int> GetSalesOrderMainLevelHistoryIdBySalesOrderId(int soid)
        {
            var salesOrderHistorybyId = await _tipsSalesServiceDbContexts.SalesOrderMainLevelHistories.Where(x => x.SalesOrderId == soid)
                                  .Select(x=>x.Id)

                                 .FirstOrDefaultAsync();

            return salesOrderHistorybyId;
        }
        public async Task<List<SOHistoryRevNoListDto>> GetSalesOrderMainLevelHistoryRevNoListBySalesOrderIdAndRevNo(int salesOrderId , int RevNo)
        {
            var salesOrderHistorybyId = await _tipsSalesServiceDbContexts.SalesOrderMainLevelHistories
                            .Where(x => x.SalesOrderId == salesOrderId && x.RevisionNumber != RevNo)
                                 .Select(s=> new SOHistoryRevNoListDto
                                 {
                                    Id = s.Id,
                                    RevisionNumber = s.RevisionNumber
                                 })
                                 .ToListAsync();

            return salesOrderHistorybyId;
        }
        public async Task<SalesOrderMainLevelHistory> GetSalesOrderMainLevelHistoryBySalesOrderHistoryIdAndRevNo(int SalesOrderHistoryId, int RevNo)
        {
            var salesOrderHistorybyId = await _tipsSalesServiceDbContexts.SalesOrderMainLevelHistories
                            .Where(x => x.Id == SalesOrderHistoryId && x.RevisionNumber == RevNo)
                             .Include(t => t.SalesOrderItemLevelHistory)
                                  .ThenInclude(p => p.SalesOrderScheduleDateHistory)
                                    .Include(o => o.SOAdditionalChargesHistory)
                                 .FirstOrDefaultAsync();

            return salesOrderHistorybyId;
        }

    }
}
