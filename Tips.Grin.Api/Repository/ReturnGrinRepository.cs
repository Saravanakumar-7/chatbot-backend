using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Repository
{
    public class ReturnGrinRepository : RepositoryBase<ReturnGrin>, IReturnGrinRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;

        public ReturnGrinRepository(TipsGrinDbContext tipsGrinDbContext) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;

        }

        public async Task<ReturnGrin> CreateReturnGrin(ReturnGrin returnGrin)
        {
            returnGrin.CreatedBy = "Admin";
            returnGrin.CreatedOn = DateTime.Now;
            returnGrin.Unit = "Bangalore";
            var result = await Create(returnGrin);
            return result;
        }

        public async Task<string> DeleteReturnGrin(ReturnGrin returnGrin)
        {
            Delete(returnGrin);
            string result = $"binning details of {returnGrin.Id} is deleted successfully!";
            return result;
        }

        public async Task<ReturnGrin> GetReturnGrinDetailsbyId(int id)
        {
            var returnGrinDetailsbyId = await _tipsGrinDbContext.ReturnGrins.Where(x => x.Id == id)
                               .Include(x => x.ReturnGrinParts)
                               .FirstOrDefaultAsync();

            return returnGrinDetailsbyId;
        }

        public Task<string> UpdateReturnGrin(ReturnGrin returnGrin)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ReturnGrinPartsListDto>> ReturnGrinPartsByPartNumber(string partNo)
        {
            IEnumerable<ReturnGrinPartsListDto> getReturnGrinPartsByPartNo = await _tipsGrinDbContext.ReturnGrinParts
                                .Select(x => new ReturnGrinPartsListDto()
                                {
                                    PartNumber = x.PartNumber,
                                    MftrNumber = x.MftrNumber,
                                    Description = x.Description,
                                })
                                .Where(x => x.PartNumber == partNo)
                              .ToListAsync();

            return getReturnGrinPartsByPartNo;
        }

        public async Task<PagedList<ReturnGrin>> GetAllReturnGrin(PagingParameter pagingParameter)
        {
            var getAllReturnGrinDetails = PagedList<ReturnGrin>.ToPagedList(FindAll()
                                .Include(t => t.ReturnGrinParts)
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllReturnGrinDetails;
        }
    }
}
          
       