using Entities.Enums;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class ItemNoWithPartTypeDto
    { 
            public string ItemNumber { get; set; }

            public PartType PartType { get; set; }
            public string MftrItemNumber { get; set; }
            public string? MaterialGroup { get; set; }

    }
    public class ItemNoWithPartTypeAndMinDto
    {
        public string ItemNumber { get; set; }

        public PartType PartType { get; set; }
        public decimal Min { get; set; }
        public string UOM { get; set; }

    }
}
