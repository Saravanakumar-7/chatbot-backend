namespace Tips.Tally.Api.Contracts
{
    public interface ITokenValidationService
    {
        Task<bool> IsTokenValid(string token);
    }
}
