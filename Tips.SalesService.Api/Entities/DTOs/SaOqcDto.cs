using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class SaOqcDto
    {

        public int? Id { get; set; }
        public string? ProjectNumber { get; set; }
        public string? FGItemNumber { get; set; }
        public string? ShopOrderNumber { get; set; }

        [Precision(18, 3)]
        public decimal? ShopOrderQty { get; set; }
        public string? SubAssemblyItemNumber { get; set; }
        public string? SAShopOrderNumber { get; }
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

    public class SaOqcDtoPost
    {


       
        public string? ProjectNumber { get; set; }
        public string? FGItemNumber { get; set; }
        public string? ShopOrderNumber { get; set; }

        [Precision(18, 3)]
        public decimal? ShopOrderQty { get; set; }
        public string? SubAssemblyItemNumber { get; set; }
        public string? SAShopOrderNumber { get; }
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
        public string? ProjectNumber { get; set; }
        public string? FGItemNumber { get; set; }
        public string? ShopOrderNumber { get; set; }

        [Precision(18, 3)]
        public decimal? ShopOrderQty { get; set; }
        public string? SubAssemblyItemNumber { get; set; }
        public string? SAShopOrderNumber { get; }
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
