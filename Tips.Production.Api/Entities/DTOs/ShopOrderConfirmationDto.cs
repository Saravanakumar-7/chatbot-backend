using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Tips.Production.Api.Entities.DTOs
{
    public class ShopOrderConfirmationDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ShopOrderNumber { get; set; }

        [Required]
        [Precision(13, 2)]
        public string? ShopOrderReleaseQty { get; set; }

        [Required]
        [Precision(13, 2)]
        public string? WipConfirmedQty { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? Unit { get; set; }

        [DefaultValue(false)]
        public bool IsOQCDone { get; set; }


        [DefaultValue(false)]
        public bool IsDeleted { get; set; }



    }


    public class ShopOrderConfirmationPostDto
    {



        [Required]
        [MaxLength(100)]
        public string? ShopOrderNumber { get; set; }

        [Required]
        [Precision(13, 2)]
        public string? ShopOrderReleaseQty { get; set; }

        [Required]
        [Precision(13, 2)]
        public string? WipConfirmedQty { get; set; }

        [DefaultValue(false)]
        public bool IsOQCDone { get; set; }


        [DefaultValue(false)]
        public bool IsDeleted { get; set; }



    }

    public class ShopOrderConfirmationUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? ShopOrderNumber { get; set; }

        [Required]
        [Precision(13, 2)]
        public string? ShopOrderReleaseQty { get; set; }

        [Required]
        [Precision(13, 2)]
        public string? WipConfirmedQty { get; set; }


        [DefaultValue(false)]
        public bool IsOQCDone { get; set; }


        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

    }
}
