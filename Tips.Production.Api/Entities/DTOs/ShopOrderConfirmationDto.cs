using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Entities.Enums;
using Tips.Production.Api.Entities.Enums;

namespace Tips.Production.Api.Entities.DTOs
{
    public class ShopOrderConfirmationDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string? ShopOrderNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }

        [Required]
        public PartType ItemType { get; set; }
        [Required]
        [Precision(13, 3)]
        public decimal ShopOrderReleaseQty { get; set; }
        [Required]
        [Precision(13, 3)]
        public decimal WipConfirmedQty { get; set; }
        public ShopOrderConformationStatus IsOQCDone { get; set; }
        public ShopOrderConformationStatus IsOQCBinningDone { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }

    public class ShopOrderConfirmationPostDto
    {
        [Required] 
        public string ShopOrderNumber { get; set; }
       
        [Required]
        public string ItemNumber { get; set; }
        public string? Description { get; set; }
        [Required]
        public PartType ItemType { get; set; }
        [Required]
        [Precision(13, 3)]
        public decimal ShopOrderReleaseQty { get; set; }
        [Required]
        [Precision(13, 3)]
        public decimal WipConfirmedQty { get; set; }

        public List<ShopOrderItemConfirmationDto> shopOrderItemConfirmations { get; set; }

        

    }
    public class ShopOrderConfirmationUpdateDto
    {
       
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string? ShopOrderNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        [Required]
        public PartType ItemType { get; set; }
        [Required]
        [Precision(13, 3)]
        public decimal ShopOrderReleaseQty { get; set; }
        [Required]
        [Precision(13, 3)]
        public decimal WipConfirmedQty { get; set; } 
        public string? Unit { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
    public class ShopOrderItemNoListDto
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
    }
    public class ShopOrderDetailsDto
    {
        public string? ShopOrderNumber { get; set; }
        public decimal ShopOrderReleaseQty { get; set; }
        public decimal WipQty { get; set; }
        public decimal OqcQty { get; set; }
        public decimal BOMVersion { get; set; }        
    } 
    public class InventoryDtoForShopOrderConfirmation
    { 

        public string? PartNumber { get; set; }
        public string? ShopOrderNumber { get; set; }

        [Precision(13, 3)]
        public decimal NewConvertedToFgQty { get; set; }
        public string? DataFrom { get; set; }
        public string? MRNumber { get; set; }

    }

}
