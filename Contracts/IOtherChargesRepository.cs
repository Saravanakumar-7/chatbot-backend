using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IOtherChargesRepository : IRepositoryBase<OtherCharges>
    {
        Task<IEnumerable<OtherCharges>> GetAllOtherCharges(SearchParames searchParams);
        Task<OtherCharges> GetOtherChargesById(int id);
        Task<IEnumerable<OtherCharges>> GetAllActiveOtherCharges(SearchParames searchParams);
        Task<int?> CreateOtherCharges(OtherCharges otherCharges);
        Task<string> UpdateOtherCharges(OtherCharges otherCharges);
        Task<string> DeleteOtherCharges(OtherCharges otherCharges);
    }
}
