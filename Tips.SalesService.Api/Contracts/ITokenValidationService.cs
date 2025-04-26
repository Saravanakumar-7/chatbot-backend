namespace Tips.SalesService.Api.Contracts
{
    public interface ITokenValidationService
    {
        Task<bool> IsTokenValid(string token);
    }
}
