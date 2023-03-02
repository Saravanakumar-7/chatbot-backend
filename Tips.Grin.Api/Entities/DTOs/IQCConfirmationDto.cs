using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class IQCConfirmationDto
    {
        public int Id { get; set; }

        public string? GrinNumber { get; set; }

        public int GrinId { get; set; }

        public string VendorName { get; set; }

        public string VendorId { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public decimal? InvoiceValue { get; set; }

        public string InvoiceNumber { get; set; }

        public string? AWBNumber1 { get; set; }
        public string? AWBNumber2 { get; set; }
        public string? BENumber { get; set; }
        public decimal? TotalInvoiceValue { get; set; }
        //public string? GrinDocuments { get; set; }
        public DateTime? AWBDate1 { get; set; }
        public DateTime? AWBDate2 { get; set; }
        public DateTime? BEDate { get; set; }
        public string Unit { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<IQCConfirmationItemsDto>? IQCConfirmationItems { get; set; }

    }
    public class IQCConfirmationPostDto
    {
        public string GrinNumber { get; set; }
        public int GrinId { get; set; } 

        public List<IQCConfirmationItemsPostDto>? IQCConfirmationItemsPostDtos { get; set; }

    }

    public class IQCConfirmationUpdateDto
    {

        public int Id { get; set; }
        public string GrinNumber { get; set; }
        public int GrinId { get; set; }
        public string Unit { get; set; } 
        public List<IQCConfirmationItemsUpdateDto>? IQCConfirmationItemsUpdateDtos { get; set; }

    }

}
