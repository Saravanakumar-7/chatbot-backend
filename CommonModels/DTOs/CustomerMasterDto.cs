using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class CustomerMasterDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "CustomerId is required")]
        public string? CustomerId { get; set; }
        
        [Required(ErrorMessage = "CustomerName is required")]
        public string? CustomerName { get; set; }
        
        public string? CustomerAliasName { get; set; }
        
        [Required(ErrorMessage = "CustomerType is required")]
        public string? CustomerType { get; set; }
        
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PinZipCode { get; set; }
        public string? Segment { get; set; }
        public string? BoardNumber { get; set; }
        public string? Website { get; set; }
        public string? GeneralEmail { get; set; }
        public string? Currency { get; set; }
        public string? GooglePinLocation { get; set; }
        public string? TypeOfCompany { get; set; }
        public string? ExportUnitType { get; set; }
        public bool GeneralMSME { get; set; } = true;
        public string? SalesManager { get; set; }
        public string? SalesManagerCode { get; set; }

        public string? Region { get; set; }
        public string? ShippingMode { get; set; }

        //RelatedLink

        public string? RelatedCustomerName { get; set; }

        public string? RelatedCustomerAlias { get; set; }

        public string? NatureOfRelationship { get; set; }

        //Dispatch

        public bool PartialDispatch { get; set; } = true;
        public bool DropShipment { get; set; } = true;
        public string? PackingInstructions { get; set; }
        public string? SpecialInstructions { get; set; }
        public bool PODReq { get; set; } = true;
        public string? ShipmentInstructions { get; set; }
        public string? PreferredFreightForwarder { get; set; }

        //Terms
        public string? Advance { get; set; }
        public string? PaymentTerms { get; set; }
        public string? INCOTerms { get; set; }
        public string? SpecialTerms { get; set; }
        public bool LDApplicable { get; set; } = true;
        public string? LDApplicableValue { get; set; }
        public bool SourceInspection { get; set; } = true;
        public string? SourceInspectionValue { get; set; }
        public string? Others { get; set; }

        //Statutory

        public bool Incorporation { get; set; } = true;
        public bool TIN { get; set; } = true;
        public bool GST { get; set; } = true;
        public bool IEC { get; set; } = true;
        public bool PAN { get; set; } = true;
        public bool UdhyamCertificate { get; set; } = true;
        public bool MSME { get; set; } = true;

        //Details

        public string? TurnOver { get; set; }
        public string? DNBNumber { get; set; }
        public string? ICRA { get; set; }
        public string? HeadCount { get; set; }
        public string? Capacity { get; set; }
        public string? FloorSpace { get; set; }
        public string? Machine { get; set; }
        public string? ToolsAndEquip { get; set; }
      
        public bool ERP { get; set; }
        public string? ERPValue { get; set; }
        public string? ESDSetup { get; set; }
        public string? HazmatSetup { get; set; }
        public string? UOM { get; set; }
        public string? Sqft { get; set; }
        public string? MachineDetails { get; set; }
        public string? ToolsandEquipDetails { get; set; }

        public bool ERPDetails { get; set; }
        public string? ESDSetupDetails { get; set; }
        public string? HazmatSetupDetails { get; set; }
        public bool OSP { get; set; }
        public string? OSPValue { get; set; }

        //CustomerApproval

        public string? ScopeOfSupply { get; set; }

        public string? CustomerCategory { get; set; }

        public string? BasisOfApproval { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalBy { get; set; }

        public string? Upload { get; set; }

        public bool ReAudit { get; set; } = true;

        public string? AuditFrequency { get; set; }
        public string? CustomerApprove { get; set; }

        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<CustomerAddressesDto>? CustomerAddresses { get; set; }
        public List<CustomerShippingAddressesDto>? CustomerShippingAddresses { get; set; }
        public List<CustomerContactsDto>? CustomerContacts { get; set; }
        public List<CustomerBankingDto>? CustomerBanking { get; set; }

        public List<CustomerMasterHeadCountingDto>? CustomerMasterHeadCountings { get; set; }
    }
    public class CustomerMasterDtoPost
    {
        [Required(ErrorMessage = "CompanyId is required")]
        public string? CustomerId { get; set; }

        [Required(ErrorMessage = "CompanyId is required")]
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        [Required(ErrorMessage = "CompanyId is required")]
        public string? CustomerType { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PinZipCode { get; set; }
        public string? Segment { get; set; }
        public string? BoardNumber { get; set; }
        public string? Website { get; set; }
        public string? GeneralEmail { get; set; }
        public string? Currency { get; set; }
        public string? GooglePinLocation { get; set; }
        public string? TypeOfCompany { get; set; }
        public string? ExportUnitType { get; set; }
        public bool GeneralMSME { get; set; } = true;
        public string? SalesManager { get; set; }
        public string? SalesManagerCode { get; set; }
        public string? Region { get; set; }
        public string? ShippingMode { get; set; }

        //RelatedLink

        public string? RelatedCustomerName { get; set; }

        public string? RelatedCustomerAlias { get; set; }

        public string? NatureOfRelationship { get; set; }

        //Dispatch

        public bool PartialDispatch { get; set; } = true;
        public bool DropShipment { get; set; } = true;
        public string? PackingInstructions { get; set; }
        public string? SpecialInstructions { get; set; }
        public bool PODReq { get; set; } = true;
        public string? ShipmentInstructions { get; set; }
        public string? PreferredFreightForwarder { get; set; }

        //Terms
        public string? Advance { get; set; }
        public string? PaymentTerms { get; set; }
        public string? INCOTerms { get; set; }
        public string? SpecialTerms { get; set; }
        public bool LDApplicable { get; set; } = true;
        public string? LDApplicableValue { get; set; }
        public bool SourceInspection { get; set; } = true;
        public string? SourceInspectionValue { get; set; }
        public string? Others { get; set; }

        //Statutory

        public bool Incorporation { get; set; } = true;
        public bool TIN { get; set; } = true;
        public bool GST { get; set; } = true;
        public bool IEC { get; set; } = true;
        public bool PAN { get; set; } = true;
        public bool UdhyamCertificate { get; set; } = true;
        public bool MSME { get; set; } = true;

        //Details

        public string? TurnOver { get; set; }
        public string? DNBNumber { get; set; }
        public string? ICRA { get; set; }
        public string? HeadCount { get; set; }
        public string? Capacity { get; set; }
        public string? FloorSpace { get; set; }
        public string? Machine { get; set; }
        public string? ToolsAndEquip { get; set; }

        public bool ERP { get; set; }
        public string? ERPValue { get; set; }
        public string? ESDSetup { get; set; }
        public string? HazmatSetup { get; set; }
        public string? UOM { get; set; }
        public string? Sqft { get; set; }
        public string? MachineDetails { get; set; }

        public string? ToolsandEquipDetails { get; set; }


        public bool ERPDetails { get; set; }
        public string? ESDSetupDetails { get; set; }
        public string? HazmatSetupDetails { get; set; }
        public bool OSP { get; set; }
        public string? OSPValue { get; set; }

        //CustomerApproval

        public string? ScopeOfSupply { get; set; }

        public string? CustomerCategory { get; set; }

        public string? BasisOfApproval { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalBy { get; set; }

        public string? Upload { get; set; }

        public bool ReAudit { get; set; } = true;

        public string? AuditFrequency { get; set; }
        public string? CustomerApprove { get; set; }
        

        public List<CustomerAddressesDtoPost>? CustomerAddress { get; set; }
        public List<CustomerShippingAddressesDtoPost>? CustomerShippingAddresses { get; set; }
        public List<CustomerContactsDtoPost>? CustomerContacts { get; set; }
        public List<CustomerBankingDtoPost>? CustomerBankings { get; set; }
        public List<CustomerMasterHeadCountingDtoPost>? CustomerMasterHeadCountings { get; set; }
    }
    public class CustomerMasterDtoUpdate
    {

    public int Id { get; set; }
        [Required(ErrorMessage = "CustomerId is required")]
        public string? CustomerId { get; set; }
        [Required(ErrorMessage = "CustomerName is required")]
        public string? CustomerName { get; set; }
    public string? CustomerAliasName { get; set; }
        [Required(ErrorMessage = "CustomerType is required")]
        public string? CustomerType { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PinZipCode { get; set; }
    public string? Segment { get; set; }
    public string? BoardNumber { get; set; }
    public string? Website { get; set; }
    public string? GeneralEmail { get; set; }
    public string? Currency { get; set; }
    public string? GooglePinLocation { get; set; }
    public string? TypeOfCompany { get; set; }
    public string? ExportUnitType { get; set; }
    public bool GeneralMSME { get; set; } = true;
    public string? SalesManager { get; set; }
        public string? SalesManagerCode { get; set; }
        public string? Region { get; set; }
        public string? ShippingMode { get; set; }

        //RelatedLink

        public string? RelatedCustomerName { get; set; }

    public string? RelatedCustomerAlias { get; set; }

    public string? NatureOfRelationship { get; set; }

    //Dispatch

    public bool PartialDispatch { get; set; } = true;
    public bool DropShipment { get; set; } = true;
    public string? PackingInstructions { get; set; }
    public string? SpecialInstructions { get; set; }
    public bool PODReq { get; set; } = true;
    public string? ShipmentInstructions { get; set; }
    public string? PreferredFreightForwarder { get; set; }

    //Terms
    public string? Advance { get; set; }
    public string? PaymentTerms { get; set; }
    public string? INCOTerms { get; set; }
    public string? SpecialTerms { get; set; }
    public bool LDApplicable { get; set; } = true;
    public string? LDApplicableValue { get; set; }
    public bool SourceInspection { get; set; } = true;
    public string? SourceInspectionValue { get; set; }
        public string? Others { get; set; }

        //Statutory

        public bool Incorporation { get; set; } = true;
    public bool TIN { get; set; } = true;
    public bool GST { get; set; } = true;
    public bool IEC { get; set; } = true;
    public bool PAN { get; set; } = true;
    public bool UdhyamCertificate { get; set; } = true;
    public bool MSME { get; set; } = true;

    //Details

    public string? TurnOver { get; set; }
    public string? DNBNumber { get; set; }
    public string? ICRA { get; set; }
    public string? HeadCount { get; set; }
    public string? Capacity { get; set; }
    public string? FloorSpace { get; set; }
    public string? Machine { get; set; }
        public string? ToolsAndEquip { get; set; }
        //public string? Tools { get; set; }
        //public string? Equip { get; set; }
        public bool ERP { get; set; }
    public string? ERPValue { get; set; }
    public string? ESDSetup { get; set; }
    public string? HazmatSetup { get; set; }
    public string? UOM { get; set; }
    public string? Sqft { get; set; }
    public string? MachineDetails { get; set; }
        public string? ToolsandEquipDetails { get; set; }
        //public string? ToolsDetials { get; set; }
        // public string? EquipDetials { get; set; }
        public bool ERPDetails { get; set; }
    public string? ESDSetupDetails { get; set; }
    public string? HazmatSetupDetails { get; set; }
    public bool OSP { get; set; }
    public string? OSPValue { get; set; }

    //CustomerApproval

    public string? ScopeOfSupply { get; set; }

    public string? CustomerCategory { get; set; }

    public string? BasisOfApproval { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ApprovalBy { get; set; }

    public string? Upload { get; set; }

    public bool ReAudit { get; set; } = true;

    public string? AuditFrequency { get; set; }
        public string? CustomerApprove { get; set; }
        //public bool IsActive { get; set; } = true;
        [Required(ErrorMessage = "Unit is required")]
        [StringLength(100, ErrorMessage = "Unit can't be longer than 100 characters")]
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }

    public List<CustomerAddressesDtoUpdate>? CustomerAddress { get; set; }
    public List<CustomerShippingAddressesDtoUpdate>? CustomerShippingAddresses { get; set; }
    public List<CustomerContactsDtoUpdate>? CustomerContacts { get; set; }
    public List<CustomerBankingDtoUpdate>? CustomerBankings { get; set; }
    public List<CustomerMasterHeadCountingDtoUpdate>? CustomerMasterHeadCountings { get; set; }

    }
    public class CustomerIdNameListDto
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }

    }
}
