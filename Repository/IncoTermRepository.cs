using Contracts;
using Entities;
using Entities.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class IncoTermRepository : RepositoryBase<IncoTerm>, IIncoTermRepository

    {
        public IncoTermRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateIncoTerm(IncoTerm incoTerm)
        {
            incoTerm.CreatedBy = "Admin";
            incoTerm.CreatedOn = DateTime.Now;
            var result = await Create(incoTerm);
            incoTerm.Unit = "Bangalore";
            return result.Id;
        }

        public async Task<string> DeleteIncoTerm(IncoTerm incoTerm)
        {
            Delete(incoTerm);
            string result = $"Inco Terms details of {incoTerm.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<IncoTerm>> GetAllActiveIncoTerm()
        {
            var incoTerms = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return incoTerms;
        }

        public async Task<IEnumerable<IncoTerm>> GetAllIncoTerm()
        {
            var incoTerms = await FindAll().ToListAsync();

            return incoTerms;
        }

        public async Task<IncoTerm> GetIncoTermById(int id)
        {
            var incoTerm = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return incoTerm;
        }

        public async Task<string> UpdateIncoTerm(IncoTerm incoTerm)
        {
            incoTerm.LastModifiedBy = "Admin";
            incoTerm.LastModifiedOn = DateTime.Now;
            Update(incoTerm);
            string result = $"Inco Term of Detail {incoTerm.Id} is updated successfully!";
            return result;
        }
    }
}
