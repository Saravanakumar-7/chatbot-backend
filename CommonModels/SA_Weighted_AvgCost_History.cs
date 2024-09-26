using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SA_Weighted_AvgCost_History
    {
        public int Id { get; set; }
        public string Itemnumber { get; set; }
        public decimal Version { get; set; }
        public decimal Avg_cost { get; set; }
        public DateTime update_date_time { get; set; }
    }
}
