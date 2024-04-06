using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class QuoteGeneralDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
         public string? PriceList { get; set; }
        public string? CustomerItemNumber { get; set; }
        [Precision(13, 3)]
        public decimal? Qty { get; set; }

        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }
        public decimal? AvailableStock { get; set; }
        public string? DiscountType { get; set; }

        [Precision(18, 3)]
        public decimal? DiscountAmount { get; set; }

        [Precision(18, 3)]
        public decimal? DiscountedUnitPrice { get; set; }
        public string? SpecialDiscountType { get; set; }

        [Precision(18, 3)]
        public decimal? SpecialDiscountAmount { get; set; }

        [Precision(18, 3)]
        public decimal? TotalDiscountedUnitPrice { get; set; }

        [Precision(18, 3)]
        public decimal? BasicAmount { get; set; }

        [Precision(18, 3)]
        public decimal? HSNNo { get; set; }

        [Precision(18, 3)]
        public decimal? IGST { get; set; }

        [Precision(18, 3)]
        public decimal? CGST { get; set; }

        [Precision(18, 3)]
        public decimal? UTGST { get; set; }

        [Precision(18, 3)]
        public decimal? SGST { get; set; }

        [Precision(18, 3)]
        public decimal? Total { get; set; }
        public string? RoomName { get; set; }
        public string? CustomFields { get; set; }


    }
    public class QuoteGeneralPostDto
    {
      
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
         public string? PriceList { get; set; }
        public string? CustomerItemNumber { get; set; }
        [Precision(13, 3)]
        public decimal? Qty { get; set; }

        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }
        public string? DiscountType { get; set; }

        [Precision(18, 3)]
        public decimal? DiscountAmount { get; set; }

        [Precision(18, 3)]
        public decimal? DiscountedUnitPrice { get; set; }
        public string? SpecialDiscountType { get; set; }

        [Precision(18, 3)]
        public decimal? SpecialDiscountAmount { get; set; }

        [Precision(18, 3)]
        public decimal? TotalDiscountedUnitPrice { get; set; }

        [Precision(18, 3)]
        public decimal? BasicAmount { get; set; }

        [Precision(18, 3)]
        public decimal? HSNNo { get; set; }

        [Precision(18, 3)]
        public decimal? IGST { get; set; }

        [Precision(18, 3)]
        public decimal? CGST { get; set; }

        [Precision(18, 3)]
        public decimal? UTGST { get; set; }

        [Precision(18, 3)]
        public decimal? SGST { get; set; }

        [Precision(18, 3)]
        public decimal? Total { get; set; }
        public string? RoomName { get; set; }
        public string? CustomFields { get; set; }
     }
    public class QuoteGeneralUpdateDto
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
         public string? PriceList { get; set; }
        public string? CustomerItemNumber { get; set; }
        [Precision(13, 3)]
        public decimal? Qty { get; set; }

        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }
        public string? DiscountType { get; set; }

        [Precision(18, 3)]
        public decimal? DiscountAmount { get; set; }

        [Precision(18, 3)]
        public decimal? DiscountedUnitPrice { get; set; }
        public string? SpecialDiscountType { get; set; }

        [Precision(18, 3)]
        public decimal? SpecialDiscountAmount { get; set; }

        [Precision(18, 3)]
        public decimal? TotalDiscountedUnitPrice { get; set; }

        [Precision(18, 3)]
        public decimal? BasicAmount { get; set; }

        [Precision(18, 3)]
        public decimal? HSNNo { get; set; }

        [Precision(18, 3)]
        public decimal? IGST { get; set; }

        [Precision(18, 3)]
        public decimal? CGST { get; set; }

        [Precision(18, 3)]
        public decimal? UTGST { get; set; }

        [Precision(18, 3)]
        public decimal? SGST { get; set; }

        [Precision(18, 3)]
        public decimal? Total { get; set; }
        public string? RoomName { get; set; }
        public string? CustomFields { get; set; }

    }
}
