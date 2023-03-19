using Contracts;
using Entities;
using Entities.Helper;
using Entities.Migrations;
using Microsoft.AspNetCore.Mvc;
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
            incoTerm.Unit = "Bangalore";
            var result = await Create(incoTerm);
           
            return result.Id;
        }

        public async Task<string> DeleteIncoTerm(IncoTerm incoTerm)
        {
            Delete(incoTerm);
            string result = $"Inco Terms details of {incoTerm.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<IncoTerm>> GetAllActiveIncoTerm([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var incotermDetails = FindAll()
                       .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.IncoTermName.Contains(searchParams.SearchValue) ||
                       inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<IncoTerm>.ToPagedList(incotermDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<IncoTerm>> GetAllIncoTerm([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var incotermDetails = FindAll().OrderByDescending(x => x.Id)
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.IncoTermName.Contains(searchParams.SearchValue) ||
                inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<IncoTerm>.ToPagedList(incotermDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }


        public async Task<IncoTerm> GetIncoTermById(int id)
        {
            var IncoTermbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return IncoTermbyId;
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
