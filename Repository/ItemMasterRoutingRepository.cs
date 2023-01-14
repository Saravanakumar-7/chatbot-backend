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
            var itemIdNoList = await TipsMasterDbContext.ItemMasters
                .Where(im => itemNumberList.Contains(im.ItemNumber))
                .Select(x => new { x.Id,x.ItemNumber}).ToListAsync();

            List<long> itemIds = itemIdNoList.Select(x=>x.Id).ToList(); 



            var getItemsRoutingDetailsForLpCosting = await TipsMasterDbContext.ItemMasterRoutings
                               .Select(c => new ItemMasterRoutingListDto()
                               {
                                   
                                   Process = c.Process,
                                   ProcessStep = c.ProcessStep,
                                   MachineHours = c.MachineHours,
                                   LaborHours = c.LaborHours,
                                   ItemNumber = itemIdNoList.Where(x=>x.Id == c.ItemMasterId)
                                   .Select(x=>x.ItemNumber).FirstOrDefault()

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