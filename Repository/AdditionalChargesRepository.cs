using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class AdditionalChargesRepository : RepositoryBase<AdditionalCharges>, IAdditionalChargesRepository
    {
        public AdditionalChargesRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateAdditionalCharges(AdditionalCharges additionalCharges)
        {
            additionalCharges.CreatedBy = "Admin";
            additionalCharges.CreatedOn = DateTime.Now;
            additionalCharges.Unit = "Bangalore";
            var result = await Create(additionalCharges);

            return result.Id;
        }

        public async Task<string> DeleteAdditionalCharges(AdditionalCharges additionalCharges)
        {
            Delete(additionalCharges);
            string result = $"AdditionalCharges details of {additionalCharges.Id} is deleted successfully!";
            return result;
        }

        public async Task<AdditionalCharges> GetAdditionalChargesById(int id)
        {
            var additionalChargesbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return additionalChargesbyId;
        }

        public async Task<IEnumerable<AdditionalCharges>> GetAllActiveAdditionalCharges()
        {
            var additionalChargesActiveDetails = await FindByCondition(x => x.ActiveStatus == true).ToListAsync();
            return additionalChargesActiveDetails;
        }

        public async Task<IEnumerable<AdditionalCharges>> GetAllAdditionalCharges()
        {
            var additionalChargesDetails = await FindAll().ToListAsync();
            return additionalChargesDetails;
        }

        public async Task<string> UpdateAdditionalCharges(AdditionalCharges additionalCharges)
        {
            additionalCharges.LastModifiedBy = "Admin";
            additionalCharges.LastModifiedOn = DateTime.Now;
            Update(additionalCharges);
            string result = $"AdditionalCharges details of {additionalCharges.Id} is updated successfully!";
            return result;
        }
    }
}
