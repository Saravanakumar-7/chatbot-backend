using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IItemMasterFileUpload
    {
        Task<IEnumerable<ItemMasterFileUpload>> GetAllItemMasterFileUploads();
        Task<ItemMasterFileUpload> GetItemMasterFileUploadById(int id);
        Task<IEnumerable<ItemMasterFileUpload>> GetAllActiveItemMasterFileUploads();
        Task<int?> CreateItemMasterFileUpload(ItemMasterFileUpload itemMasterFileUpload);
        Task<string> UpdateItemMasterFileUpload(ItemMasterFileUpload itemMasterFileUpload);
        Task<string> DeleteItemMasterFileUpload(ItemMasterFileUpload itemMasterFileUpload);
    }
}
