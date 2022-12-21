namespace Tips.SalesService.Api.Entities.DTOs
{
    public class MaterialTransactionNoteDto
    {
        public int? Id { get; set; }
        public string? MTNNO { get; set; }
        public string? ProjectNo { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<MaterialTransactionNoteItemDto>? MaterialTransactionNoteItems { get; set; }
    }

    public class MaterialTransactionNoteDtoPost
    {
        public string? MTNNO { get; set; }
        public string? ProjectNo { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<MaterialTransactionNoteItemDtoPost>? MaterialTransactionNoteItems { get; set; }
    }

    public class MaterialTransactionNoteDtoUpdate
    {
        public int? Id { get; set; }
        public string? MTNNO { get; set; }
        public string? ProjectNo { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<MaterialTransactionNoteItemDtoUpdate>? MaterialTransactionNoteItems { get; set; }
    }
}