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
        IVolumeUomRepository VolumeUomRepo { get; }
        IWeightUomRepository WeightUomRepository { get; }
        IBankRepository BankRepository { get; }
        IDepartmentRepository DepartmentRepository { get; }
        ICurrencyRepository CurrencyRepository { get; }
        IIncoTermRepository IncoTermRepository { get; }
        IBasicOfApprovalRepository BasicOfApprovalRepository { get; }
        IVendorCategoryRepository VendorCategoryRepository { get; }
        IVendorDepartmentRepository VendorDepartmentRepository { get; }
        IScopeOfSupplyRepository ScopeOfSupplyRepository { get; }
        IDeliveryTermRepository DeliveryTermRepo { get; }
        IVendorRepository VendorRepository { get; }
        void SaveAsync();
    }
}
