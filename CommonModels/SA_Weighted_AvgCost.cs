using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SA_Weighted_AvgCost
    {
        public int Id { get; set; }
        public string? Itemnumber { get; set; }
        public int Version { get; set; }
        public int Avg_cost { get; set; }
        public DateOnly? update_date { get; set; }
        public TimeOnly? updated_time { get; set; }
    }
}
