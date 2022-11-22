using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Repository
{
    public class ShopOrderConfirmationRepository : RepositoryBase<ShopOrderConfirmation>, IShopOrderConfirmationRepository
    {
        private TipsProductionDbContext _tipsProductionDbContext;

        public ShopOrderConfirmationRepository(TipsProductionDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsProductionDbContext = repositoryContext;
        }

        public async Task<long> CreateShopOrderConfirmation(ShopOrderConfirmation shopOrderConfirmation)
        {
            shopOrderConfirmation.LastModifiedBy = "Admin";
            shopOrderConfirmation.LastModifiedOn = DateTime.Now;
            shopOrderConfirmation.CreatedBy = "Admin";
            shopOrderConfirmation.CreatedOn = DateTime.Now;
            var result = await Create(shopOrderConfirmation);
            return result.Id;
        }

        public async Task<IEnumerable<ShopOrderConfirmation>> GetAllShopOrderConfirmation()
        {
            var shopOrderConfirmationList = await FindAll().ToListAsync();
            return (shopOrderConfirmationList);

        }

        public async Task<ShopOrderConfirmation> GetShopOrderConfirmationById(int id)
        {
            var shopOrderConditionList = await 
                            FindByCondition(x => x.Id == id)
                             .FirstOrDefaultAsync();
            return shopOrderConditionList;
        }

        public async Task<string> UpdateShopOrderConfirmation(ShopOrderConfirmation shopOrderConfirmation)
        {
            shopOrderConfirmation.LastModifiedBy = "Admin";
            shopOrderConfirmation.LastModifiedOn = DateTime.Now;
            Update(shopOrderConfirmation);
            string result = $"LeadTime details of {shopOrderConfirmation.Id} is updated successfully!";
            return result;
        }

        public async Task<IEnumerable<ShopOrderConfirmation>> GetAllShopOrderConfirmationByShopOrderNo(string shopOrderNo)
        {
            var shopOrderConfirmationList = await FindByCondition(x => x.ShopOrderNo ==shopOrderNo).ToListAsync();
                            // .FirstOrDefaultAsync();
            return shopOrderConfirmationList;

        }
        
        public async Task<IEnumerable<ShopOrderConfirmation>> GetOpenDataForOqcByShopOrderNo(string shopOrderNo)
        {
            var shopOrderConfirmationList = await FindByCondition(x => x.ShopOrderNo == shopOrderNo &&  x.IsOQCDone == false).ToListAsync();
            // .FirstOrDefaultAsync();
            return shopOrderConfirmationList;

        }
        
   }
    
}
