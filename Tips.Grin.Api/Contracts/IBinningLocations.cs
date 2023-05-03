using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IBinningLocations 
    {
        Task<string> UpdateBinning(BinningLocation binningLocation);
    }
}
