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
        //private ILeadTimeRepository _leadTimeRepo;
        private  ICustomerTypeRepository? _customerTypeRepo;

        public RepositoryWrapperForMaster(TipsMasterDbContext? tipsMasterDbContext)
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
                return _customerTypeRepo; }
        }

        public ILeadTimeRepository leadTimeRepository => throw new NotImplementedException();

        public void SaveAsync()
        {
            _tipsMasterDbContext.SaveChanges();
        }

        
    }
}
