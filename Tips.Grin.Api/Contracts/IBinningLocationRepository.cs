using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IBinningLocationRepository 
    {
        Task<string> UpdateBinning(BinningLocation binningLocation);
        Task<IEnumerable<BinningLocation>> GetBinningLocationDetailsbyGrinPartId(int id);
    }
}
