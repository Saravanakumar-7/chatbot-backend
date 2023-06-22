using Microsoft.EntityFrameworkCore;
using Entities.Enums;

namespace Tips.Production.Api.Entities.DTOs
{
    public class MaterialRequestItemsDto
    {
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        public string MRNumber { get; set; }
        public string? PartDescription { get; set; }
        //public string? MftrPartNumber { get; set; }
        public PartType PartType { get; set; }
        public string? Stock { get; set; }

        [Precision(13, 3)]
        public decimal? IssuedQty { get; set; }

        public bool IssueStatus { get; set; }

        [Precision(13, 3)]
        public decimal? RequiredQty { get; set; }

        public List<MRStockDetailsDto> MRStockDetails { get; set; }



    }
    public class MaterialRequestItemPostDto
    {

        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        //public string MRNumber { get; set; }
        //public string? MftrPartNumber { get; set; }
        public PartType PartType { get; set; } 

        [Precision(13, 3)]
        public decimal? IssuedQty { get; set; }
        public string? Stock { get; set; }
        

        [Precision(13, 3)]
        public decimal? RequiredQty { get; set; }

        public List<MRStockDetailsPostDto> MRStockDetails { get; set; }


    }
    public class MaterialRequestItemUpdateDto
    {

        public string? PartNumber { get; set; }
        public string MRNumber { get; set; }
        public string? PartDescription { get; set; }

        [Precision(13, 3)]
        public decimal? IssuedQty { get; set; }
        //public string? MftrPartNumber { get; set; }
        public PartType PartType { get; set; }
        public string? Stock { get; set; }
        

        [Precision(13, 3)]
        public decimal? RequiredQty { get; set; }

        public List<MRStockDetailsUpdateDto> MRStockDetails { get; set; }


    }
//    public class UpdateInventoryBalanceQty
//    {
//     public string? PartNumber { get; set; } 
//    public List<InventoryUpdateDtoForMRWarehouse> MRStockDetails { get; set; }
//}
//public class InventoryUpdateDtoForMRWarehouse
//{
//        public string? Warehouse { get; set; }
//        public string? Location { get; set; }

//        [Precision(13, 3)]
//        public decimal LocationStock { get; set; }
//    }


    public class UpdateInventoryBalanceQty
    {
        public string? PartNumber { get; set; }
        public List<InventoryUpdateDtoForMRWarehouse> MRNWarehouseList { get; set; }
    }

    public class InventoryUpdateDtoForMRWarehouse
    {
        public string? Warehouse { get; set; }
        public string? Location { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }

        [Precision(13, 3)]
        public decimal LocationStock { get; set; }
    }


}
