using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ItemMasterRoutingRepository : RepositoryBase<ItemMasterRouting>, IItemMasterRoutingRepository
    {
        public ItemMasterRoutingRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public Task<int?> CreateItemMasterRouting(ItemMasterRouting itemMasterRouting)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteItemMasterRouting(ItemMasterRouting itemMasterRouting)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ItemMasterRouting>> GetAllActiveItemMasterRoutings()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ItemMasterRouting>> GetAllItemMasterRoutings()
        {
            throw new NotImplementedException();
        }

        public async Task<ItemMasterRouting> GetAllItemsProcessList(int id)
        {
            var getRountingList = await TipsMasterDbContext.ItemMasterRoutings
                                   .Where(x => x.ItemMasterId == id).FirstOrDefaultAsync();
            return getRountingList;

        } 

        public Task<ItemMasterRouting> GetItemMasterRoutingById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateItemMasterRouting(ItemMasterRouting itemMasterRouting)
        {
            throw new NotImplementedException();
        }
    }
}