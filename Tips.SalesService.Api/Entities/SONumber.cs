using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.SalesService.Api.Entities.DTOs;
namespace Tips.SalesService.Api.Entities
{
    public class SONumber
    {
        public int Id { get; set; }
        public int CurrentValue { get; set; }
    }
}
 
