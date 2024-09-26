using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Entities.Enums;

namespace Tips.Production.Api.Entities.DTOs
{
    public class MaterialReturnNoteItemDto
    {
        public int? Id { get; set; }
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNumber { get; set; }
        public PartType PartType { get; set; }
        public decimal? ReturnQty { get; set; }
        public MaterialStatus MrnStatus { get; set; }
        public decimal? ScrapQty { get; set; }
        public string? Remarks { get; set; }
        public List<MRNWarehouseDetailsDto> MRNWarehouseList { get; set; }

    }

    public class MaterialReturnNoteItemPostDto
    {
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public PartType PartType { get; set; }
        [Precision(13, 3)]
        public decimal ReturnQty { get; set; }
        public decimal? ScrapQty { get; set; }
        public string? Remarks { get; set; }
        public List<MRNWarehouseDetailsPostDto> MRNWarehouseList { get; set; }

    }

    public class MaterialReturnNoteItemUpdateDto
    {
        //public int Id { get; set; }
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public PartType PartType { get; set; }
        [Precision(13, 3)]
        public decimal ReturnQty { get; set; }
        public MaterialStatus MrnStatus { get; set; }
        public decimal? ScrapQty { get; set; }
        public string? Remarks { get; set; }
        public List<MRNWarehouseDetailsUpdateDto> MRNWarehouseList { get; set; }

    }
   
}