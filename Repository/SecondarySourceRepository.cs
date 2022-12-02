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
            var result = await Create(secondarySource);
            secondarySource.Unit = "Bangalore";
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

            var secondarySegments = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return secondarySegments;
        }

        public async Task<IEnumerable<SecondarySource>> GetAllSecondarySources()
        {

            var secondarySegmentsList = await FindAll().ToListAsync();

            return secondarySegmentsList;
        }

        public async Task<SecondarySource> GetSecondarySourceById(int id)
        {
            var secondarySegments = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return secondarySegments;
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
