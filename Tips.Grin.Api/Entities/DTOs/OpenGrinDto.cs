using Entities;
using Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class OpenGrinDto
    {
        public int Id { get; set; }
        public string? OpenGrinNumber { get; set; }
        public string? SenderName { get; set; }
        public string? SenderId { get; set; }
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
        public string? SenderName { get; set; }
        public string? SenderId { get; set; }
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
        public string? SenderName { get; set; }
        public string? SenderId { get; set; }
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
    public class OGInventoryTranctionDto
    {
        public string PartNumber { get; set; }
        public string? LotNumber { get; set; }

        public string MftrPartNumber { get; set; }


        public string Description { get; set; }

        public bool IsStockAvailable { get; set; }
        public string ProjectNumber { get; set; }
        public decimal Issued_Quantity { get; set; }
        public string? UOM { get; set; }

        public string? Warehouse { get; set; }
        public string? From_Location { get; set; }
        public string? TO_Location { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; }
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }

    }

    public class OpenGrinSearchDto
    {
        public List<string>? OpenGrinNumber { get; set; }
        public List<string>? SenderName { get; set; }
        //public List<string>? ReturnedBy { get; set; }
        public List<string>? ReceiptRefNo { get; set; }

    }
    public class OpenGrinDataListDto
    {
        public int Id { get; set; }
        public string? OpenGrinNumber { get; set; }
        public string? SenderName { get; set; }
        public string? SenderId { get; set; }
        public string? Remarks { get; set; }
        public string? ReturnedBy { get; set; }
        public string? ReceiptRefNo { get; set; }
        public bool CustomerSupplied { get; set; }
    }
}