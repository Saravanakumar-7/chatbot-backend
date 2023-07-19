namespace Tips.Grin.Api.Entities.DTOs
{
    public class OpenGrinDetailsDto
    {
        public int Id { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
    }
    public class OpenGrinDetailsPostDto
    {
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
    }
    public class OpenGrinDetailsUpdateDto
    {
        //public int Id { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
    }
}
