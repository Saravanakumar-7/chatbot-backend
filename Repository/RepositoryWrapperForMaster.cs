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

        private IUOMRepository _uomRepo;
        private IUOCRepository _uocRepo;
        private ICommodityRepository _commodityRepo;
        private ILocationsRepository _locationsRepo;

        private IItemMasterRepository _itemMasterRepo;
        private ICustomerMasterRepository _customermasterRepo;
        private ICompanyMasterRepository _companyMasterRepo;

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
        //private IVendorContactRepository? _vendorContactRepository;
        //private IVendorBankingRepository? _vendorBankingRepository;
        //private IVendorAddressRepository? _vendorAddressRepository;


        public RepositoryWrapperForMaster(TipsMasterDbContext tipsMasterDbContext)
        {
            _tipsMasterDbContext = tipsMasterDbContext;
        }

        public IUOMRepository UOMRepository
        {
            get
            {
                if (_uomRepo == null)
                {
                    _uomRepo = new UOMRepository(_tipsMasterDbContext);
                }
                return _uomRepo;
            }
        }

        public IUOCRepository UOCRepository
        {
            get
            {
                if (_uocRepo == null)
                {
                    _uocRepo = new UOCRepository(_tipsMasterDbContext);
                }
                return _uocRepo;
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

        public ICustomerMasterRepository Customermasterrepository
        {
            get
            {
                if (_customermasterRepo == null)
                {
                    _customermasterRepo = new CustomerMasterRepository(_tipsMasterDbContext);
                }
                return _customermasterRepo;
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
        public IItemmasterAlternate ItemmasterAlternateRepository => throw new NotImplementedException();

        public IItemMasterApprovedVendor ItemMasterApprovedVendorRepository => throw new NotImplementedException();

        public IItemMasterFileUpload ItemMasterFileUploadRepository => throw new NotImplementedException();

        public IItemMasterRouting ItemMasterRoutingRepository => throw new NotImplementedException();

        public IItemMasterWarehouse ItemMasterWarehouseRepository => throw new NotImplementedException();


        public IVendorContactRepository VendorContactRepository => throw new NotImplementedException();

        public IVendorAddressRepository VendorAddressRepository => throw new NotImplementedException();

        public IVendorBankingRepository VendorBankingRepository => throw new NotImplementedException();



        public ICustomerBankingsRepository CustomerBankingsRepository => throw new NotImplementedException();

        public ICustomerAddressesRepository CustomerAddressesRepository => throw new NotImplementedException();

        public ICustomerShippingAddressesRepository CustomerShippingAddressesRepository => throw new NotImplementedException();

        public ICustomerContactsRepository CustomerContactsRepository => throw new NotImplementedException();


        public ICompanyAddressesRepository CompanyAddressesRepository => throw new NotImplementedException();

        public ICompanyContactsRepository CompanyContactsRepository => throw new NotImplementedException();

        public ICompanyBankingRepository CompanyBankingRepository => throw new NotImplementedException();

        public void SaveAsync()
        {
            _tipsMasterDbContext.SaveChanges();
        }

    }
}
