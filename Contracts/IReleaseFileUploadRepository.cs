using Entities.DTOs;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{

    public interface IReleaseFileUploadRepository : IRepositoryBase<ReleaseFileUpload>
    {
        Task<int?> CreateReleaseFileUploadDocument(ReleaseFileUpload releaseFileUpload);
        //Task<int?> CreateImageUploadDocument(FileUpload fileUpload);
        Task<List<ReleaseFileUploadDto>> GetReleaseFileUploadDownloadUrlDetails(string FileIds);
        Task<ReleaseFileUpload> GetReleaseFileUploadByIdAsync(int id);
        void Delete(ReleaseFileUpload releaseFileUpload);
    }

}
