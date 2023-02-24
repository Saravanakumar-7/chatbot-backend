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
using Microsoft.AspNetCore.Mvc;

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
            var date = DateTime.Now;
            grins.CreatedBy = "Admin";
            grins.CreatedOn = date.Date;
            //Guid grinId = Guid.NewGuid();
            //grins.GrinNumber = "GR-" + grinId.ToString();
            grins.Unit = "Bangalore";       

            var result = await Create(grins);
            return result.Id;
        }

        public async Task<int?> GetGrinNumberAutoIncrementCount(DateTime date)
        {
            var getGrinNumberAutoIncrementCount = _tipsGrinDbContext.Grins.Where(x => x.CreatedOn == date.Date).Count();

            return getGrinNumberAutoIncrementCount;
        }


        public async Task<string> DeleteGrin(Grins grins)
        {
            Delete(grins);
            string result = $"Grin details of {grins.Id} is deleted successfully!";
            return result;
        } 

        public async Task<PagedList<Grins>> GetAllActiveGrin([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            var allActiveGrinDetails = FindAll()
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.GrinNumber.Contains(searchParams.SearchValue) ||
                   inv.VendorId.Contains(searchParams.SearchValue) || inv.VendorName.Contains(searchParams.SearchValue))))
                 .Include(t => t.GrinParts);
            return PagedList<Grins>.ToPagedList(allActiveGrinDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
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

        public async Task<PagedList<Grins>> GetAllGrin([FromQuery] PagingParameter pagingParameter,[FromQuery] SearchParams searchParams)
        {
            var getAllGrinDetails = FindAll()
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.GrinNumber.Contains(searchParams.SearchValue) ||
                 inv.VendorId.Contains(searchParams.SearchValue) || inv.VendorName.Contains(searchParams.SearchValue))))
               .Include(t => t.GrinParts)
               .ThenInclude(d => d.ProjectNumbers);

            return PagedList<Grins>.ToPagedList(getAllGrinDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        private void ThenInclude(Func<object, ProjectNumbers> value)
        {
            throw new NotImplementedException();
        }

        public async Task<Grins> GetGrinById(int id)
        {
            var grinDetailsbyId = await _tipsGrinDbContext.Grins.Where(x => x.Id == id)
                                //.Include(c=>c.GrinDocuments)
                                //.Include(m => m.GrinParts)
                                //.ThenInclude(n=>n.cocUpload)
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

        public async Task<PagedList<GrinParts>> GetAllGrinParts([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            var getAllGrinParts = FindAll()
                 .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
                 inv.ItemDescription.Contains(searchParams.SearchValue) || inv.PONumber.Contains(searchParams.SearchValue)
                 || inv.MftrItemNumber.Contains(searchParams.SearchValue) || inv.ManufactureBatchNumber.Contains(searchParams.SearchValue)
                 )))
                .Include(t => t.ProjectNumbers);

            return PagedList<GrinParts>.ToPagedList(getAllGrinParts, pagingParameter.PageNumber, pagingParameter.PageSize);
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
