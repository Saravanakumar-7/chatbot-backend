using Entities;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Contracts
{
    public interface IBinningRepository : IRepositoryBase<Binning>
    {
        Task<IEnumerable<Binning>> GetAllBinningDetails();
        Task<IEnumerable<Binning>> GetBinningDetailsByGrinNo(string grinNo);
        Task<string> UpdateBinning(Binning binning);
        Task<Binning> GetBinningDetailsbyId(int id);
        Task<Binning> CreateBinning(Binning binning);
        Task<string> DeleteBinning(Binning binning);


        public void SaveAsync();
    }
}
