using Entities;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Security.Claims;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Repository
{
    public class InventoryForServiceItemsRepository : RepositoryBase<InventoryForServiceItems>, IInventoryForServiceItemsRepository
    {
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public InventoryForServiceItemsRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateInventoryForServiceItems(InventoryForServiceItems inventoryforserviceitems)
        {
            inventoryforserviceitems.CreatedBy = _createdBy;
            inventoryforserviceitems.CreatedOn = DateTime.Now;
            inventoryforserviceitems.Unit = _unitname;
            if (inventoryforserviceitems.Balance_Quantity == 0) inventoryforserviceitems.IsStockAvailable = false;
            else inventoryforserviceitems.IsStockAvailable = true;
            var result = await Create(inventoryforserviceitems);

            return result.Id;
        }
        public async Task<InventoryForServiceItems> GetInventoryForServiceDetailsByGrinNoandGrinId(string GrinNo, int GrinPartsId, string ItemNumber, string ProjectNumber)
        {
            var getInventoryDetailsById = await _tipsWarehouseDbContext.InventoryForServiceItems.Where(x => x.GrinNo == GrinNo &&
                                        x.GrinPartId == GrinPartsId && x.PartNumber == ItemNumber &&
                                        x.ProjectNumber == ProjectNumber && x.IsStockAvailable == true)
                          .FirstOrDefaultAsync();

            return getInventoryDetailsById;
        }
        public async Task<InventoryForServiceItems> GetInventoryForServiceItemsById(int id)
        {
            var getInventoryById = await _tipsWarehouseDbContext.InventoryForServiceItems.Where(x => x.Id == id).FirstOrDefaultAsync();
            return getInventoryById;
        }
        public async Task<string> UpdateInventoryForServiceItems(InventoryForServiceItems inventoryForServiceItems)
        {
            inventoryForServiceItems.LastModifiedBy = _createdBy;
            inventoryForServiceItems.LastModifiedOn = DateTime.Now;
            if (inventoryForServiceItems.Balance_Quantity == 0) inventoryForServiceItems.IsStockAvailable = false;
            else inventoryForServiceItems.IsStockAvailable = true;
            Update(inventoryForServiceItems);
            string result = $"materialIssue of Detail {inventoryForServiceItems.Id} is updated successfully!";
            return result;
        }
    }
}
