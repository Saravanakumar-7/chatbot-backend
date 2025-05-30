using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Contracts
{
    public interface IBinningRepository : IRepositoryBase<Binning>
    {
        //Task<PagedList<Binning>> GetAllBinningDetails(PagingParameter pagingParameter, SearchParams searchParams);
        Task<IEnumerable<Binning>> GetBinningDetailsByGrinNo(string grinNo);
        Task<string> UpdateBinning(Binning binning);
        Task<Binning> GetBinningDetailsbyId(int id);
        Task<Binning> CreateBinning(Binning binning);
        Task<string> DeleteBinning(Binning binning);
        Task<PagedList<GrinAndBinningDetailsDto>> GetAllBinningDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams);
        Task<IEnumerable<BinningIdNameListDto>> GetAllActiveBinningNameList();
        Task<IEnumerable<Binning>> GetAllBinningWithItems(BinningSearchDto binningSearchDto);
        Task<IEnumerable<Binning>> SearchBinning([FromQuery] SearchParames searchParames);
        Task<IEnumerable<Binning>> SearchBinningDate([FromQuery] SearchDateParames searchParames);
        Task<Binning> GetExistingBinningDetailsByGrinNo(string grinNo);
        Task<Binning> GetBinningDetailsByGrinNumber(string grinNumber);

        Task<IEnumerable<BinningSPReportAvi>> GetBinningSPReportWithParamForAvi(string? ponumber, string? grinnumber, string? itemnumber,
                                                                                                    string? projectnumber);
        Task<IEnumerable<BinningSPReportAvi>> GetBinningSPReportWithDateForAvi(DateTime? FromDate, DateTime? ToDate);
        public void SaveAsync();
    }
}
