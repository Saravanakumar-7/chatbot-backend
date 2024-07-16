namespace Tips.Grin.Api.Entities.Dto
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

}
