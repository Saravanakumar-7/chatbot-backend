using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Repository
{
    public class ReturnGrinRepository : RepositoryBase<ReturnGrin>, IReturnGrinRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ReturnGrinRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext; 
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<ReturnGrin> CreateReturnGrin(ReturnGrin returnGrin)
        {
            returnGrin.CreatedBy = _createdBy;
            returnGrin.CreatedOn = DateTime.Now;
            returnGrin.Unit = _unitname;
            var result = await Create(returnGrin);
            return result;
        }

        public async Task<string> DeleteReturnGrin(ReturnGrin returnGrin)
        {
            Delete(returnGrin);
            string result = $"binning details of {returnGrin.Id} is deleted successfully!";
            return result;
        }

        public async Task<ReturnGrin> GetReturnGrinDetailsbyId(int id)
        {
            var returnGrinDetailsbyId = await _tipsGrinDbContext.ReturnGrins.Where(x => x.Id == id)
                               .Include(x => x.ReturnGrinParts)
                               .FirstOrDefaultAsync();

            return returnGrinDetailsbyId;
        }

        public Task<string> UpdateReturnGrin(ReturnGrin returnGrin)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ReturnGrinPartsListDto>> ReturnGrinPartsByPartNumber(string partNo)
        {
            IEnumerable<ReturnGrinPartsListDto> getReturnGrinPartsByPartNo = await _tipsGrinDbContext.ReturnGrinParts
                                .Select(x => new ReturnGrinPartsListDto()
                                {
                                    PartNumber = x.PartNumber,
                                    MftrNumber = x.MftrNumber,
                                    Description = x.Description,
                                })
                                .Where(x => x.PartNumber == partNo)
                              .ToListAsync();

            return getReturnGrinPartsByPartNo;
        }

        public async Task<PagedList<ReturnGrin>> GetAllReturnGrin([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            /* var getAllReturnGrinDetails = PagedList<ReturnGrin>.ToPagedList(FindAll()
                                 .Include(t => t.ReturnGrinParts)
                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

             return getAllReturnGrinDetails;*/

            var getAllReturnGrins = FindAll()
             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.Customername.Contains(searchParams.SearchValue)
             || inv.CustomerId.Equals(int.Parse(searchParams.SearchValue)))))
             .Include(t => t.ReturnGrinParts);


            return PagedList<ReturnGrin>.ToPagedList(getAllReturnGrins, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
    }
}
          
       