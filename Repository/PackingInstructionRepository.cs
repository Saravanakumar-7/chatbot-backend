using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class PackingInstructionRepository : RepositoryBase<PackingInstruction>, IPackingInstructionRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public PackingInstructionRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreatePackingInstruction(PackingInstruction packingInstruction)
        {
            packingInstruction.CreatedBy = _createdBy;
            packingInstruction.CreatedOn = DateTime.Now;
            packingInstruction.Unit = _unitname;
            var result = await Create(packingInstruction);
            
            return result.Id;
        }

        public async Task<string> DeletePackingInstruction(PackingInstruction packingInstruction)
        {
            Delete(packingInstruction);
            string result = $"AuditFrequency details of {packingInstruction.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<PackingInstruction>> GetAllActivePackingInstruction([FromQuery] SearchParames searchParams)
        {
            var packingInstructionDetails = FindAll()
                                         .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.PackingInstructionsName.Contains(searchParams.SearchValue) ||
                                   inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return packingInstructionDetails;
        }

        public async Task<IEnumerable<PackingInstruction>> GetAllPackingInstruction([FromQuery] SearchParames searchParams)
        {
            var packingInstructionDetails = FindAll()
                                         .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.PackingInstructionsName.Contains(searchParams.SearchValue) ||
                                   inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return packingInstructionDetails;
        }

        public async Task<PackingInstruction> GetPackingInstructionById(int id)
        {
            var PackingInstructionbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return PackingInstructionbyId;
        }

        public async Task<string> UpdatePackingInstruction(PackingInstruction packingInstruction)
        {
            packingInstruction.LastModifiedBy = _createdBy;
            packingInstruction.LastModifiedOn = DateTime.Now;
            Update(packingInstruction);
            string result = $"UpdatePackingInstruction details of {packingInstruction.Id} is updated successfully!";
            return result;
        }
    }
}
