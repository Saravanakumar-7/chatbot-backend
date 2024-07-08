using Entities;
using Tips.Grin.Api.Entities.DTOs;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tips.Grin.Api.Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Controllers;

namespace Tips.Grin.Api.Contracts
{
    public interface IIQCConfirmationRepository : IRepositoryBase<IQCConfirmation>
    {
        Task<PagedList<IQCConfirmation>> GetAllIqcDetails( PagingParameter pagingParameter, SearchParams searchParams);
        Task<IQCConfirmation> GetIqcDetailsbyGrinNo(string grinNumber);
        Task<string> UpdateIqc(IQCConfirmation iQCConfirmation);
        Task<IEnumerable<IQCConfirmationIdNameListDto>> GetAllActiveIQCConfirmationNameList();
        Task<IQCConfirmation> GetIqcDetailsbyId(int id);              
        Task<int?> CreateIqc(IQCConfirmation iQCConfirmation);
        Task<IEnumerable<IQCConfirmation>> GetAllIQCConfirmationWithItems(IQCConfirmationSearchDto iQCConfirmationSearch);
        Task<IEnumerable<IQCConfirmation>> SearchIQCConfirmation([FromQuery] SearchParames searchParames);
        Task<IEnumerable<IQCConfirmation>> SearchIQCConfirmationDate([FromQuery] SearchDateParames searchParames);
        Task<PagedList<IQCConfirmation_SPReport>> GetIQCConfirmationSPReport(PagingParameter pagingParameter);
        Task<IEnumerable<IQCConfirmation_SPReport>> GetIQCConfirmationSPReportWithParam(string? GrinNumber, string? itemNo);
        Task<IEnumerable<IQCConfirmationSPReportForTrans>> GetIQCConfirmationSPReportWithParamForTrans(string? GrinNumber, string? itemNo, string? ProjectNumber);
        Task<IEnumerable<IQCConfirmation_SPReport>> GetIQCConfirmationSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<IQCConfirmationSPReportForTrans>> GetIQCConfirmationSPReportWithDateForTrans(DateTime? FromDate, DateTime? ToDate);
        Task<PagedList<IQCConfirmationSPReportForTrans>> GetIQCConfirmationSPReportForTrans(PagingParameter pagingParameter);

    }
}
