using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Tips.Production.Api.Entities.Enums;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities.DTOs
{
    public class MaterialRequestsDto
    {
        public int Id { get; set; }

        public string? MRNumber { get; set; }
        public string? ProjectNumber { get; set; }
        //public string? FGItemNumber { get; set; }
        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? IssuedBy { get; set; }
        public string? IssuedTo { get; set; }
        public DateTime? IssuedOn { get; set; }
        public decimal? SumOfIssuedQty { get; set; }
        public IssuedStatus IssuedStatus { get; set; }

        [DefaultValue(0)]
        public ShortStatus StatusOfShort { get; set; }
        public MaterialStatus MrStatus { get; set; }
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public ShopOrderType ShopOrderType { get; set; }

        public List<MaterialRequestItemsDto>? MaterialRequestItems { get; set; }
    }

    public class MaterialRequestsPostDto
    {



        public string? ProjectNumber { get; set; }
  
        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }

        public string? IssuedTo { get; set; }
        public ShopOrderType ShopOrderType { get; set; }


        public List<MaterialRequestItemPostDto>? MaterialRequestItems { get; set; }
    }

    public class MaterialRequestUpdateDto
    {
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }
        //public string? FGItemNumber { get; set; }
        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string MRNumber { get; set; }
        public string? IssuedTo { get; set; }
        public string Unit { get; set; }


        public List<MaterialRequestItemUpdateDto>? MaterialRequestItems { get; set; }
    }
    public class IssueMaterialRequestUpdateDto
    {
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }
        //public string? FGItemNumber { get; set; }
        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string MRNumber { get; set; }
        public string? IssuedTo { get; set; }
        public string Unit { get; set; }


        public List<IssueMaterialRequestItemUpdateDto>? MaterialRequestItems { get; set; }
    }
    public class MaterialRequestIdNoDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "MRNumber is required")]
        public string? MRNumber { get; set; }
        public string? ProjectNumber { get; set; }
    }
    public class MaterialRequestSearchDto
    {
        public List<string>? MRNumber { get; set; }
        public List<string>? ProjectNumber { get; set; }
        public List<string>? FGShopOrderNumber { get; set; }
        public List<string>? SAShopOrderNumber { get; set; }
        public List<string>? ShopOrderNumber { get; set; }

    }
    public class MaterialRequestsReportDto
    {
        public int Id { get; set; }

        public string? MRNumber { get; set; }
        public string? ProjectNumber { get; set; }
        //public string? FGItemNumber { get; set; }
        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? IssuedBy { get; set; }
        public DateTime? IssuedOn { get; set; }

        public IssuedStatus IssuedStatus { get; set; }

        [DefaultValue(0)]
        public ShortStatus StatusOfShort { get; set; }
        public MaterialStatus MrStatus { get; set; }
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<MaterialRequestItemsReportDto>? MaterialRequestItems { get; set; }
    }
    public class MaterialRequestReportWithParamDto
    {
        public string? MRNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? KPN { get; set; }
        public string? ShopOrderNumber { get; set; }
    }
    public class MaterialRequestReportWithParamDtoForTrans
    {
        public string? MRNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? Itemnumber { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? IssuedStatus { get; set; }
    }
    public class MaterialRequestReportWithDateDtoForTrans
    {
        public  DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? IssuedStatus { get; set; }
    }
    public class MaterialIssueAgainstMaterialRequestReportWithParamDto
    {
        public string? MRNumber { get; set; }
        public string? ProjectType {  get; set; }
        public string? ProjectNumber { get; set; }
        public string? KPN { get; set; }
        public string? ShopOrderNumber { get; set; }
    }
    public class MaterialRequestSpReportForTrans
    {
        public string? MRNumber { get; set; }
        public string? ProjectType { get; set; }
        public int? Itemtype { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
        public decimal? RequestQty { get; set; }
        public string? Itemnumber { get; set; }
        public string? MPN { get; set; }
        public string? Description { get; set; }
        public decimal? IssuedQty { get; set; }
        public string? Uom { get; set; }
        public DateTime? MRDate { get; set; }
        public int? MrStatus { get; set; }
    }
    public class MaterialIssueAgainstMRSPReportForkeus
    {
        public string? MRNumber { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ProjectType { get; set; }
        public PartType? ItemType { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? indent_qnty { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? Description { get; set; }
        public decimal? Qty { get; set; }
        public string? Uom { get; set; }
        public string? IssuedBy { get; set; }
        public string? CreatedBy { get; set; }


    }


}


