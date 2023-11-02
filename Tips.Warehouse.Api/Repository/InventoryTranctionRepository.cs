using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Repository
{
    public class InventoryTranctionRepository : RepositoryBase<InventoryTranction>, IInventoryTranctionRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public InventoryTranctionRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<InventoryTranction> CreateInventoryTransaction(InventoryTranction inventoryTranction)
        {
            inventoryTranction.CreatedBy = _createdBy;
            inventoryTranction.CreatedOn = DateTime.Now;
            inventoryTranction.Unit = _unitname;
            var result = await Create(inventoryTranction);

            return result;
        }

        public async Task<string> DeleteInventoryTranction(InventoryTranction inventoryTranction)
        {
            Delete(inventoryTranction);
            string result = $"NaterialIssue details of {inventoryTranction.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<InventoryTranction>> GetInventoryTranctionDetailsByItemNumberandLocation(string ItemNumber, string Location, string Warehouse, string projectNumber)

        {
            var inventoryInventoryTranctionDetailsByItemAndLoc = await _tipsWarehouseDbContext.InventoryTranctions
                .Where(x => x.PartNumber == ItemNumber && x.ProjectNumber == projectNumber && x.From_Location == Location && x.Warehouse == Warehouse && x.IsStockAvailable == true).ToListAsync();

            return inventoryInventoryTranctionDetailsByItemAndLoc;
        }
        //Get InventoryTranction WIP from location and warehouse
        public async Task<List<InventoryTranction>> GetWIPInventoryTranctionDetailsByItemNo(string ItemNumber, string ShopOrderNumber)
        {
            var inventoryTranctionDetail = await _tipsWarehouseDbContext.InventoryTranctions.Where(x => x.PartNumber == ItemNumber
            && x.IsStockAvailable == true && x.From_Location == "WIP" && x.Warehouse == "WIP" && x.shopOrderNo == ShopOrderNumber)
                          .ToListAsync();
            return inventoryTranctionDetail;
        }
        public async Task<List<InventoryTranctionBalanceQtyMaterialIssue>> GetInventoryTranctionStockByItemAndProjectNo(string itemNumber, string projectNumber)
        {
            string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };


            List<InventoryTranctionBalanceQtyMaterialIssue> result = await _tipsWarehouseDbContext.InventoryTranctions
                   .Where(x => x.PartNumber == itemNumber && x.ProjectNumber == projectNumber && x.IsStockAvailable == true && !skipWareHouse.Contains(x.Warehouse))
                   .GroupBy(l => new { l.PartNumber, l.ProjectNumber })
                   .Select(group => new InventoryTranctionBalanceQtyMaterialIssue
                   {
                       PartNumber = group.Key.PartNumber,
                       Issued_Quantity = group.Sum(c => c.Issued_Quantity),
                       ProjectNumber = group.Key.ProjectNumber
                   }).ToListAsync();

            return result;
        }
        public async Task<InventoryTranction> GetInventoryTranctionDetailsByItemNoandProjectNoandShopOrderNo(string ItemNumber, string ProjectNumber, string shopOrderNo)
        {
            var inventoryTranctionDetailsById = await _tipsWarehouseDbContext.InventoryTranctions.Where(x => x.PartNumber == ItemNumber && x.ProjectNumber == ProjectNumber && x.shopOrderNo == shopOrderNo)

                          .FirstOrDefaultAsync();

            return inventoryTranctionDetailsById;
        }

        public async Task<List<InventoryTranction>> GetInventoryTranctionDetailsByItemNoandProjectNo(string ItemNumber, string ProjectNo)
        {
            string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };
            var inventoryTranctionDetail = await _tipsWarehouseDbContext.InventoryTranctions.Where(x => x.PartNumber == ItemNumber
            && x.IsStockAvailable == true && x.ProjectNumber == ProjectNo && !skipWareHouse.Contains(x.Warehouse))
                          .ToListAsync();

            return inventoryTranctionDetail;
        }

        public async Task<PagedList<InventoryTranction>> GetAllInventoryTranction([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {


            var getAllInventoryTranction = FindAll().OrderByDescending(x => x.Id)
                   .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ProjectNumber.Contains(searchParams.SearchValue) ||
                   inv.PartNumber.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue)
                   || inv.MftrPartNumber.Contains(searchParams.SearchValue))));

            return PagedList<InventoryTranction>.ToPagedList(getAllInventoryTranction, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<InventoryTranction> GetInventoryTranctionById(int id)
        {
            var getInventoryTranctionById = await _tipsWarehouseDbContext.InventoryTranctions.Where(x => x.Id == id)

                          .FirstOrDefaultAsync();

            return getInventoryTranctionById;
        }
        public async Task<IEnumerable<InventoryTranction>> GetInventoryTranctionDetailsByItemNoandLocationandwarehouse(string ItemNumber, string Location, string Warehouse, string projectNumber)

        {
            var getInventoryTranctionDetailsByItemAndLoc = await _tipsWarehouseDbContext.InventoryTranctions
                .Where(x => x.PartNumber == ItemNumber && x.From_Location == Location && x.Warehouse == Warehouse
                && x.IsStockAvailable == true && x.ProjectNumber == projectNumber).ToListAsync();

            return getInventoryTranctionDetailsByItemAndLoc;
        }
        public async Task<InventoryTranction> GetInventoryTranctionDetailsByGrinNoandGrinId(string GrinNo, int GrinPartsId, string ItemNumber, string ProjectNumber)
        {
            var inventoryTranctionDetailsById = await _tipsWarehouseDbContext.InventoryTranctions.Where(x => x.GrinNo == GrinNo &&
                                        x.GrinPartId == GrinPartsId && x.PartNumber == ItemNumber &&
                                        x.ProjectNumber == ProjectNumber && x.IsStockAvailable == true)
                          .FirstOrDefaultAsync();

            return inventoryTranctionDetailsById;
        }

        public async Task<string> UpdateInventoryTraction(InventoryTranction inventoryTranction)
        {
            inventoryTranction.LastModifiedBy = _createdBy;
            inventoryTranction.LastModifiedOn = DateTime.Now;
            Update(inventoryTranction);
            string result = $"materialIssue of Detail {inventoryTranction.Id} is updated successfully!";
            return result;
        }
    }
}
