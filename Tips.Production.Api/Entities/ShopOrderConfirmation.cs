

using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Tips.Production.Api.Entities
{
    public class ShopOrderConfirmation
    {
        [Required]
        public int Id { get; set; }

        [Required] 
        public string? ShopOrderNumber { get; set; }

        public string? ItemNumber { get; set; }
        public string? ItemDescription { get; set; }

        [Required]
        public PartType ItemType { get; set; }
        [Required]
        [Precision(13, 3)]
        public decimal ShopOrderReleaseQty { get; set; }
        [Required]
        [Precision(13, 3)]
        public decimal WipConfirmedQty { get; set; }
        [DefaultValue(false)]
        public bool IsOQCDone { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
      
    }
}
