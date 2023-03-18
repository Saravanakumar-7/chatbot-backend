using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Tips.Production.Api.Entities.Enums;
using Entities.Enums;

namespace Tips.Production.Api.Entities
{
    public class MaterialReturnNoteItem
    {
        public int? Id { get; set; }
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNumber { get; set; }
        public PartType PartType { get; set; }   
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Open;

        [Precision(13,3)]
        public decimal ReturnQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int? MaterialReturnNoteId { get; set; }
        public MaterialReturnNote? MaterialReturnNote { get; set; }
    }
}
