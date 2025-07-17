using Entities.Helper;
using Entities;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Grin.Api.Contracts
{
    public interface IOpenGrinRepository : IRepositoryBase<OpenGrin>
    {
        Task<PagedList<OpenGrin>> GetAllOpenGrinDetails(PagingParameter pagingParameter, SearchParams searchParams);
        Task<string> UpdateOpenGrin(OpenGrin openGrin);
        Task<OpenGrin> GetOpenGrinDetailsbyId(int id);
        Task<string> GenerateOpenGrinNumber();
        Task<OpenGrin> CreateOpenGrin(OpenGrin openGrin);
        Task<string> DeleteOpenGrin(OpenGrin openGrin);
        Task<OpenGrinDetails> GetOpenGrinPartDetailsbyId(int id);
        Task<OpenGrinParts> GetOpenGrinPartsbyId(int id);
        Task<IEnumerable<OpenGrin>> GetAllOpenGrinWithItems(OpenGrinSearchDto openGrinSearchDto);
        Task<IEnumerable<OpenGrin>> SearchOpenGrin(SearchParames searchParames);
        Task<IEnumerable<OpenGrin>> SearchOpenGrinDate(SearchDateParames searchParames);
        Task<IEnumerable<OpenGrinDataListDto>> GetAllOpenGrinDataList();
        Task<string> GenerateOpenGrinNumberForAvision();
        Task<PagedList<OpenGrin_SPReport>> GetOpenGrinSPReport(PagingParameter pagingParameter);
        Task<PagedList<OpenGrinSpReportForTrans>> GetOpenGrinSPReportForTrans(PagingParameter pagingParameter);
        Task<IEnumerable<OpenGrin_SPReport>> GetOpenGrinSPReportWithParam(string? openGrinNumber, string? senderName, string? receiptRefNo);
        Task<IEnumerable<OpenGrinSpReportForTrans>> GetOpenGrinSPReportWithParamForTrans(string? itemNumber, string? openGrinNumber, string? senderName, string? receiptRefNo
                                                                                                                                         , string? ProjectNumber);
        Task<IEnumerable<OpenGrinSpReportForTrans>> GetOpenGrinSPReportWithParamForAvi(string? openGrinNumber, string? senderName, string? receiptRefNo
                                                                                                                                         , string? ProjectNumber);
        Task<IEnumerable<OpenGrin_SPReport>> GetOpenGrinSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<OpenGrinSpReportForTrans>> GetOpenGrinSPReportWithDateForTrans(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<OpenGrinSpReportForTrans>> GetOpenGrinSPReportWithDateForAvi(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<OpenGrinPartSORefDto>> GetAllOpenGrinSORefDetails();
        Task<string> UpdateOpenGrinDetails(OpenGrin openGrin);
    }
}
