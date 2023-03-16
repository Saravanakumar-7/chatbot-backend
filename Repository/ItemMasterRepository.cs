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
using Microsoft.AspNetCore.Mvc;
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

        public async Task<PagedList<ItemMaster>> GetAllActiveItemMasters([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var itemmasterDetails = FindAll()
                             .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
                                inv.Description.Contains(searchParams.SearchValue) || inv.ItemType.Equals(int.Parse(searchParams.SearchValue)) || inv.Commodity.Contains(searchParams.SearchValue)
                                || inv.MaterialGroup.Contains(searchParams.SearchValue))))
                               .Include(t => t.ItemmasterAlternate)
                         .Include(t => t.ItemMasterApprovedVendor)
                         .Include(t => t.ItemMasterFileUpload)
                          .Include(d => d.ItemMasterRouting)
                          .Include(d => d.ItemMasterWarehouse);

            return PagedList<ItemMaster>.ToPagedList(itemmasterDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<ItemMaster>> GetAllItemMasters([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var itemmasterDetails = FindAll().OrderByDescending(x => x.Id)
                 .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
             inv.Description.Contains(searchParams.SearchValue) || inv.ItemType.Equals(int.Parse(searchParams.SearchValue)) || inv.Commodity.Contains(searchParams.SearchValue) || inv.MaterialGroup.Contains(searchParams.SearchValue))))
                          .Include(t => t.ItemmasterAlternate)
                         .Include(t => t.ItemMasterApprovedVendor)
                         .Include(t => t.ItemMasterFileUpload)
                          .Include(d => d.ItemMasterRouting)
                          .Include(d => d.ItemMasterWarehouse);

            return PagedList<ItemMaster>.ToPagedList(itemmasterDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        


        public async Task<PagedList<ItemMaster>> GetAllFGItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
             

             var itemmasterFgDetails = FindAll().OrderByDescending(a => a.ItemType == PartType.FG)
                  .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
              inv.Description.Contains(searchParams.SearchValue) || inv.ItemType.Equals(int.Parse(searchParams.SearchValue)))))
                           .Include(t => t.ItemmasterAlternate)
                          .Include(t => t.ItemMasterApprovedVendor)
                          .Include(t => t.ItemMasterFileUpload)
                           .Include(d => d.ItemMasterRouting)
                           .Include(d => d.ItemMasterWarehouse);

             return PagedList<ItemMaster>.ToPagedList(itemmasterFgDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

         }



        public async Task<PagedList<ItemMaster>> GetAllSAPurchasePartItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            


            var itemmasterSADetails = FindAll().OrderByDescending(a => a.ItemType == PartType.SA || a.ItemType == PartType.PurchasePart)
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
            inv.Description.Contains(searchParams.SearchValue) || inv.ItemType.Equals(int.Parse(searchParams.SearchValue)))))
                         .Include(t => t.ItemmasterAlternate)
                        .Include(t => t.ItemMasterApprovedVendor)
                        .Include(t => t.ItemMasterFileUpload)
                         .Include(d => d.ItemMasterRouting)
                         .Include(d => d.ItemMasterWarehouse);

            return PagedList<ItemMaster>.ToPagedList(itemmasterSADetails, pagingParameter.PageNumber, pagingParameter.PageSize);




        }
        public async Task<PagedList<ItemMaster>> GetAllSAItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            

            var itemmasterSADetails = FindAll().OrderByDescending(a => a.ItemType == PartType.SA)
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
            inv.Description.Contains(searchParams.SearchValue) || inv.ItemType.Equals(int.Parse(searchParams.SearchValue)))))
                         .Include(t => t.ItemmasterAlternate)
                        .Include(t => t.ItemMasterApprovedVendor)
                        .Include(t => t.ItemMasterFileUpload)
                         .Include(d => d.ItemMasterRouting)
                         .Include(d => d.ItemMasterWarehouse);

            return PagedList<ItemMaster>.ToPagedList(itemmasterSADetails, pagingParameter.PageNumber, pagingParameter.PageSize);

        }
        public async Task<PagedList<ItemMaster>> GetAllFgSaItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        { 

            var itemmasterFgSADetails = FindAll().OrderByDescending(a => a.ItemType == PartType.SA || a.ItemType == PartType.FG)
               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
           inv.Description.Contains(searchParams.SearchValue) || inv.ItemType.Equals(int.Parse(searchParams.SearchValue)))))
                         .Include(c => c.FileUpload)
                            .Include(x => x.ImageUpload)
                            .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse);

            return PagedList<ItemMaster>.ToPagedList(itemmasterFgSADetails, pagingParameter.PageNumber, pagingParameter.PageSize);

        }

        //sa,fg, and fru

        public async Task<PagedList<ItemMaster>> GetAllFgSaFruItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {


            var itemmasterFgSaFRUDetails = FindAll().OrderByDescending(a => a.ItemType == PartType.SA || a.ItemType == PartType.FG || a.ItemType == PartType.FRU)
               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ItemNumber.Contains(searchParams.SearchValue) ||
           inv.Description.Contains(searchParams.SearchValue) || inv.ItemType.Equals(int.Parse(searchParams.SearchValue)))))
                         .Include(c => c.FileUpload)
                            .Include(x => x.ImageUpload)
                            .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse);

            return PagedList<ItemMaster>.ToPagedList(itemmasterFgSaFRUDetails, pagingParameter.PageNumber, pagingParameter.PageSize);


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
