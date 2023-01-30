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
        
            shopOrderConfirmation.CreatedBy = "Admin";
            shopOrderConfirmation.CreatedOn = DateTime.Now;
            shopOrderConfirmation.Unit = "Banglore";
            var result = await Create(shopOrderConfirmation);
            return result.Id;
        }

        public async Task<IEnumerable<ShopOrderConfirmation>> GetAllShopOrderConfirmations()
        {
            var shopOrderConfirmationDetails = await FindAll().ToListAsync();
            return (shopOrderConfirmationDetails);

        }

        public async Task<ShopOrderConfirmation> GetShopOrderConfirmationById(int id)
        {
            var shopOrderConfirmationDetailById = await 
                            FindByCondition(x => x.Id == id)
                             .FirstOrDefaultAsync();
            return shopOrderConfirmationDetailById;
        }

        public async Task<string> UpdateShopOrderConfirmation(ShopOrderConfirmation shopOrderConfirmation)
        {
            shopOrderConfirmation.LastModifiedBy = "Admin";
            shopOrderConfirmation.LastModifiedOn = DateTime.Now;
            Update(shopOrderConfirmation);
            string result = $"ShopOrderConfirmation details of {shopOrderConfirmation.Id} is updated successfully!";
            return result;
        }

        public async Task<IEnumerable<ShopOrderConfirmation>> GetAllShopOrderConfirmationByShopOrderNo(string shopOrderNo)
        {
            var shopOrderConfirmationByShopOrderNoList = await FindByCondition(x => x.ShopOrderNumber ==shopOrderNo).ToListAsync();                
            return shopOrderConfirmationByShopOrderNoList;

        }
        
        public async Task<IEnumerable<ShopOrderConfirmation>> GetOpenDataForOqcByShopOrderNo(string shopOrderNo)
        {
            var openDataForOqcByShopOrderNoList = await FindByCondition(x => x.ShopOrderNumber == shopOrderNo &&  x.IsOQCDone == false).ToListAsync();           
            return openDataForOqcByShopOrderNoList;

        }
        
   }
    
}
