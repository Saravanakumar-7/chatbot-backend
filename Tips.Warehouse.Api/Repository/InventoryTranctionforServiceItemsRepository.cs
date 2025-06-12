using System.Security.Claims;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Repository
{
    public class InventoryTranctionforServiceItemsRepository : RepositoryBase<InventoryTranctionforServiceItems>, IInventoryTranctionforServiceItemsRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public InventoryTranctionforServiceItemsRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<InventoryTranctionforServiceItems> CreateInventoryTranctionforServiceItems(InventoryTranctionforServiceItems inventoryTranction)
        {
            inventoryTranction.Issued_By = _createdBy;
            inventoryTranction.Issued_DateTime = DateTime.Now;
            inventoryTranction.CreatedBy = _createdBy;
            inventoryTranction.CreatedOn = DateTime.Now;
            inventoryTranction.Unit = _unitname;
            var result = await Create(inventoryTranction);

            return result;
        }
    }
}
