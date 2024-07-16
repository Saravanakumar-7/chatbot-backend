using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Repository
{
    public class OpenGrinForBinningRepository : RepositoryBase<OpenGrinForBinning>, IOpenGrinForBinningRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public OpenGrinForBinningRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<PagedList<OpenGrinForBinning>> GetAllOpenGrinForBinningDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            var openGrinForBinningDetails = FindAll()
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || (inv.OpenGrinNumber != null && inv.OpenGrinNumber.Contains(searchParams.SearchValue)) ||
                   (inv.Id != null && inv.Id.ToString().Contains(searchParams.SearchValue))
                ))).OrderByDescending(x => x.Id);


            return PagedList<OpenGrinForBinning>.ToPagedList(openGrinForBinningDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

        }

        public async Task<OpenGrinForBinning> GetOpenGrinForBinningDetailsbyId(int id)
        {
            var openGrinForBinningDetails = await _tipsGrinDbContext.OpenGrinForBinnings.Where(x => x.Id == id)
                              .Include(t => t.OpenGrinForBinningItems)
                              .ThenInclude(x => x.OpenGrinForBinningLocations)
                           .FirstOrDefaultAsync();

            return openGrinForBinningDetails;
        }

        public async Task<OpenGrinForBinning> GetOpenGrinForBinningDetailsByOpenGrinNo(string openGrinNo)
        {
                var openGrinForBinningDetails = await _tipsGrinDbContext.OpenGrinForBinnings
                    .Where(x => x.OpenGrinNumber == openGrinNo )
                   .Include(t => t.OpenGrinForBinningItems)
                   .ThenInclude(l=>l.OpenGrinForBinningLocations)

                           .FirstOrDefaultAsync();

                return openGrinForBinningDetails;
            
        }
        public async Task<OpenGrinForBinning> GetExistingOpenGrinForBinningDetailsByOpenGrinNo(string openGrinNo)
        {
            var openGrinForbinningDetailsByOpenGrinNo = await _tipsGrinDbContext.OpenGrinForBinnings.Where(x => x.OpenGrinNumber == openGrinNo)
                                        .Include(x => x.OpenGrinForBinningItems)
                                         .ThenInclude(l => l.OpenGrinForBinningLocations)
                                         .FirstOrDefaultAsync();
            return openGrinForbinningDetailsByOpenGrinNo;
        }
        public async Task<string> UpdateOpenGrinForBinning(OpenGrinForBinning openGrinForBinning)
        {
            openGrinForBinning.LastModifiedBy = _createdBy;
            openGrinForBinning.LastModifiedOn = DateTime.Now;
            Update(openGrinForBinning);
            string result = $"OpenGrinForBinning details of {openGrinForBinning.Id} is updated successfully!";
            return result;
        }
        public async Task<OpenGrinForBinning> CreateOpenGrinForBinning(OpenGrinForBinning openGrinForBinning)
        {

            openGrinForBinning.CreatedBy = _createdBy;
            openGrinForBinning.CreatedOn = DateTime.Now;
            openGrinForBinning.Unit = _unitname;
            var result = await Create(openGrinForBinning);
            return result;
        }
    }
}
