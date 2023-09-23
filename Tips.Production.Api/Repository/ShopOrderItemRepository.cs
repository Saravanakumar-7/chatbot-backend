using System.Security.Claims;
using Entities.DTOs;
using Entities;
using Entities.Enums;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; 


namespace Tips.Production.Api.Repository
{
    public class ShopOrderItemRepository : RepositoryBase<ShopOrderItem>, IShopOrderItemRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;

        public ShopOrderItemRepository(TipsProductionDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;

        }

        public Task<decimal?> GetNotShortCloseQty(string fgItemNumber,string saItemNumber, string projectNumber, string salesOrderNumber)
        {

        //    var shopOrderIds = _tipsProductionDbContext.ShopOrders
        //                         .Where(im => im.ItemNumber == saItemNumber)
        //                         .Select(x => x.Id)
        //                         .Distinct()
        //                         .ToListAsync();

        //    List<int> itemNos = shopOrderIds.t; // itemNos now contains the list of IDs

        //    var totalReleaseQty =  _tipsProductionDbContext.ShopOrderItems
        //        .Where(x => x.FGItemNumber == fgItemNumber &&
        //                    x.ProjectNumber == projectNumber &&
        //                    x.SalesOrderNumber == salesOrderNumber &&
        //                    itemNos.Contains(x.ShopOrderId)) // Use the list of IDs here
        //        .SumAsync(x => x.ReleaseQty);
            return null;

        }

        public Task<IEnumerable<ShopOrderItem>> GetAllShopOrderItems()
        {
            throw new NotImplementedException();
        }

        public async Task<ShopOrderItem> GetShopOrderItemById(int id)
        {
            var shopOrderDetailById = await _tipsProductionDbContext.ShopOrderItems
                           .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();

            return shopOrderDetailById;
        }

        public async Task<long> CreateShopOrderItem(ShopOrderItem shopOrderItem)
        {
            var result = await Create(shopOrderItem);
            return result.Id;
        }

        public async Task<string> UpdateShopOrderItem(ShopOrderItem shopOrderItem)
        {
            Update(shopOrderItem);
            string result = $"ShopOrderItem details of {shopOrderItem.Id} is updated successfully!";
            return result;
        }
    }
}
