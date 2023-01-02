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
        IBasisOfApprovalRepository BasisOfApprovalRepository { get; }
        IVendorCategoryRepository VendorCategoryRepository { get; }
        IVendorDepartmentRepository VendorDepartmentRepository { get; }
        IScopeOfSupplyRepository ScopeOfSupplyRepository { get; }
        IDeliveryTermRepository DeliveryTermRepo { get; }

        IVendorRepository VendorRepository { get; }

        IVendorContactRepository VendorContactRepository { get; }

        IVendorAddressRepository VendorAddressRepository { get; }   

        IVendorBankingRepository VendorBankingRepository { get; }

        IProcessRepository ProcessRepository { get; }

        IShipmentInstructionsRepository ShipmentInstructionsRepository { get; }
        ISegmentRepository SegmentRepository { get; }
        IQuoteTermsRepository QuoteTermsRepository { get; }
        IRiskCategoryRepository RiskCategoryRepository { get; }
        IWarehouseRepository WarehouseRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IPreferredFreightForwarderRepository PreferredFreightForwarderRepository { get; }
        IGST_PercentageRepository GST_PercentageRepository { get; }
        IShipmentModeRepository ShipmentModeRepository { get; }
        IPriceListRepository PriceListRepository { get; }

        IEnggBomAlternatesRepository EnggBomAlternatesRepository { get; }

        IEnggBomChildItemRepository EnggBomChildItemRepository { get; }

        IEnggBomNREConsumableRepository EnggBomNREConsumableRepository { get; } 

        IEnggBomRepository EnggBomRepository { get; } 
        IPartTypesRepository partTypesRepository { get; }
        ILeadRepository LeadRepository { get; }
        IDemoStatusRepository DemoStatusRepository { get; }

        ILeadStatusRepository LeadStatusRepository { get; }

        ILeadTypeRepository LeadTypeRepository { get; }
        ISecondarySourceRepository secondarySourceRepository { get; }

        ISourceRepository sourceRepository { get; }
        
        IReleaseEnggBomRepository ReleaseEnggBomRepository { get; }
        IReleaseCostBomRepository ReleaseCostBomRepository { get; }
        IReleaseProductBomRepository ReleaseProductBomRepository { get; }

        IEnggBomGroupRepository EnggBomGroupRepository { get; }
        IEnggCustomFieldRepository EnggCustomFieldRepository { get; }

        IEngineeringCustomFieldRepository EngineeringCustomFieldRepository { get; }

        void SaveAsync();
    }
}
