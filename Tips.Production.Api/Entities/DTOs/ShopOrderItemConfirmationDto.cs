using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Production.Api.Entities.DTOs
{
    public class ShopOrderItemConfirmationDto
    {
        public string? PartNumber { get; set; }
        public string? ShopOrderNumber { get; set; }

        [Precision(13, 3)]
        public decimal NewConvertedToFgQty { get; set; }
        public string? DataFrom { get; set; } 
        public string? MRNumber { get; set; } 
    }
    
}
