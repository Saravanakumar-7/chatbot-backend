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
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DeliveryTermRepository : RepositoryBase<DeliveryTerm>, IDeliveryTermRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public DeliveryTermRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateDeliveryTerm(DeliveryTerm deliveryTerm)
        {
            deliveryTerm.CreatedBy = _createdBy;
            deliveryTerm.CreatedOn = DateTime.Now;
            deliveryTerm.Unit = _unitname;
            var result = await Create(deliveryTerm);
            return result.Id;
        }

        public async Task<string> DeleteDeliveryTerm(DeliveryTerm deliveryTerm)
        {
            Delete(deliveryTerm);
            string result = $"Delivery Terms details of {deliveryTerm.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<DeliveryTerm>> GetAllActiveDeliveryTerms()
        {
            var deliveryTermDetails = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return deliveryTermDetails;

        }

        public async Task<IEnumerable<DeliveryTerm>> GetAllDeliveryTerms()
        {

            var deliveryTermDetails = await FindAll().ToListAsync();
            return deliveryTermDetails;

        }

        public async Task<DeliveryTerm> GetDeliveryTermById(int id)
        {
            var DeliveryTermbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return DeliveryTermbyId;
          
        }

        public async Task<string> UpdateDeliveryTerm(DeliveryTerm deliveryTerm)
        {
            deliveryTerm.LastModifiedBy = _createdBy;
            deliveryTerm.LastModifiedOn = DateTime.Now;
            Update(deliveryTerm);
            string result = $"Delivery Term of Detail {deliveryTerm.Id} is updated successfully!";
            return result;
        
        }
    }
}
