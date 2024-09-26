namespace Tips.Purchase.Api.Entities.Dto
{
    public class PrSpecialInstructionDto
    {
        public int Id { get; set; }
        public string? SpecialInstruction { get; set; }
    }
    public class PrSpecialInstructionPostDto
    {
        public string? SpecialInstruction { get; set; }
    }
    public class PrSpecialInstructionUpdateDto
    {
        public string? SpecialInstruction { get; set; }
    }
}
