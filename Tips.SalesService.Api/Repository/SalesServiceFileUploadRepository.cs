using Entities;
using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Repository
{
    public class SalesServiceFileUploadRepository : RepositoryBase<SalesServiceFileUpload>, ISalesServiceFileUploadRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public SalesServiceFileUploadRepository(TipsSalesServiceDbContext tipsMasterDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsMasterDbContext)
        {
            _tipsSalesServiceDbContext = tipsMasterDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int?> CreateSalesServiceFileUploadDocument(SalesServiceFileUpload fileUpload)
        {
            fileUpload.CreatedBy = _createdBy;
            fileUpload.CreatedOn = DateTime.Now;
            var result = await Create(fileUpload);
            return result.Id;
        }
        public async Task<List<SalesServiceFileUploadDto>> GetDownloadUrlDetails(string FileIds)
        {
            List<SalesServiceFileUploadDto> fileUploads = new List<SalesServiceFileUploadDto>();
            if (FileIds != null)
            {
                string[]? ids = FileIds.Split(',');

                for (int i = 0; i < ids.Count(); i++)
                {
                    SalesServiceFileUploadDto getDownloadDetails = await _tipsSalesServiceDbContext.SalesServiceFileUpload
                                .Where(b => b.Id == Convert.ToInt32(ids[i]))
                                .Select(x => new SalesServiceFileUploadDto()
                                {
                                    Id = x.Id,
                                    FileName = x.FileName,
                                    FileExtension = x.FileExtension,
                                    FilePath = x.FilePath,
                                    FileByte = x.FileByte
                                }).FirstOrDefaultAsync();
                    if (getDownloadDetails != null)
                        fileUploads.Add(getDownloadDetails);
                }
            }
            return fileUploads;
        }
    }
}
