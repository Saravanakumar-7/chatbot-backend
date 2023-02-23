using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DeliveryTermRepository : RepositoryBase<DeliveryTerm>, IDeliveryTermRepository
    {
        public DeliveryTermRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateDeliveryTerm(DeliveryTerm deliveryTerm)
        {
            deliveryTerm.CreatedBy = "Admin";
            deliveryTerm.CreatedOn = DateTime.Now;
            deliveryTerm.Unit = "Bangalore";
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
            var AllActiveDeliveryTerm = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveDeliveryTerm;

        }

        public async Task<IEnumerable<DeliveryTerm>> GetAllDeliveryTerms()
        {

            var GetallDeliveryTerms = await FindAll().ToListAsync();

            return GetallDeliveryTerms;
            
        }

        public async Task<DeliveryTerm> GetDeliveryTermById(int id)
        {
            var DeliveryTermbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return DeliveryTermbyId;
          
        }

        public async Task<string> UpdateDeliveryTerm(DeliveryTerm deliveryTerm)
        {
            deliveryTerm.LastModifiedBy = "Admin";
            deliveryTerm.LastModifiedOn = DateTime.Now;
            Update(deliveryTerm);
            string result = $"Delivery Term of Detail {deliveryTerm.Id} is updated successfully!";
            return result;
        
        }
    }
}
