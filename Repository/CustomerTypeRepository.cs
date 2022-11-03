using Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CustomerTypeRepository : RepositoryBase<CustomerType>, ICustomerTypeRepository
    {
        public CustomerTypeRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
