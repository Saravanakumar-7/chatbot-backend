using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository
{
    public class BHKRepository : RepositoryBase<BHK>, IBHKRepository
    {
        public BHKRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        { }
        public async Task<int?> CreateBHK(BHK bHK)
        {
            bHK.CreatedBy = "Admin";
            bHK.CreatedOn = DateTime.Now;
            bHK.Unit = "Bangalore";
            var result = await Create(bHK); return result.Id;
        }
        public async Task<string> DeleteBHK(BHK bHK)
        {
            Delete(bHK);
            string result = $"bHK details of {bHK.Id} is deleted successfully!";
            return result;
        }
        public async Task<PagedList<BHK>> GetAllActiveBHK([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var bhkDetails = FindAll()
                        .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BHKType.Contains(searchParams.SearchValue) ||
                        inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<BHK>.ToPagedList(bhkDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<PagedList<BHK>> GetAllBHK([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var bhkDetails = FindAll().OrderByDescending(x => x.Id)
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.BHKType.Contains(searchParams.SearchValue) ||
              inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<BHK>.ToPagedList(bhkDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<BHK> GetBHKById(int id)
        {
            var BHKyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync(); return BHKyId;
        }
        public async Task<string> UpdateBHK(BHK bHK)
        {
            bHK.LastModifiedBy = "Admin";
            bHK.LastModifiedOn = DateTime.Now;
            Update(bHK);
            string result = $"bHK details of {bHK.Id} is updated successfully!";
            return result;
        }
    }
}