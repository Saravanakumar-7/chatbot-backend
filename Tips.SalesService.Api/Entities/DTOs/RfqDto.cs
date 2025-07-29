using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities.Enum;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class RfqDto
    {
        public int Id { get; set; }
        public string? LeadId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }

        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }
        public string RfqNumber { get; set; }
        public string? SalesPerson { get; set; }
        public string? CustomerRfqNumber { get; set; }
        public string? CustomerId { get; set; }
        public bool? isSourcingAvailable { get; set; }
        public bool? IsCsComplete { get; set; }
        public CsStatus CsComplete { get; set; }
        public CsRelease IsCsRelease { get; set; }
        public bool? IsEnggComplete { get; set; }
        public EnggStatus EnggComplete { get; set; }
        public CsRelease IsEnggRelease { get; set; }
        public bool? IsSourcing { get; set; }
        public bool? IsLpCosting { get; set; }
        public bool? IsLpRelease { get; set; } 

        [DefaultValue(0)]
        public CsRelease ReleaseStatus { get; set; }

        public string? ReasonForModification { get; set; }

        public DateTime? RequestReceivedate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public bool IsModified { get; set; }
        public string? SBU { get; set; }
        public string Unit { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class RfqPostDto
    {
        public string? LeadId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        //[Required]
        public string? RfqNumber { get; set; }
        public string? SalesPerson { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerRfqNumber { get; set; }
        public DateTime? RequestReceivedate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public string? SBU { get; set; }

    }
    public class RfqUpdateDto
    {
        public string? LeadId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        //[Required]
        public string RfqNumber { get; set; }
        public string? SalesPerson { get; set; }
        public string? ReasonForModification { get; set; }

        public string? CustomerId { get; set; }
        public string? CustomerRfqNumber { get; set; }
        public DateTime? RequestReceivedate { get; set; }
        public DateTime? QuoteExpectdate { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public string? SBU { get; set; }
        public string Unit { get; set; }
        public string? Remarks { get; set; }

    }
    public class RfqNumberListDto
    {
        public int Id { get; set; }       
        public string RfqNumber { get; set; }
        public string? SalesPerson { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public decimal? RevisionNumber { get; set; }
        public CsRelease IsEnggReleased { get; set; }
    }
    public class RevNumberByRfqNumberListDto
    {
        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }
    }
    public class LatestRfqNumberListDto
    {
        public decimal? RevisionNumber { get; set; }
        public string RfqNumber { get; set; }
        public string? SalesPerson { get; set; }
    }
    public class RfqSpReportDto
    {
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public string? RfqNumber { get; set; }
    }

    public class RFQSalesorderConfirmationSPReportDTO
    {
        public string? CustomerName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? KPN { get; set; }
        public string? SOStatus { get; set; }
        public string? ProjectNumber { get; set; }
    }
    public class RFQSalesorderConfirmationSPReport
    {
        public string? SalesOrderNumber { get; set; }
        public int? SOStatus { get; set; }
        public string? ProjectNumber { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? LeadId { get; set; }
        public string? OrderType { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? ProductType { get; set; }
        public string? MaterialGroup { get; set; }
        public string? ItemType { get; set; }
        public string? SalesPerson { get; set; }
        public DateTime? SODate { get; set; }
        public string? KPN { get; set; }
        public string? KPNDescription { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public string? PriceList { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? BasicAmount { get; set; }
        public string? DiscountType { get; set; }
        public string? Discount { get; set; }
        public decimal? SGST { get; set; }
        public decimal? CGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? UTGST { get; set; }
        public DateTime? RequestedDate { get; set; }
        public decimal? ItemPriceList { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? OrderQty { get; set; }
        public decimal? DispatchQty { get; set; }
        public decimal? BalanceQty { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public decimal? ConfirmationQty { get; set; }
    }
    public class ODORfqNumberListDto
    {
        public int Id { get; set; }
        public string? RfqNumber { get; set; }
    }
}
