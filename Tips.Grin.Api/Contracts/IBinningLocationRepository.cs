using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Contracts
{
    public interface IBinningLocationRepository 
    {
        Task<string> UpdateBinning(BinningLocation binningLocation);
        Task<IEnumerable<BinningLocation>> GetBinningLocationDetailsbyGrinPartId(int id);
        Task<List<BinningQuantityDto>> GetListOfBinningQtyByItemNoListByProjectNo(string projectNo, string itemNumber);
        Task<List<BinningQuantityDto>> GetListOfBinningQtyByItemNoListByMultipleProjectNo(string itemNumber, List<string> projectNo);
    }
}
