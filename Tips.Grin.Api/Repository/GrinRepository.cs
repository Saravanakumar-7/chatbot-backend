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
using Entities.DTOs;

namespace Tips.Grin.Api.Repository
{
    public class GrinRepository : RepositoryBase<Grins>, IGrinRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        public GrinRepository(TipsGrinDbContext tipsGrinDbContext) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
        } 
        public async Task<int?> CreateGrin(Grins grins)
        {
            grins.CreatedBy = "Admin";
            grins.CreatedOn = DateTime.Now;
            Guid grinId = Guid.NewGuid();
            grins.GrinNumber = "GR-" + grinId.ToString();
            grins.Unit = "Bangalore";       

            var result = await Create(grins);
            return result.Id;
        }        

       
         
        public async Task<string> DeleteGrin(Grins grins)
        {
            Delete(grins);
            string result = $"Grin details of {grins.Id} is deleted successfully!";
            return result;
        } 

        public async Task<IEnumerable<Grins>> GetAllActiveGrin()
        {
            var allActiveGrinDetails = await FindAll().ToListAsync();
            return allActiveGrinDetails;
        }

        public async Task<IEnumerable<GrinNumberListDto>> GetAllActiveGrinNoList()
        {
            IEnumerable<GrinNumberListDto> allActiveGrinNoList= await _tipsGrinDbContext.Grins
                                .Select(x => new GrinNumberListDto()
                                {                                  

                                    Id= x.Id,
                                    GrinNumber=x.GrinNumber
                                })
                              .ToListAsync();

            return allActiveGrinNoList;
        }

        public async Task<PagedList<Grins>> GetAllGrin(PagingParameter pagingParameter)
        {
            var getAllGrinDetails= PagedList<Grins>.ToPagedList (FindAll()
                                .Include(t => t.GrinParts)
                                .ThenInclude(t => t.ProjectNumbers) 
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllGrinDetails;
        }

        public async Task<Grins> GetGrinById(int id)
        {
            var grinDetailsbyId = await _tipsGrinDbContext.Grins.Where(x => x.Id == id)
                               .Include(x => x.GrinParts)
                               .ThenInclude(t => t.ProjectNumbers)
                               .FirstOrDefaultAsync();

            return grinDetailsbyId;
        }
         

        public async Task<string> UpdateGrin(Grins grins)
        {
            grins.LastModifiedBy = "Admin";
            grins.LastModifiedOn = DateTime.Now;
            Update(grins);
            string result = $"Grin Detail {grins.Id} is updated successfully!";
            return result;
        }

       

    }
    public class UploadDocumentRepository : RepositoryBase<DocumentUpload>, IDocumentUploadRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        public UploadDocumentRepository(TipsGrinDbContext tipsGrinDbContext) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
        }

        //public async Task<GrinParts> UpdateQty(int grinParts)
        //{
        //    var data = await _tipsGrinDbContext.GrinParts.Where(x => x.Id == grinParts).FirstOrDefaultAsync();
        //    data.grin = grinParts;
        //    Update(data);
        //    return result;
        //}

        public async Task<int?> CreateUploadDocumentGrin(DocumentUpload documentUpload)
        {
            documentUpload.CreatedBy = "Admin";
            documentUpload.CreatedOn = DateTime.Now;
            documentUpload.LastModifiedBy = "Admin";
            documentUpload.LastModifiedOn = DateTime.Now;

            var result = await Create(documentUpload);
            return result.Id;
        }
    }
    public class GrinPartsRepository : RepositoryBase<GrinParts>, IGrinPartsRepository
    {
        private TipsGrinDbContext _tipsGrinDbContexts;
        public GrinPartsRepository(TipsGrinDbContext tipsGrinDbContext) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContexts = tipsGrinDbContext;
        }
         
        public async Task<GrinParts> UpdateGrinPartsQty(int GrinPartId, string AcceptedQty, string RejectedQty)
        {
            var data = await _tipsGrinDbContexts.GrinParts.Where(x => x.Id == GrinPartId).FirstOrDefaultAsync();
            data.AcceptedQty = Convert.ToDecimal(AcceptedQty);
            data.RejectedQty = Convert.ToDecimal(RejectedQty);             
            return data; 
        }

        public async Task<string> UpdateGrinQty(GrinParts grinParts)
        {
            grinParts.LastModifiedBy = "Admin";
            grinParts.LastModifiedOn = DateTime.Now;
            Update(grinParts);
            string result = $"Grin Detail {grinParts.Id} is updated successfully!";
            return result;
        }

    }
}
