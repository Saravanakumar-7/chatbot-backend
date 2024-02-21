using Entities;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IFileUploadRepository : IRepositoryBase<FileUpload>
    {
        Task<int?> CreateFileUploadDocument(FileUpload fileUpload);
        Task<int?> CreateImageUploadDocument(FileUpload fileUpload);
        Task<List<FileUploadDto>> GetDownloadUrlDetails(string FileIds);
        Task<FileUpload> GetFileUploadByIdAsync(int id);
        void Delete(FileUpload fileUpload);
    } 
}
