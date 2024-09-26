using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Repository
{
    public class BTODeliveryOrderInventoryHistoryRepository : RepositoryBase<BTODeliveryOrderInventoryHistory>, IBTODeliveryOrderInventoryHistoryRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public BTODeliveryOrderInventoryHistoryRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }


        public async Task<long> CreateBTODeliveryOrderInventoryHistory(BTODeliveryOrderInventoryHistory bTODeliveryOrderInventoryHistory)
        {
            bTODeliveryOrderInventoryHistory.CreatedBy = _createdBy;
            bTODeliveryOrderInventoryHistory.CreatedOn = DateTime.Now;
            bTODeliveryOrderInventoryHistory.Unit = _unitname;
            var result = await Create(bTODeliveryOrderInventoryHistory);
            return result.Id;
        }
    }
}