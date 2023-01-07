using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class CompanyMasterDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "CompanyId is required")]
        public string? CompanyId { get; set; }
        [Required(ErrorMessage = "CompanyName is required")]
        public string? CompanyName { get; set; }

        public string? CompanyAliasName { get; set; }
        [Required(ErrorMessage = "CompanyType is required")]
        public string? CompanyType { get; set; }

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

        //Related Company

        public string? RelatedCompanyName { get; set; }

        public string? RelatedCompanyAlias { get; set; }

        public string? NatureOfRelationship { get; set; }

        //term

        public string? Advance { get; set; }

        public string? PaymentTerms { get; set; }

        public string? IncoTerm { get; set; }

        public string? SpecialTerms { get; set; }

        public string? PreferredFreightForwader { get; set; }
        public string? Others { get; set; }



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
        public string? Sqft { get; set; }

        public string? Machine { get; set; }
        public string? ToolsandEquip { get; set; }
        public string? ERPvalue { get; set; }

        public bool ERP { get; set; } = true;

        public string? ESDSetup { get; set; }
        public string? HazmatSetup { get; set; }

        public string? OSPvalue { get; set; }
        public bool OSP { get; set; } = true;

        //Company Approval
        public string? ScopeOfSupply { get; set; }

        public string? CompanyCategory { get; set; }

        public string? BasisOfApproval { get; set; }

        public bool InventoryItem { get; set; } = true;
        public string? CustomerApprove { get; set; }

        public string? ApprovalStatus { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalBy { get; set; }

        public string? Upload { get; set; }

        public bool ReAudit { get; set; } = true;

        public string? AuditFrequency { get; set; }
        
        
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<CompanyAddressesDto>? CompanyAddresses { get; set; }
        public List<CompanyContactsDto>? CompanyContacts { get; set; }
        public List<CompanyBankingDto>? CompanyBankings { get; set; }
        public List<CompanyMasterHeadCountingDto>? CompanyMasterHeadCountings { get; set; }

    }

    public class CompanyMasterDtoPost
    {

        [Required(ErrorMessage = "CompanyId is required")]
        public string? CompanyId { get; set; }
        [Required(ErrorMessage = "CompanyName is required")]
        public string? CompanyName { get; set; }

        public string? CompanyAliasName { get; set; }
        [Required(ErrorMessage = "CompanyType is required")]
        public string? CompanyType { get; set; }

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

        //Related Company

        public string? RelatedCompanyName { get; set; }

        public string? RelatedCompanyAlias { get; set; }

        public string? NatureOfRelationship { get; set; }

        //term

        public string? Advance { get; set; }

        public string? PaymentTerms { get; set; }

        public string? IncoTerm { get; set; }

        public string? SpecialTerms { get; set; }

        public string? PreferredFreightForwader { get; set; }
        public string? Others { get; set; }



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
        public string? Sqft { get; set; }

        public string? Machine { get; set; }
        public string? ToolsandEquip { get; set; }
        public string? ERPvalue { get; set; }

        public bool ERP { get; set; } = true;

        public string? ESDSetup { get; set; }
        public string? HazmatSetup { get; set; }

        public string? OSPvalue { get; set; }
        public bool OSP { get; set; } = true;

        //Company Approval
        public string? ScopeOfSupply { get; set; }

        public string? CompanyCategory { get; set; }

        public string? BasisOfApproval { get; set; }

        public bool InventoryItem { get; set; } = true;
        public string? CustomerApprove { get; set; }

        public string? ApprovalStatus { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalBy { get; set; }

        public string? Upload { get; set; }

        public bool ReAudit { get; set; } = true;

        public string? AuditFrequency { get; set; }
       

        public List<CompanyAddressesDtoPost>? CompanyAddresses { get; set; }
        public List<CompanyContactsDtoPost>? CompanyContacts { get; set; }
        public List<CompanyBankingDtoPost>? CompanyBankings { get; set; }
        public List<CompanyMasterHeadCountingDtoPost>? CompanyMasterHeadCountings { get; set; }

    }

    public class CompanyMasterDtoUpdate
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "CompanyId is required")]
        public string? CompanyId { get; set; }
        [Required(ErrorMessage = "CompanyName is required")]
        public string? CompanyName { get; set; }

        public string? CompanyAliasName { get; set; }
        [Required(ErrorMessage = "CompanyType is required")]
        public string? CompanyType { get; set; }

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

        //Related Company

        public string? RelatedCompanyName { get; set; }

        public string? RelatedCompanyAlias { get; set; }

        public string? NatureOfRelationship { get; set; }

        //term

        public string? Advance { get; set; }

        public string? PaymentTerms { get; set; }

        public string? IncoTerm { get; set; }

        public string? SpecialTerms { get; set; }

        public string? PreferredFreightForwader { get; set; }
        public string? Others { get; set; }



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
        public string? Sqft { get; set; }

        public string? Machine { get; set; }
        public string? ToolsandEquip { get; set; }
        public string? ERPvalue { get; set; }

        public bool ERP { get; set; } = true;

        public string? ESDSetup { get; set; }
        public string? HazmatSetup { get; set; }

        public string? OSPvalue { get; set; }
        public bool OSP { get; set; } = true;

        //Company Approval
        public string? ScopeOfSupply { get; set; }

        public string? CompanyCategory { get; set; }

        public string? BasisOfApproval { get; set; }

        public bool InventoryItem { get; set; } = true;
        public string? CustomerApprove { get; set; }

        public string? ApprovalStatus { get; set; }

        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalBy { get; set; }

        public string? Upload { get; set; }

        public bool ReAudit { get; set; } = true;

        public string? AuditFrequency { get; set; }
        
        [Required(ErrorMessage = "Unit is required")]
        [StringLength(100, ErrorMessage = "Unit can't be longer than 100 characters")]
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<CompanyAddressesDtoUpdate>? CompanyAddresses { get; set; }
        public List<CompanyContactsDtoUpdate>? CompanyContacts { get; set; }
        public List<CompanyBankingDtoUpdate>? CompanyBankings { get; set; }
        public List<CompanyMasterHeadCountingDtoUpdate>? CompanyMasterHeadCountings { get; set; }

    }

    public class CompanyIdNameListDto
    {
        public int Id { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAliasName { get; set; }
    }
}