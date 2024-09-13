using System.Security.Claims;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Repository
{
    public class AdvitaShopOrderDetailsRepository : RepositoryBase<AdvitaShopOrderDetails>, IAdvitaShopOrderDetailsRepository
    {
        private AdvitaTipsProductionDbContext _advitaTipsProductionDbContext;
        private TipsProductionDbContext _tipsProductionDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;

        public AdvitaShopOrderDetailsRepository(AdvitaTipsProductionDbContext repositoryContext_1, TipsProductionDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext, repositoryContext_1)
        {
            _advitaTipsProductionDbContext = repositoryContext_1;
            _tipsProductionDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int?> CreateAdvitaShopOrderDetails(AdvitaShopOrderDetails advitaShopOrderDetails)
        {
            DateTime dateCreated = DateTime.Now;
            advitaShopOrderDetails.Shop_Order_Date = dateCreated.ToString();
            advitaShopOrderDetails.Created_On = dateCreated.ToString();
            advitaShopOrderDetails.Trans_Uploaded_On=dateCreated;
            advitaShopOrderDetails.Created_By = _createdBy;
            var result = await CreateAdvita(advitaShopOrderDetails);
            return result.Trans_Unique_Id;
        }
    }
}
