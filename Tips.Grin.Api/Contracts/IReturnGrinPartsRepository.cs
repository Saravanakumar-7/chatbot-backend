using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IReturnGrinPartsRepository
    {
        Task<int?> CreateReturnGrinParts(ReturnGrinParts returnGrinParts);
        Task<string> UpdateReturnGrinParts(ReturnGrinParts returnGrinParts);

        Task<ReturnGrinParts> GetReturnGrinPartsDetailsbyId(int id);

        Task<ReturnGrinParts> ReturnGrinPartsByPartNumber(string partNo);

        public void SaveAsync();
    }
}
