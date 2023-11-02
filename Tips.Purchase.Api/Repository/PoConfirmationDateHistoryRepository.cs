using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Repository
{
    public class PoConfirmationDateHistoryRepository : RepositoryBase<PoConfirmationDateHistory>, IPoConfirmationDateHistoryRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public PoConfirmationDateHistoryRepository(TipsPurchaseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsPurchaseDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<long> CreatePoConfirmationDateHistory(PoConfirmationDateHistory poConfirmationDateHistory)
        {
            poConfirmationDateHistory.CreatedBy = _createdBy;
            poConfirmationDateHistory.CreatedOn = DateTime.Now;
           // poConfirmationDateHistory.LastModifiedBy = "Admin";
           // poConfirmationDateHistory.LastModifiedOn = DateTime.Now;
            var result = await Create(poConfirmationDateHistory);
            return result.Id;
        }
    }
}
