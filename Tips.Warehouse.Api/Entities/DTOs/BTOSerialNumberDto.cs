namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class BTOSerialNumberDto
    {
        public int? Id { get; set; }
        public string? SerialNumber { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class BTOSerialNumberDtoPost
    {
        public string? SerialNumber { get; set; }
    
    }
    public class BTOSerialNumberDtoUpdate
    {

        public string? SerialNumber { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
