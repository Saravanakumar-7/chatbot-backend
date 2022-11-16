using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IPaymentTermRepository : IRepositoryBase<PaymentTerm>
    {
        Task<IEnumerable<PaymentTerm>> GetAllpaymentTerms();
        Task<PaymentTerm> GetpaymentTermById(int id);
        Task<IEnumerable<PaymentTerm>> GetAllActivepaymentTerms();
        Task<int?> CreatePaymentTerm(PaymentTerm paymentTerm);
        Task<string> UpdatePaymentTerm(PaymentTerm paymentTerm);
        Task<string> DeletePaymentTerm(PaymentTerm paymentTerm);
    }
}
