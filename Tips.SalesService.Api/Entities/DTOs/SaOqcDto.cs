using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class SaOqcDto
    {

        public int? Id { get; set; }
        public string? SAShopOrderNo { get; set; }

        public string? ProjectNo { get; set; }

        public string? SAItemNumber { get; set; }
        [Precision(18,3)]
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

    public class SaOqcDtoPost
    {
        
       
        public string? SAShopOrderNo { get; set; }

        public string? ProjectNo { get; set; }

        public string? SAItemNumber { get; set; }

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
    public class SaOqcDtoUpdate
    {
        public int? Id { get; set; }
        
        public string? SAShopOrderNo { get; set; }

        public string? ProjectNo { get; set; }

        public string? SAItemNumber { get; set; }

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
