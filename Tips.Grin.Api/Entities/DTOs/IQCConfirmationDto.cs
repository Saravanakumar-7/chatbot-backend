using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Entities.Enums;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class IQCConfirmationDto
    {
        public int Id { get; set; }

        public string? GrinNumber { get; set; }
        [Required]
        public string VendorName { get; set; }

        [Required]
        public string VendorId { get; set; }

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
        public int GrinId { get; set; }

        public string Unit { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<IQCConfirmationItemsDto>? IQCConfirmationItems { get; set; }

    }
    public class IQCConfirmationPostDto
    {
        public string GrinNumber { get; set; }
        public int GrinId { get; set; } 

        public List<IQCConfirmationItemsPostDto>? IQCConfirmationItemsPostDtos { get; set; }

    }

    public class IQCConfirmationUpdateDto
    {

        public int Id { get; set; }
        public string GrinNumber { get; set; }
        public int GrinId { get; set; }
        public string Unit { get; set; } 
        public List<IQCConfirmationItemsUpdateDto>? IQCConfirmationItemsUpdateDtos { get; set; }

    }
    public class IQCConfirmationIdNameListDto
    {
        public int Id { get; set; }
        public string? GrinNumber { get; set; }
    }
    public class IQCConfirmationSearchDto
    {
        public List<string>? GrinNumber { get; set; }
        public List<string>? VendorName { get; set; }
        public List<string>? VendorId { get; set; }
        public List<string>? InvoiceNumber { get; set; }
    }
    public class IQCConfirmationReportDto
    {
        public int Id { get; set; }

        public string? GrinNumber { get; set; }
        [Required]
        public string VendorName { get; set; }

        [Required]
        public string VendorId { get; set; }

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
        public int GrinId { get; set; }

        public string Unit { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<IQCConfirmationItemsReportDto>? IQCConfirmationItems { get; set; }

    }
    public class IQCConfirmationSaveDto
    {
        public string? GrinNumber { get; set; }
        public int GrinId { get; set; }

        public IQCConfirmationItemsSaveDto? IQCConfirmationItemsPostDtos { get; set; }

    }
    public class IQCInventoryTranctionDto
    {
        public string PartNumber { get; set; }
        public string? LotNumber { get; set; }

        public string MftrPartNumber { get; set; }

        public bool IsStockAvailable { get; set; }
        public string Description { get; set; }


        public string ProjectNumber { get; set; }
        public decimal Issued_Quantity { get; set; }
        public string? UOM { get; set; }

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

    }
    public class IQCConfirmationReportWithParamDto
    {
        public string? GrinNumber { get; set; }
        public string? ItemNumber { get; set; }
    }
}
