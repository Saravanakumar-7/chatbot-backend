using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IDeliveryTermRepository
    {
        Task<IEnumerable<DeliveryTerm>> GetAllDeliveryTerms();
        Task<DeliveryTerm> GetDeliveryTermById(int id);
        Task<IEnumerable<DeliveryTerm>> GetAllActiveDeliveryTerms();
        Task<int?> CreateDeliveryTerm(DeliveryTerm deliveryTerm);
        Task<string> UpdateDeliveryTerm(DeliveryTerm deliveryTerm);
        Task<string> DeleteDeliveryTerm(DeliveryTerm deliveryTerm);
    }
}
