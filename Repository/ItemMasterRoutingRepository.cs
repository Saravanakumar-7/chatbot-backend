using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ItemMasterRoutingRepository : RepositoryBase<ItemMasterRouting>, IItemMasterRoutingRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ItemMasterRoutingRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }


        public async Task<List<ItemMasterRoutingListDto>> GetItemsRoutingDetailsForLpCosting(List<string> itemNumberList)
        {
            var itemIdNoList = await TipsMasterDbContext.ItemMasters
                .Where(im => itemNumberList.Contains(im.ItemNumber))
                .Select(x => new { x.Id,x.ItemNumber}).ToListAsync();

            List<long> itemIds = itemIdNoList.Select(x=>x.Id).ToList();

           // IEnumerable<ItemMasterRouting> itemMasterRoutings = await TipsMasterDbContext.ItemMasterRoutings
                               //.Where(x => itemIds.Contains(x.ItemMasterId)).ToListAsync();


            List<ItemMasterRoutingListDto> getItemsRoutingDetailsForLpCosting = await TipsMasterDbContext.ItemMasterRoutings
                               .Where(x => itemIds.Contains(x.ItemMasterId))
                               .Select(c => new ItemMasterRoutingListDto()
                               {
                                   ProcessSteps = c.ProcessStep,
                                   MachineHrs = c.MachineHours,
                                   LabourHrs = c.LaborHours,
                                   ItemNumber = itemIdNoList.Where(x => x.Id == c.ItemMasterId)
                                   .Select(x => x.ItemNumber).FirstOrDefault()
                               })
                               .ToListAsync();

            return getItemsRoutingDetailsForLpCosting;


        }

        public async Task<List<ItemMasterRoutingListDto>> GetItemsRoutingDetailsForLpCosting(string itemNumber)
        {
            var itemIdNo = await TipsMasterDbContext.ItemMasters
                .Where(im =>im.ItemNumber == itemNumber)
                .Select(x => new { x.Id, x.ItemNumber }).FirstOrDefaultAsync();


            List<ItemMasterRoutingListDto> getItemsRoutingDetailsForLpCosting = await TipsMasterDbContext.ItemMasterRoutings
                               .Where(x => x.ItemMasterId == itemIdNo.Id)
                               .Select(c => new ItemMasterRoutingListDto()
                               {
                                   ProcessSteps = c.ProcessStep,
                                   MachineHrs = c.MachineHours,
                                   LabourHrs = c.LaborHours,
                                   ItemNumber = itemIdNo.ItemNumber
                               })
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