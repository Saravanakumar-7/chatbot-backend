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

namespace Tips.Grin.Api.Contracts
{
    public interface IIQCConfirmationRepository
    {
        Task<IEnumerable<IQCConfirmation>> GetAllIqcDetails();
        Task<IQCConfirmation> GetIqcDetailByGrinNo(int id);
        Task<int?> CreateIqc(IQCConfirmation iQCConfirmation);
        Task<int?> CreateIqcById(IQCConfirmation iQCConfirmation);
        Task<string> UpdateIqc(IQCConfirmation iQCConfirmation);
        Task<string> DeleteIqc(IQCConfirmation iQCConfirmation); 

    }
}
