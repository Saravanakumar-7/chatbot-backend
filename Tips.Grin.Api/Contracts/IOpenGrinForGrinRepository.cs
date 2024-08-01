using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Contracts
{
    public interface IOpenGrinForGrinRepository : IRepositoryBase<OpenGrinForGrin>
    {
        Task<PagedList<OpenGrinForGrin>> GetAllOpenGrinForGrin(PagingParameter pagingParameter,SearchParams searchParams);
        Task<int?> CreateOpenGrinForGrin(OpenGrinForGrin openGrinForGrins);
        Task<IEnumerable<OpenGrinForGrinSPReport>> GetOpenGrinForGrinSPReportWithParam(string? openGrinNumber, string? senderName, string? receiptRefNo);
        Task<IEnumerable<OpenGrinForGrinSPReport>> GetOpenGrinForGrinSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<OpenGrinForGrin> GetOpenGrinForGrinDetailsbyId(int id);
        Task<string> GenerateOpenGrinForGrinNumber();
        Task<string> GenerateOpenGrinForGrinNumberForAvision();
        Task<OpenGrinForGrin> GetOpenGrinForGrinDetailsByOpenGrinNo(string openGrinNumber);
        Task<string> UpdateOpenGrinForGrin(OpenGrinForGrin openGrinForGrin);
        Task<int?> GetOpenGrinForGrinIqcStatusCount(string openGrinNumber);
        Task<int?> GetOpenGrinForGrinbinningStatusCount(string openGrinNo);
        Task<IEnumerable<OpenGrinNoForOpenGrinIqcAndBinning>> GetAllOpenGrinNumberForOpenGrinIqc();
        Task<IEnumerable<OpenGrinNoForOpenGrinIqcAndBinning>> GetAllOpenGrinNumberForOpenGrinBinning();
    }
}
