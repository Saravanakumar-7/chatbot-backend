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
    public class EmailTemplateRepository : RepositoryBase<EmailTemplate>, IEmailTemplateRepository
    {
        private TipsMasterDbContext _tipsMasterDbContext;
        public EmailTemplateRepository(TipsMasterDbContext tipsMasterDbContext):base(tipsMasterDbContext)
        {
            _tipsMasterDbContext = tipsMasterDbContext;
        }
        public async Task<EmailTemplate> GetEmailTemplatebyProcessType(string ProcessType)
        { 
            var temp= await _tipsMasterDbContext.emailtemplate.Where(x=>x.ProcessType==ProcessType).FirstOrDefaultAsync();
            return temp;
        }
    }
}
