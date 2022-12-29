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
    public class SecondarySourceRepository : RepositoryBase<SecondarySource>, ISecondarySourceRepository
    {
        public SecondarySourceRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateSecondarySource(SecondarySource secondarySource)
        {
            secondarySource.CreatedBy = "Admin";
            secondarySource.CreatedOn = DateTime.Now;
            secondarySource.Unit = "Bangalore";
            var result = await Create(secondarySource);
            
            return result.Id;

        }

        public async Task<string> DeleteSecondarySource(SecondarySource secondarySource)
        {
            Delete(secondarySource);
            string result = $"secondarySource details of {secondarySource.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<SecondarySource>> GetAllActiveSecondarySources()
        {

            var AllActiveSecondarySources = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveSecondarySources;
        }

        public async Task<IEnumerable<SecondarySource>> GetAllSecondarySources()
        {

            var GetallSecondarySourcesList = await FindAll().ToListAsync();

            return GetallSecondarySourcesList;
        }

        public async Task<SecondarySource> GetSecondarySourceById(int id)
        {
            var SecondarySourcesbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return SecondarySourcesbyId;
        }
        public async Task<string> UpdateSecondarySource(SecondarySource secondarySource)
        {

            secondarySource.LastModifiedBy = "Admin";
            secondarySource.LastModifiedOn = DateTime.Now;
            Update(secondarySource);
            string result = $"secondarySource details of {secondarySource.Id} is updated successfully!";
            return result;
        }
    }
}
