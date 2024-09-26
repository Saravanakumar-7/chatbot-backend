using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEmailIDsRepository : IRepositoryBase<EmailIDs>
    {
        Task<List<EmailIDs>> GetEmailIdDetailsbyOperation(string Operations);
    }
}
