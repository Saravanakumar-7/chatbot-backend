using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class OpenGrinForBinningDto
    {
        [Key]
        public int Id { get; set; }
        public string? OpenGrinNumber { get; set; }
        public bool IsOpenGrinForBinningCompleted { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<OpenGrinForBinningItemsDto>? OpenGrinForBinningItems { get; set; }
    }
    public class OpenGrinForBinningPostDto
    {
        public string? OpenGrinNumber { get; set; }

        public List<OpenGrinForBinningItemsPostDto>? OpenGrinForBinningItems { get; set; }
    }
    public class OpenGrinForBinningUpdateDto
    {
        public int Id { get; set; }
        public string? OpenGrinNumber { get; set; }

        public List<OpenGrinForBinningItemsUpdateDto>? OpenGrinForBinningItems { get; set; }
    }
    public class OpenGrinForBinningSaveDto
    {
        public string? OpenGrinNumber { get; set; }


        public OpenGrinForBinningItemsSaveDto? OpenGrinForBinningItemsSaveDto { get; set; }
    }
}
