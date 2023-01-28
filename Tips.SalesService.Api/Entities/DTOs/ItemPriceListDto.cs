using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class ItemPriceListDto
    {
        public int? Id { get; set; }
        public string? ItemNUmber { get; set; }
        public string? Description { get; set; }

        [Precision(13, 3)]
        public decimal? Qty { get; set; }
        public string? UOC { get; set; }
        public decimal? LeastCost { get; set; }

        [Precision(18, 3)]
        public decimal? LeastCostPlus { get; set; }

        [Precision(18, 3)]
        public decimal? LeastCostminus { get; set; }


        [Precision(18, 3)]
        public decimal? DiscountPlus { get; set; }

        [Precision(18, 3)]
        public decimal? DiscountMinus { get; set; }

        [Precision(18, 3)]
        public decimal? Markup { get; set; }
        public string? PriceListName { get; set; }
        public DateTime? ValidThrough { get; set; }
        public bool? IsDiscountApplicable { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int? ReleaseLpId { get; set; }
        public ReleaseLp? ReleaseLp { get; set; }
    }

    public class ItemPriceListPostDto
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }

        [Precision(13, 3)]
        public decimal? Qty { get; set; }
        public string? UOC { get; set; }
        public decimal? LeastCost { get; set; }

        [Precision(18, 3)]
        public decimal? LeastCostPlus { get; set; }

        [Precision(18, 3)]
        public decimal? LeastCostminus { get; set; }


        [Precision(18, 3)]
        public decimal? DiscountPlus { get; set; }

        [Precision(18, 3)]
        public decimal? DiscountMinus { get; set; }

        [Precision(18, 3)]
        public decimal? Markup { get; set; }
        public string? PriceListName { get; set; }
        public DateTime? ValidThrough { get; set; }
        public bool? IsDiscountApplicable { get; set; }
    }
    public class ItemPriceListUpdateDto
    {
        public int? Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }

        [Precision(13, 3)]
        public decimal? Qty { get; set; }
        public string? UOC { get; set; }
        public decimal? LeastCost { get; set; }

        [Precision(18, 3)]
        public decimal? LeastCostPlus { get; set; }

        [Precision(18, 3)]
        public decimal? LeastCostminus { get; set; }


        [Precision(18, 3)]
        public decimal? DiscountPlus { get; set; }

        [Precision(18, 3)]
        public decimal? DiscountMinus { get; set; }

        [Precision(18, 3)]
        public decimal? Markup { get; set; }      

        public string? PriceListName { get; set; }
        public DateTime? ValidThrough { get; set; }
        public bool? IsDiscountApplicable { get; set; }


    }
}
