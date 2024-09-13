using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Misc;
using System.Linq.Expressions;
using System.Security.Claims;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Repository
{
    public class POBreakDownForAviRepository : RepositoryBase<POCollectionTrackerForAvi>, IPOBreakDownForAviRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;

        public POBreakDownForAviRepository(TipsPurchaseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsPurchaseDbContext = repositoryContext;
        }

        public Task<POBreakDownForAvi> Create(POBreakDownForAvi entity)
        {
            throw new NotImplementedException();
        }

        public async Task<int?> CreatePOBreakDownForAvi(POBreakDownForAvi pocollectionTrackerItem)
        {
            var result = await Create(pocollectionTrackerItem);
            return result.Id;
        }

        public void Delete(POBreakDownForAvi entity)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeletePOBreakDownForAvi(POBreakDownForAvi pocollectionTrackerItem)
        {
            throw new NotImplementedException();
        }

        public IQueryable<POBreakDownForAvi> FindByCondition(Expression<Func<POBreakDownForAvi, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<POBreakDownForAvi>> GetAllPOBreakDownForAvi()
        {
            throw new NotImplementedException();
        }

        public Task<POBreakDownForAvi> GetPOBreakDownForAviById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(POBreakDownForAvi entity)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UpdatePOBreakDownForAvi(POBreakDownForAvi pocollectionTrackerItem)
        {
            Update(pocollectionTrackerItem);
            string result = $"SOBreakDown details of {pocollectionTrackerItem.Id} is updated successfully!";
            return result;
        }

        IQueryable<POBreakDownForAvi> Contracts.IRepositoryBase<POBreakDownForAvi>.FindAll()
        {
            throw new NotImplementedException();
        }
    }
}
