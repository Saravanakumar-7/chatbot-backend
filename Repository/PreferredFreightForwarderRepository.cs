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
    internal class PreferredFreightForwarderRepository : RepositoryBase<PreferredFreightForwarder>, IPreferredFreightForwarderRepository
    {
        public PreferredFreightForwarderRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreatePreferredFreightForwarder(PreferredFreightForwarder preferredFreightForwarder)
        {
            preferredFreightForwarder.CreatedBy = "Admin";
            preferredFreightForwarder.CreatedOn = DateTime.Now;
            var result = await Create(preferredFreightForwarder);
            return result.Id;
        }

        public async Task<string> DeletePreferredFreightForwarder(PreferredFreightForwarder preferredFreightForwarder)
        {

            Delete(preferredFreightForwarder);
            string result = $"AuditFrequency details of {preferredFreightForwarder.Id} is deleted successfully!";
            return result;
        }

        public Task<IEnumerable<PreferredFreightForwarder>> GetAllActivePreferredFreightForwarders()
        {
            throw new NotImplementedException();
        }

        public Task<PreferredFreightForwarder> GetPreferredFreightForwarderById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PreferredFreightForwarder>> GetPreferredFreightForwarders()
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdatePreferredFreightForwarder(PreferredFreightForwarder preferredFreightForwarder)
        {
            throw new NotImplementedException();
        }
    }
}
