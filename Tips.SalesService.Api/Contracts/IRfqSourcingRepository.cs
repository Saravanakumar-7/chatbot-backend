using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface IRfqSourcingRepository 
    {
        Task<PagedList<RfqSourcing>> GetAllRfqSourcing(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<RfqSourcing> GetRfqSourcingById(int id);
        Task<RfqSourcing> GetRfqSourcingDetailsByRfqNo(string rfqNo);
        Task<RfqSourcingVendorDetailsDto> GetRfqSourcingVendorDetails(string ProjectNumber, string ItemNumber, string VendorId);
        Task<int?> CreateRfqSourcing(RfqSourcing rfqSourcing);
        Task<string> UpdateRfqSourcing(RfqSourcing rfqSourcing);
        Task<string> DeleteRfqSourcing(RfqSourcing rfqSourcing);
        Task<IEnumerable<SourcingSPReport>> GetSourcingSPReportWithParam(string Vendor);
        public void SaveAsync();
    }
}
