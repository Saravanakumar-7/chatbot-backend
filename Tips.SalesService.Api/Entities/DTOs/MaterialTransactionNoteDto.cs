namespace Tips.SalesService.Api.Entities.DTOs
{
    public class MaterialTransactionNoteDto
    {
        public int? Id { get; set; }
        public string? MTNNumber { get; set; }
        public string? ProjectNUmber { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<MaterialTransactionNoteItemDto>? MaterialTransactionNoteItemDtos { get; set; }
    }

    public class MaterialTransactionNotePostDto
    {
        public string? MTNNumber { get; set; }
        public string? ProjectNUmber { get; set; }       

        public List<MaterialTransactionNoteItemPostDto>? MaterialTransactionNoteItemPostDtos { get; set; }
    }

    public class MaterialTransactionNoteUpdateDto
    {
        public int? Id { get; set; }
        public string? MTNNumber { get; set; }
        public string? ProjectNUmber { get; set; }

        public string Unit { get; set; }      

        public List<MaterialTransactionNoteItemUpdateDto>? MaterialTransactionNoteItemUpdateDtos { get; set; }
    }
}