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
    public class SourceRepository : RepositoryBase<Source>, ISourceRepository
    {
        public SourceRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateSource(Source source)
        {
            source.CreatedBy = "Admin";
            source.CreatedOn = DateTime.Now;
            source.Unit = "Bangalore";
            var result = await Create(source);
         
            return result.Id;

        }

        public async Task<string> DeleteSource(Source source)
        {
            Delete(source);
            string result = $"source details of {source.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Source>> GetAllActiveSources()
        {

            var AllActiveSecondarySources = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveSecondarySources;
        }

        public async Task<IEnumerable<Source>> GetAllSources()
        {

            var GetallSources = await FindAll().ToListAsync();

            return GetallSources;
        }

        public async Task<Source> GetSourceById(int id)
        {
            var SourcesbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return SourcesbyId;
        }
        public async Task<string> UpdateSource(Source source)
        {

            source.LastModifiedBy = "Admin";
            source.LastModifiedOn = DateTime.Now;
            Update(source);
            string result = $"Source details of {source.Id} is updated successfully!";
            return result;
        }
    }
}
