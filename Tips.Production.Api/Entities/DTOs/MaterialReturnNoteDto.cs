using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Tips.Production.Api.Entities.Enums;

namespace Tips.Production.Api.Entities.DTOs
{
    public class MaterialReturnNoteDto
    {
        public int? Id { get; set; }
        public string? MRNNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public PartType ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? Unit { get; set; }
        public MaterialStatus MrnStatus { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<MaterialReturnNoteItemDto>? MaterialReturnNoteItems { get; set; }
    }

    public class MaterialReturnNotePostDto
    {
        public string? ProjectNumber { get; set; }
        public PartType ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }

        public List<MaterialReturnNoteItemPostDto>? MaterialReturnNoteItems { get; set; }

    }

    public class MaterialReturnNoteUpdateDto
    {
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }
        public PartType ShopOrderType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? Unit { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<MaterialReturnNoteItemUpdateDto>? MaterialReturnNoteItems { get; set; }
    }

    public class MaterialReturnNoteIdNameList
    {
        public int? Id { get; set; }
        public string? MRNNumber { get; set; }
    }
    public class MaterialReturnNoteSearchDto
    {
        public List<string>? ProjectNumber { get; set; }
        public List<string>? MRNNumber { get; set; }
        public List<string>? ShopOrderNumber { get; set; }
    }
    public class MRNUpdateInventoryBalanceQty
    {
        public string? PartNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? MRNNumber { get; set; }
        public List<MRNInventoryUpdateDto> MRNDetails { get; set; }
    }


    public class MRNInventoryUpdateDto
    {
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        [Precision(13, 3)]
        public decimal Qty { get; set; }
        [Precision(13, 3)]
        public decimal LocationStock { get; set; }
        public bool IsMRNIssueDone { get; set; }
    }
    public class MRNInventoryChanges
    {
        public MRNIssueDetailsofMIandMR? Data { get; set; }
        public string? Message { get; set; }
        public bool Success { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
    public class MRNIssueDetailsofMIandMR
    {
        public string ShopOrderNumber { get; set; }
        public List<MIDetailsfromMRN>? mIDetailsfromMRN { get; set; }
        public List<MRDetailsfromMRN>? mRDetailsfromMRN { get; set; }
    }
    public class MIDetailsfromMRN
    {
        public string PartNumber { get; set; }
        public decimal? QtyUsed { get; set; }
    }
    public class MRDetailsfromMRN
    {
        public string MRNumber { get; set; }
        public List<MIDetailsfromMRN>? items { get; set; }
    }
}
