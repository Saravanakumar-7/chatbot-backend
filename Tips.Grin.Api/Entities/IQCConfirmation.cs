using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Grin.Api.Entities
{
    public class IQCConfirmation
    {
        [Key]
        public int Id { get; set; }

        public string? GrinNumber { get; set; }

        public string? VendorName { get; set; }

        public string? InvoiceNumber { get; set; }

        public string? ItemNumber { get; set; }

        public string? ProjectNumber { get; set; }

        public int GrinPartId { get; set; }

        public string? PONumber { get; set; }
        public string? AWBNumber1 { get; set; }
        public string? AWBNumber2 { get; set; }
        public string? BENumber { get; set; }
        public int? TotalInvoice { get; set; }
        public string? GrinDocuments { get; set; }
        public DateTime? AWBDate1 { get; set; }
        public DateTime? AWBDate2 { get; set; }

        public DateTime? BEDate { get; set; }

        [Precision(13, 3)]
        public decimal? AcceptedQty { get; set; }

        [Precision(13, 3)]
        public decimal? RejectedQty { get; set; }

        public bool IsDeleted { get; set; } = false;


        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<IQCConfirmationItems>? IQCConfirmationItems { get; set; }

       

        


    }
}
