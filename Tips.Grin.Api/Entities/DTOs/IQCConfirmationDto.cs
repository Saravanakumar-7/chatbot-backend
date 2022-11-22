using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class IQCConfirmationDto
    {
        public int Id { get; set; }

        public string? GrinNumber { get; set; }

        public string? ItemNumber { get; set; }

        public int ItemId { get; set; }

        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal? AcceptedQunatity { get; set; }

        [Precision(13, 3)]
        public decimal? RejectedQunatity { get; set; }

        public bool IsBinningDone { get; set; } = false;

        public bool IsDeleted { get; set; } = false;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public string Unit { get; set; }

        public List<GrinPartsDto>? GrinPartsDtos { get; set; }

    }
    public class IQCConfirmationPostDto
    {
        public string? GrinNumber { get; set; }

        public string? ItemNumber { get; set; }

        public int ItemId { get; set; }

        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal? AcceptedQunatity { get; set; }

        [Precision(13, 3)]
        public decimal? RejectedQunatity { get; set; }

        public bool IsBinningDone { get; set; } = false;

        public bool IsDeleted { get; set; } = false;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public string Unit { get; set; }
        public List<GrinPartsPostDto>? grinPartsPostDtos { get; set; }

    }
    public class IQCConfirmationUpdateDto
    {
    public int Id { get; set; }
    
    public string? GrinNumber { get; set; }

    public string? ItemNumber { get; set; }

    public int ItemId { get; set; }

    public string? ProjectNumber { get; set; }

    [Precision(13, 3)]
    public decimal? AcceptedQunatity { get; set; }

    [Precision(13, 3)]
    public decimal? RejectedQunatity { get; set; }

    public bool IsBinningDone { get; set; } = false;

    public bool IsDeleted { get; set; } = false;
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }

    public string Unit { get; set; }
     public List<GrinPartsUpdateDto>? GrinPartsUpdateDtos { get; set; }

    }




}
