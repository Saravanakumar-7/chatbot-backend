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
        IPurchaseGroupRepository PurchaseGroupRepository { get; }
        ICostCenterRepository CostCenterRepository { get; }
        ICostingMethodRepository CostingMethodRepository { get; }
        IAuditFrequencyRepository AuditFrequencyRepository { get; }
        INatureOfRelationshipRepository NatureOfRelationshipRepository { get; }
        ILanguageRepository LanguageRepository { get; }
        ISalutationsRepository SalutationsRepository { get; }
        IExportUnitTypeRepository ExportUnitTypeRepository { get; }
        ITypeOfCompanyRepository TypeOfCompanyRepository { get; }
        IPaymentTermRepository PaymentTermRepository { get; }
        IPackingInstructionRepository PackingInstructionRepository { get; }
        IMaterialTypeRepository MaterialTypeRepository { get; }
        IProcurementTypeRepository ProcurementTypeRepository { get; }

        IUOMRepository UOMRepository { get; }
        IUOCRepository UOCRepository { get; }
        ICommodityRepository CommodityRepository{ get; }
        ILocationsRepository LocationsRepository { get; }

        ICompanyMasterRepository CompanyMasterRepository { get; }
        ICompanyAddressesRepository CompanyAddressesRepository { get; }
        ICompanyContactsRepository CompanyContactsRepository { get; }
        ICompanyBankingRepository CompanyBankingRepository { get; }

        ICustomerMasterRepository CustomerMasterRepository { get; }
        ICustomerBankingsRepository CustomerBankingsRepository { get; }
        ICustomerAddressesRepository CustomerAddressesRepository { get; }
        ICustomerShippingAddressesRepository CustomerShippingAddressesRepository { get; }
        ICustomerContactsRepository CustomerContactsRepository { get; }

        IItemMasterRepository ItemMasterRepository { get; }
        IItemmasterAlternate ItemmasterAlternateRepository { get; }
        IItemMasterApprovedVendor ItemMasterApprovedVendorRepository { get; }
        IItemMasterFileUpload ItemMasterFileUploadRepository { get; }
        IItemMasterRouting ItemMasterRoutingRepository { get; }
        IItemMasterWarehouse ItemMasterWarehouseRepository { get; }
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

        IVendorContactRepository VendorContactRepository { get; }

        IVendorAddressRepository VendorAddressRepository { get; }   

        IVendorBankingRepository VendorBankingRepository { get; }
        void SaveAsync();
    }
}
