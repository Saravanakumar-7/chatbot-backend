using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class Inventory_TransactionDto
    {
        public int Id { get; set; }

        [Display(Name = "Part Number")]
        public string? PartNumber { get; set; }

        [Display(Name = "Mftr Part Number")]
        public string? MftrPartNumber { get; set; }

        [Display(Name = "Description")]
        [StringLength(250, ErrorMessage = "String length exceeded")]
        public string? Description { get; set; }

        [Display(Name = "Project Number")]
        public string? ProjectNumber { get; set; }

        [Display(Name = "Issued Quantity")]
        [Precision(13, 3)]
        public decimal? Issued_Quantity { get; set; }

        [Display(Name = "UOM")]
        public string? UOM { get; set; }

        [DataType(DataType.DateTime), Display(Name = "Issued DateTime")]
        public DateTime Issued_DateTime { get; set; }

        [StringLength(100), Display(Name = " Issued By")]
        public string? Issued_By { get; set; }
        public string? ShopOrderId { get; set; }
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }

        [Display(Name = "BOM Version No")]
        public decimal BOM_Version_No { get; set; }

        [Display(Name = "From Location")]
        [StringLength(100, ErrorMessage = "String length exceeded")]
        public string? From_Location { get; set; }

        [Display(Name = "To Location")]
        [StringLength(100, ErrorMessage = "String length exceeded")]
        public string? TO_Location { get; set; }

        public bool ModifiedStatus { get; set; } = false;

        [Display(Name = "Unit Name")]
        public string Unit { get; set; }

        public string? GrinMaterialType { get; set; } = "Bought Out";

        public string? Remarks { get; set; }

        [StringLength(100), Display(Name = "Created By")]
        public string? CreatedBy { get; set; }


        [DataType(DataType.DateTime), Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }


        [StringLength(100), Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }


        [DataType(DataType.DateTime), Display(Name = "Last Modified On")]
        public DateTime? LastModifiedOn { get; set; }

    }
}
