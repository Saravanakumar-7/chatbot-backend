using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryWrapperForMaster
    {
        ILeadTimeRepository leadTimeRepository { get; }
        ICustomerTypeRepository CustomerTypeRepository { get; }
        IMaterialTypeRepository MaterialTypeRepository { get; }
        IProcurementTypeRepository ProcurementTypeRepository { get; }
        void SaveAsync();
    }
}
