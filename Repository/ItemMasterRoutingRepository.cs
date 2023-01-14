using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        
        public async Task<List<ItemMasterRoutingListDto>> GetItemsRoutingDetailsForLpCosting(List<string> itemNumberList)
        {
            List<long> itemIds = await TipsMasterDbContext.ItemMasters
                .Where(im => itemNumberList.Contains(im.ItemNumber))
                .Select(x=> x.Id).ToListAsync();

            var getItemsRoutingDetailsForLpCosting = await TipsMasterDbContext.ItemMasterRoutings
                               .Select(c => new ItemMasterRoutingListDto()
                               {
                                   
                                   Process = c.Process,
                                   ProcessStep = c.ProcessStep,
                                   MachineHours = c.MachineHours,
                                   LaborHours = c.LaborHours

                               }).Where(x => itemIds.Contains(x.Id))
                             .ToListAsync();

            return getItemsRoutingDetailsForLpCosting;


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