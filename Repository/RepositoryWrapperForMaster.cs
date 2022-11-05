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
        private  TipsMasterDbContext _tipsMasterDbContext;
        private ILeadTimeRepository _leadTimeRepo;
        private  ICustomerTypeRepository _customerTypeRepo;
        private IMaterialTypeRepository _materialTypeRepo;
        private IProcurementTypeRepository _procurementTypeRepo;

        public RepositoryWrapperForMaster(TipsMasterDbContext? tipsMasterDbContext,
            ILeadTimeRepository leadTimeRepo,
            ICustomerTypeRepository customerTypeRepository,
            IMaterialTypeRepository materialTypeRepo,
            IProcurementTypeRepository procurementTypeRepo)
        {
            _tipsMasterDbContext = tipsMasterDbContext;
            _leadTimeRepo = leadTimeRepo;
            _customerTypeRepo = customerTypeRepository;
            _materialTypeRepo = materialTypeRepo;
            _procurementTypeRepo = procurementTypeRepo;
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
        public void SaveAsync()
        {
            _tipsMasterDbContext.SaveChanges();
        }

        
    }
}
