namespace Tips.Production.Api.Entities.DTOs
{
    public class EmailIDsDto
    {
        public Datum[] data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }

    public class Datum
    {
        public int id { get; set; }
        public string operation { get; set; }
        public string emailIds { get; set; }
        public string? host { get; set; }
        public int? port { get; set; }
        public string? password { get; set; }
    }
    public class Data1
    {
        public int id { get; set; }
        public string processType { get; set; }
        public string template { get; set; }
        public string subject { get; set; }
    }
    public class EmailTemplateDto
    {
        public Data1 data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public int statusCode { get; set; }
    }
}
