using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IFormsAccessRepository : IRepositoryBase<FormsAccess>
    {
        Task<IEnumerable<FormsAccess>> GetAllFormsAccess();
    }
}
