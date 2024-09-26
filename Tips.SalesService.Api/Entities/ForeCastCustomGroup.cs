using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities
{
    public class ForeCastCustomGroup
    {
        [Key]
        public int Id { get; set; }
        public string CustomGroupName { get; set; }
        public string Remarks { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
