using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ItemMasterRepository : RepositoryBase<ItemMaster>, IItemMasterRepository
    {
        public ItemMasterRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<long> CreateItemMaster(ItemMaster itemMaster)
        {
            itemMaster.CreatedBy = "Admin";
            itemMaster.CreatedOn = DateTime.Now;
            itemMaster.Unit = "Bangalore";
            var result = await Create(itemMaster);

            return result.Id;
        }

        public async Task<string> DeleteItemMaster(ItemMaster itemMaster)
        {
            Delete(itemMaster);
            string result = $"ItemMaster details of {itemMaster.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<ItemMaster>> GetAllActiveItemMasters()
        {
            var getAllActiveItemMasters = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return getAllActiveItemMasters;
        }

        public async Task<PagedList<ItemMaster>> GetAllItemMasters(PagingParameter pagingParameter)
        {
            var getAllItemMasters = PagedList<ItemMaster>.ToPagedList(FindAll()
                .Include(c=>c.FileUpload)
                .Include(x=>x.ImageUpload)
                                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .OrderByDescending(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllItemMasters;
        }



        public async Task<IEnumerable<ItemMaster>> GetAllFGItems()
        {
            var getAllFGItems = await FindAll().Where(a => a.ItemType == PartType.FG)
                 .Include(c => c.FileUpload)
                .Include(x => x.ImageUpload)
                                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .ToListAsync();

            return getAllFGItems;
        }
        public async Task<IEnumerable<ItemMaster>> GetAllSAPurchasePartItems()
        {
            var getAllFGSAItems = await FindAll().Where(a => a.ItemType == PartType.SA || a.ItemType == PartType.PurchasePart)
                 .Include(c => c.FileUpload)
                .Include(x => x.ImageUpload)
                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .ToListAsync();

            return getAllFGSAItems;
        }
        public async Task<IEnumerable<ItemMaster>> GetAllSAItems()
        {
            var getAllSAItems = await FindAll().Where(a => a.ItemType == PartType.SA)
                 .Include(c => c.FileUpload)
                .Include(x => x.ImageUpload)
                                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .ToListAsync();

            return getAllSAItems;
        }
        public async Task<IEnumerable<ItemMaster>> GetAllFgSaItems()
        {
            var getAllFGSAItems = await FindAll().Where(a => a.ItemType == PartType.SA || a.ItemType == PartType.FG)
                 .Include(c => c.FileUpload)
                .Include(x => x.ImageUpload)
                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .ToListAsync();

            return getAllFGSAItems;
        }

        //sa,fg, and fru

        public async Task<IEnumerable<ItemMaster>> GetAllFgSaFruItems()
        {
            var getAllFGSAItems = await FindAll().Where(a => a.ItemType == PartType.SA || a.ItemType == PartType.FG|| a.ItemType == PartType.FRU)
                 .Include(c => c.FileUpload)
                .Include(x => x.ImageUpload)
                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .ToListAsync();

            return getAllFGSAItems;
        }

        public async Task<ItemMaster> GetItemMasterById(int id)
        {
            var getItemMasterById = await TipsMasterDbContext.ItemMasters
                            .Where(x => x.Id == id)
                            .Include(c=>c.FileUpload)
                            .Include(b=>b.ImageUpload)
                            .Include(t => t.ItemmasterAlternate)
                            .Include(x => x.ItemMasterApprovedVendor)
                            .Include(m => m.ItemMasterFileUpload)
                            .Include(s => s.ItemMasterRouting)
                            .Include(p => p.ItemMasterWarehouse)
                            .FirstOrDefaultAsync();


            return getItemMasterById;
        }

        public async Task<string> UpdateItemMaster(ItemMaster itemMaster)
        {
            itemMaster.LastModifiedBy = "Admin";
            itemMaster.LastModifiedOn = DateTime.Now;
            Update(itemMaster);
            string result = $"ItemMaster details of {itemMaster.Id} is updated successfully!";
            return result;
        }
        public async Task<IEnumerable<ItemMasterIdNoListDto>> GetAllActiveItemMasterIdNoList()
        {
            IEnumerable<ItemMasterIdNoListDto> getAllActiveItemMasterIdNoListDto = await TipsMasterDbContext.ItemMasters
                                .Select(c => new ItemMasterIdNoListDto()
                                {
                                    id = c.Id,
                                    ItemNumber = c.ItemNumber,
                                    Description = c.Description,

                                })
                              .ToListAsync();

            return getAllActiveItemMasterIdNoListDto;
        }
        public async Task<ItemMaster> GetItemMasterByItemNumber(string ItemNumber)
        {
            var getItemMasterByItemNo = await FindByCondition(x => x.ItemNumber == ItemNumber)
                 .Include(c => c.FileUpload)
                .Include(x => x.ImageUpload)
                 .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(p => p.ItemMasterWarehouse)

                             .FirstOrDefaultAsync();
            return getItemMasterByItemNo;
        }

    }

    public class FileUploadDocumentRepository : RepositoryBase<FileUpload>, IFileUploadRepository
    {
         public FileUploadDocumentRepository(TipsMasterDbContext tipsMasterDbContext) : base(tipsMasterDbContext)
        {

        } 
        public async Task<int?> CreateFileUploadDocument(FileUpload fileUpload)
        {
            fileUpload.CreatedBy = "Admin";
            fileUpload.CreatedOn = DateTime.Now;
            fileUpload.LastModifiedBy = "Admin";
            fileUpload.LastModifiedOn = DateTime.Now;
            var result = await Create(fileUpload);
            return result.Id;
        }

        public async Task<int?> CreateImageUploadDocument(FileUpload fileUpload)
        {
            fileUpload.CreatedBy = "Admin";
            fileUpload.CreatedOn = DateTime.Now;
            fileUpload.LastModifiedBy = "Admin";
            fileUpload.LastModifiedOn = DateTime.Now;
            var result = await Create(fileUpload);
            return result.Id;
        }

    }

    public class ImageUploadDocumentRepository : RepositoryBase<ImageUpload>, IImageUploadRepository
    {
        public ImageUploadDocumentRepository(TipsMasterDbContext tipsMasterDbContext) : base(tipsMasterDbContext)
        {

        } 

        public async Task<int?> ImageUploadDocument(ImageUpload imageUpload)
        {
            imageUpload.CreatedBy = "Admin";
            imageUpload.CreatedOn = DateTime.Now;
            imageUpload.LastModifiedBy = "Admin";
            imageUpload.LastModifiedOn = DateTime.Now;
            var result = await Create(imageUpload);
            return result.Id;
        }

    }

}
