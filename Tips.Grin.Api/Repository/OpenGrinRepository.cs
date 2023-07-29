using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Repository
{
    public class OpenGrinRepository : RepositoryBase<OpenGrin>, IOpenGrinRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public OpenGrinRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<OpenGrin> CreateOpenGrin(OpenGrin openGrin)
        {
            openGrin.CreatedBy = _createdBy;
            openGrin.CreatedOn = DateTime.Now;
            openGrin.Unit = _unitname;
            var result = await Create(openGrin);
            return result;
        }

        public async Task<string> DeleteOpenGrin(OpenGrin openGrin)
        {
            Delete(openGrin);
            string result = $"OpenGrin details of {openGrin.Id} is deleted successfully!";
            return result;
        }
        public async Task<string> GenerateOpenGrinNumber()
        {
            using var transaction = await _tipsGrinDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var poNumberEntity = await _tipsGrinDbContext.OpenGrinNumbers.SingleAsync();
                poNumberEntity.CurrentValue += 1;
                _tipsGrinDbContext.Update(poNumberEntity);
                await _tipsGrinDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"OpenGrin-{poNumberEntity.CurrentValue:D5}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<PagedList<OpenGrin>> GetAllOpenGrinDetails(PagingParameter pagingParameter, SearchParams searchParams)
        {
            var openGrinDetails = FindAll()
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.SenderName.Contains(searchParams.SearchValue)
                || inv.SenderId.Contains(searchParams.SearchValue))));

            return PagedList<OpenGrin>.ToPagedList(openGrinDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<OpenGrin> GetOpenGrinDetailsbyId(int id)
        {
            var openGrinDetailsById = await _tipsGrinDbContext.OpenGrin.Where(x => x.Id == id)
                               .Include(t => t.OpenGrinParts)
                               .ThenInclude(o=>o.OpenGrinDetails)
                            .FirstOrDefaultAsync();

            return openGrinDetailsById;
        }
        public async Task<OpenGrinParts> GetOpenGrinPartsbyId(int id)
        {
            var openGrinPartDetailsById = await _tipsGrinDbContext.OpenGrinParts.Where(x => x.Id == id)
                            .FirstOrDefaultAsync();

            return openGrinPartDetailsById;
        }
        public async Task<OpenGrinDetails> GetOpenGrinPartDetailsbyId(int id)
        {
            var openGrinPartDetailsById = await _tipsGrinDbContext.OpenGrinDetails.Where(x => x.Id == id)
                            .FirstOrDefaultAsync();

            return openGrinPartDetailsById;
        }

        public async Task<string> UpdateOpenGrin(OpenGrin openGrin)
        {
            openGrin.LastModifiedBy = _createdBy;
            openGrin.LastModifiedOn = DateTime.Now;
            Update(openGrin);
            string result = $"OpenGrin details of {openGrin.Id} is updated successfully!";
            return result;
        }
    }
}
