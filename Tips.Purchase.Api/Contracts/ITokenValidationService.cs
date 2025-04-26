namespace Tips.Purchase.Api.Contracts
{
    public interface ITokenValidationService
    {
        Task<bool> IsTokenValid(string token);
    }
}
