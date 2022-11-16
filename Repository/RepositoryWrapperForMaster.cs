using Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryWrapperForMaster : IRepositoryWrapperForMaster
    {
        private TipsMasterDbContext _tipsMasterDbContext;
        private ILeadTimeRepository _leadTimeRepo;
        private ICustomerTypeRepository _customerTypeRepo;
        private IMaterialTypeRepository _materialTypeRepo;
        private IProcurementTypeRepository _procurementTypeRepo;
        private IItemMasterRepository _itemMasterRepo;

        private IDeliveryTermRepository? _deliveryTermRepo;
        private IVolumeUomRepository? _volumeUomRepo;
        private IWeightUomRepository? _weightUomRepo;

        private IIncoTermRepository? _incoTermRepo;
        private IDepartmentRepository? _departmentRepo;
        private IBankRepository? _bankRepo;
        private ICurrencyRepository? _currency;

        private IBasicOfApprovalRepository? _basicOfApproval;
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

        private IShipmentInstructionsRepository? _shipmentInstructionsRepo;
        private ICategoryRepository? _categoryRepo;
        private IRiskCategoryRepository? _riskCategoryRepo;
        private IQuoteTermsRepository? _quoteTermsRepo;
        private IWarehouseRepository? _warehouseRepository;
        private ISegmentRepository _segmentRepository;

        private ICustomerMasterRepository? _customerMasterRepo;
         

        private ICompanyMasterRepository? _companyMasterRepo;
         

        private IUOCRepository? _uOCRepo;
        private IUOMRepository? _uOMRepo;
        private ICommodityRepository? _commodityRepo;
        private ILocationsRepository? _locationsRepo;


        public RepositoryWrapperForMaster(TipsMasterDbContext tipsMasterDbContext)
        {
            _tipsMasterDbContext = tipsMasterDbContext;
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
                    _uOMRepo = new UOMRepository(_tipsMasterDbContext);
                }
                return _uOMRepo;
            }
        }
        public IPaymentTermRepository paymentTermRepository
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
        public IPackingInstructionRepository packingInstructionRepository
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
        public IBasicOfApprovalRepository BasicOfApprovalRepository
        {
            get
            {
                if (_basicOfApproval == null)
                {
                    _basicOfApproval = new BasicOfApprovalRepository(_tipsMasterDbContext);
                }
                return _basicOfApproval;
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
        public ICostingMethodRepository costingMethodRepository
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

        public ICostingMethodRepository CostingMethodRepository => throw new NotImplementedException();

        public IExportUnitTypeRepository exportUnitTypeRepository
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

        public IExportUnitTypeRepository ExportUnitTypeRepository => throw new NotImplementedException();

        public ISalutationsRepository salutationsRepository
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

        public ISalutationsRepository SalutationsRepository => throw new NotImplementedException();

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
        public INatureOfRelationshipRepository natureOfRelationshipRepository
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

        public INatureOfRelationshipRepository NatureOfRelationshipRepository => throw new NotImplementedException();

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


        public IVendorContactRepository VendorContactRepository => throw new NotImplementedException();

        public IVendorAddressRepository VendorAddressRepository => throw new NotImplementedException();

        public IVendorBankingRepository VendorBankingRepository => throw new NotImplementedException();
        
        public ITypeOfCompanyRepository typeOfCompanyRepository=> throw new NotImplementedException();

        public IPaymentTermRepository PaymentTermRepository => throw new NotImplementedException();

        public IPackingInstructionRepository PackingInstructionRepository => throw new NotImplementedException();

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

        public IItemMasterRouting ItemMasterRoutingRepository => throw new NotImplementedException();

        public IItemMasterWarehouse ItemMasterWarehouseRepository => throw new NotImplementedException();

        public IPreferredFreightForwarderRepository preferredFreightForwarderRepository => throw new NotImplementedException();

        public void SaveAsync()
        {
            _tipsMasterDbContext.SaveChanges();
        }

    }
}
