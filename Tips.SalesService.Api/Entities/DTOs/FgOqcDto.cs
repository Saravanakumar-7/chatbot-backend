using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class FgOqcDto
    {
        public int? Id { get; set; }
        public string? ProjectNumber { get; set; }

        public string? FGItemNumber { get; set; }

        public string? ShopOrderNumber { get; set; }

        [Precision(18,3)]
        public decimal? ShopOrderQty { get; set; }
        [Precision(18, 3)]
        public decimal? PendingQty { get; set; }
        [Precision(18, 3)]
        public decimal? AcceptedQty { get; set; }
        [Precision(18, 3)]
        public decimal? RejectedQty { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class FgOqcDtoPost
    {

        public string? ProjectNumber { get; set; }

        public string? ItemNumber { get; set; }

        public string? ShopOrderNumber { get; set; }

        [Precision(18, 3)]
        public decimal? ShopOrderQty { get; set; }
        [Precision(18, 3)]
        public decimal? PendingQty { get; set; }
        [Precision(18, 3)]
        public decimal? AcceptedQty { get; set; }
        [Precision(18, 3)]
        public decimal? RejectedQty { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class FgOqcDtoUpdate
    {
        public int? Id { get; set; }
        
        [StringLength(100, ErrorMessage = "FgQoc can't be longer than 100 characters")]
        public string? ProjectNumber { get; set; }

        public string? ItemNumber { get; set; }

        public string? ShopOrderNumber { get; set; }

        [Precision(18, 3)]
        public decimal? ShopOrderQty { get; set; }
        [Precision(18, 3)]
        public decimal? PendingQty { get; set; }
        [Precision(18, 3)]
        public decimal? AcceptedQty { get; set; }
        [Precision(18, 3)]
        public decimal? RejectedQty { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
