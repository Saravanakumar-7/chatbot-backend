using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class Addproject
    {
        public int Id { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectQuantity { get; set; }
        public string Project1{ get; set; }

    }
}
