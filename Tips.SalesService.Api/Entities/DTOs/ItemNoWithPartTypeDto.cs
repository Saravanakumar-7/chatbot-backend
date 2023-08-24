using Entities.Enums;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class ItemNoWithPartTypeDto
    { 
            public string ItemNumber { get; set; }

            public PartType PartType { get; set; }
       
    }
}
