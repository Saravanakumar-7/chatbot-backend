using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Entities
{
    public class SaOqc
    {
        public int? Id { get; set; }
        public string? SAShopOrderNo { get; set; }

        public string? ProjectNo { get; set; }

        public string? FGItemNumber { get; set; }

        [Precision(18, 3)]
        public decimal? SAShopOrderQty { get; set; }
        [Precision(18, 3)]
        public decimal? SAPendingQty { get; set; }
        [Precision(18, 3)]
        public decimal? SAAcceptedQty { get; set; }
        [Precision(18, 3)]
        public decimal? SARejectedQty { get; set; }

        public string Unit { get; set; }


        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
