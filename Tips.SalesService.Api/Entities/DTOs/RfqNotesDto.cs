using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class RfqNotesDto
    {
        public int Id { get; set; }
        public string? FromCSCategory { get; set; }

        public string? FromEnggCategory { get; set; }

        public string? FromEnggNotes { get; set; }
        public string? FromCSNotes { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class RfqNotesPostDto
    {
        [StringLength(500, ErrorMessage = "FromCSCategory can't be longer than 500 characters")]

        public string? FromCSCategory { get; set; }

        [StringLength(500, ErrorMessage = "FromEnggCategory can't be longer than 500 characters")]

        public string? FromEnggCategory { get; set; }

        [StringLength(500, ErrorMessage = "FromEnggNotes can't be longer than 500 characters")]

        public string? FromEnggNotes { get; set; }
        [StringLength(500, ErrorMessage = "FromCSNotes can't be longer than 500 characters")]

        public string? FromCSNotes { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class RfqNotesUpdateDto
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "FromCSCategory can't be longer than 500 characters")]

        public string? FromCSCategory { get; set; }

        [StringLength(500, ErrorMessage = "FromEnggCategory can't be longer than 500 characters")]

        public string? FromEnggCategory { get; set; }

        [StringLength(500, ErrorMessage = "FromEnggNotes can't be longer than 500 characters")]

        public string? FromEnggNotes { get; set; }
        [StringLength(500, ErrorMessage = "FromCSNotes can't be longer than 500 characters")]

        public string? FromCSNotes { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
