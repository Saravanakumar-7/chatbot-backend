using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
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
    public class PaymentTermRepository : RepositoryBase<PaymentTerm>, IPaymentTermRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public PaymentTermRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreatePaymentTerm(PaymentTerm paymentTerm)
        {
            paymentTerm.CreatedBy = _createdBy;
            paymentTerm.CreatedOn = DateTime.Now;
            paymentTerm.Unit = _unitname;
            var result = await Create(paymentTerm);
           
            return result.Id;
        }

        public async Task<string> DeletePaymentTerm(PaymentTerm paymentTerm)
        {

            Delete(paymentTerm);
            string result = $"PaymentTerm details of {paymentTerm.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<PaymentTerm>> GetAllActivepaymentTerms()
        {

            var AllActivePaymentTerms = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivePaymentTerms;
        }

        public async Task<IEnumerable<PaymentTerm>> GetAllpaymentTerms()
        {

            var GetallPaymentTerms = await FindAll().ToListAsync();

            return GetallPaymentTerms;
        }

        public async Task<PaymentTerm> GetpaymentTermById(int id)
        {

            var PaymentTermbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return PaymentTermbyId;
        }

        public async Task<string> UpdatePaymentTerm(PaymentTerm paymentTerm)
        {
            paymentTerm.LastModifiedBy = _createdBy;
            paymentTerm.LastModifiedOn = DateTime.Now;
            Update(paymentTerm);
            string result = $"PaymentTerm details of {paymentTerm.Id} is updated successfully!";
            return result;
        }
    }
}
