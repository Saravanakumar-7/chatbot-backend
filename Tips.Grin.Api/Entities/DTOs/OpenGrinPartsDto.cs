using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class OpenGrinPartsDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public bool Returnable { get; set; }
        public PartType ItemType { get; set; }
        public string? UOM { get; set; }
        [Precision(18, 3)]
        public decimal Qty { get; set; }
        public string? SerialNo { get; set; }
        public string? ReferenceSONumber { get; set; }
        public string? Remarks { get; set; }
        public List<OpenGrinDetailsDto> OpenGrinDetails { get; set; }
    }
        public class OpenGrinPartsPostDto
        {
            public string? ItemNumber { get; set; }
            public string? Description { get; set; }
            public bool Returnable { get; set; }
            public PartType ItemType { get; set; }
            public string? UOM { get; set; }
            [Precision(18, 3)]
            public decimal Qty { get; set; }
            public string? SerialNo { get; set; }
            public string? ReferenceSONumber { get; set; }
            public string? Remarks { get; set; }
            public List<OpenGrinDetailsPostDto> OpenGrinDetails { get; set; }
    }
     public class OpenGrinPartsUpdateDto
     {
            //public int Id { get; set; }
            public string? ItemNumber { get; set; }
            public string? Description { get; set; }
            public bool Returnable { get; set; }
            public PartType ItemType { get; set; }
            public string? UOM { get; set; }
            [Precision(18, 3)]
            public decimal Qty { get; set; }
            public string? SerialNo { get; set; }
            public string? ReferenceSONumber { get; set; }
            public string? Remarks { get; set; }
            public List<OpenGrinDetailsUpdateDto> OpenGrinDetails { get; set; }
    }

    public class OpenGrinPartSORefDto
    {
        public string? ReferenceSONumber { get; set; }
    }
}
