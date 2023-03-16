using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    internal class PackingInstructionRepository : RepositoryBase<PackingInstruction>, IPackingInstructionRepository
    {
        public PackingInstructionRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreatePackingInstruction(PackingInstruction packingInstruction)
        {
            packingInstruction.CreatedBy = "Admin";
            packingInstruction.CreatedOn = DateTime.Now;
            packingInstruction.Unit = "Bangalore";
            var result = await Create(packingInstruction);
            
            return result.Id;
        }

        public async Task<string> DeletePackingInstruction(PackingInstruction packingInstruction)
        {
            Delete(packingInstruction);
            string result = $"AuditFrequency details of {packingInstruction.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<PackingInstruction>> GetAllActivePackingInstruction([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {

            var natureOfRelationshipDetails = FindAll()
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.PackingInstructionsName.Contains(searchParams.SearchValue) ||
           inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<PackingInstruction>.ToPagedList(natureOfRelationshipDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<PackingInstruction>> GetAllPackingInstruction([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {

            var natureOfRelationshipDetails = FindAll().OrderByDescending(x => x.Id)
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.PackingInstructionsName.Contains(searchParams.SearchValue) ||
           inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<PackingInstruction>.ToPagedList(natureOfRelationshipDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PackingInstruction> GetPackingInstructionById(int id)
        {
            var PackingInstructionbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return PackingInstructionbyId;
        }

        public async Task<string> UpdatePackingInstruction(PackingInstruction packingInstruction)
        {
            packingInstruction.LastModifiedBy = "Admin";
            packingInstruction.LastModifiedOn = DateTime.Now;
            Update(packingInstruction);
            string result = $"UpdatePackingInstruction details of {packingInstruction.Id} is updated successfully!";
            return result;
        }
    }
}
