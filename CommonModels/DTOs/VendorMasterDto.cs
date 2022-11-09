using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Entities.DTOs
{
    public class VendorMasterDto
    {
        public int Id { get; set; }

        public string VendorId { get; set; }

        public string VendorName { get; set; }

        public string? VendorAliasName { get; set; }

        public string VendorType { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        public string? PinZipCode { get; set; }

        public string? PurchaseGroup { get; set; }
        public string? BoardNumber { get; set; }
        public string? Website { get; set; }
        public string? GeneralEmail { get; set; }
        public string? Currency { get; set; }
        public string? GooglePinLocation { get; set; }

        public string? TypeOfCompany { get; set; }
        public string? ExportUnitType { get; set; }

        public bool GeneralMSME { get; set; } = true;

        //Related Vendor

        public string? RelatedVendorName { get; set; }

        public string? RelatedVendorAlias { get; set; }

        public string? NatureOfRelationship { get; set; }

        //term

        public string? TermAdvance { get; set; }

        public string? PaymentTerms { get; set; }

        public string? IncoTerm { get; set; }

        public string? SpecialTerms { get; set; }

        public string? PreferredFreightForwader { get; set; }

        //saturation

        public bool InCoporation { get; set; } = true;
        public bool TIN { get; set; } = true;
        public bool GST { get; set; } = true;
        public bool IEC { get; set; } = true;
        public bool PAN { get; set; } = true;
        public bool UdhyamCertificate { get; set; } = true;
        public bool MSME { get; set; } = true;

        //certification Docs

        public bool ISO { get; set; } = true;
        public bool AS { get; set; } = true;
        public bool Medical { get; set; } = true;
        public bool NADCAP { get; set; } = true;

        //details tab

        public string? TurnOver3years { get; set; }

        public string? DNBHooversNumber { get; set; }

        public string? ICRA { get; set; }

        public string? HeadCount { get; set; }

        public string? Capacity { get; set; }
        public string? UOM { get; set; }
        public string? FloorSpace { get; set; }
        public string? sqft { get; set; }

        public string? Machine { get; set; }
        public string? ToolsandEquip { get; set; }
        public string? ERPvalue { get; set; }

        public bool ERP { get; set; } = true;

        public string? ESDSetup { get; set; }
        public string? HazmatSetup { get; set; }

        public string? OSPvalue { get; set; }
        public bool OSP { get; set; } = true;

        //vendor Approval
        public string? ScopeOfSupply { get; set; }

        public string? VendorCategory { get; set; }

        public string? BankOfApproval { get; set; }

        public bool InventoryItem { get; set; } = true;

        public string? ApprovalStatus { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalBy { get; set; }

        public string? Upload { get; set; }

        public bool ReAudit { get; set; } = true;

        public string? AuditFrequency { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<VendorAddressDto>? Addresses { get; set; }
        public List<VendorContactsDto> Contacts { get; set; }
        public List<VendorBankingDto> VendorBankings { get; set; }
    }


    public class VendorMasterPostDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "VendorId is required")]
        [StringLength(100, ErrorMessage = "VendorId can't be longer than 100 characters")]
        public string VendorId { get; set; }

        [Required(ErrorMessage = "VendorName is required")]
        [StringLength(100, ErrorMessage = "VendorName can't be longer than 100 characters")]
        public string VendorName { get; set; }

        public string? VendorAliasName { get; set; }
        
        [Required(ErrorMessage = "VendorType is required")]
        [StringLength(500, ErrorMessage = "VendorType can't be longer than 500 characters")]
        public string VendorType { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        public string? PinZipCode { get; set; }

        public string? PurchaseGroup { get; set; }
        public string? BoardNumber { get; set; }
        public string? Website { get; set; }

        public string? GeneralEmail { get; set; }
        public string? Currency { get; set; }
        public string? GooglePinLocation { get; set; }

        public string? TypeOfCompany { get; set; }
        public string? ExportUnitType { get; set; }

        public bool GeneralMSME { get; set; } = true;

        //Related Vendor

        public string? RelatedVendorName { get; set; }

        public string? RelatedVendorAlias { get; set; }

        public string? NatureOfRelationship { get; set; }

        //term

        public string? TermAdvance { get; set; }

        public string? PaymentTerms { get; set; }

        public string? IncoTerm { get; set; }

        public string? SpecialTerms { get; set; }

        public string? PreferredFreightForwader { get; set; }

        //saturation

        public bool InCoporation { get; set; } = true;
        public bool TIN { get; set; } = true;
        public bool GST { get; set; } = true;
        public bool IEC { get; set; } = true;
        public bool PAN { get; set; } = true;
        public bool UdhyamCertificate { get; set; } = true;
        public bool MSME { get; set; } = true;

        //certification Docs

        public bool ISO { get; set; } = true;
        public bool AS { get; set; } = true;
        public bool Medical { get; set; } = true;
        public bool NADCAP { get; set; } = true;

        //details tab

        public string? TurnOver3years { get; set; }

        public string? DNBHooversNumber { get; set; }

        public string? ICRA { get; set; }

        public string? HeadCount { get; set; }

        public string? Capacity { get; set; }
        public string? UOM { get; set; }
        public string? FloorSpace { get; set; }
        public string? sqft { get; set; }

        public string? Machine { get; set; }
        public string? ToolsandEquip { get; set; }
        public string? ERPvalue { get; set; }

        public bool ERP { get; set; } = true;

        public string? ESDSetup { get; set; }
        public string? HazmatSetup { get; set; }

        public string? OSPvalue { get; set; }
        public bool OSP { get; set; } = true;

        //vendor Approval
        public string? ScopeOfSupply { get; set; }

        public string? VendorCategory { get; set; }

        public string? BankOfApproval { get; set; }

        public bool InventoryItem { get; set; } = true;

        public string? ApprovalStatus { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalBy { get; set; }

        public string? Upload { get; set; }

        public bool ReAudit { get; set; } = true;

        public string? AuditFrequency { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<VendorAddressPostDto>? Addresses { get; set; }
        public List<VendorContactsPostDto>? Contacts { get; set; }
        public List<VendorBankingPostDto>? VendorBankings { get; set; }

    }
    public class VendorMasterUpdateDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "VendorId is required")]
        [StringLength(100, ErrorMessage = "VendorId can't be longer than 100 characters")]

        public string VendorId { get; set; }

        [Required(ErrorMessage = "VendorName is required")]
        [StringLength(100, ErrorMessage = "VendorName can't be longer than 100 characters")]

        public string VendorName { get; set; }

        public string? VendorAliasName { get; set; }
        [Required(ErrorMessage = "VendorType is required")]
        [StringLength(500, ErrorMessage = "VendorType can't be longer than 500 characters")]

        public string VendorType { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        public string? PinZipCode { get; set; }

        public string? PurchaseGroup { get; set; }
        public string? BoardNumber { get; set; }
        public string? Website { get; set; }
        public string? GeneralEmail { get; set; }
        public string? Currency { get; set; }
        public string? GooglePinLocation { get; set; }

        public string? TypeOfCompany { get; set; }
        public string? ExportUnitType { get; set; }

        public bool GeneralMSME { get; set; } = true;

        //Related Vendor

        public string? RelatedVendorName { get; set; }

        public string? RelatedVendorAlias { get; set; }

        public string? NatureOfRelationship { get; set; }

        //term

        public string? TermAdvance { get; set; }

        public string? PaymentTerms { get; set; }

        public string? IncoTerm { get; set; }

        public string? SpecialTerms { get; set; }

        public string? PreferredFreightForwader { get; set; }

        //saturation

        public bool InCoporation { get; set; } = true;
        public bool TIN { get; set; } = true;
        public bool GST { get; set; } = true;
        public bool IEC { get; set; } = true;
        public bool PAN { get; set; } = true;
        public bool UdhyamCertificate { get; set; } = true;
        public bool MSME { get; set; } = true;

        //certification Docs

        public bool ISO { get; set; } = true;
        public bool AS { get; set; } = true;
        public bool Medical { get; set; } = true;
        public bool NADCAP { get; set; } = true;

        //details tab

        public string? TurnOver3years { get; set; }

        public string? DNBHooversNumber { get; set; }

        public string? ICRA { get; set; }

        public string? HeadCount { get; set; }

        public string? Capacity { get; set; }
        public string? UOM { get; set; }
        public string? FloorSpace { get; set; }
        public string? sqft { get; set; }

        public string? Machine { get; set; }
        public string? ToolsandEquip { get; set; }
        public string? ERPvalue { get; set; }

        public bool ERP { get; set; } = true;

        public string? ESDSetup { get; set; }
        public string? HazmatSetup { get; set; }

        public string? OSPvalue { get; set; }
        public bool OSP { get; set; } = true;

        //vendor Approval
        public string? ScopeOfSupply { get; set; }

        public string? VendorCategory { get; set; }

        public string? BankOfApproval { get; set; }

        public bool InventoryItem { get; set; } = true;

        public string? ApprovalStatus { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalBy { get; set; }
        public string? Upload { get; set; }
        public bool ReAudit { get; set; } = true;
        public string? AuditFrequency { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<VendorAddressUpdateDto>? Addresses { get; set; }
        public List<VendorContactsUpdateDto>? Contacts { get; set; }
        public List<VendorBankingUpdateDto>? VendorBankings { get; set; }
    }
}
