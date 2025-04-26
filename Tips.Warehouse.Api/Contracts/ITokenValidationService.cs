namespace Tips.Warehouse.Api.Contracts
{
    public interface ITokenValidationService
    {
        Task<bool> IsTokenValid(string token);
    }
}
