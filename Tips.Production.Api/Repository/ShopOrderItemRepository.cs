using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Repository
{
    public class ShopOrderItemRepository : RepositoryBase<ShopOrderItem>, IShopOrderItemRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;

        public ShopOrderItemRepository(TipsProductionDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;

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
