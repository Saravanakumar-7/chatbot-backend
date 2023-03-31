using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class FormsAccessRepository : RepositoryBase<FormsAccess>, IFormsAccessRepository
    {
        public FormsAccessRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<FormsAccess>> GetAllFormsAccess()
        {
            var formsAccessDetails =await TipsMasterDbContext.FormsAccesses.ToListAsync();

            return formsAccessDetails;
        }
    }
}
