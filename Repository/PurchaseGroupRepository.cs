using Contracts;
using Entities;
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
    public class PurchaseGroupRepository : RepositoryBase<PurchaseGroup>, IPurchaseGroupRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public PurchaseGroupRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreatePurchaseGroup(PurchaseGroup purchaseGroup)
        {
            purchaseGroup.CreatedBy = _createdBy;
            purchaseGroup.CreatedOn = DateTime.Now;
            purchaseGroup.Unit = _unitname;
            var result = await Create(purchaseGroup);

            return result.Id;
        }

        public async Task<string> DeletePurchaseGroup(PurchaseGroup purchaseGroup)
        {
            Delete(purchaseGroup);
            string result = $"PurchaseGroup details of {purchaseGroup.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<PurchaseGroup>> GetAllActivePurchaseGroups([FromQuery] SearchParames searchParams)
        {
            var purchaseGroupDetails = FindAll()
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.PurchaseGroupName.Contains(searchParams.SearchValue) ||
          inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return purchaseGroupDetails;
        }

        public async Task<IEnumerable<PurchaseGroup>> GetAllPurchaseGroups([FromQuery] SearchParames searchParams)
        {
            var purchaseGroupDetails = FindAll()
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.PurchaseGroupName.Contains(searchParams.SearchValue) ||
          inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return purchaseGroupDetails;
        }
      

        public async Task<PurchaseGroup> GetPurchaseGroupById(int id)
        {
            var PurchaseGroupbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return PurchaseGroupbyId;
        }

        public async Task<string> UpdatePurchaseGroup(PurchaseGroup purchaseGroup)
        {
            purchaseGroup.LastModifiedBy = _createdBy;
            purchaseGroup.LastModifiedOn = DateTime.Now;
            Update(purchaseGroup);
            string result = $"PurchaseGroup details of {purchaseGroup.Id} is updated successfully!";
            return result;
        }
    }
}
