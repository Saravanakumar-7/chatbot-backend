using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

namespace Tips.Grin.Api.Repository
{
    public class OpenGrinRepository : RepositoryBase<OpenGrin>, IOpenGrinRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;

        public OpenGrinRepository(TipsGrinDbContext tipsGrinDbContext) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;

        }

        public async Task<OpenGrin> CreateOpenGrin(OpenGrin openGrin)
        {
            openGrin.CreatedBy = "Admin";
            openGrin.CreatedOn = DateTime.Now;
            openGrin.Unit = "Bangalore";
            var result = await Create(openGrin);
            return result;
        }

        public async Task<string> DeleteOpenGrin(OpenGrin openGrin)
        {
            Delete(openGrin);
            string result = $"OpenGrin details of {openGrin.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<OpenGrin>> GetAllOpenGrinDetails(PagingParameter pagingParameter, SearchParams searchParams)
        {
            var openGrinDetails = FindAll()
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CustomerName.Contains(searchParams.SearchValue)
                || inv.CustomerId.Contains(searchParams.SearchValue))));

            return PagedList<OpenGrin>.ToPagedList(openGrinDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<OpenGrin> GetOpenGrinDetailsbyId(int id)
        {
            var openGrinDetailsById = await _tipsGrinDbContext.OpenGrin.Where(x => x.Id == id)
                               .Include(t => t.OpenGrinParts)
                            .FirstOrDefaultAsync();

            return openGrinDetailsById;
        }

        public async Task<string> UpdateOpenGrin(OpenGrin openGrin)
        {
            openGrin.LastModifiedBy = "Admin";
            openGrin.LastModifiedOn = DateTime.Now;
            Update(openGrin);
            string result = $"OpenGrin details of {openGrin.Id} is updated successfully!";
            return result;
        }
    }
}
