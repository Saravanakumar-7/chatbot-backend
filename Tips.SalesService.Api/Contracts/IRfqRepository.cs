using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface IRfqRepository : IRepositoryBase<Rfq>
    {
        Task<PagedList<Rfq>> GetAllRfq(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<PagedList<Rfq>> GetAllRfqs(PagingParameter pagingParameter,SearchParammes searchParammes);
        Task<Rfq> GetRfqById(int id);
        Task<IEnumerable<LatestRfqNumberListDto>> GetAllActiveLatestRfqNumbers();
        Task<int?> CreateRfq(Rfq rfq);
        Task<string> UpdateRfq(Rfq rfq);
        Task<string> DeleteRfq(Rfq rfq);
        Task<int?> GetRfqNumberAutoIncrementCount(DateTime date);
        Task<string> GetRfqNumberAutoIncrementNumber();
        Task<string> GenerateRFQNumber();
        Task<string> GenerateRFQNumberAvision();
        
        //Task<string> GenerateRFQTrascconNumber();
        Task<IEnumerable<RfqNumberListDto>> GetAllActiveRfqNumberList();
        Task<IEnumerable<RfqNumberListDto>> GetAllRfqNumberList();
        Task<IEnumerable<RfqNumberListDto>> GetAllActiveRfqNumberListByCustomerId(string CustomerId);
        Task<Rfq> RfqSourcingByRfqNumbers(string id);
        Task<Rfq> RfqCsByRfqNumbers(string id);
        Task<Rfq> RfqEnggByRfqNumbers(string id);
        Task<Rfq> RfqDetailsByRfqNumbers(string rfqNumber);
        Task<int> GetLastestRfqRevNoByRfqNumber(string rfqNumber);
        Task<Rfq> GetCustomerIdByRfqNumber(string rfqnumber);
        Task<IEnumerable<Rfq>> GetRevNumberByRfqNumberList(string rfqnumber);
        Task<Rfq> RfqLpcostingByRfqNumbers(string RfqNumber, decimal? RevisionNumber);
        Task<Rfq> RfqLpCostingReleaseByRfqNumbers(string id);
        Task<Rfq> GetRfqDeatailsByRfqNoAndRevNo(string rfqNumber, int revisionNumber);
        Task<Rfq> UpdateRfqRevNo(Rfq rfq, string serverKey);
        Task<Rfq> RfqDetailsById(int rfqId);
        Task<string> GenerateRFQNumberForTransccon();
   
        Task<IEnumerable<RfqSPReportForKeus>> GetRfqSPReportForKeus(string CustomerName, string CustomerId, string RfqNumber);
        Task<IEnumerable<RfqSPReportForKeus>> GetRfqSPReportWithDateForKeus(DateTime? FromDate, DateTime? ToDate);

        Task<IEnumerable<RfqSPReport>> GetRfqSPReport(string CustomerName, string CustomerId, string RfqNumber);
        Task<IEnumerable<RfqSPReport>> GetRfqSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<RFQSalesorderConfirmationSPReport>> GetRFQSalesorderConfirmationSPReportWithParamForTrans(string CustomerName, string SalesOrderNumber, string KPN
                                                                                                                    , string SOStatus, string ProjectNumber);
        Task<IEnumerable<RFQSalesorderConfirmationSPReport>> GetRFQSalesorderConfirmationSPReportWithDateForTrans(DateTime? FromDate, DateTime? ToDate);
    }
}
