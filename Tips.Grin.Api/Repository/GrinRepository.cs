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

        /// <summary>
        /// This Method should be changed ,because it will create duplicate GrinNumber
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<int?> GetGrinNumberAutoIncrementCount(DateTime date)
        {
            var getGrinNumberAutoIncrementCount = _tipsGrinDbContext.Grins.Where(x => x.CreatedOn == date.Date).Count();

            return getGrinNumberAutoIncrementCount;
        }

        public async Task<IEnumerable<GetDownloadUrlDto>> GetDownloadUrlDetails(string grinNumber)
        {

            IEnumerable<GetDownloadUrlDto> getDownloadDetails = await _tipsGrinDbContext.DocumentUploads
                                .Where(b => b.ParentId == grinNumber)
                                .Select(x => new GetDownloadUrlDto()
                                {
                                    Id = x.Id,
                                    FileName = x.FileName,
                                    FileExtension = x.FileExtension,
                                    FilePath = x.FilePath
                                })
                              .ToListAsync();

            return getDownloadDetails;
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
            var getAllGrinDetails = FindAll().OrderByDescending(x=>x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.GrinNumber.Contains(searchParams.SearchValue) ||
                 inv.VendorId.Contains(searchParams.SearchValue) || inv.VendorName.Contains(searchParams.SearchValue))))
              .Include(t => t.GrinDocuments)
              .Include(t => t.GrinParts)
              //.ThenInclude(c=>c.CoCUpload)
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
                                    .Include(t => t.GrinDocuments)
              .Include(t => t.GrinParts)
              //.ThenInclude(c => c.CoCUpload)
              .Include(t => t.GrinParts)
               .ThenInclude(d => d.ProjectNumbers)
                               .FirstOrDefaultAsync();

            return grinDetailsbyId;
        }
        public async Task<Grins> GetGrinByGrinNo(string grinNumber)
        {
            var grinDetailsbyGrin = await _tipsGrinDbContext.Grins.Where(x => x.GrinNumber == grinNumber)
                                    
              .Include(t => t.GrinParts)
              
                               .FirstOrDefaultAsync();

            return grinDetailsbyGrin;
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
        public async Task<DocumentUpload> GetUploadDocById(int id)
        {
            var grinUploadDocFileNameById = await _tipsGrinDbContext.DocumentUploads
                .Where(x => x.Id == id).FirstOrDefaultAsync();

            return grinUploadDocFileNameById;
        }
        public async Task<string> DeleteUploadFile(DocumentUpload documentUpload)
        {
            Delete(documentUpload);
            string result = $"DocumentUpload details of {documentUpload.Id} is deleted successfully!";
            return result;
        } 

        public async Task<int?> CreateUploadDocumentGrin(DocumentUpload documentUpload)
        {
            documentUpload.CreatedBy = "Admin";
            documentUpload.CreatedOn = DateTime.Now;
            documentUpload.LastModifiedBy = "Admin";
            documentUpload.LastModifiedOn = DateTime.Now;

            var result = await Create(documentUpload);
            return result.Id;
        }
 

        public async Task<IEnumerable<GetDownloadUrlDto>> GetGrinDownloadUrlDetails(string grinNumber)
        { 

            IEnumerable<GetDownloadUrlDto> getDownloadDetails = await _tipsGrinDbContext.DocumentUploads
                                .Where(x =>x.ParentId == grinNumber)
                                .Select(x => new GetDownloadUrlDto()
                                {
                                    Id = x.Id,
                                    FileName = x.FileName,
                                    FileExtension = x.FileExtension,
                                    FilePath = x.FilePath
                                })
                              .ToListAsync();

            return getDownloadDetails;
        }

        public async Task<IEnumerable<GetDownloadUrlDto>> GetGrinPartsDownloadUrlDetails(string grinNumber)
        {
            var grinnumbers = grinNumber + "-" + "I";
            IEnumerable<GetDownloadUrlDto> getDownloadDetails = await _tipsGrinDbContext.DocumentUploads
                                .Where(x => x.ParentId == grinnumbers)
                                .Select(x => new GetDownloadUrlDto()
                                {
                                    Id = x.Id,
                                    FileName = x.FileName,
                                    FileExtension = x.FileExtension,
                                    FilePath = x.FilePath
                                })
                              .ToListAsync();

            return getDownloadDetails;
        }

        public async Task<string> DeleteGrinPartsUploadDocByGrinNo(string grinnumber)
        {
            var documentDetails = await _tipsGrinDbContext.DocumentUploads.Where(x => x.ParentId == grinnumber).FirstOrDefaultAsync();
            Delete(documentDetails);
            string result = $"DocumentUpload details of {documentDetails.Id} is deleted successfully!";
            return result;
        }
        public async Task<int?> GetDocumentDetailsByGrinNo(string grinnumber)
        {
            var grinUploadDocFileNameById =  _tipsGrinDbContext.DocumentUploads
               .Where(x => x.ParentId == grinnumber).Count();

            return grinUploadDocFileNameById;
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
        public async Task<string> DeleteGrinParts(GrinParts grinParts)
        {
            //var grinPartDetails = await _tipsGrinDbContexts.GrinParts
            //    .Include(t => t.ProjectNumbers)
            //    .Include(t => t.CoCUpload).FirstOrDefaultAsync();

            Delete(grinParts);
            string result = $"GrinParts details of {grinParts.Id} is deleted successfully!";
            return result;
        }
        public async Task<GrinParts> GetGrinPartsById(int id)
        {
            var grinPartsDetailsbyId = await _tipsGrinDbContexts.GrinParts.Where(x => x.Id == id)

               .Include(d => d.ProjectNumbers)
                               .FirstOrDefaultAsync();

            return grinPartsDetailsbyId;
        }

        public async Task<GrinParts> DeleteGrinPartsById(int id)
        {
            var grinPartsDetailsbyId = await _tipsGrinDbContexts.GrinParts.Where(x => x.Id == id)

               .Include(d => d.ProjectNumbers)
               .Include(d => d.CoCUpload)
                               .FirstOrDefaultAsync();

            return grinPartsDetailsbyId;
        }

        public async Task<GrinParts> GetGrinPartsDetailsbyGrinPartId(int GrinPartId)
        {
            var grinPartsDetails = await _tipsGrinDbContexts.GrinParts.Where(x => x.Id == GrinPartId).FirstOrDefaultAsync();
            return grinPartsDetails;
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
            string result = $"GrinParts Detail {grinParts.Id} is updated successfully!";
            return result;
        }
         

        //pass grinparts id and get the details

        //public async Task<string> GetGrinPartsDetailsByPartsId(GrinParts grinParts)
        //{
        //    grinParts.LastModifiedBy = "Admin";
        //    grinParts.LastModifiedOn = DateTime.Now;
        //    Update(grinParts);
        //    string result = $"Grin Detail {grinParts.Id} is updated successfully!";
        //    return result;
        //}

    }
}
