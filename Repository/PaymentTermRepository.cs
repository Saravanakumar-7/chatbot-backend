using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class PaymentTermRepository : RepositoryBase<PaymentTerm>, IPaymentTermRepository
    {
        public PaymentTermRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreatePaymentTerm(PaymentTerm paymentTerm)
        {
            paymentTerm.CreatedBy = "Admin";
            paymentTerm.CreatedOn = DateTime.Now;
            var result = await Create(paymentTerm);
            return result.Id;
        }

        public async Task<string> DeletePaymentTerm(PaymentTerm paymentTerm)
        {

            Delete(paymentTerm);
            string result = $"AuditFrequency details of {paymentTerm.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<PaymentTerm>> GetAllActivepaymentTerms()
        {

            var PaymentTermList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return PaymentTermList;
        }

        public async Task<IEnumerable<PaymentTerm>> GetAllpaymentTerms()
        {

            var PaymentTermList = await FindAll().ToListAsync();

            return PaymentTermList;
        }

        public async Task<PaymentTerm> GetpaymentTermById(int id)
        {

            var PaymentTerm = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return PaymentTerm;
        }

        public async Task<string> UpdatePaymentTerm(PaymentTerm paymentTerm)
        {
            paymentTerm.LastModifiedBy = "Admin";
            paymentTerm.LastModifiedOn = DateTime.Now;
            Update(paymentTerm);
            string result = $"AuditFrequency details of {paymentTerm.Id} is updated successfully!";
            return result;
        }
    }
}
