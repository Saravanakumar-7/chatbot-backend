using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IPurchaseGroupRepository : IRepositoryBase<PurchaseGroup>

    {
       
            Task<IEnumerable<PurchaseGroup>> GetAllPurchaseGroups();
            Task<PurchaseGroup> GetPurchaseGroupById(int id);
            Task<IEnumerable<PurchaseGroup>> GetAllActivePurchaseGroups();
            Task<int?> CreatePurchaseGroup(PurchaseGroup purchaseGroup);
            Task<string> UpdatePurchaseGroup(PurchaseGroup purchaseGroup);
            Task<string> DeletePurchaseGroup(PurchaseGroup purchaseGroup);
        }
    }

