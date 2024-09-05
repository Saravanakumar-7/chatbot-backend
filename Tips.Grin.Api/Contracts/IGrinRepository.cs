    using Entities; 
using Tips.Grin.Api.Entities.DTOs;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tips.Grin.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Grin.Api.Contracts
{
    public interface IGrinRepository : IRepositoryBase<Grins>
    {
        Task<PagedList<Grins>> GetAllGrin(PagingParameter pagingParameter,SearchParams searchParams);
        Task<Grins> GetGrinById(int id);
        Task<IEnumerable<Grins>> GetAllActiveGrin();
        Task<int?> CreateGrin(Grins grins);
        Task<string> GenerateGrinNumber();
        Task<int?> GetGrinNumberAutoIncrementCount(DateTime date);
        Task<Grins> GetGrinByGrinNo(string grinNumber);
        Task<string> UpdateGrin(Grins grins);
        Task<string> UpdateGrin_ForTally(Grins grins);
        Task<string> DeleteGrin(Grins grins);
        Task<IEnumerable<Grins>> GetAllGrinsWithItems(GrinSearchDto grinSearchDto);
        Task<IEnumerable<Grins>> SearchGrins([FromQuery] SearchParames searchParames);
        Task<IEnumerable<Grins>> SearchGrinsDate([FromQuery] SearchDateParames searchParames);
        Task<IEnumerable<Grins>> GetGrinDetailsByGrinIds(List<int> grinIds);
        Task<IEnumerable<GrinNumberListDto>> GetAllActiveGrinNoList();
        Task<IEnumerable<GrinNoForIqcAndBinning>> GetAllGrinNumberWhereBinningComplete();
        Task<IEnumerable<GrinNoForIqcAndBinning>> GetAllGrinNumberForIqc();
        Task<IEnumerable<GrinNoForIqcAndBinning>> GetAllGrinNumberForBinning();
        Task<int?> GetGrinIqcStatusCount(string grinNo);
        Task<int?> GetGrinbinningStatusCount(string grinNo);
        Task<string> GenerateGrinNumberForAvision();
        Task<IEnumerable<Grin_ReportSP>> GetGrinSPReportWithParam(string? GrinNumber, string? VendorName, string? PONumber, string? KPN, string? MPN, 
                                                                                                                            string? Warehouse, string? Location);
        Task<IEnumerable<GrinSPReportForTrans>> GetGrinSPReportWithParamForTrans(string? GrinNumber, string? VendorName, string? PONumber,
                                                                                                    string? ItemNumber, string? MPN, string? Warehouse, string? Location,
                                                                                                    string? ProjectNumber);
        Task<PagedList<Grin_ReportSP>> GetGrinSPReport(PagingParameter pagingParameter);
        Task<PagedList<GrinSPReportForTrans>> GetGrinSPReportForTrans(PagingParameter pagingParameter);
        Task<IEnumerable<Grin_ReportSP>> GetGrinSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<GrinSPReportForTrans>> GetGrinSPReportWithDateForTrans(DateTime? FromDate, DateTime? ToDate);
    }
}
