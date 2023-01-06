using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Purchase.Api.Entities
{
    public class PurchaseRequisition
    {
        public int Id { get; set; }

        public string? PRNumber { get; set; }

        public DateTime? PRDate { get; set; }

        public int? RevisionNumber { get; set; }

        public string? ProcurementType { get; set; }

        public string? Purpose { get; set; }

        public string? PRFiles { get; set; }

        public string? DeliveryTerms { get; set; }

        public string? PaymentTerms { get; set; }

        public string? ShippingMode { get; set; }

        public string? ShipTo { get; set; }

        public string? BillTo { get; set; }

        public string? RetentionPeriod { get; set; }

        public string? SpecialTermsConditions { get; set; }

        public bool PRApprovalI { get; set; } = false;
        public string? PRApprovedIBy { get; set; }
        public DateTime PRApprovedIDate { get; set; }

        public bool PRApprovalII { get; set; } = false;
        public string? PRApprovedIIBy { get; set; }
        public DateTime PRApprovedIIDate { get; set; }

        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PrItem>? PrItemList { get; set; }     

    }
}
