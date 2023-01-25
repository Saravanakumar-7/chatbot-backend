namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class DoSerialNumberDto
    {
        public int? Id { get; set; }
        public string SerialNumber { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class DoSerialNumberDtoPost
    {
      
        public string SerialNumber { get; set; }
        
    }
    public class DoSerialNumberDtoUpdate
    {
        public int? Id { get; set; }
        public string SerialNumber { get; set; }       
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
