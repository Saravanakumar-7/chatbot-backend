using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities.DTOs
{
    public class MaterialReturnNoteDto
    {
        public int? Id { get; set; }

        public string? MRNNumber { get; set; }
        public string? ProjectNumber { get; set; }

        public string? ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<MaterialReturnNoteItemDto>? MaterialReturnNoteItems { get; set; }
    }

    public class MaterialReturnNoteDtoPost
    {
        public string? MRNNumber { get; set; }
        public string? ProjectNumber { get; set; }

        public string? ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }

        public List<MaterialReturnNoteItemDtoPost>? MaterialReturnNoteItems { get; set; }

    }

    public class MaterialReturnNoteDtoUpdate
    {
        public int? Id { get; set; }

        public string? MRNNumber { get; set; }
        public string? ProjectNumber { get; set; }

        public string? ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<MaterialReturnNoteItemDtoUpdate>? MaterialReturnNoteItems { get; set; }
    }


}
