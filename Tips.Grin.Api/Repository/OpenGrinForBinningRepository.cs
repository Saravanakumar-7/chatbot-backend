using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

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

        public async Task<PagedList<OpenGrinForBinningDto>> GetAllOpenGrinForBinningDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            List<string> openGrinNumberList = _tipsGrinDbContext.OpenGrinForGrins.Select(x => x.OpenGrinNumber).ToList();

            var openGrinForBinningOpenGrinNoList = await _tipsGrinDbContext.OpenGrinForBinnings
                                    .Where(x => openGrinNumberList.Contains(x.OpenGrinNumber))
                                    .Select(x => new { x.OpenGrinNumber, x.Id })
                                    .Distinct().OrderByDescending(x => x.Id) // Ensure unique pairs of GrinNumber-Id
                                    .ToListAsync();

            var openGrinNumbers = openGrinForBinningOpenGrinNoList.Select(b => b.OpenGrinNumber).ToList();


            var openGrinForGrinDetails_1 = await _tipsGrinDbContext.OpenGrinForGrins.Where(x => openGrinNumbers.Contains(x.OpenGrinNumber)).ToListAsync();
            var openGrinForGrinDetails = openGrinForGrinDetails_1
                .Select(openGrinNumber => new OpenGrinForBinningDto
                {
                Id = openGrinForBinningOpenGrinNoList.Where(b => b.OpenGrinNumber == openGrinNumber.OpenGrinNumber).Select(x => x.Id).FirstOrDefault(),
                OpenGrinNumber = openGrinNumber.OpenGrinNumber,
                SenderId = openGrinNumber.SenderId,
                SenderName = openGrinNumber.SenderName,
                ReceiptRefNo = openGrinNumber.ReceiptRefNo,
                CreatedBy = openGrinNumber.CreatedBy,
                CreatedOn = openGrinNumber.CreatedOn,
                LastModifiedBy = openGrinNumber.LastModifiedBy,
                LastModifiedOn = openGrinNumber.LastModifiedOn
                }).OrderByDescending(x => x.Id).ToList();

            if (!string.IsNullOrWhiteSpace(searchParams.SearchValue))
            {
                string searchTerm = searchParams.SearchValue.Trim();

                openGrinForGrinDetails = openGrinForGrinDetails
                              .Where(dto =>
                                  dto.OpenGrinNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                  dto.SenderName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                  dto.ReceiptRefNo.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).OrderByDescending(x => x.Id).ToList();
            }


            return PagedList<OpenGrinForBinningDto>.ToPagedList(openGrinForGrinDetails.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize);

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
