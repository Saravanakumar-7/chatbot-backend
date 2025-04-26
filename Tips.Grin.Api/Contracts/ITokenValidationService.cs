namespace Tips.Grin.Api.Contracts
{
    public interface ITokenValidationService
    {
        Task<bool> IsTokenValid(string token);
    }
}
