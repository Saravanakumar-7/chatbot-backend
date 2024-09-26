using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Production.Api.Entities
{
    public class ShopOrderConfirmationItems
    {
         
        public string? PartNumber { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
        public string? ProjectNumber { get; set; }
        public PartType PartType { get; set; }
        public string? UOM { get; set; }   

        [Precision(13, 3)]
        public decimal RequiredQty { get; set; }

        [Precision(13, 3)]
        public decimal ConvertedToFgQty { get; set; }

        [Precision(13, 3)]
        public decimal BalanceQty { get; set; }
        public string? ReferenceIdFrom { get; set; }

        [Precision(13, 3)]
        public decimal IssuedQty { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? MRNumber { get; set; }

    }
}
