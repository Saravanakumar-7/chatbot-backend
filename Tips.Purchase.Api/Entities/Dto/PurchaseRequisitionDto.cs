using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Entities.DTOs
{
    public class PurchaseRequisitionDto
    {
        public int Id { get; set; }

        public string? PrNumber { get; set; }

        public DateTime? PrDate { get; set; }

        public int? RevisionNumber { get; set; }

        public string? ProcurementType { get; set; }

        public string? Purpose { get; set; }

        public string? PrFiles { get; set; }

        public string? DeliveryTerms { get; set; }
        public bool PrApprovalI { get; set; }
        public DateTime PrApprovedIDate { get; set; }
        public string? PrApprovedIBy { get; set; }
        public bool PrApprovalII { get; set; }
        public DateTime PrApprovedIIDate { get; set; }
        public string? PrApprovedIIBy { get; set; }
        public string? PaymentTerms { get; set; }

        public string? ShippingMode { get; set; }
        public PrStatus PrStatus { get; set; }

        public string? RetentionPeriod { get; set; }

        public string? SpecialTermsConditions { get; set; }

        public string? Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PrItemsDto>? PrItemsDtoList { get; set; }
    }

    public class PurchaseRequisitionPostDto
    {
        //public string? PRNumber { get; set; }

        public DateTime? PrDate { get; set; }

        //public int? RevisionNumber { get; set; }

        public string? ProcurementType { get; set; }

        public string? Purpose { get; set; }

        public string? PrFiles { get; set; }

        public string? DeliveryTerms { get; set; }

        public string? PaymentTerms { get; set; }

        public string? ShippingMode { get; set; }
         

        public string? RetentionPeriod { get; set; }

        public string? SpecialTermsConditions { get; set; }
     
        public List<PrItemsPostDto>? PrItemsDtoPostList { get; set; }

    }

    public class PurchaseRequisitionUpdateDto
    {
        //public int Id { get; set; }

        public string? PrNumber { get; set; }

        public DateTime? PrDate { get; set; }

        //public int? RevisionNumber { get; set; }

        public string? ProcurementType { get; set; }

        public string? Purpose { get; set; }

        public string? PrFiles { get; set; }

        public string? DeliveryTerms { get; set; }

        public string? PaymentTerms { get; set; }

        public string? ShippingMode { get; set; }
         

        public string? RetentionPeriod { get; set; }

        public string? SpecialTermsConditions { get; set; }
   
        public string? Unit { get; set; }

        //public string? LastModifiedBy { get; set; }
        //public DateTime? LastModifiedOn { get; set; }

        public List<PrItemsUpdateDto>? PrItemsDtoUpdateList { get; set; }

    }

    public class PurchaseRequisitionIdNameListDto
    {
        public int Id { get; set; }
        public string? PrNumber { get; set; }
        public DateTime? PrDate { get; set; }

        public int? RevisionNumber { get; set; }

        public string? ProcurementType { get; set; }

        public string? Purpose { get; set; }

        public string? PrFiles { get; set; }

        public string? DeliveryTerms { get; set; }
        public bool PrApprovalI { get; set; }
        public DateTime PrApprovedIDate { get; set; }
        public string? PrApprovedIBy { get; set; }
        public bool PrApprovalII { get; set; }
        public DateTime PrApprovedIIDate { get; set; }
        public string? PrApprovedIIBy { get; set; }
        public string? PaymentTerms { get; set; }

        public string? ShippingMode { get; set; }


        public string? RetentionPeriod { get; set; }

        public string? SpecialTermsConditions { get; set; }

        public string? Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class PurchaseRequisitionSearchDto
    {
        public List<string>? PrNumber { get; set; }
        public List<string>? ProcurementType { get; set; }
        public List<string>? ShippingMode { get; set; }
        public List<Status>? PRStatus { get; set; }
    }
    public class PurchaseRequistionRevNoListDto
    {
        public int? RevisionNumber { get; set; }

    }
    public class PurchaseRequisitionReportDto
    {
        public int Id { get; set; }

        public string? PrNumber { get; set; }

        public DateTime? PrDate { get; set; }

        public int? RevisionNumber { get; set; }

        public string? ProcurementType { get; set; }

        public string? Purpose { get; set; }

        public string? PrFiles { get; set; }

        public string? DeliveryTerms { get; set; }
        public bool PrApprovalI { get; set; }
        public bool PrApprovalII { get; set; }
        public string? PaymentTerms { get; set; }

        public string? ShippingMode { get; set; }


        public string? RetentionPeriod { get; set; }

        public string? SpecialTermsConditions { get; set; }

        public string? Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<PrItemsReportDto>? PrItemsDtoList { get; set; }
    }
    public class ItemMasterFileUploadDtoList
    {
        [Key]
        public int Id { get; set; }

        public string? FileName { get; set; }

        public string? FileExtension { get; set; }

        public string? FilePath { get; set; }

        public string? FileByte { get; set; }
        public string? DocumentFrom { get; set; }

        public string? ParentId { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
    public class PurchaseRequisitionSPReportWithParamDTO
    {
        public string? PrNumber { get; set; }
        public string? ProcurementType { get; set; }
        public string? ShippingMode { get; set; }
        public string? PrStatus { get; set; }
    }
}
