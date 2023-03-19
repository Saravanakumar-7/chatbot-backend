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
    public class AuditFrequencyRepository : RepositoryBase<AuditFrequency>, IAuditFrequencyRepository
    {
        public AuditFrequencyRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateAuditFrequency(AuditFrequency auditFrequency)
        {
            auditFrequency.CreatedBy = "Admin";
            auditFrequency.CreatedOn = DateTime.Now;
            auditFrequency.Unit = "Bangalore";
            var result = await Create(auditFrequency);
            
            return result.Id;
            
        }

        public async Task<string> DeleteAuditFrequency(AuditFrequency auditFrequency)
        {
            Delete(auditFrequency);
            string result = $"AuditFrequency details of {auditFrequency.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<AuditFrequency>> GetAllActiveAuditFrequencies([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {

            var getAllActiveAuditFrequencies = FindAll()
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.AuditFrequencyName.Contains(searchParams.SearchValue) ||
          inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<AuditFrequency>.ToPagedList(getAllActiveAuditFrequencies, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<AuditFrequency>> GetAllAuditFrequencies([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {

            var getAllAuditFrequencies = FindAll().OrderByDescending(x => x.Id)
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.AuditFrequencyName.Contains(searchParams.SearchValue) ||
                    inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<AuditFrequency>.ToPagedList(getAllAuditFrequencies, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<AuditFrequency> GetAuditFrequenyById(int id)
        {
            var AuditFrequencyyid = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return AuditFrequencyyid;
        }       
        public async Task<string> UpdateAuditFrequency(AuditFrequency auditFrequency)
        {

            auditFrequency.LastModifiedBy = "Admin";
            auditFrequency.LastModifiedOn = DateTime.Now;
            Update(auditFrequency);
            string result = $"AuditFrequency details of {auditFrequency.Id} is updated successfully!";
            return result;
        }
    }
}
