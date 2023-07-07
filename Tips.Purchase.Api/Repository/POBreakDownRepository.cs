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
    public class POBreakDownRepository : RepositoryBase<POCollectionTracker>, IPOBreakDownRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;

        public POBreakDownRepository(TipsPurchaseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsPurchaseDbContext = repositoryContext;
        }

        public Task<POBreakDown> Create(POBreakDown entity)
        {
            throw new NotImplementedException();
        }

        public async Task<int?> CreatePOBreakDown(POBreakDown pocollectionTrackerItem)
        {
            var result = await Create(pocollectionTrackerItem);
            return result.Id;
        }

        public void Delete(POBreakDown entity)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeletePOBreakDown(POBreakDown pocollectionTrackerItem)
        {
            throw new NotImplementedException();
        }

        public IQueryable<POBreakDown> FindByCondition(Expression<Func<POBreakDown, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<POBreakDown>> GetAllPOBreakDown()
        {
            throw new NotImplementedException();
        }

        public Task<POBreakDown> GetPOBreakDownById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(POBreakDown entity)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UpdatePOBreakDown(POBreakDown pocollectionTrackerItem)
        {
            Update(pocollectionTrackerItem);
            string result = $"SOBreakDown details of {pocollectionTrackerItem.Id} is updated successfully!";
            return result;
        }

        IQueryable<POBreakDown> Contracts.IRepositoryBase<POBreakDown>.FindAll()
        {
            throw new NotImplementedException();
        }
    }
}
