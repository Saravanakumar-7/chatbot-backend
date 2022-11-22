using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Entities;
using Tips.Grin.Api.Entities.DTOs;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Tips.Grin.Api.Repository
{
    public class IQCConfirmationRepository : RepositoryBase<IQCConfirmation>, IIQCConfirmationRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;

        public IQCConfirmationRepository(TipsGrinDbContext tipsGrinDbContext) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;

        }

        public async Task<int?> CreateIqc(IQCConfirmation iQCConfirmation)
        {
            iQCConfirmation.CreatedBy = "Admin";
            iQCConfirmation.CreatedOn = DateTime.Now;
            iQCConfirmation.LastModifiedBy = "Admin";
            iQCConfirmation.LastModifiedOn = DateTime.Now;
            //iQCConfirmation.Unit = "Bangalore";

            var result = await Create(iQCConfirmation);
            return result.Id;
        }

        public Task<int?> CreateIqcById(IQCConfirmation iQCConfirmation)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteIqc(IQCConfirmation iQCConfirmation)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IQCConfirmation>> GetAllIqcDetails()
        {
            throw new NotImplementedException();
        }

        public Task<IQCConfirmation> GetIqcDetailByGrinNo(int id)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateIqc(IQCConfirmation iQCConfirmation)
        {
            throw new NotImplementedException();
        }
    }
}
