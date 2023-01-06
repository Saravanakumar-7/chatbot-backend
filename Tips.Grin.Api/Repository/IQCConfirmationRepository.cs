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
            iQCConfirmation.Unit = "Bangalore";
            var result = await Create(iQCConfirmation);
            return result.Id;
        }
         

        public async Task<IEnumerable<IQCConfirmation>> GetAllIqcDetails()
        {
            var getallIQCList = await FindAll().ToListAsync();
            return (getallIQCList);

        }
        public async Task<IEnumerable<IQCConfirmation>> GetIqcDetailsbyGrinNo(string grinNumber)
    {
            var iQCDetail = await FindByCondition(x => x.GrinNumber == grinNumber).ToListAsync();
            return iQCDetail;
        }

        public async Task<string> UpdateIqc(IQCConfirmation iQCConfirmation)
        {
            iQCConfirmation.LastModifiedBy = "Admin";
            iQCConfirmation.LastModifiedOn = DateTime.Now;
            Update(iQCConfirmation);
            string result = $"IQC details of {iQCConfirmation.Id} is updated successfully!";
            return result;
        }


        public async Task<IQCConfirmation> GetIqcDetailsbyId(int id)
        {
            var iQCDetailById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return iQCDetailById;
        }
 
    }

}






