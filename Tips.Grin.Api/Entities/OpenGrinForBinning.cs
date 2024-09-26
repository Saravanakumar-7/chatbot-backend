using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class OpenGrinForBinning
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

        public List<OpenGrinForBinningItems>? OpenGrinForBinningItems { get; set; }
    }
}
