namespace CommonModels
{
    public class CustomerType
    {
        public int? Id { get; set; }
        public string? CustomerTypeName { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;

    }
}