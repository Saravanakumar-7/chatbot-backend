using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class ProjectNumbersDto
    {
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }
        [Precision(18, 3)]
        public decimal? ProjectQty { get; set; }
        //[Precision(18, 3)]
        //public decimal? RemainingAccptedQty { get; set; }
        [Precision(13, 3)]
        public decimal RejectReturnQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }

    public class ProjectNumbersDtoPost
    {
        public string? ProjectNumber { get; set; }


        [Precision(18, 3)]

        public decimal? ProjectQty { get; set; }

    }

    public class ProjectNumbersDtoUpdate
    {

        public string? ProjectNumber { get; set; }


        [Precision(18, 3)]
        public decimal? ProjectQty { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class ProjectNumbersReportDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? GrinNumber { get; set; }
        public string? ProjectNumber { get; set; }


        [Precision(18, 3)]
        public decimal? ProjectQty { get; set; }


    }
    public class GrinUpdateProjectBalQtyDetailsDto
    {
        public string ItemNumber { get; set; }
        public string ProjectNumber { get; set; }
        public decimal ProjectQty { get; set; }
        public int PoItemId { get; set; }

    }
    public class ProjectNumberscalculationofAvgcostDto
    {
        public string? ProjectNumber { get; set; }


        [Precision(18, 3)]
        public decimal? ProjectQty { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
