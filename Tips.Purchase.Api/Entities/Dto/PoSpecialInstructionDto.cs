using System.ComponentModel.DataAnnotations;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PoSpecialInstructionDto
    {
        [Key]
        public int Id { get; set; }
        public string? SpecialInstruction { get; set; }
    }
    public class PoSpecialInstructionPostDto
    {
        public string? SpecialInstruction { get; set; }
    }
    public class PoSpecialInstructionUpdateDto
    {
        public string? SpecialInstruction { get; set; }
    }
    public class PoSpecialInstructionShortCloseDto
    {
        //[Key]
        //public int Id { get; set; }
        public string? SpecialInstruction { get; set; }
    }
}
