using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Grin.Api.Entities
{
    public class IQCConfirmation
    {
        [Key]
        public int Id { get; set; }
        public string GrinNumber { get; set; }
        public int GrinId { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<IQCConfirmationItems>? IQCConfirmationItems { get; set; }


       

        


    }
}
