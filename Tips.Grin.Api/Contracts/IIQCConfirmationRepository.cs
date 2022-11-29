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
    public interface IIQCConfirmationRepository : IRepositoryBase<IQCConfirmation>
    {
        Task<IEnumerable<IQCConfirmation>> GetAllIqcDetails();
        Task<IEnumerable<IQCConfirmation>> GetIqcDetailsbyGrinNo(string grinNumber);
        Task<string> UpdateIqc(IQCConfirmation iQCConfirmation);
        
        Task<IQCConfirmation> GetIqcDetailsbyId(int id);              
        Task<int?> CreateIqc(IQCConfirmation iQCConfirmation);         

    }
}
