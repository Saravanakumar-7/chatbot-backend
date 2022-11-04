using Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DeliveryTermRepository : RepositoryBase<CustomerType>, IDeliveryTermRepository
    {
        public DeliveryTermRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public Task<int?> CreateDeliveryTerm(DeliveryTerm deliveryTerm)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteDeliveryTerm(DeliveryTerm deliveryTerm)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeliveryTerm>> GetAllActiveDeliveryTerms()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeliveryTerm>> GetAllDeliveryTerms()
        {
            throw new NotImplementedException();
        }

        public Task<DeliveryTerm> GetDeliveryTermById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateDeliveryTerm(DeliveryTerm deliveryTerm)
        {
            throw new NotImplementedException();
        }
    }
}
