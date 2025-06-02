using System.ComponentModel;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class poconfirmation_report_Dto
    {
        public string? VendorId { get; set; }
        public string? VendorName { get; set; }
        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public string? PRNumber { get; set; }
        public decimal? PRQty { get; set; }
        public int? RevisionNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public decimal? POQnty { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal? BalanceQty { get; set; }
        public string? Currency { get; set; }
        public string? UOM { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? BalanceValue { get; set; }
        public string? POApprovedIBy { get; set; }
        public DateTime? POApprovedIDate { get; set; }
        public string? POApprovedIIBy { get; set; }
        public DateTime? POApprovedIIDate { get; set; }
        public int? PoStatus { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public decimal? ConfirmationQty { get; set; }

    }
    public class poconfirmation_report_with_pagination_Dto
    {
        public string? VendorId { get; set; }
        public string? VendorName { get; set; }
        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public string? PRNumber { get; set; }
        public decimal? PRQty { get; set; }
        public int? RevisionNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public decimal? POQnty { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal? BalanceQty { get; set; }
        public string? Currency { get; set; }
        public string? ProjectNumber { get; set; }
        public string? UOM { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? PaymentTerms { get; set; }
        public decimal? BalanceValue { get; set; }
        public string? POApprovedIBy { get; set; }
        public DateTime? POApprovedIDate { get; set; }
        public string? POApprovedIIBy { get; set; }
        public DateTime? POApprovedIIDate { get; set; }
        public int? PoStatus { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public decimal? ConfirmationQty { get; set; }

    }
    public class podeliveryschedule_report_Dto
    {
        public string? VendorId { get; set; }
        public string? VendorName { get; set; }
        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public string? PRNumber { get; set; }
        public decimal? PRQty { get; set; }
        public int? RevisionNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public decimal? POQnty { get; set; }
        public decimal? ScheduleQty { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal? BalanceQty { get; set; }
        public string? Currency { get; set; }
        public string? UOM { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? BalanceValue { get; set; }
        public string? POApprovedIBy { get; set; }
        public DateTime? POApprovedIDate { get; set; }
        public string? POApprovedIIBy { get; set; }
        public DateTime? POApprovedIIDate { get; set; }
        public int? PoStatus { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ScheduleDate { get; set; }
    }
    public class podeliveryschedule_report_with_parameters_with_pagination_Dto
    {
        public string? VendorId { get; set; }
        public string? VendorName { get; set; }
        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public string? PRNumber { get; set; }
        public decimal? PRQty { get; set; }
        public int? RevisionNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public decimal? POQnty { get; set; }
        public decimal? ScheduleQty { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal? BalanceQty { get; set; }
        public string? Currency { get; set; }
        public string? ProjectNumber { get; set; }
        public string? UOM { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? PaymentTerms { get; set; }
        public decimal? BalanceValue { get; set; }
        public string? POApprovedIBy { get; set; }
        public DateTime? POApprovedIDate { get; set; }
        public string? POApprovedIIBy { get; set; }
        public DateTime? POApprovedIIDate { get; set; }
        public int? PoStatus { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ScheduleDate { get; set; }
    }
    //public class poproject_report_Dto
    //{
    //    public string? VendorId { get; set; }
    //    public string? VendorName { get; set; }
    //    public string? PONumber { get; set; }
    //    public DateTime? PODate { get; set; }
    //    public string? PRNumber { get; set; }
    //    public decimal? PRQty { get; set; }
    //    public int? RevisionNumber { get; set; }
    //    public string? ProjectNumber { get; set; }
    //    public decimal? ProjectQty { get; set; }
    //    public string? ItemNumber { get; set; }
    //    public string? MftrItemNumber { get; set; }
    //    public string? ItemDescription { get; set; }
    //    public decimal? POQnty { get; set; }
    //    public decimal? ReceivedQty { get; set; }
    //    public decimal? BalanceQty { get; set; }
    //    public string? Currency { get; set; }
    //    public string? UOM { get; set; }
    //    public decimal? UnitPrice { get; set; }
    //    public decimal? BalanceValue { get; set; }
    //    public string? POApprovedIBy { get; set; }
    //    public DateTime? POApprovedIDate { get; set; }
    //    public string? POApprovedIIBy { get; set; }
    //    public DateTime? POApprovedIIDate { get; set; }
    //    public int? PoStatus { get; set; }
    //    public string? CreatedBy { get; set; }
    //    public DateTime? CreatedOn { get; set; }
    //}
        public class PurchaseOrder_ReportGetDto
    {
        public string? ItemNumber { get; set; }
        public string? PONumbers { get; set; } 
        public string? VendorName { get; set; }
        public string? POStatus { get; set; }
        public string? Approval { get; set; }
        public string? ProjectNumber { get; set; }
        public string? RecordType { get; set; }
    }
    public class PurchaseOrderProLimitSPReportDto
    {
        public string? ItemNumber { get; set; }
        public string? PONumbers { get; set; }
        public string? VendorName { get; set; }
        public string? POStatus { get; set; }
        public string? Approval { get; set; }
        public string? ProjectNumber { get; set; }
        public string? RecordType { get; set; }
        public int? Offset { get; set; }
        public int? Limit { get; set; }
    }
    public class PurchaseOrderConfor_ReportGetDto
    {
        public string? ItemNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? PONumbers { get; set; }
        public string? VendorName { get; set; }
        public string? POStatus { get; set; }
        public string? Approval { get; set; }
        public string? RecordType { get; set; }
    }
    public class PurchaseOrderLimitSPReportDto
    {
        public string? ItemNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? PONumbers { get; set; }
        public string? VendorName { get; set; }
        public string? POStatus { get; set; }
        public string? Approval { get; set; }
        public string? RecordType { get; set; }
        public int? Offset { get; set; }
        public int? Limit { get; set; }
    }
    public class PurchaseOrderDate_ReportGetDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Approval { get; set; }
        public string? RecordType { get; set; }
    }
    public class PurchaseOrderDateLimitSPReportDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Approval { get; set; }
        public string? RecordType { get; set; }
        public int? Offset { get; set; }
        public int? Limit { get; set; }
    }
}
