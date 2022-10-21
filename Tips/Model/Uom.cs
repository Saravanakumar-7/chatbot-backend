using System.ComponentModel.DataAnnotations;

namespace Tips.Model
{
    public class Uom
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UomName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }
}
