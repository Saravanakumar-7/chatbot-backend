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
        public string? GrinDocuments { get; set; }
        public bool TallyStatus { get; set; } = false;
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
        public string? GrinDocuments { get; set; }
        public bool TallyStatus { get; set; } = false;

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
        public string? GrinDocuments { get; set; }
        public string? GateEntryNo { get; set; }
        public decimal? TotalInvoiceValue { get; set; }
        public bool TallyStatus { get; set; } = false;

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
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }
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
        public DateTime Issued_DateTime { get; set; }
        public string? Issued_By { get; set; }
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
        public string? Remarks { get; set; }

    }
    public class grinInventoryTrasactionPostDto
    {
        public string PartNumber { get; set; }
        public string LotNumber { get; set; }
        public string MftrPartNumber { get; set; }
        public string Description { get; set; }
        public PartType PartType { get; set; }
        public string ProjectNumber { get; set; }
        public InventoryType TransactionType { get; set; }
        [Precision(18, 2)]
        public decimal Issued_Quantity { get; set; }

        public string? UOM { get; set; }

        public DateTime Issued_DateTime { get; set; }

        public string Issued_By { get; set; }
        public string? ShopOrderId { get; set; }
        public string ReferenceID { get; set; }
        public string ReferenceIDFrom { get; set; }

        public decimal? BOM_Version_No { get; set; }

        public string? From_Location { get; set; }
        public string TO_Location { get; set; }

        public bool? ModifiedStatus { get; set; } = false;

        public string? Unit { get; set; }

        public string? GrinMaterialType { get; set; } = "Bought Out";

        public string? Remarks { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public string? shopOrderNo { get; set; }
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
        public string? GrinDocuments { get; set; }

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
        public string? VendorName { get; set; }

        [Required]
        public string? VendorId { get; set; }
        public string? VendorNumber { get; set; }

        [Required]
        public string? InvoiceNumber { get; set; }

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
        public string? GrinDocuments { get; set; }

        //public string? GrinDocuments { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<GrinPartsReportDto>? GrinParts { get; set; }
        public List<OtherChargesDto>? OtherCharges { get; set; }

    }
    public class GrinReportWithParamDto
    {
        public string GrinNumber { get; set; }
        public string VendorName { get; set; }
        public string PONumber { get; set; }
        public string KPN { get; set; }
        public string MPN { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
    }
    public class GrinReportWithParamForTransDto
    {
        public string GrinNumber { get; set; }
        public string VendorName { get; set; }
        public string PONumber { get; set; }
        public string ItemNumber { get; set; }
        public string MPN { get; set; }
        public string ProjectNumber { get; set; }

    }

    public class GrinReportWithParamForAviDto
    {
        public string GrinNumber { get; set; }
        public string VendorName { get; set; }
        public string PONumber { get; set; }
        public string ItemNumber { get; set; }
        public string MPN { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public string ProjectNumber { get; set; }

    }
    public class GrinSPReportForAvi
    {
        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public string? GrinNumber { get; set; }
        public DateTime? GrinDate { get; set; }
        public string? GateEntryNo { get; set; }
        public DateTime? GateEntryDate { get; set; }
        public string? VendorName { get; set; }
        public string? VendorId { get; set; }
        public string? VendorAddress { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public string? MPN { get; set; }
        public string? ManufactureBatchNumber { get; set; }
        public string? LotNumber { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Qty { get; set; }
        public decimal? AcceptedQty { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public decimal? SGST { get; set; }
        public decimal? CGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? UTGST { get; set; }
        public decimal? totalvalue { get; set; }
        public string? Remarks { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public decimal? ProjectQty { get; set; }
        public bool? TallyStatus { get; set; }
        public string? BENumber { get; set; }
    }
    public class GrinIQCConfirmationSaveDto
    {
        public string? GrinNumber { get; set; }
        public int GrinId { get; set; }
        public string VendorName { get; set; }
        public string VendorId { get; set; }
        public string? VendorNumber { get; set; }
        public GrinIQCConfirmationItemsSaveDto GrinIQCConfirmationItemsPostDtos { get; set; }

    }
    public class Data
    {
        public int id { get; set; }
        public string processType { get; set; }
        public string template { get; set; }
        public string subject { get; set; }
    }
    public class EmailTemplateDto
    {
        public Data data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }
    public class InventoryDto
    {
        public List<GrinInventoryDto> data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }
    public class GrinandIqcDetail
    {
        public List<Grins> grins { get; set; }
        public List<IQCConfirmation>? iqcs { get; set; }
    }

    public class GrinComsumpReportDto
    {
        public string? GrinNumber { get; set; }
        public DateTime? GrinDate { get; set; }
        public string? VendorName { get; set; }
        public string? PartNumber { get; set; }
        public string? PONumber { get; set; }
        public decimal BOENo { get; set; }
        public decimal? GrinQty { get; set; }
        public decimal GrinUnitPrice { get; set; }
        public decimal Tax { get; set; }
        public decimal OtherCosts { get; set; }
        public string? UOM { get; set; }
        public string? UOC { get; set; }

    }
    public class PurchaseInventorySPReportDto
    {
        public string? InvoiceNumber { get; set; }
        public string? GRINNumber { get; set; }
        public string? KPN { get; set; }
        public string? VendorName { get; set; }
    }
    public class PurchaseInventorySPReport
    {
        public string? Invoice_Date { get; set; }
        public string? Invoice_No { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? VoucherType { get; set; }
        public string? VendorName { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? SupplierPinCode { get; set; }
        public string? State { get; set; }
        public string? PlaceOfSupply { get; set; }
        public string? Country { get; set; }
        public string? GSTNNumber { get; set; }
        public string? ConsignorFromName { get; set; }
        public string? ConsignorFrom_Add1 { get; set; }
        public string? ConsignorFrom_Add2 { get; set; }
        public string? ConsignorFrom_Add3 { get; set; }
        public string? ConsignorFrom_State { get; set; }
        public string? ConsignorFrom_Place { get; set; }
        public string? ConsignorFrom_Pincode { get; set; }
        public string? ConsignorFrom_GSTIN { get; set; }
        public string? TinNo { get; set; }
        public string? CSTNo { get; set; }
        public string? GSTRegistrationType { get; set; }
        public string? ReceiptNote_No { get; set; }
        public string? ReceiptNote_Date { get; set; }
        public string? OrderNo { get; set; }
        public string? OrderDate { get; set; }
        public string? Order_DueDate { get; set; }
        public string? LR_No { get; set; }
        public string? Despatch_Through { get; set; }
        public string? Destination { get; set; }
        public string? TermsOfPayment { get; set; }
        public string? Other_Reference { get; set; }
        public string? TermsOfDelivery { get; set; }
        public string? Purchase_Ledger { get; set; }
        public string? ItemName { get; set; }
        public string? HSN_Code { get; set; }
        public string? ItemDescription { get; set; }
        public decimal? TaxRate { get; set; }
        public string? BatchNo { get; set; }
        public string? MfgDate { get; set; }
        public string? ExpDate { get; set; }
        public decimal? Qty { get; set; }
        public string? UOM { get; set; }
        public decimal? Rate { get; set; }
        public string? Discount { get; set; }
        public decimal? Amount { get; set; }
        public string? OtherCharges_1_Ledger { get; set; }
        public string? OtherCharges_1_Amount { get; set; }
        public string? OtherCharges_2_Ledger { get; set; }
        public string? OtherCharges_2_Amount { get; set; }
        public string? OtherCharges_3_Ledger { get; set; }
        public string? OtherCharges_3_Amount { get; set; }
        public string? OtherCharges_4_Ledger { get; set; }
        public string? OtherCharges_4_Amount { get; set; }
        public string? OtherCharges_5_Ledger { get; set; }
        public string? OtherCharges_5_Amount { get; set; }
        public decimal? CSGT_Ledger { get; set; }
        public decimal? CGSTAmount { get; set; }
        public decimal? SGST_Ledger { get; set; }
        public decimal? SGSTAmount { get; set; }
        public decimal? IGST_Ledger { get; set; }
        public decimal? IGSTAmount { get; set; }
        public string? CessLedger { get; set; }
        public string? CessAmount { get; set; }
        public string? Roundoff_Ledger { get; set; }
        public string? Roundoff_Amount { get; set; }
        public string? CostCenter { get; set; }
        public string? Godown { get; set; }
        public string? Narration { get; set; }
        public string? eWay_BillNo { get; set; }
        public string? eWay_BillDate { get; set; }
        public string? Consolidated_eWay_BillNo { get; set; }
        public string? Consolidated_eWay_Date { get; set; }
        public string? SubType { get; set; }
        public string? DocumentType { get; set; }
        public string? Statusof_eWayBill { get; set; }
        public string? TransportMode { get; set; }
        public string? Distance { get; set; }
        public string? Transporter_Name { get; set; }
        public string? Vehicle_Number { get; set; }
        public string? Vehicle_Type { get; set; }
        public string? Doc_or_AirWay_BillNo { get; set; }
        public string? DocDate { get; set; }
        public string? Transporter_ID { get; set; }
    }
    public class PoAndGrinUnitPriceSPReportDto
    {
        public string? GrinNumber { get; set; }
        public string? VendorName { get; set; }
        public string? PONumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? ProjectNumber { get; set; }

    }
    public class PoAndGrinUnitPriceSPReport
    {
        public string? PONumber { get; set; }
        public string? PartNumber { get; set; }
        public string? MPN { get; set; }
        public string? Description { get; set; }
        public decimal? POQty { get; set; }
        public string? POUOM { get; set; }
        public decimal? POUnitPrice { get; set; }
        public string? POUOC { get; set; }
        public string? GrinNumber { get; set; }
        public decimal? GRINQty { get; set; }
        public string? GRINUOM { get; set; }
        public decimal? GRINUnitPrice { get; set; }
        public string? GRINUOC { get; set; }
        public decimal? UnitPriceDifference { get; set; }
        public string? projectnumber { get; set; }
        public string? VendorName { get; set; }
        public DateTime? grindate { get; set; }
    }


}
