using Entities.DTOs;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class GrinDto
    {
        public int Id { get; set; }
        public string? GrinNumber { get; set; }

        //[Required]
        //public string PONumber { get; set; }

        [Required]
        public string VendorName { get; set; }

        [Required]
        public string VendorId { get; set; }
        public string? VendorNumber { get; set; }
        [Required]
        public string InvoiceNumber { get; set; }

        public decimal? InvoiceValue { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? AWBNumber1 { get; set; }

        public DateTime? AWBDate1 { get; set; }

        public string? AWBNumber2 { get; set; }

        public DateTime? AWBDate2 { get; set; }

        public string? BENumber { get; set; }

        public DateTime? BEDate { get; set; }

        public decimal? TotalInvoiceValue { get; set; }
        [Precision(13, 3)]
        public decimal? Freight { get; set; }
        [Precision(13, 3)]
        public decimal? Insurance { get; set; }
        [Precision(13, 3)]
        public decimal? LoadingorUnLoading { get; set; }
        public DateTime? GateEntryDate { get; set; }
        [Precision(13, 3)]
        public decimal? CurrencyConversion { get; set; }
        [Precision(13, 3)]
        public decimal? Transport { get; set; }
        [Precision(13, 3)]
        public decimal? BECurrencyValue { get; set; }
        public List<DocumentUploadDto>? GrinDocuments { get; set; }

        //public string? GrinDocuments { get; set; }
        public string? GateEntryNo { get; set; }
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<GrinPartsDto>? GrinParts { get; set; }
        public List<OtherChargesDto>? OtherCharges { get; set; }

    }
    public class GrinPostDto
    {


        //[Required(ErrorMessage = "PONumber is required")]
        //[StringLength(100, ErrorMessage = "ItemDescription can't be longer than 100 characters")]
        //public string PONumber { get; set; }

        [Required(ErrorMessage = "VendorName is required")]
        [StringLength(100, ErrorMessage = "ItemDescription can't be longer than 100 characters")]
        public string VendorName { get; set; }

        [Required(ErrorMessage = "VendorId is required")]
        [StringLength(100, ErrorMessage = "ItemDescription can't be longer than 100 characters")]

        public string VendorId { get; set; }
        public string? VendorNumber { get; set; }

        [Required(ErrorMessage = "InvoiceNumber is required")]
        [StringLength(100, ErrorMessage = "ItemDescription can't be longer than 100 characters")]
        public string InvoiceNumber { get; set; }

        public decimal? InvoiceValue { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? AWBNumber1 { get; set; }

        public DateTime? AWBDate1 { get; set; }

        public string? AWBNumber2 { get; set; }

        public DateTime? AWBDate2 { get; set; }

        public string? BENumber { get; set; }

        public DateTime? BEDate { get; set; }

        public decimal? TotalInvoiceValue { get; set; }
        [Precision(13, 3)]
        public decimal? Freight { get; set; }
        [Precision(13, 3)]
        public decimal? Insurance { get; set; }
        [Precision(13, 3)]
        public decimal? LoadingorUnLoading { get; set; }
        public DateTime? GateEntryDate { get; set; }
        [Precision(13, 3)]
        public decimal? CurrencyConversion { get; set; }
        [Precision(13, 3)]
        public decimal? Transport { get; set; }
        [Precision(13, 3)]
        public decimal? BECurrencyValue { get; set; }
        public string? GateEntryNo { get; set; }
        public List<DocumentUploadPostDto>? GrinDocuments { get; set; }

        //public string? GrinDocuments { get; set; }


        public List<GrinPartsPostDto>? GrinParts { get; set; }
        public List<OtherChargesPostDto>? OtherCharges { get; set; }

    }
    public class GrinUpdateDto
    {
        public int Id { get; set; }

        //[Required(ErrorMessage = "PONumber is required")]
        //public string PONumber { get; set; }

        [Required(ErrorMessage = "VendorName is required")]
        public string VendorName { get; set; }

        [Required(ErrorMessage = "VendorId is required")]
        public string VendorId { get; set; }
        public string? VendorNumber { get; set; }

        [Required(ErrorMessage = "InvoiceNumber is required")]

        public string InvoiceNumber { get; set; }

        public decimal? InvoiceValue { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? AWBNumber1 { get; set; }

        public DateTime? AWBDate1 { get; set; }

        public string? AWBNumber2 { get; set; }

        public DateTime? AWBDate2 { get; set; }

        public string? BENumber { get; set; }

        public DateTime? BEDate { get; set; }

        [Precision(13, 3)]
        public decimal? Freight { get; set; }
        [Precision(13, 3)]
        public decimal? Insurance { get; set; }
        [Precision(13, 3)]
        public decimal? LoadingorUnLoading { get; set; }
        public DateTime? GateEntryDate { get; set; }
        [Precision(13, 3)]
        public decimal? CurrencyConversion { get; set; }
        [Precision(13, 3)]
        public decimal? Transport { get; set; }
        [Precision(13, 3)]
        public decimal? BECurrencyValue { get; set; }
        public List<DocumentUploadUpdateDto>? GrinDocuments { get; set; }
        public string? GateEntryNo { get; set; }

        //public string? GrinDocuments { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<GrinPartsUpdateDto>? GrinParts { get; set; }
        public List<OtherChargesUpdateDto>? OtherCharges { get; set; }

    }
    public class GrinNumberListDto
    {
        public int Id { get; set; }
        public string? GrinNumber { get; set; }
        public string InvoiceNumber { get; set; }

    }
    public class GrinSearchDto
    {
        public List<string>? GrinNumber { get; set; }
        public List<string>? VendorName { get; set; }
        public List<string>? VendorId { get; set; }
        public List<string>? InvoiceNumber { get; set; }
    }

    public class GrinInventoryDto
    { 
        public string PartNumber { get; set; }
        public string? LotNumber { get; set; }

        public string MftrPartNumber { get; set; }

        
        public string Description { get; set; }

      
        public string ProjectNumber { get; set; }
         public decimal Balance_Quantity { get; set; }
         public string? UOM { get; set; }

         public string? Warehouse { get; set; }
         public string? Location { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; }
         public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }

    }
    public class GrinInventoryTranctionDto
    {
        public string PartNumber { get; set; }
        public string? LotNumber { get; set; }

        public string MftrPartNumber { get; set; }

        public bool IsStockAvailable { get; set; }
        public string Description { get; set; }


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
    public class GrinNoForIqcAndBinning
    {
        public string? GrinNumber { get; set; }
        public int GrinId { get; set; }

    }
    public class GrinItemMasterEnggDto
    {
        public int Id { get; set; }
        public string? GrinNumber { get; set; }

        //[Required]
        //public string PONumber { get; set; }

        [Required]
        public string VendorName { get; set; }

        [Required]
        public string VendorId { get; set; }
        public string? VendorNumber { get; set; }
        [Required]
        public string InvoiceNumber { get; set; }

        public decimal? InvoiceValue { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? AWBNumber1 { get; set; }

        public DateTime? AWBDate1 { get; set; }

        public string? AWBNumber2 { get; set; }

        public DateTime? AWBDate2 { get; set; }

        public string? BENumber { get; set; }

        public DateTime? BEDate { get; set; }

        public decimal? TotalInvoiceValue { get; set; }
        [Precision(13, 3)]
        public decimal? Freight { get; set; }
        [Precision(13, 3)]
        public decimal? Insurance { get; set; }
        [Precision(13, 3)]
        public decimal? LoadingorUnLoading { get; set; }
        public DateTime? GateEntryDate { get; set; }
        [Precision(13, 3)]
        public decimal? CurrencyConversion { get; set; }
        [Precision(13, 3)]
        public decimal? Transport { get; set; }
        [Precision(13, 3)]
        public decimal? BECurrencyValue { get; set; }
        public string? GateEntryNo { get; set; }
        public List<DocumentUploadDto>? GrinDocuments { get; set; }

        //public string? GrinDocuments { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<GrinPartsItemMasterEnggDto>? GrinParts { get; set; }
        public List<OtherChargesDto>? OtherCharges { get; set; }

    }
    public class GrinReportDto
    {
        public int Id { get; set; }
        public string? GrinNumber { get; set; }

        //[Required]
        //public string PONumber { get; set; }

        [Required]
        public string VendorName { get; set; }

        [Required]
        public string VendorId { get; set; }
        public string? VendorNumber { get; set; }

        [Required]
        public string InvoiceNumber { get; set; }

        public decimal? InvoiceValue { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? AWBNumber1 { get; set; }

        public DateTime? AWBDate1 { get; set; }

        public string? AWBNumber2 { get; set; }

        public DateTime? AWBDate2 { get; set; }

        public string? BENumber { get; set; }

        public DateTime? BEDate { get; set; }

        public decimal? TotalInvoiceValue { get; set; }
        [Precision(13, 3)]
        public decimal? Freight { get; set; }
        [Precision(13, 3)]
        public decimal? Insurance { get; set; }
        [Precision(13, 3)]
        public decimal? LoadingorUnLoading { get; set; }
        public DateTime? GateEntryDate { get; set; }
        [Precision(13, 3)]
        public decimal? CurrencyConversion { get; set; }
        [Precision(13, 3)]
        public decimal? Transport { get; set; }
        [Precision(13, 3)]
        public decimal? BECurrencyValue { get; set; }
        public List<DocumentUploadDto>? GrinDocuments { get; set; }

        //public string? GrinDocuments { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<GrinPartsReportDto>? GrinParts { get; set; }
        public List<OtherChargesDto>? OtherCharges { get; set; }

    }
    public class GrinReportWithParam
    {
        public string? GrinNumber { get; set; }

        public string VendorName { get; set; }

        public string PONumber { get; set; }
        public string KPN { get; set; }

        public string MPN { get; set; }

        public string? Warehouse { get; set; }

        public string? Location { get; set; }
    }
}
