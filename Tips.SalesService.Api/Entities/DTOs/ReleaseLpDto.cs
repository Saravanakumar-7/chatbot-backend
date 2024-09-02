using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class ReleaseLpDto
    {
        public int? Id { get; set; }
        public string? RfqNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? Rev { get; set; }
        public DateTime? DateOnLpCreation { get; set; }
        public string? ItemNumber { get; set; }
        public string? RLpItemNo { get; set; }
        public string? Description { get; set; }
        public decimal? Qty { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal? LeastCost { get; set; }

        [Precision(13, 3)]
        public decimal? LeastCostPlus { get; set; }

        [Precision(13, 3)]
        public decimal? LeastCostminus { get; set; }

        [Precision(13, 3)]
        public decimal? DiscountPlus { get; set; }

        [Precision(13, 3)]
        public decimal? DiscountMinus { get; set; }

        [Precision(18, 3)]
        public decimal? Markup { get; set; }
        public string? PriceListName { get; set; }
        public DateTime? ValidThrough { get; set; }
        public bool? IsDiscountApplicable { get; set; }
        public decimal? RevisionNumber { get; set; }
        public string? Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }

    public class ReleaseLpDtoPost
    {

        public string? RfqNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? Rev { get; set; }
        public DateTime? DateOnLpCreation { get; set; }
        public string? ItemNumber { get; set; }
        public string? RLpItemNo { get; set; }
        public string? Description { get; set; }
        public decimal? RevisionNumber { get; set; }
        [Precision(18, 3)]
        public decimal? Qty { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal? LeastCost { get; set; }

        [Precision(13, 3)]
        public decimal? LeastCostPlus { get; set; }

        [Precision(13, 3)]
        public decimal? LeastCostminus { get; set; }

        public decimal? Markup { get; set; }
        [Precision(18, 3)]
        public string? PriceListName { get; set; }
        public DateTime? ValidThrough { get; set; }
        public bool? IsDiscountApplicable { get; set; }
        [Precision(13, 3)]
        public decimal? DiscountPlus { get; set; }

        [Precision(13, 3)]
        public decimal? DiscountMinus { get; set; }
     
    }

    public class ReleaseLpDtoUpdate
    {
        public int? Id { get; set; }
        public string? RfqNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? Rev { get; set; }
        public DateTime? DateOnLpCreation { get; set; }
        public string? ItemNumber { get; set; }
        public string? RLpItemNo { get; set; }
        public string? Description { get; set; }

        [Precision(18, 3)]
        public decimal? Qty { get; set; }

        public string? UOC { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal? LeastCost { get; set; }

        [Precision(13, 3)]
        public decimal? LeastCostPlus { get; set; }

        [Precision(13, 3)]
        public decimal? LeastCostminus { get; set; }

        public decimal? Markup { get; set; }
        [Precision(18, 3)]
        public string? PriceListName { get; set; }
        public DateTime? ValidThrough { get; set; }
        public bool? IsDiscountApplicable { get; set; }
        [Precision(13, 3)]
        public decimal? DiscountPlus { get; set; }

        [Precision(13, 3)]
        public decimal? DiscountMinus { get; set; }
        public string Unit { get; set; }

    }
}