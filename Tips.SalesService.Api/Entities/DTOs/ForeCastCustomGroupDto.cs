using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class ForeCastCustomGroupDto
    {
        public int Id { get; set; }
        public string CustomGroupName { get; set; }
        public string Remarks { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class ForeCastCustomGroupDtoPost
    {
        [Required(ErrorMessage = "CustomGroupName is required")]
        [StringLength(500, ErrorMessage = "CustomGroupName can't be longer than 500 characters")]
        public string CustomGroupName { get; set; }
        public string Remarks { get; set; }
       
    }
    public class ForeCastCustomGroupDtoUpdate
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "CustomGroupName is required")]
        [StringLength(500, ErrorMessage = "CustomGroupName can't be longer than 500 characters")]
        public string CustomGroupName { get; set; }
        public string Remarks { get; set; }
        public string Unit { get; set; }
    }

    public class ListOfForecastCustomGroupDto
    {
        public int Id { get; set; }
        public string? CustomGroupName { get; set; }
    }

}
