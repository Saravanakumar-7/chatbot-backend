using System.ComponentModel.DataAnnotations.Schema;
using Tips.SalesService.Api.Entities.Dto;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class CollectionTrackerDto
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string? Remarks { get; set; }
        public decimal TotalSumOfSOAmount { get; set; }
        public decimal AmountRecieved { get; set; }
        public decimal AlreadyRecieved { get; set; }
        public decimal AlreadyRecievedWithPercentage { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentRefNo { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<SOBreakDownDto>? SOBreakDown { get; set; }
    }
    public class CollectionTrackerPostDto
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalSumOfSOAmount { get; set; }
        public string? Remarks { get; set; }
        public decimal AmountRecieved { get; set; }
        public decimal AlreadyRecieved { get; set; }
        public decimal AlreadyRecievedWithPercentage { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentRefNo { get; set; }
        public List<SOBreakDownPostDto>? SOBreakDown { get; set; }

    }
    public class CollectionTrackerUpdateDto
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string? Remarks { get; set; }
        public decimal TotalSumOfSOAmount { get; set; }
        public decimal AmountRecieved { get; set; }
        public decimal AlreadyRecieved { get; set; }
        public decimal AlreadyRecievedWithPercentage { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentRefNo { get; set; }
        public string Unit { get; set; }
        public List<SOBreakDownUpdateDto>? SOBreakDown { get; set; }

    }

    public class CollectionTrackerDetailsDto
    {
        public decimal? TotalSumOfSOAmount { get; set; }
        public decimal AlreadyRecieved { get; set; }
        public List<OpenSalesOrderDetailsDto>? OpenSalesOrderDetails { get; set; }
    }
    public class OpenSalesOrderDetailsDto
    {
       public int SalesOrderId { get; set; }
        public string SalesOrderNo { get; set; }
        public string TypeOfSolution { get; set; }
        public decimal? TotalValue { get; set; }
        public decimal? PendingValue { get; set; }
        public decimal AmountRecieved { get; set; }
    }

    public class OpenSalesOrderDetailsForKeusDto
    {
        public int SalesOrderId { get; set; }
        public string SalesOrderNo { get; set; }
        public string TypeOfSolution { get; set; }
        public decimal? TotalValue { get; set; }
        public decimal? PendingValue { get; set; }
        public decimal AmountRecieved { get; set; }
    }

    public class CollectionTrackerSearchDto
    {
        public List<string> CustomerId { get; set; }
        public List<string> CustomerName { get; set; }
        public List<string>? Remarks { get; set; }
    }
    public class CollectionTrackerSPResportSPResportDTO
    {
        public string? CustomerId { get; set; }
    }
    
    public class CollectionTrackerByCustomerIdSPReportDTO
    {
        public string? CustomerId { get; set; }
    }
    public class CollectionTrackerWithCustomerWiseSPReportDTO
    {
        public string? CustomerId { get; set; }
    }
    public class CollectionTrackerWithSalesOrderNoWiseSPReportDTO
    {
        public string? SalesOrderNumber { get; set; }
    }
    public class AdvanceReceivedEntryLevelSPResportDTO
    {
        public string? CustomerId { get; set; }
        public string? TypeOfSolution { get; set; }
    }
    public class AdvanceReceivedEntryLevelSPResport
    {
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public DateTime? so_createddate { get; set; }
        public decimal? TotalSumOfSOAmount { get; set; }
        public decimal? AlreadyRecieved { get; set; }
        public decimal? AmountRecieved { get; set; }
        public decimal? TotalAdvance { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public DateTime? entrydate { get; set; }
    }

    public class FirstAdvanceReceivedEntryLevelSPResport
    {
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public DateTime? so_createddate { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public decimal? TotalSumOfSOAmount { get; set; }
        public decimal? AdvanceReceived { get; set; }
        public decimal? Percentage_of_Adv_rec { get; set; }
        public DateTime? entrydate { get; set; }
        public string? CreatedBy { get; set; }
        public string? PaymentMode { get; set; }
        public string? PaymentRefNo { get; set; }
    }

    public class LatestAdvanceReceivedEntryLevelSPResport
    {
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public DateTime? so_createddate { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public decimal? TotalSumOfSOAmount { get; set; }
        public decimal? AdvanceReceived { get; set; }
        public decimal? Percentage_of_Adv_rec { get; set; }
    }

}
