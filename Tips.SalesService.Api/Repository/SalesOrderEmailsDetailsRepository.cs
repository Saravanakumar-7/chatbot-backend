using System.Security.Claims;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Repository
{
    public class SalesOrderEmailsDetailsRepository : RepositoryBase<SalesOrderEmailsDetails>, ISalesOrderEmailsDetailsRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        public SalesOrderEmailsDetailsRepository(TipsSalesServiceDbContext repositoryContext, IHttpClientFactory clientFactory, HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsSalesServiceDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _config = config;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
            _clientFactory = clientFactory;
        }
        public async Task<int> CreateSalesOrderEmailsDetails(SalesOrderEmailsDetails salesOrderEmailsDetails)
        {
            //salesOrderEmailsDetails.SentBy = _createdBy;
            //salesOrderEmailsDetails.SentOn = DateTime.Now;
            var result = await Create(salesOrderEmailsDetails);
            return result.Id;
        }
    }
}
