using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IGST_PercentageRepository : IRepositoryBase<GST_Percentage>
    {
        Task<IEnumerable<GST_Percentage>> GetAllGST_Percentages();
        Task<GST_Percentage> GetGST_PercentageById(int id);
        Task<IEnumerable<GST_Percentage>> GetAllActiveGST_Percentages();
        Task<int?> CreateGST_Percentage(GST_Percentage gst_Percentage);
        Task<string> UpdateGST_Percentage(GST_Percentage gst_Percentage);
        Task<string> DeleteGST_Percentage(GST_Percentage gst_Percentage);
    }
}
