using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class BinningDto
    {
        public int Id { get; set; }
        public string? GrinNo { get; set; }
        public string? ItemNo { get; set; }
        public int ItemId { get; set; }
        public string ProjectNo { get; set; }

        [Precision(13, 3)]
        public decimal? Qunatity { get; set; }

        public string Location { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? LastModifiedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public string Unit { get; set; }

    }


    public class BinningPostDto
    {
        
        public string? GrinNo { get; set; }
        public string? ItemNo { get; set; }
        public int ItemId { get; set; }
        public string ProjectNo { get; set; }
        [Precision(13, 3)]
        public decimal? Qunatity { get; set; }
        public string Location { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string Unit { get; set; }

    }

    public class BinningUpdateDto
    {

        public string? GrinNo { get; set; }
        public string? ItemNo { get; set; }
        public int ItemId { get; set; }
        public string ProjectNo { get; set; }
        [Precision(13, 3)]
        public decimal? Qunatity { get; set; }
        public string Location { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string Unit { get; set; }

    }

}
