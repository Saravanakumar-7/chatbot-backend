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
    public class SourceDetailsRepository : RepositoryBase<SourceDetails>, ISourceDetailsRepository
    {
        public SourceDetailsRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        { }
        public async Task<int?> CreateSourceDetails(SourceDetails sourceDetails)
        {
            sourceDetails.CreatedBy = "Admin";
            sourceDetails.CreatedOn = DateTime.Now;
            sourceDetails.Unit = "Bangalore";
            var result = await Create(sourceDetails); return result.Id;
        }
        public async Task<string> DeleteSourceDetails(SourceDetails sourceDetails)
        {
            Delete(sourceDetails);
            string result = $"sourceDetails details of {sourceDetails.Id} is deleted successfully!";
            return result;
        }
        public async Task<IEnumerable<SourceDetails>> GetAllActiveSourceDetails()
        {
            var AllActivesourceDetails = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActivesourceDetails;
        }
        public async Task<IEnumerable<SourceDetails>> GetAllSourceDetails()
        {
            var GetallSourceDetails = await FindAll().ToListAsync(); return GetallSourceDetails;
        }
        public async Task<SourceDetails> GetSourceDetailsById(int id)
        {
            var sourceDetailsbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); return sourceDetailsbyId;
        }
        public async Task<string> UpdateSourceDetails(SourceDetails sourceDetails)
        {
            sourceDetails.LastModifiedBy = "Admin";
            sourceDetails.LastModifiedOn = DateTime.Now;
            Update(sourceDetails);
            string result = $"sourceDetails details of {sourceDetails.Id} is updated successfully!";
            return result;
        }
    }
}