using Org.BouncyCastle.Crypto.Macs;
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
 
        IFileUploadRepository FileUploadRepository { get; } 

 IUnitRepository unitRepository { get; }
    IImageUploadRepository ImageUploadRepository { get; } 

        ILeadWebsiteRepository leadWebsiteRepository { get; }
        IStageOfConstructionRepository StageOfConstructionRepository { get; }
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
        IItemMasterRoutingRepository ItemMasterRoutingRepository { get; }
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

         IProductTypeRepository ProductTypeRepository { get; }
        ITypeSolutionRepository TypeSolutionRepository { get; }

        IRoomNameRepository RoomNameRepository { get; }

        ICityRepository CityRepository { get; }
        IBHKRepository BHKRepository { get; } 
        ILightningDesignerRepository LightningDesignerRepository { get; }
        IPmcContractorRepository PmcContractorRepository { get; }
        IStateRepository StateRepository { get; }
        IArchitectureRepository ArchitectureRepository { get; }
        IProjectNameRepository ProjectNameRepository { get; }
        ISourceDetailsRepository SourceDetailsRepository { get; }
        ISFTRepository SFTRepository { get; }
         ITypeOfHomeRepository TypeOfHomeRepository { get; }
        IRoleRepository RoleRepository { get; }
        IRoleAccessRepository RoleAccessRepository { get; }
        IRegistrationFormRepository RegistrationFormRepository { get; }
        IUserAccessRepository UserAccessRepository { get; }
        IFormsAccessRepository FormsAccessRepository { get; }
        IOrderTypeRepository OrderTypeRepository { get; }
        IIssuingStockRepository IssuingStockRepository { get; }
        IAdditionalChargesRepository AdditionalChargesRepository { get; }
        ICompanyCategoryRepository CompanyCategoryRepository { get; }
        ICustomerCategoryRepository CustomerCategoryRepository { get; }
        IVendorRelatedVendorRepository VendorRelatedVendorRepository { get; }
        ICustomerRelatedCustomerRepository CustomerRelatedCustomerRepository { get; }
        IOtherChargesRepository OtherChargesRepository { get; }
        ICompanyApprovalRepository CompanyApprovalRepository { get; }
        ICompanyFileUploadRepository CompanyFileUploadRepository { get; }
        INoOfRoomRepository NoOfRoomRepository { get; }
        ITypeOfRoomRepository TypeOfRoomRepository { get; }
        IConvertionrateRepository ConvertionrateRepository { get; }
        void SaveAsync();
    }
}
