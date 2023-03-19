using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
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
            natureOfRelationship.Unit = "Bangalore";
            var result = await Create(natureOfRelationship);
            
            return result.Id;
        }

        public async Task<string> DeleteNatureOfRelationship(NatureOfRelationship natureOfRelationship)
        {
            Delete(natureOfRelationship);
            string result = $"NatureOfRelationship details of {natureOfRelationship.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<NatureOfRelationship>> GetAllActiveNatureOfRelationships([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {

            var natureOfRelationshipDetails = FindAll()
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.NatureOfRelationshipName.Contains(searchParams.SearchValue) ||
            inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<NatureOfRelationship>.ToPagedList(natureOfRelationshipDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<NatureOfRelationship>> GetAllNatureOfRelationships([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var natureOfRelationshipDetails = FindAll().OrderByDescending(x => x.Id)
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.NatureOfRelationshipName.Contains(searchParams.SearchValue) ||
            inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<NatureOfRelationship>.ToPagedList(natureOfRelationshipDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<NatureOfRelationship> GetNatureOfRelationshipById(int id)
        {

            var NatureOfRelationshipbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return NatureOfRelationshipbyId;
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
