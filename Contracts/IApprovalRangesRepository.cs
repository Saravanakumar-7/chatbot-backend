using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IApprovalRangesRepository
    {
        Task<int?> CreateApprovalRanges(ApprovalRanges ApprovalRanges);
        Task<ApprovalRanges> GetApprovalRangesById(int id);
        Task<ApprovalRanges> GetApprovalRangesByProcurementType(string ProcurementType);
    }
}
