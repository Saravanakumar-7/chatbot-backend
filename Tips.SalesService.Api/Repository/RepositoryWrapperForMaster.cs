using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Contracts;

namespace Tips.SalesService.Api.Repository

{
    public class RepositoryWrapperForMaster 
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private IRfqRepository _rfqRepository;

        public RepositoryWrapperForMaster(TipsSalesServiceDbContext tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
        }


        public IRfqRepository RfqRepository
        {
            get
            {
                if (_rfqRepository == null)
                {
                    _rfqRepository = new RfqRepository(_tipsSalesServiceDbContext);
                }
                return _rfqRepository;
            }
        }

        public void SaveAsync()
        {
            throw new NotImplementedException();
        }
    }
}
