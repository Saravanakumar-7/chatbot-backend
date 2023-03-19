using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Purchase.Api.Entities
{
    public class PurchaseRequisition
    {
        [Key]
        public int Id { get; set; }

        public string? PrNumber { get; set; }

        public DateTime? PrDate { get; set; }

        public int? RevisionNumber { get; set; }

        public string? ProcurementType { get; set; }

        public string? Purpose { get; set; }

        public List<DocumentUpload>? PrFiles { get; set; }

        public string? DeliveryTerms { get; set; }

        public string? PaymentTerms { get; set; }

        public string? ShippingMode { get; set; }
         

        public string? RetentionPeriod { get; set; }

        public string? SpecialTermsConditions { get; set; }

        [DefaultValue(0)]
        public Status Status { get; set; }

        [DefaultValue(false)]
        public bool IsShortClosed { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }

        public bool PrApprovalI { get; set; } = false;
        public string? PrApprovedIBy { get; set; }
        public DateTime PrApprovedIDate { get; set; }

        public bool PrApprovalII { get; set; } = false;
        public string? PrApprovedIIBy { get; set; }
        public DateTime PrApprovedIIDate { get; set; }



        public string? Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PrItem>? PrItemList { get; set; }     

    }
}
