using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class PurchaseRequistion
    {
        [Key]
        public int Id { get; set; }

        public string? PRNumber { get; set; }

        public DateTime? PRDate { get; set; }

        public int? RevisionNumber { get; set; }

        public string? ProcurementType { get; set; }

        public string? Purpose { get; set; }

        public string? PRFiles { get; set; }

        public string? ItemMaster { get; set; }

        public string? MftrItemNumber { get; set; }

        public string? Description { get; set; }

        public string? UOM { get; set; }

        public int? Quantity { get; set; }

        public string? SpecialInstruction { get; set; }

        public string? DeliveryTerms { get; set; }

        public string? PaymentTerms { get; set; }
        public string? ShippingMode { get; set; }

        public string? ShipTo { get; set; }

        public string? BillTo { get; set; }

        public string? RetentionPeriod { get; set; }

        public string? SpecialTermsConditions { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PRAddProject>? PRAddProjects { get; set; }



    }
}
