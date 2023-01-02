using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEngineeringCustomFieldRepository : IRepositoryBase<EngineeringCustomField>
    {
        Task<int?> CreateEnggCustField(EngineeringCustomField engineeringCustomField);

    }
}
