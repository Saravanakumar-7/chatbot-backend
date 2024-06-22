using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IQuoteEmailsDetailsRepository : IRepositoryBase<QuoteEmailsDetails>
    {
        Task<long> CreateQuoteEmailsDetails(QuoteEmailsDetails quoteEmailsDetails);
    }
}
