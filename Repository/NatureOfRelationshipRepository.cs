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
    internal class NatureOfRelationshipRepository : RepositoryBase<NatureOfRelationship>, INatureOfRelationshipRepository
    {
        public NatureOfRelationshipRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateNatureOfRelationship(NatureOfRelationship natureOfRelationship)
        {
            natureOfRelationship.CreatedBy = "Admin";
            natureOfRelationship.CreatedOn = DateTime.Now;
            var result = await Create(natureOfRelationship);
            return result.Id;
        }

        public async Task<string> DeleteNatureOfRelationship(NatureOfRelationship natureOfRelationship)
        {
            Delete(natureOfRelationship);
            string result = $"NatureOfRelationship details of {natureOfRelationship.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<NatureOfRelationship>> GetAllActiveNatureOfRelationships()
        {

            var natureOfRelationshipList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return natureOfRelationshipList;
        }

        public async Task<IEnumerable<NatureOfRelationship>> GetAllNatureOfRelationships()
        {
            var natureOfRelationshipList = await FindAll().ToListAsync();

            return natureOfRelationshipList;
        }

        public async Task<NatureOfRelationship> GetNatureOfRelationshipById(int id)
        {

            var natureOfRelationship = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return natureOfRelationship;
        }

        public async Task<string> UpdateNatureOfRelationship(NatureOfRelationship natureOfRelationship)
        {
            natureOfRelationship.LastModifiedBy = "Admin";
            natureOfRelationship.LastModifiedOn = DateTime.Now;
            Update(natureOfRelationship);
            string result = $"Customer Type details of {natureOfRelationship.Id} is updated successfully!";
            return result;
        }
    }
}
