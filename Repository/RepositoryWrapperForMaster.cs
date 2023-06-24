using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.ReleaseCostBomRepository;

namespace Repository
{
    public class RepositoryWrapperForMaster : IRepositoryWrapperForMaster
    {
        private TipsMasterDbContext _tipsMasterDbContext;
        private ILeadTimeRepository _leadTimeRepo;
        private IImageUploadRepository _imageUploadRepository;
        private ICustomerTypeRepository _customerTypeRepo;
        private IUnitRepository _unitRepository;
        private IHttpContextAccessor _httpContextAccessor;


        private IMaterialTypeRepository _materialTypeRepo;
        private IProcurementTypeRepository _procurementTypeRepo;
        private IItemMasterRepository _itemMasterRepo;
        private IFileUploadRepository _fileUploadRepo;
        private IEnggBomRepository? _enggBomRepository;
        private ITypeSolutionRepository _typeSolutionRepository;
        private IProductTypeRepository? _productTypeRepository;
        private ILeadWebsiteRepository _leadWebsiteRepository;
        private IRoomNameRepository? _roomNameRepository;
        private IEnggBomNREConsumableRepository? _enggBomNREConsumableRepository;

        private IStageOfConstructionRepository? _stageOfConstructionRepository;
        private ITypeOfHomeRepository? _typeOfHome;
        private IBHKRepository? _bHKRepository;
        private ISourceDetailsRepository? _sourceDetailsRepository;
        private IProjectNameRepository? _projectNameRepository;
        private ISFTRepository? _sFTRepository;
        private IStateRepository? _stateRepository;
        private ICityRepository? _cityRepository;
        private IPmcContractorRepository? _pmcContractorRepository;
        private IArchitectureRepository? _architectureRepository;
        private ILightningDesignerRepository? _lightningDesignerRepository;
 
        private IItemMasterRoutingRepository? _itemmasterRoutingRepository;

        private IDeliveryTermRepository? _deliveryTermRepo;
        private IVolumeUomRepository? _volumeUomRepo;
        private IWeightUomRepository? _weightUomRepo;

        private IIncoTermRepository? _incoTermRepo;
        private IDepartmentRepository? _departmentRepo;
        private IBankRepository? _bankRepo;
        private ICurrencyRepository? _currency;

        private IBasisOfApprovalRepository? _basisOfApproval;
        private IScopeOfSupplyRepository? _scopeOfSupply;
        private IVendorCategoryRepository? _vendorCategory;
        private IVendorDepartmentRepository? _vendorDepartment;
        private IVendorRepository? _vendorRepository;



        private IPurchaseGroupRepository? _purchaseGroupRepo;   
        private ICostCenterRepository? _costCenterRepo; 
        private ICostingMethodRepository? _costingMethodRepo;   
        private IAuditFrequencyRepository? _auditFrequencyRepo; 
        private INatureOfRelationshipRepository? _abilityOfRelationshipRepo;    
        private ILanguageRepository? _languageRepo; 
        private ISalutationsRepository? _salutationsRepo;   
        private IExportUnitTypeRepository? _exportUnitTypeRepo; 
        private ITypeOfCompanyRepository? _typeOfCompanyRepo;   
        private IPaymentTermRepository? _paymentTermRepo;   
        private IPackingInstructionRepository? _packingInstructionRepo;
        private IPreferredFreightForwarderRepository? _preferredFreightForwarderRepo;
        private IGST_PercentageRepository? _gstpercentageRepo;
        private IPriceListRepository? _pricelistRepo;
        private IShipmentModeRepository? _shipmentmodeRepo;

        private IShipmentInstructionsRepository? _shipmentInstructionsRepo;
        private ICategoryRepository? _categoryRepo;
        private IRiskCategoryRepository? _riskCategoryRepo;
        private IQuoteTermsRepository? _quoteTermsRepo;
        private IWarehouseRepository? _warehouseRepository;
        private ISegmentRepository _segmentRepository;

        private ICustomerMasterRepository? _customerMasterRepo;
         

        private ICompanyMasterRepository? _companyMasterRepo;

        private ILeadRepository? _leadRepo;

        private IUOCRepository? _uOCRepo;
        private IUOMRepository? _uOMRepo;
        private ICommodityRepository? _commodityRepo;
        private ILocationsRepository? _locationsRepo;
        private IProcessRepository? _processRepo;
        private IPartTypesRepository? _parttypesRepo;
        private IDemoStatusRepository? _demoStatusRepo;
        private ILeadStatusRepository? _leadStatusRepo;
        private ILeadTypeRepository? _leadTypeRepo;
        private ISecondarySourceRepository? _secondarySourceRepo;
        private ISourceRepository? _sourceRepo;
        private IReleaseEnggBomRepository? _releaseEnggBomRepo;
        private IReleaseCostBomRepository? _releaseCostBomRepo;
        private IReleaseProductBomRepository? _releaseProductBomRepo;
        private IEnggBomGroupRepository? _enggbomGroupRepo;
        private IEnggCustomFieldRepository? _enggcustomFieldRepo;
        private IRoleRepository? _roleRepository;
        private IRoleAccessRepository? _roleAccessRepository; 
        private IRegistrationFormRepository? _registrationFormRepo;
        private IUserAccessRepository? _userAccessRepository;
        private IFormsAccessRepository? _formsAccessRepository;
        private IOrderTypeRepository? _orderTypeRepository;
        private IIssuingStockRepository? issuingStockRepository;
        private IAdditionalChargesRepository? _additionalChargesRepository;
        private ICompanyCategoryRepository? _companyCategoryRepository;
        private ICustomerCategoryRepository? _customerCategoryRepository;
        public RepositoryWrapperForMaster(TipsMasterDbContext tipsMasterDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _tipsMasterDbContext = tipsMasterDbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public ICompanyCategoryRepository CompanyCategoryRepository
        {
            get
            {
                if (_companyCategoryRepository == null)
                {
                    _companyCategoryRepository = new CompanyCategoryRepository(_tipsMasterDbContext);
                }
                return _companyCategoryRepository;
            }
        }
        public ICustomerCategoryRepository CustomerCategoryRepository
        {
            get
            {
                if (_customerCategoryRepository == null)
                {
                    _customerCategoryRepository = new CustomerCategoryRepository(_tipsMasterDbContext);
                }
                return _customerCategoryRepository;
            }
        }

        public IAdditionalChargesRepository AdditionalChargesRepository
        {
            get
            {
                if (_additionalChargesRepository == null)
                {
                    _additionalChargesRepository = new AdditionalChargesRepository(_tipsMasterDbContext);
                }
                return _additionalChargesRepository;
            }
        }

        public IIssuingStockRepository IssuingStockRepository
        {
            get
            {
                if (issuingStockRepository == null)
                {
                    issuingStockRepository = new IssuingStockRepository(_tipsMasterDbContext);
                }
                return issuingStockRepository;
            }
        }

        public IOrderTypeRepository OrderTypeRepository
        {
            get
            {
                if (_orderTypeRepository == null)
                {
                    _orderTypeRepository = new OrderTypeRepository(_tipsMasterDbContext);
                }
                return _orderTypeRepository;
            }
        }

        public IEnggBomRepository EnggBomRepository
        {
            get
            {
                if (_enggBomRepository == null)
                {
                    _enggBomRepository = new EngineeringBomRepository(_tipsMasterDbContext);
                }
                return _enggBomRepository;
            }
        }
        public ILeadWebsiteRepository leadWebsiteRepository
        {
            get
            {
                if (_leadWebsiteRepository == null)
                {
                    _leadWebsiteRepository = new LeadWebsiteRepository(_tipsMasterDbContext);
                }
                return _leadWebsiteRepository;
            }
        }
        public IUnitRepository unitRepository
        {
            get
            {
                if (_unitRepository == null)
                {
                    _unitRepository = new UnitRepository(_tipsMasterDbContext);
                }
                return _unitRepository;
            }
        }
        public ISourceDetailsRepository SourceDetailsRepository
        {
            get
            {
                if (_sourceDetailsRepository == null)
                {
                    _sourceDetailsRepository = new SourceDetailsRepository(_tipsMasterDbContext);
                }
                return _sourceDetailsRepository;
            }
        }
        public IStageOfConstructionRepository StageOfConstructionRepository
        {
            get
            {
                if (_stageOfConstructionRepository == null)
                {
                    _stageOfConstructionRepository = new StageOfConstructionRepository(_tipsMasterDbContext);
                }
                return _stageOfConstructionRepository;
            }
        }
        public IPmcContractorRepository PmcContractorRepository
        {
            get
            {
                if (_pmcContractorRepository == null)
                {
                    _pmcContractorRepository = new PmcContractorRepository(_tipsMasterDbContext);
                }
                return _pmcContractorRepository;
            }
        }
        public ICityRepository CityRepository
        {
            get
            {
                if (_cityRepository == null)
                {
                    _cityRepository = new CityRepository(_tipsMasterDbContext);
                }
                return _cityRepository;
            }
        }
        public IArchitectureRepository ArchitectureRepository
        {
            get
            {
                if (_architectureRepository == null)
                {
                    _architectureRepository = new ArchitectureRepository(_tipsMasterDbContext);
                }
                return _architectureRepository;
            }
        }
        public ILightningDesignerRepository LightningDesignerRepository
        {
            get
            {
                if (_lightningDesignerRepository == null)
                {
                    _lightningDesignerRepository = new LightningDesignerRepository(_tipsMasterDbContext);
                }
                return _lightningDesignerRepository;
            }
        }
        public IStateRepository StateRepository
        {
            get
            {
                if (_stateRepository == null)
                {
                    _stateRepository = new StateRepository(_tipsMasterDbContext);
                }
                return _stateRepository;
            }
        }
        public IProjectNameRepository ProjectNameRepository
        {
            get
            {
                if (_projectNameRepository == null)
                {
                    _projectNameRepository = new ProjectNameRepository(_tipsMasterDbContext);
                }
                return _projectNameRepository;
            }
        }

        public IBHKRepository BHKRepository
        {
            get
            {
                if (_bHKRepository == null)
                {
                    _bHKRepository = new BHKRepository(_tipsMasterDbContext);
                }
                return _bHKRepository;
            }
        }
        public ISFTRepository SFTRepository
        {
            get
            {
                if (_sFTRepository == null)
                {
                    _sFTRepository = new SFTRepository(_tipsMasterDbContext);
                }
                return _sFTRepository;
            }
        }
        public ITypeOfHomeRepository TypeOfHomeRepository
        {
            get
            {
                if (_typeOfHome == null)
                {
                    _typeOfHome = new TypeOfHomeRepository(_tipsMasterDbContext);
                }
                return _typeOfHome;
            }
        }

        public IFileUploadRepository FileUploadRepository
        {
            get
            {
                if (_fileUploadRepo == null)
                {
                    _fileUploadRepo = new FileUploadDocumentRepository(_tipsMasterDbContext);
                }
                return _fileUploadRepo;
            }
        }
        public IImageUploadRepository ImageUploadRepository
        {
            get
            {
                if (_imageUploadRepository == null)
                {
                    _imageUploadRepository = new ImageUploadDocumentRepository(_tipsMasterDbContext);
                }
                return _imageUploadRepository;
            }
        }


        public IEnggBomNREConsumableRepository EnggBomNREConsumableRepository
        {
            get
            {
                if (_enggBomNREConsumableRepository == null)
                {
                    _enggBomNREConsumableRepository = new EngineeringNREConsumableRepository(_tipsMasterDbContext);
                }
                return _enggBomNREConsumableRepository;
            }
        }

        public IItemMasterRoutingRepository ItemMasterRoutingRepository
        {
            get
            {
                if (_itemmasterRoutingRepository == null)
                {
                    _itemmasterRoutingRepository = new ItemMasterRoutingRepository(_tipsMasterDbContext);
                }
                return _itemmasterRoutingRepository;
            }
        }
        public IRoomNameRepository RoomNameRepository
        {
            get
            {
                if (_roomNameRepository == null)
                {
                    _roomNameRepository = new RoomNameRepository(_tipsMasterDbContext);
                }
                return _roomNameRepository;
            }
        }
        public ITypeSolutionRepository TypeSolutionRepository
        {
            get
            {
                if (_typeSolutionRepository == null)
                {
                    _typeSolutionRepository = new TypeSolutionRepository(_tipsMasterDbContext);
                }
                return _typeSolutionRepository;
            }
        }

        public IProductTypeRepository ProductTypeRepository
        {
            get
            {
                if (_productTypeRepository == null)
                {
                    _productTypeRepository = new ProductTypeRepository(_tipsMasterDbContext);
                }
                return _productTypeRepository;
            }
        }

        public ISourceRepository sourceRepository
        {
            get
            {
                if (_sourceRepo == null)
                {
                    _sourceRepo = new SourceRepository(_tipsMasterDbContext);
                }
                return _sourceRepo;
            }
        }
        public ISecondarySourceRepository secondarySourceRepository
        {
            get
            {
                if (_secondarySourceRepo == null)
                {
                    _secondarySourceRepo = new SecondarySourceRepository(_tipsMasterDbContext);
                }
                return _secondarySourceRepo;
            }
        }
        public ILeadStatusRepository LeadStatusRepository
        {
            get
            {
                if (_leadStatusRepo == null)
                {
                    _leadStatusRepo = new LeadStatusRepository(_tipsMasterDbContext);
                }
                return _leadStatusRepo;
            }
        }
        public ILeadTypeRepository LeadTypeRepository
        {
            get
            {
                if (_leadTypeRepo == null)
                {
                    _leadTypeRepo = new LeadTypeRepository(_tipsMasterDbContext);
                }
                return _leadTypeRepo;
            }
        }

        public IDemoStatusRepository DemoStatusRepository
        {
            get
            {
                if (_demoStatusRepo == null)
                {
                    _demoStatusRepo = new DemoStatusRepository(_tipsMasterDbContext);
                }
                return _demoStatusRepo;
            }
        }

        public ILeadRepository LeadRepository
        {
            get
            {
                if (_leadRepo == null)
                {
                    _leadRepo = new LeadRepository(_tipsMasterDbContext);
                }
                return _leadRepo;
            }
        }
        public IPartTypesRepository partTypesRepository
        {
            get
            {
                if (_parttypesRepo == null)
                {
                    _parttypesRepo = new PartTypesRepository(_tipsMasterDbContext);
                }
                return _parttypesRepo;
            }
        }
        public IProcessRepository ProcessRepository
        {
            get
            {
                if (_processRepo == null)
                {
                    _processRepo = new ProcessRepository(_tipsMasterDbContext);
                }
                return _processRepo;
            }
        }
        public IPreferredFreightForwarderRepository PreferredFreightForwarderRepository
        {
            get
            {
                if (_preferredFreightForwarderRepo == null)
                {
                    _preferredFreightForwarderRepo = new PreferredFreightForwarderRepository(_tipsMasterDbContext);
                }
                return _preferredFreightForwarderRepo;
            }
        }

        public IPreferredFreightForwarderRepository preferredFreightForwarderRepository => throw new NotImplementedException();

        public IPriceListRepository PriceListRepository
        {
            get
            {
                if (_pricelistRepo == null)
                {
                    _pricelistRepo = new PriceListRepository(_tipsMasterDbContext);
                }
                return _pricelistRepo;
            }
        }
        public IShipmentModeRepository ShipmentModeRepository
        {
            get
            {
                if (_shipmentmodeRepo == null)
                {
                    _shipmentmodeRepo = new ShipmentModeRepository(_tipsMasterDbContext);
                }
                return _shipmentmodeRepo;
            }
        }
        public IGST_PercentageRepository GST_PercentageRepository
        {
            get
            {
                if (_gstpercentageRepo == null)
                {
                    _gstpercentageRepo = new GST_PercentageRepository(_tipsMasterDbContext);
                }
                return _gstpercentageRepo;
            }
        }
        public ILocationsRepository LocationsRepository
        {
            get
            {
                if (_locationsRepo == null)
                {
                    _locationsRepo = new LocationsRepository(_tipsMasterDbContext);
                }
                return _locationsRepo;
            }
        }
        public ICommodityRepository CommodityRepository
        {
            get
            {
                if (_commodityRepo == null)
                {
                    _commodityRepo = new CommodityRepository(_tipsMasterDbContext);
                }
                return _commodityRepo;
            }
        }
        public IUOCRepository UOCRepository
        {
            get
            {
                if (_uOCRepo == null)
                {
                    _uOCRepo = new UOCRepository(_tipsMasterDbContext);
                }
                return _uOCRepo;
            }
        }
        public IUOMRepository UOMRepository
        {
            get
            {
                if (_uOMRepo == null)
                {
                    _uOMRepo = new UOMRepository(_tipsMasterDbContext, _httpContextAccessor);
                }
                return _uOMRepo;
            }
        }
        public IPaymentTermRepository PaymentTermRepository
        {
            get
            {
                if (_paymentTermRepo == null)
                {
                    _paymentTermRepo = new PaymentTermRepository(_tipsMasterDbContext);
                }
                return _paymentTermRepo;
            }
        }

        //public IPaymentTermRepository PaymentTermRepository => throw new NotImplementedException();

        public ISegmentRepository SegmentRepository
        {
            get
            {
                if (_segmentRepository == null)
                {
                    _segmentRepository = new SegmentRepository(_tipsMasterDbContext);
                }
                return _segmentRepository;
            }
        }
        public IWarehouseRepository WarehouseRepository
        {
            get
            {
                if (_warehouseRepository == null)
                {
                    _warehouseRepository = new WarehouseRepository(_tipsMasterDbContext);
                }
                return _warehouseRepository;
            }
        }

        public IQuoteTermsRepository QuoteTermsRepository
        {
            get
            {
                if (_quoteTermsRepo == null)
                {
                    _quoteTermsRepo = new QuoteTermsRepository(_tipsMasterDbContext);
                }
                return _quoteTermsRepo;
            }
        }

        public IRiskCategoryRepository RiskCategoryRepository
        {
            get
            {
                if (_riskCategoryRepo == null)
                {
                    _riskCategoryRepo = new RiskCategoryRepository(_tipsMasterDbContext);
                }
                return _riskCategoryRepo;
            }
        }
        public ICategoryRepository CategoryRepository
        {
            get
            {
                if (_categoryRepo == null)
                {
                    _categoryRepo = new CategoryRepository(_tipsMasterDbContext);
                }
                return _categoryRepo;
            }
        }

        public IShipmentInstructionsRepository ShipmentInstructionsRepository
        {
            get
            {
                if (_shipmentInstructionsRepo == null)
                {
                    _shipmentInstructionsRepo = new ShipmentInstructionsRepository(_tipsMasterDbContext);
                }
                return _shipmentInstructionsRepo;
            }
        }


        public ICompanyMasterRepository CompanyMasterRepository
        {
            get
            {
                if (_companyMasterRepo == null)
                {
                    _companyMasterRepo = new CompanyMasterRepository(_tipsMasterDbContext);
                }
                return _companyMasterRepo;
            }
        }
         
        public ICustomerMasterRepository CustomerMasterRepository
        {
            get
            {
                if (_customerMasterRepo == null)
                {
                    _customerMasterRepo = new CustomerMasterRepository(_tipsMasterDbContext);
                }
                return _customerMasterRepo;
            }
        }

        public ICustomerMasterRepository Customermasterrepository => throw new NotImplementedException();

        public ILeadTimeRepository leadTimeRepository
        {
            get
            {
                if (_leadTimeRepo == null)
                {
                    _leadTimeRepo = new LeadTimeRepository(_tipsMasterDbContext);
                }
                return _leadTimeRepo;
            }
        }
        public ICustomerTypeRepository CustomerTypeRepository
        {
            get
            {
                if (_customerTypeRepo == null)
                {
                    _customerTypeRepo = new CustomerTypeRepository(_tipsMasterDbContext);
                }
                return _customerTypeRepo;
            }
        }
        public IPackingInstructionRepository PackingInstructionRepository
        {
            get
            {
                if (_packingInstructionRepo == null)
                {
                    _packingInstructionRepo = new PackingInstructionRepository(_tipsMasterDbContext);
                }
                return _packingInstructionRepo;
            }
        }
        public IMaterialTypeRepository MaterialTypeRepository
        {
            get
            {
                if (_materialTypeRepo == null)
                {
                    _materialTypeRepo = new MaterialTypeRepository(_tipsMasterDbContext);
                }
                return _materialTypeRepo;
            }
        }


        public IProcurementTypeRepository ProcurementTypeRepository
        {
            get
            {
                if (_procurementTypeRepo == null)
                {
                    _procurementTypeRepo = new ProcurementTypeRepository(_tipsMasterDbContext);
                }
                return _procurementTypeRepo;
            }
        }

        public IItemMasterRepository ItemMasterRepository
        {
            get
            {
                if (_itemMasterRepo == null)
                {
                    _itemMasterRepo = new ItemMasterRepository(_tipsMasterDbContext);
                }
                return _itemMasterRepo;
            }
        }

        public IDeliveryTermRepository DeliveryTermRepo
        {
            get
            {
                if (_deliveryTermRepo == null)
                {
                    _deliveryTermRepo = new DeliveryTermRepository(_tipsMasterDbContext);
                }
                return _deliveryTermRepo;
            }
        }

        public IVolumeUomRepository VolumeUomRepo
        {
            get
            {
                if (_volumeUomRepo == null)
                {
                    _volumeUomRepo = new VolumeUomRepository(_tipsMasterDbContext);
                }
                return _volumeUomRepo;
            }
        }

        public IWeightUomRepository WeightUomRepository
        {
            get
            {
                if (_weightUomRepo == null)
                {
                    _weightUomRepo = new WeightUomRepository(_tipsMasterDbContext);
                }
                return _weightUomRepo;
            }
        }

        public IBankRepository BankRepository
        {
            get
            {
                if (_bankRepo == null)
                {
                    _bankRepo = new BankTermRepository(_tipsMasterDbContext);
                }
                return _bankRepo;
            }
        }

        public IDepartmentRepository DepartmentRepository
        {
            get
            {
                if (_departmentRepo == null)
                {
                    _departmentRepo = new DepartmentRepository(_tipsMasterDbContext);
                }
                return _departmentRepo;
            }
        }

        public IIncoTermRepository IncoTermRepository
        {
            get
            {
                if (_incoTermRepo == null)
                {
                    _incoTermRepo = new IncoTermRepository(_tipsMasterDbContext);
                }
                return _incoTermRepo;
            }
        }

        public ICurrencyRepository CurrencyRepository
        {
            get
            {
                if (_currency == null)
                {
                    _currency = new CurrencyRepository(_tipsMasterDbContext);
                }
                return _currency;
            }
        }
        public IBasisOfApprovalRepository BasisOfApprovalRepository
        {
            get
            {
                if (_basisOfApproval == null)
                {
                    _basisOfApproval = new BasisOfApprovalRepository(_tipsMasterDbContext);
                }
                return _basisOfApproval;
            }
        }

        public IVendorCategoryRepository VendorCategoryRepository
        {
            get
            {
                if (_vendorCategory == null)
                {
                    _vendorCategory = new VendorCategoryRepository(_tipsMasterDbContext);
                }
                return _vendorCategory;
            }
        }

        public IVendorDepartmentRepository VendorDepartmentRepository
        {
            get
            {
                if (_vendorDepartment == null)
                {
                    _vendorDepartment = new VendorDepartmentRepository(_tipsMasterDbContext);
                }
                return _vendorDepartment;
            }
        }

        public IScopeOfSupplyRepository ScopeOfSupplyRepository
        {
            get
            {
                if (_scopeOfSupply == null)
                {
                    _scopeOfSupply = new ScopeOfSupplyRepository(_tipsMasterDbContext);
                }
                return _scopeOfSupply;
            }
        }

        public IVendorRepository VendorRepository
        {
            get
            {
                if (_vendorRepository == null)
                {
                    _vendorRepository = new VendorRepository(_tipsMasterDbContext);
                }
                return _vendorRepository;
            }
        }
        public IPurchaseGroupRepository PurchaseGroupRepository
        {
            get
            {
                if (_purchaseGroupRepo == null)
                {
                    _purchaseGroupRepo = new PurchaseGroupRepository(_tipsMasterDbContext);
                }
                return _purchaseGroupRepo;
            }
        }
        public ICostCenterRepository CostCenterRepository
        {
            get
            {
                if (_costCenterRepo == null)
                {
                    _costCenterRepo = new CostCenterRepository(_tipsMasterDbContext);
                }
                return _costCenterRepo;
            }
        }
        public ICostingMethodRepository CostingMethodRepository
        {
            get
            {
                if (_costingMethodRepo == null)
                {
                    _costingMethodRepo = new CostingMethodRepository(_tipsMasterDbContext);
                }
                return _costingMethodRepo;
            }
        }

      //  public ICostingMethodRepository CostingMethodRepository => throw new NotImplementedException();

        public IExportUnitTypeRepository ExportUnitTypeRepository
        {
            get
            {
                if (_exportUnitTypeRepo == null)
                {
                    _exportUnitTypeRepo = new ExportUnitTypeRepository(_tipsMasterDbContext);
                }
                return _exportUnitTypeRepo;
            }
        }

        //public IExportUnitTypeRepository ExportUnitTypeRepository => throw new NotImplementedException();

        public ISalutationsRepository SalutationsRepository
        {
            get
            {
                if (_salutationsRepo == null)
                {
                    _salutationsRepo = new SalutationsRepository(_tipsMasterDbContext);
                }
                return _salutationsRepo;
            }
        }

        //public ISalutationsRepository SalutationsRepository => throw new NotImplementedException();

        public ILanguageRepository LanguageRepository
        {
            get
            {
                if (_languageRepo == null)
                {
                    _languageRepo = new LanguageRepository(_tipsMasterDbContext);
                }
                return _languageRepo;
            }
        }
        public ITypeOfCompanyRepository TypeOfCompanyRepository
        {
            get
            {
                if (_typeOfCompanyRepo == null)
                {
                    _typeOfCompanyRepo = new TypeOfCompanyRepository(_tipsMasterDbContext);
                }
                return _typeOfCompanyRepo;
            }
        }
        public INatureOfRelationshipRepository NatureOfRelationshipRepository
        {
            get
            {
                if (_abilityOfRelationshipRepo == null)
                {
                    _abilityOfRelationshipRepo = new NatureOfRelationshipRepository(_tipsMasterDbContext);
                }
                return _abilityOfRelationshipRepo;
            }
        }

        // INatureOfRelationshipRepository NatureOfRelationshipRepository => throw new NotImplementedException();

        public IAuditFrequencyRepository AuditFrequencyRepository
        {
            get
            {
                if (_auditFrequencyRepo == null)
                {
                    _auditFrequencyRepo = new AuditFrequencyRepository(_tipsMasterDbContext);
                }
                return _auditFrequencyRepo;
            }
        }

        public IReleaseEnggBomRepository ReleaseEnggBomRepository
        {
            get
            {
                if (_releaseEnggBomRepo == null)
                {
                    _releaseEnggBomRepo = new ReleaseEnggBomRepository(_tipsMasterDbContext);
                }
                return _releaseEnggBomRepo;
            }
        }


        public IReleaseCostBomRepository ReleaseCostBomRepository
        {
            get
            {
                if (_releaseCostBomRepo == null)
                {
                    _releaseCostBomRepo = new ReleaseCostBomRepository(_tipsMasterDbContext);
                }
                return _releaseCostBomRepo;
            }
        }


        public IReleaseProductBomRepository ReleaseProductBomRepository
        {
            get
            {
                if (_releaseProductBomRepo == null)
                {
                    _releaseProductBomRepo = new ReleaseProductBomRepository(_tipsMasterDbContext);
                }
                return _releaseProductBomRepo;
            }
        }

        public IEnggBomGroupRepository EnggBomGroupRepository
        {
            get
            {
                if (_enggbomGroupRepo == null)
                {
                    _enggbomGroupRepo = new EnggBomGroupRepository(_tipsMasterDbContext);
                }
                return _enggbomGroupRepo;
            }
        }

        public IEnggCustomFieldRepository EnggCustomFieldRepository
        {
            get
            {
                if (_enggcustomFieldRepo == null)
                {
                    _enggcustomFieldRepo = new EnggCustomFieldRepository(_tipsMasterDbContext);
                }
                return _enggcustomFieldRepo;
            }
        }

        public IRoleRepository RoleRepository
        {
            get
            {
                if (_roleRepository == null)
                {
                    _roleRepository = new RoleRepository(_tipsMasterDbContext);
                }
                return _roleRepository;
            }
        }

        public IRoleAccessRepository RoleAccessRepository
        {
            get
            {
                if (_roleAccessRepository == null)
                {
                    _roleAccessRepository = new RoleAccessRepository(_tipsMasterDbContext);
                }
                return _roleAccessRepository;
            }
        }

        public IRegistrationFormRepository RegistrationFormRepository
        {
            get
            {
                if (_registrationFormRepo == null)
                {
                    _registrationFormRepo = new RegistrationFormRepository(_tipsMasterDbContext, _httpContextAccessor);
                }
                return _registrationFormRepo;
            }
        }

        public IUserAccessRepository UserAccessRepository
        {
            get
            {
                if (_userAccessRepository == null)
                {
                    _userAccessRepository = new UserAccessRepository(_tipsMasterDbContext, _httpContextAccessor);
                }
                return _userAccessRepository;
            }
        }

        public IFormsAccessRepository FormsAccessRepository
        {
            get
            {
                if (_formsAccessRepository == null)
                {
                    _formsAccessRepository = new FormsAccessRepository(_tipsMasterDbContext);
                }
                return _formsAccessRepository;
            }
        }

        public IVendorContactRepository VendorContactRepository => throw new NotImplementedException();

        public IVendorAddressRepository VendorAddressRepository => throw new NotImplementedException();

        public IVendorBankingRepository VendorBankingRepository => throw new NotImplementedException();
        
        //public ITypeOfCompanyRepository TypeOfCompanyRepository => throw new NotImplementedException();

        //public IPaymentTermRepository PaymentTermRepository => throw new NotImplementedException();

        //public IPackingInstructionRepository PackingInstructionRepository => throw new NotImplementedException();

        public ICompanyAddressesRepository CompanyAddressesRepository => throw new NotImplementedException();

        public ICompanyContactsRepository CompanyContactsRepository => throw new NotImplementedException();

        public ICompanyBankingRepository CompanyBankingRepository => throw new NotImplementedException();

        public ICustomerBankingsRepository CustomerBankingsRepository => throw new NotImplementedException();

        public ICustomerAddressesRepository CustomerAddressesRepository => throw new NotImplementedException();

        public ICustomerShippingAddressesRepository CustomerShippingAddressesRepository => throw new NotImplementedException();

        public ICustomerContactsRepository CustomerContactsRepository => throw new NotImplementedException();

        public IItemmasterAlternate ItemmasterAlternateRepository => throw new NotImplementedException();

        public IItemMasterApprovedVendor ItemMasterApprovedVendorRepository => throw new NotImplementedException();

        public IItemMasterFileUpload ItemMasterFileUploadRepository => throw new NotImplementedException();

 
        public IItemMasterWarehouse ItemMasterWarehouseRepository => throw new NotImplementedException();

        public IEnggBomAlternatesRepository EnggBomAlternatesRepository => throw new NotImplementedException();

        public IEnggBomChildItemRepository EnggBomChildItemRepository => throw new NotImplementedException();

 
        //public IEnggBomNREConsumableRepository EnggBomNREConsumableRepository => throw new NotImplementedException();

        //add new
        //public IEnggBomNREConsumableRepository EnggBomNREConsumableRepository => throw new NotImplementedException();


        //public IItemMasterRoutingRepository ItemMasterRoutingRepository => throw new NotImplementedException();




        //public ICustomerInfoRepository CustomerInfoRepository => throw new NotImplementedException();

        //public IPreferredFreightForwarderRepository PreferredFreightForwarderRepository => throw new NotImplementedException();



        public void SaveAsync()
        {
            _tipsMasterDbContext.SaveChanges();
        }

    }
}
