using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IAdditionalChargesRepository : IRepositoryBase<AdditionalCharges>
    {
        Task<IEnumerable<AdditionalCharges>> GetAllAdditionalCharges();
        Task<AdditionalCharges> GetAdditionalChargesById(int id);
        Task<IEnumerable<AdditionalCharges>> GetAllActiveAdditionalCharges();
        Task<int?> CreateAdditionalCharges(AdditionalCharges additionalCharges);
        Task<string> UpdateAdditionalCharges(AdditionalCharges additionalCharges);
        Task<string> DeleteAdditionalCharges(AdditionalCharges additionalCharges);
    }
}

