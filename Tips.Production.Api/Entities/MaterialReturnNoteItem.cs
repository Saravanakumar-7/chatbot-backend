using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Tips.Production.Api.Entities
{
    public class MaterialReturnNoteItem
    {
        [Key]
        public int? Id { get; set; }
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? PartType { get; set; }
        
        public string? Warehouse { get; set; }

        public string? Location { get; set; }

        [Precision(18,3)]

        public decimal? ReturnQty { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int? MaterialRequestId { get; set; }
        public MaterialReturnNote? MaterialReturnNote { get; set; }
    }
}
