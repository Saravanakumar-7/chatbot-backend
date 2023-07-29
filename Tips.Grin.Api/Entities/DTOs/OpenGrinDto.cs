using Entities;
using Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class OpenGrinDto
    {
        public int Id { get; set; }        
        public string? OpenGrinNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public string? Remarks { get; set; }
        public string? ReturnedBy { get; set; }
        public string? ReceiptRefNo { get; set; }
        public bool CustomerSupplied { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<OpenGrinPartsDto> OpenGrinParts { get; set; }
    }
    public class OpenGrinPostDto
    {
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public string? Remarks { get; set; }
        public string? ReturnedBy { get; set; }
        public string? ReceiptRefNo { get; set; }
        public bool CustomerSupplied { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<OpenGrinPartsPostDto> OpenGrinParts { get; set; }
    }
    public class OpenGrinUpdateDto
    {
        //public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public string? Remarks { get; set; }
        public string? ReturnedBy { get; set; }
        public string? ReceiptRefNo { get; set; }
        public bool CustomerSupplied { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<OpenGrinPartsUpdateDto> OpenGrinParts { get; set; }
    }
    public class OGInventoryDtoPost
    {
        [Required]
        public string PartNumber { get; set; }
        public string? LotNumber { get; set; }

        [Required]
        public string MftrPartNumber { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ProjectNumber { get; set; }
        [Required]
        public decimal Balance_Quantity { get; set; }
        [Required]
        public string? UOM { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Unit { get; set; }

        [Required]
        public string? Warehouse { get; set; }
        [Required]
        public string? Location { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; }
        [Required]
        public string? ReferenceID { get; set; }
        [Required]
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }

    }
    public class OpenGrinSearchDto
    {
        public List<string>? OpenGrinNumber { get; set; }
        public List<string>? CustomerName { get; set; }
        public List<string>? ReturnedBy { get; set; }
        public List<string>? ReceiptRefNo { get; set; }

    }
}
