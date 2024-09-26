using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class CustomerOtherUploadsDto
    {
        public int Id { get; set; }
        public string? Incorporation { get; set; }
        public string? TIN { get; set; }
        public string? GST { get; set; }
        public string? IEC { get; set; }
        public string? PAN { get; set; }
        public string? Udhyam_Certificate { get; set; }
        public string? MSME { get; set; }
        public string? Cancelled_Cheque { get; set; }
        public string? Other { get; set; }
        public int CustomerId { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class CustomerOtherUploadsPostDto
    {
        public string? Incorporation { get; set; }
        public string? TIN { get; set; }
        public string? GST { get; set; }
        public string? IEC { get; set; }
        public string? PAN { get; set; }
        public string? Udhyam_Certificate { get; set; }
        public string? MSME { get; set; }
        public string? Cancelled_Cheque { get; set; }
        public string? Other { get; set; }
        public int CustomerId { get; set; }
    }
    public class CustomerOtherUploadsUpdateDto
    {
        public int Id { get; set; }
        public string? Incorporation { get; set; }
        public string? TIN { get; set; }
        public string? GST { get; set; }
        public string? IEC { get; set; }
        public string? PAN { get; set; }
        public string? Udhyam_Certificate { get; set; }
        public string? MSME { get; set; }
        public string? Cancelled_Cheque { get; set; }
        public string? Other { get; set; }
        public int CustomerId { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
