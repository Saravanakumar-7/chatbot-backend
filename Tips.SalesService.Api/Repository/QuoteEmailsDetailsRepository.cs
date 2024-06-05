using System.Security.Claims;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Repository
{
    public class QuoteEmailsDetailsRepository : RepositoryBase<QuoteEmailsDetails>, IQuoteEmailsDetailsRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        public QuoteEmailsDetailsRepository(TipsSalesServiceDbContext repositoryContext, IHttpClientFactory clientFactory, HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
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
        public async Task<long> CreateQuoteEmailsDetails(QuoteEmailsDetails quoteEmailsDetails)
        {
            quoteEmailsDetails.SentBy = _createdBy;
            quoteEmailsDetails.SentOn = DateTime.Now;
            var result = await Create(quoteEmailsDetails);
            return result.Id;
        }
    }
}
