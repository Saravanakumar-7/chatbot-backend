using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BatchRepository : RepositoryBase<Batch>, IBatchRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public BatchRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateBatch(Batch batch)
        {
            batch.CreatedBy = _createdBy;
            batch.CreatedOn = DateTime.Now;
            batch.Unit = _unitname;
            var result = await Create(batch);

            return result.Id;
        }

        public async Task<string> DeleteBatch(Batch batch)
        {
            Delete(batch);
            string result = $"Batch details of {batch.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Batch>> GetAllActiveBatches([FromQuery] SearchParames searchParams)
        {
            var BatchDetails = FindAll()
                             .Where(inv => inv.IsActive && ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BatchNumber.Contains(searchParams.SearchValue) ||
                                    inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return BatchDetails;
        }

        public async Task<IEnumerable<Batch>> GetAllBatches([FromQuery] SearchParames searchParams)
        {
            var BatchDetails = FindAll().OrderByDescending(x => x.Id)
                            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BatchNumber.Contains(searchParams.SearchValue) ||
                                   inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return BatchDetails;
        }

        public async Task<Batch> GetBatchById(int id)
        {
            var BatchById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return BatchById;
        }

        public async Task<string> UpdateBatch(Batch batch)
        {
            batch.LastModifiedBy = _createdBy;
            batch.LastModifiedOn = DateTime.Now;
            Update(batch);
            string result = $"Batch details of {batch.Id} is updated successfully!";
            return result;
        }
    }
}
