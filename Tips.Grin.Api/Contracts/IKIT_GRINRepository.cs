using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Contracts
{
    public interface IKIT_GRINRepository : IRepositoryBase<KIT_GRIN>
    {
        Task<string> GenerateKIT_GrinNumberForAvision();
        Task<string> GenerateKIT_GrinNumber();
        Task<int?> CreateKIT_Grin(KIT_GRIN grins);
        Task<IEnumerable<KIT_GrinNoForKIT_IqcAndKIT_Binning>> GetAllKIT_GrinNumberForKIT_IQC();
        Task<KIT_GRIN> GetKIT_GrinById(int id);
        Task<KIT_GRIN> GetKIT_GrinByIdWithNoTracking(int id);
        Task<string> UpdateKIT_GRINDetails(KIT_GRIN kIT_GRIN);
        Task<string> UpdateKIT_GRIN(KIT_GRIN kIT_GRIN);
        Task<PagedList<KIT_GRIN>> GetAllKIT_GRIN([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams);
        Task<KIT_GRIN?> GetKIT_GrinByKIT_GrinNumber(string kIT_GrinNumber);
        Task<IEnumerable<KIT_GrinNoForKIT_IqcAndKIT_Binning>> GetAllKIT_GrinNumberForKIT_Binning();
        Task<IEnumerable<KITGrinSPReport>> KITGrinSPReportwithparameterForAvi(string? KIT_GrinNumber, string? VendorName, string? PONumber, string? ItemNumber, string? MPN, string? Warehouse, string? Location, string? ProjectNumber);
        Task<IEnumerable<KITGrinSPReport>> KITGrinSPReportwithDateForAvi(DateTime? FromDate, DateTime? ToDate);
    }
}
