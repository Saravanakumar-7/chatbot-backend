using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Contracts
{
    public interface IBinningRepository : IRepositoryBase<Binning>
    {
        Task<PagedList<Binning>> GetAllBinningDetails(PagingParameter pagingParameter, SearchParams searchParams);
        Task<IEnumerable<Binning>> GetBinningDetailsByGrinNo(string grinNo);
        Task<string> UpdateBinning(Binning binning);
        Task<Binning> GetBinningDetailsbyId(int id);
        Task<Binning> CreateBinning(Binning binning);
        Task<string> DeleteBinning(Binning binning);
        Task<IEnumerable<BinningIdNameListDto>> GetAllActiveBinningNameList();

        public void SaveAsync();
    }
}
