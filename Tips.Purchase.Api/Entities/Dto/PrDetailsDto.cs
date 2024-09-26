namespace Tips.Purchase.Api.Entities.Dto
{
    public class PrDetailsDto
    {
        public int Id { get; set; }
        public string? PRNumber { get; set; }
        public decimal? Qty { get; set; }
        public List<PrDetailDocumentUploadDto>? PrDetailDocumentUploadDtos { get; set; }
    }
    public class PrDetailsPostDto
    {
        public string? PRNumber { get; set; }
        public decimal? Qty { get; set; }
        public List<PrDetailDocumentUploadPostDto>? PrDetailDocumentUploadPostDtos { get; set; }
    }
    public class PrDetailsUpdateDto
    {
        public string? PRNumber { get; set; }
        public decimal? Qty { get; set; }
        public List<PrDetailDocumentUploadUpdateDto>? PrDetailDocumentUploadUpdateDtos { get; set; }
    }
}
