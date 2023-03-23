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
        public List<MRNWarehouseDetailsDto> MRNWarehouseList { get; set; }

    }

    public class MaterialReturnNoteItemPostDto
    {
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? PartType { get; set; }
        public List<MRNWarehouseDetailsPostDto> MRNWarehouseList { get; set; }

    }

    public class MaterialReturnNoteItemUpdateDto
    {
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? PartType { get; set; }
        public List<MRNWarehouseDetailsUpdateDto> MRNWarehouseList { get; set; }

    }

}
