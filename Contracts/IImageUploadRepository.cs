using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IImageUploadRepository : IRepositoryBase<ImageUpload>
    {
         Task<int?> ImageUploadDocument(ImageUpload imageUpload);
        Task<string?> GetImageFileByte(string filename);
        Task DeleteImage(int Id);
    }
}
