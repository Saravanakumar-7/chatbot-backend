using ModelContextProtocol.Server;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Text.Json;

namespace Tips.AiAgent.Api.McpTools
{
    [McpServerToolType]
    public class DoWorkOrderReportTool
    {
        [McpServerTool, Description("Fetches the Delivery Order Work Order report by calling the DO_Report_withparameter_for_akash stored procedure. Returns report data as JSON.")]
        public static async Task<string> DoWorkOrderReport(
            IConfiguration configuration,
            string? doNumber = null,
            string? customerName = null,
            string? salesOrderNumber = null,
            string? productType = null,
            string? warehouse = null,
            string? location = null,
            string? itemNumber = null,
            string? mpn = null,
            string? projectNumber = null)
        {
            var connectionString = configuration["MySqlconnection:connectionString"];
            var results = new List<Dictionary<string, object?>>();

            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand("tras_getapcs_warehouse.DO_Report_withparameter_for_akash", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@p_DONumber", doNumber ?? "");
            command.Parameters.AddWithValue("@p_CustomerName", customerName ?? "");
            command.Parameters.AddWithValue("@p_SalesOrderNumber", salesOrderNumber ?? "");
            command.Parameters.AddWithValue("@p_ProductType", productType ?? "");
            command.Parameters.AddWithValue("@p_Warehouse", warehouse ?? "");
            command.Parameters.AddWithValue("@p_Location", location ?? "");
            command.Parameters.AddWithValue("@p_ItemNumber", itemNumber ?? "");
            command.Parameters.AddWithValue("@p_MPN", mpn ?? "");
            command.Parameters.AddWithValue("@p_ProjectNumber", projectNumber ?? "");

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

            return JsonSerializer.Serialize(new
            {
                recordCount = results.Count,
                data = results
            });
        }
    }
}
