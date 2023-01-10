using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities.DTOs
{
    public class MaterialReturnNoteItemDto
    {

        public int? Id { get; set; }
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? PartType { get; set; }

        public string? Warehouse { get; set; }

        public string? Location { get; set; }

        [Precision(18, 3)]

        public decimal? ReturnQty { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class MaterialReturnNoteItemDtoPost
    {
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? PartType { get; set; }

        public string? Warehouse { get; set; }

        public string? Location { get; set; }

        [Precision(18, 3)]

        public decimal? ReturnQty { get; set; }
    }

    public class MaterialReturnNoteItemDtoUpdate
    {
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? PartType { get; set; }

        public string? Warehouse { get; set; }

        public string? Location { get; set; }

        [Precision(18, 3)]

        public decimal? ReturnQty { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

}
