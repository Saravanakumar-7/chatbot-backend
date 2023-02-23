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
            preferredFreightForwarder.Unit = "Bangalore";
            var result = await Create(preferredFreightForwarder);
            
            return result.Id;
        }

        public async Task<string> DeletePreferredFreightForwarder(PreferredFreightForwarder preferredFreightForwarder)
        {
            Delete(preferredFreightForwarder);
            string result = $"preferredFreightForwarder details of {preferredFreightForwarder.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<PreferredFreightForwarder>> GetAllActivePreferredFreightForwarders()
        {

            var AllActivePreferredFreightForwarder = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivePreferredFreightForwarder;
        }

        public async Task<IEnumerable<PreferredFreightForwarder>> GetAllPreferredFreightForwarders()
        {
            var GetallPreferredFreightForwarderlist = await FindAll().ToListAsync();

            return GetallPreferredFreightForwarderlist;
        }

        public async Task<PreferredFreightForwarder> GetPreferredFreightForwarderById(int id)
        {
            var PreferredFreightForwarderbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return PreferredFreightForwarderbyId;
        }

        public async Task<string> UpdatePreferredFreightForwarder(PreferredFreightForwarder preferredFreightForwarder)
        {
            preferredFreightForwarder.LastModifiedBy = "Admin";
            preferredFreightForwarder.LastModifiedOn = DateTime.Now;
            Update(preferredFreightForwarder);
            string result = $"PreferredFreightForwarder details of {preferredFreightForwarder.Id} is updated successfully!";
            return result;
        }
    }
}
