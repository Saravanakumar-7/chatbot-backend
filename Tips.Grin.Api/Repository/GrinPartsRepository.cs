using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Tips.Grin.Api.Repository
{
    public class GrinPartsRepository : RepositoryBase<GrinParts>, IGrinPartsRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public GrinPartsRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<PagedList<GrinParts>> GetAllGrinParts([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            var getAllGrinParts = FindAll()
                 .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
                 inv.ItemDescription.Contains(searchParams.SearchValue) || inv.PONumber.Contains(searchParams.SearchValue)
                 || inv.MftrItemNumber.Contains(searchParams.SearchValue) || inv.ManufactureBatchNumber.Contains(searchParams.SearchValue)
                 )))
                .Include(t => t.ProjectNumbers);

            return PagedList<GrinParts>.ToPagedList(getAllGrinParts, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<string> DeleteGrinParts(GrinParts grinParts)
        {
            //var grinPartDetails = await _tipsGrinDbContext.GrinParts
            //    .Include(t => t.ProjectNumbers)
            //    .Include(t => t.CoCUpload).FirstOrDefaultAsync();

            Delete(grinParts);
            string result = $"GrinParts details of {grinParts.Id} is deleted successfully!";
            return result;
        }
        public async Task<GrinParts> GetGrinPartsById(int id)
        {
            var grinPartsDetailsbyId = await _tipsGrinDbContext.GrinParts.Where(x => x.Id == id)

               .Include(d => d.ProjectNumbers)
                               .FirstOrDefaultAsync();

            return grinPartsDetailsbyId;
        }

        public async Task<GrinParts> DeleteGrinPartsById(int id)
        {
            var grinPartsDetailsbyId = await _tipsGrinDbContext.GrinParts.Where(x => x.Id == id)

               .Include(d => d.ProjectNumbers)
               .Include(d => d.CoCUpload)
                               .FirstOrDefaultAsync();

            return grinPartsDetailsbyId;
        }

        public async Task<GrinParts> GetGrinPartsDetailsbyGrinPartId(int GrinPartId)
        {
            var grinPartsDetails = await _tipsGrinDbContext.GrinParts.Where(x => x.Id == GrinPartId)
                .Include(x=>x.ProjectNumbers)
                .FirstOrDefaultAsync();
            return grinPartsDetails;
        }

        public async Task<IEnumerable<GrinParts>> GetGrinPartsDetailsByGrinPartIds(List<int> grinPartIds)
        {
            var grinPartsDetailsList = await FindByCondition(x => grinPartIds.Contains(x.Id)).Include(p => p.ProjectNumbers).ToListAsync();
            return grinPartsDetailsList;
        }


        public async Task<GrinParts> UpdateGrinPartsQty(int GrinPartId, string AcceptedQty, string RejectedQty)
        {
            var data = await _tipsGrinDbContext.GrinParts.Where(x => x.Id == GrinPartId).FirstOrDefaultAsync();
            data.AcceptedQty = Convert.ToDecimal(AcceptedQty);
            data.RejectedQty = Convert.ToDecimal(RejectedQty);
            return data;
        }

        public async Task<string> UpdateGrinQty(GrinParts grinParts)
        {
            grinParts.LastModifiedBy = _createdBy;
            grinParts.LastModifiedOn = DateTime.Now;
            Update(grinParts);
            string result = $"GrinParts Detail {grinParts.Id} is updated successfully!";
            return result;
        }


        //pass grinparts id and get the details

        //public async Task<string> GetGrinPartsDetailsByPartsId(GrinParts grinParts)
        //{
        //    grinParts.LastModifiedBy = "Admin";
        //    grinParts.LastModifiedOn = DateTime.Now;
        //    Update(grinParts);
        //    string result = $"Grin Detail {grinParts.Id} is updated successfully!";
        //    return result;
        //}

    }

}
