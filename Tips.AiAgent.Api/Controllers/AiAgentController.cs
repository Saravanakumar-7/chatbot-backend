using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Tips.AiAgent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AiAgentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AiAgentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            try
            {
                var text = request.Text?.Trim() ?? "";
                var lower = text.ToLower();

                if (lower == "hi" || lower == "hello")
                {
                    return Ok(new ChatResponse
                    {
                        Text = "Hello! 👋<br/><br/>How can I help you today?",
                        IsBot = true
                    });
                }

                if (lower.Contains("open delivery order report") || lower == "2")
                {
                    var reportData = await FetchReportData("tras_getapcs_warehouse.Open_Delivery_Order_Report_withparameter_tras", new Dictionary<string, string>
                    {
                        { "@p_OpenDONumber", "" }, { "@p_CustomerName", "" }, { "@p_IssuedTo", "" }, { "@p_ItemNumber", "" }, { "@p_MPN", "" },
                        { "@p_Warehouse", "" }, { "@p_Location", "" }, { "@p_ODOType", "" }, { "@p_ProjectNumber", "" }
                    });
                    return GenerateReportResponse("Open Delivery Order Report", "odoreport", reportData.Count);
                }
                else if (lower.Contains("odo return report") || lower == "4")
                {
                    var reportData = await FetchReportData("tras_getapcs_warehouse.Return_Open_DeliveryOrder_Report_withparameters_tras", new Dictionary<string, string>
                    {
                        { "@p_DONumber", "" }, { "@p_CustomerName", "" }, { "@p_CustomerAliasName", "" }, { "@p_LeadId", "" },
                        { "@p_IssuedTo", "" }, { "@p_Location", "" }, { "@p_Warehouse", "" }, { "@p_KPN", "" }, { "@p_MPN", "" },
                        { "@p_ODOType", "" }, { "@p_ProjectNumber", "" }
                    });
                    return GenerateReportResponse("ODO Return Report", "returnodoreport", reportData.Count);
                }
                else if (lower.Contains("do return report") || lower == "3")
                {
                    var reportData = await FetchReportData("tras_getapcs_warehouse.returndeliveryorder_with_returntable_with_parameters_tras", new Dictionary<string, string>
                    {
                        { "@p_ReturnBTONumber", "" }, { "@p_CustomerName", "" }, { "@p_CustomerAliasName", "" }, { "@p_CustomerLeadid", "" },
                        { "@p_SalesOrderNumber", "" }, { "@p_ProductType", "" }, { "@p_TypeOfSolution", "" }, { "@p_Warehouse", "" },
                        { "@p_Location", "" }, { "@p_KPN", "" }, { "@p_MPN", "" }, { "@p_ProjectNumber", "" }
                    });
                    return GenerateReportResponse("DO Return Report", "returndoreport", reportData.Count);
                }
                else if (lower.Contains("work order") && lower.Contains("delivery order") || lower == "5")
                {
                    var reportData = await FetchReportData("tras_getapcs_warehouse.DO_Report_withparameter_for_akash", new Dictionary<string, string>
                    {
                        { "@p_DONumber", "" }, { "@p_CustomerName", "" }, { "@p_SalesOrderNumber", "" }, { "@p_ProductType", "" },
                        { "@p_Warehouse", "" }, { "@p_Location", "" }, { "@p_ItemNumber", "" }, { "@p_MPN", "" }, { "@p_ProjectNumber", "" }
                    });
                    return GenerateReportResponse("Delivery Order With Work Order No Report", "doworkorderreport", reportData.Count);
                }
                else if (lower.Contains("delivery order report") || lower == "1")
                {
                    var reportData = await FetchDoReportData();
                    return GenerateReportResponse("Delivery Order Report", "doreport", reportData.Count);
                }

                // Intent detection: check if the user is asking for a Delivery Order report
                // Added spelling tolerance for "delvery", "deliver", etc.
                var doKeywords = new[] { "delivery", "deliver", "doreport", "do report", "d.o", "i need do", "i need the do", "i need delivery", "the do" };
                bool isBroadDoRequest = doKeywords.Any(k => lower.Contains(k));

                if (isBroadDoRequest)
                {
                    return Ok(new ChatResponse
                    {
                        Text = "Sure 👌<br/><br/>I can help you with <strong>Delivery Order reports</strong>. 📦<br/><br/>" +
                               "I found several <strong>reports in this category</strong> available in the system:<br/><br/>" +
                               "1. <strong>Delivery Order Report</strong><br/>" +
                               "2. <strong>Open Delivery Order Report</strong><br/>" +
                               "3. <strong>DO Return Report</strong><br/>" +
                               "4. <strong>ODO Return Report</strong><br/>" +
                               "5. <strong>Delivery Order With Work Order No Report</strong><br/><br/>" +
                               "📌 <strong>Which specific report do you need?</strong><br/><br/>" +
                               "You can type the full name, for example:<br/>" +
                               "• \"Open Delivery Order Report\"",
                        IsBot = true
                    });
                }

                // Help / Capability Intent
                var helpKeywords = new[] { "help", "guide", "what can you do", "capabilities", "what reports", "how to" };
                if (helpKeywords.Any(k => lower.Contains(k)) || lower == "reports")
                {
                    return Ok(new ChatResponse
                    {
                        Text = "I'm your <strong>AI Report Assistant</strong>! 📊<br/><br/>" +
                               "I can currently help you generate and download <strong>Delivery Order</strong> related reports.<br/><br/>" +
                               "🚀 <strong>To get started:</strong><br/>" +
                               "Just type the name of the report you need, or type <strong>\"Delivery Order\"</strong> to see all available reports in that category.<br/><br/>" +
                               "💡 <em>I'll be able to support Sales, Purchase, and GRIN reports very soon!</em>",
                        IsBot = true
                    });
                }

                // Smart Fallback (Direct & Scalable)
                return Ok(new ChatResponse
                {
                    Text = "I'm sorry, I don't recognize that command. 🧐<br/><br/>" +
                           "Please <strong>type the correct keywords</strong> or the report name so I can help you accurately.<br/><br/>" +
                           "You can try typing <strong>\"Reports\"</strong> to see what I can generate, or type <strong>\"Help\"</strong> for a quick guide.",
                    IsBot = true
                });
            }
            catch (Exception ex)
            {
                return Ok(new ChatResponse
                {
                    Text = $"Error: {ex.Message}",
                    IsBot = true
                });
            }
        }

        private IActionResult GenerateReportResponse(string reportTitle, string action, int count)
        {
            return Ok(new ChatResponseWithAction
            {
                Text = $"✅ <strong>{reportTitle} Generated</strong><br/><br/>" +
                       $"Your <strong>{reportTitle}</strong> is ready. 📦<br/>" +
                       $"The report contains <strong>{count} records</strong> based on the latest available data.<br/><br/>" +
                       "📥 <strong>Would you like to download the report in Excel format?</strong><br/><br/>" +
                       "You can reply with \"Yes, download Excel\" to download the file.",
                IsBot = true,
                Action = action,
                RecordCount = count
            });
        }

        [AllowAnonymous]
        [HttpGet("doreport/excel")]
        public async Task<IActionResult> DownloadDoReportExcel()
        {
            return await GenerateExcelReport("DeliveryOrderReport", "tras_getapcs_warehouse.Delivery_Order_Report_withparameter_tras", new Dictionary<string, string>
            {
                { "@p_DONumber", "" }, { "@p_CustomerName", "" }, { "@p_SalesOrderNumber", "" }, { "@p_ProductType", "" },
                { "@p_Warehouse", "" }, { "@p_Location", "" }, { "@p_ItemNumber", "" }, { "@p_MPN", "" }, { "@p_ProjectNumber", "" }
            });
        }

        [AllowAnonymous]
        [HttpGet("odoreport/excel")]
        public async Task<IActionResult> DownloadOdoReportExcel()
        {
            return await GenerateExcelReport("OpenDeliveryOrderReport", "tras_getapcs_warehouse.Open_Delivery_Order_Report_withparameter_tras", new Dictionary<string, string>
            {
                { "@p_OpenDONumber", "" }, { "@p_CustomerName", "" }, { "@p_IssuedTo", "" }, { "@p_ItemNumber", "" }, { "@p_MPN", "" },
                { "@p_Warehouse", "" }, { "@p_Location", "" }, { "@p_ODOType", "" }, { "@p_ProjectNumber", "" }
            });
        }

        [AllowAnonymous]
        [HttpGet("returndoreport/excel")]
        public async Task<IActionResult> DownloadReturnDoReportExcel()
        {
            return await GenerateExcelReport("ReturnDOReport", "tras_getapcs_warehouse.returndeliveryorder_with_returntable_with_parameters_tras", new Dictionary<string, string>
            {
                { "@p_ReturnBTONumber", "" }, { "@p_CustomerName", "" }, { "@p_CustomerAliasName", "" }, { "@p_CustomerLeadid", "" },
                { "@p_SalesOrderNumber", "" }, { "@p_ProductType", "" }, { "@p_TypeOfSolution", "" }, { "@p_Warehouse", "" },
                { "@p_Location", "" }, { "@p_KPN", "" }, { "@p_MPN", "" }, { "@p_ProjectNumber", "" }
            });
        }

        [AllowAnonymous]
        [HttpGet("returnodoreport/excel")]
        public async Task<IActionResult> DownloadReturnOdoReportExcel()
        {
            return await GenerateExcelReport("ReturnOpenDOReport", "tras_getapcs_warehouse.Return_Open_DeliveryOrder_Report_withparameters_tras", new Dictionary<string, string>
            {
                { "@p_DONumber", "" }, { "@p_CustomerName", "" }, { "@p_CustomerAliasName", "" }, { "@p_LeadId", "" },
                { "@p_IssuedTo", "" }, { "@p_Location", "" }, { "@p_Warehouse", "" }, { "@p_KPN", "" }, { "@p_MPN", "" },
                { "@p_ODOType", "" }, { "@p_ProjectNumber", "" }
            });
        }

        [AllowAnonymous]
        [HttpGet("doworkorderreport/excel")]
        public async Task<IActionResult> DownloadDoWorkOrderReportExcel()
        {
            return await GenerateExcelReport("DOWorkOrderReport", "tras_getapcs_warehouse.DO_Report_withparameter_for_akash", new Dictionary<string, string>
            {
                { "@p_DONumber", "" }, { "@p_CustomerName", "" }, { "@p_SalesOrderNumber", "" }, { "@p_ProductType", "" },
                { "@p_Warehouse", "" }, { "@p_Location", "" }, { "@p_ItemNumber", "" }, { "@p_MPN", "" }, { "@p_ProjectNumber", "" }
            });
        }

        private async Task<IActionResult> GenerateExcelReport(string reportName, string spName, Dictionary<string, string> parameters)
        {
            try
            {
                var reportData = await FetchReportData(spName, parameters);

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add(reportName);

                if (reportData.Count > 0)
                {
                    var headers = reportData[0].Keys.ToArray();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = worksheet.Cell(1, i + 1);
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.SteelBlue;
                        cell.Style.Font.FontColor = XLColor.White;
                    }

                    for (int row = 0; row < reportData.Count; row++)
                    {
                        var record = reportData[row];
                        for (int col = 0; col < headers.Length; col++)
                        {
                            var value = record[headers[col]];
                            worksheet.Cell(row + 2, col + 1).Value = value?.ToString() ?? "";
                        }
                    }
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                return File(
                    stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"{reportName}.xlsx"
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        public async Task<List<Dictionary<string, object?>>> FetchReportData(string spName, Dictionary<string, string> parameters)
        {
            var connectionString = _configuration["MySqlconnection:connectionString"];
            var results = new List<Dictionary<string, object?>>();

            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(spName, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                results.Add(row);
            }

            return results;
        }

        private async Task<List<Dictionary<string, object?>>> FetchDoReportData()
        {
            return await FetchReportData("tras_getapcs_warehouse.Delivery_Order_Report_withparameter_tras", new Dictionary<string, string>
            {
                { "@p_DONumber", "" }, { "@p_CustomerName", "" }, { "@p_SalesOrderNumber", "" }, { "@p_ProductType", "" },
                { "@p_Warehouse", "" }, { "@p_Location", "" }, { "@p_ItemNumber", "" }, { "@p_MPN", "" }, { "@p_ProjectNumber", "" }
            });
        }
    }

    public class ChatRequest
    {
        public string Text { get; set; } = string.Empty;
    }

    public class ChatResponse
    {
        public string Text { get; set; } = string.Empty;
        public bool IsBot { get; set; }
    }

    public class ChatResponseWithAction : ChatResponse
    {
        public string? Action { get; set; }
        public int RecordCount { get; set; }
    }
}
