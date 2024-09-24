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
using Tips.Production.Api.Entities.Enums;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Repository
{
    public class ShopOrderItemRepository : RepositoryBase<ShopOrderItem>, IShopOrderItemRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;
        private AdvitaTipsProductionDbContext _advitaTipsProductionDbContext;
        public ShopOrderItemRepository(TipsProductionDbContext repositoryContext, AdvitaTipsProductionDbContext advitaTipsProductionDbContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext,advitaTipsProductionDbContext)
        {
            _advitaTipsProductionDbContext= advitaTipsProductionDbContext;
            _tipsProductionDbContext = repositoryContext;

        }

        //public async Task<decimal?> GetNotShortCloseQty(string fgItemNumber, string saItemNumber, string projectNumber, string salesOrderNumber)
        //{

        //    List<int> shopOrderIds = await _tipsProductionDbContext.ShopOrders
        //                        .Where(im => im.ItemNumber == saItemNumber)
        //                        .Select(x => x.Id)
        //                        .Distinct()
        //                        .ToListAsync();

        //    decimal? totalReleaseQty = await _tipsProductionDbContext.ShopOrderItems
        //                            .Where(x => x.FGItemNumber == fgItemNumber &&
        //                                        x.ProjectNumber == projectNumber &&
        //                                        x.SalesOrderNumber == salesOrderNumber &&
        //                                        shopOrderIds.Contains(x.ShopOrderId))
        //                            .SumAsync(x => x.ReleaseQty);
        //    return totalReleaseQty;

        //}
        public async Task<List<ShopOrderShortCloseDto>> GetNotShortCloseQty(string fgItemNumber, string saItemNumber, string projectNumber,
                                                                     string salesOrderNumber)
        {
            var shopOrderData = await _tipsProductionDbContext.ShopOrders
                .Where(im => im.ItemNumber == saItemNumber)
                .Select(x => new
                {
                    x.Id,
                    x.WipQty,
                    x.Status
                })
                .ToListAsync();

            var shopOrderIds = shopOrderData.Select(x => x.Id).Distinct().ToList();

            var shopOrderItems = await _tipsProductionDbContext.ShopOrderItems
                .Where(x => x.FGItemNumber == fgItemNumber &&
                            x.ProjectNumber == projectNumber &&
                            x.SalesOrderNumber == salesOrderNumber &&
                            shopOrderIds.Contains(x.ShopOrderId))
                .ToListAsync(); 

            var shopOrderShortCloseDetails = shopOrderItems.Select(s => new ShopOrderShortCloseDto
            {
                SOReleaseQty = s.ReleaseQty,
                WipQty = shopOrderData.Where(o => o.Id == s.ShopOrderId).Select(x=>x.WipQty).FirstOrDefault(),
                Status = shopOrderData.Where(o => o.Id == s.ShopOrderId).Select(x => x.Status).FirstOrDefault()
            }).ToList();

            return shopOrderShortCloseDetails;


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
        public async Task<int?> GetShopOrderItemShortCloseCount(int shopOrderId)
        {
            var shopOrderItemOpenStatusCount = _tipsProductionDbContext.ShopOrderItems
                                        .Where(x => x.ShopOrderId == shopOrderId && x.Status != OrderStatus.ShortClose).Count();

            return shopOrderItemOpenStatusCount;
        }
    }
}
