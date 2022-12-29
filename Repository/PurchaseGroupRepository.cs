using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class PurchaseGroupRepository : RepositoryBase<PurchaseGroup>, IPurchaseGroupRepository

    {
        public PurchaseGroupRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreatePurchaseGroup(PurchaseGroup purchaseGroup)
        {
            purchaseGroup.CreatedBy = "Admin";
            purchaseGroup.CreatedOn = DateTime.Now;
            purchaseGroup.Unit = "Bangalore";
            var result = await Create(purchaseGroup);

            return result.Id;
        }

        public async Task<string> DeletePurchaseGroup(PurchaseGroup purchaseGroup)
        {
            Delete(purchaseGroup);
            string result = $"PurchaseGroup details of {purchaseGroup.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<PurchaseGroup>> GetAllActivePurchaseGroups()
        {
            var AllActivepurchaseGroups = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivepurchaseGroups;
        }

        public async Task<IEnumerable<PurchaseGroup>> GetAllPurchaseGroups()
        {
            var GetallPurchaseGroups = await FindAll().ToListAsync();

            return GetallPurchaseGroups;
        }
      

        public async Task<PurchaseGroup> GetPurchaseGroupById(int id)
        {
            var PurchaseGroupbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return PurchaseGroupbyId;
        }

        public async Task<string> UpdatePurchaseGroup(PurchaseGroup purchaseGroup)
        {
            purchaseGroup.LastModifiedBy = "Admin";
            purchaseGroup.LastModifiedOn = DateTime.Now;
            Update(purchaseGroup);
            string result = $"PurchaseGroup details of {purchaseGroup.Id} is updated successfully!";
            return result;
        }
    }
}
