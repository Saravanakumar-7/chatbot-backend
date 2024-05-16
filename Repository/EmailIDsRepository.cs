using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EmailIDsRepository : RepositoryBase<EmailIDs>, IEmailIDsRepository
    {
        private TipsMasterDbContext _tipsMasterDbContext;
        public EmailIDsRepository(TipsMasterDbContext tipsMasterDbContext) : base(tipsMasterDbContext)
        {
            _tipsMasterDbContext = tipsMasterDbContext;
        }
        public async Task<List<EmailIDs>> GetEmailIdDetailsbyOperation(string Operations)
        {
                var ListOps = Operations.Split(',');
                var details = await FindAll().Where(x => ListOps.Contains(x.Operation)).ToListAsync();
                //var details = await FindAll().Where(x => x.Operation == ListOps[0] || x.Operation == ListOps[1]).ToListAsync();
                return details;
           
        }
    }
}
