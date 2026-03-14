using ModelContextProtocol.Server;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Text.Json;

namespace Tips.AiAgent.Api.McpTools
{
    [McpServerToolType]
    public class ReturnOdoReportTool
    {
        [McpServerTool, Description("Fetches the Return Open Delivery Order report by calling the Return_Open_DeliveryOrder_Report_withparameters_tras stored procedure. Returns report data as JSON.")]
        public static async Task<string> ReturnOdoReport(
            IConfiguration configuration,
            string? odoNumber = null,
            string? customerName = null,
            string? customerAliasName = null,
            string? leadId = null,
            string? issuedTo = null,
            string? location = null,
            string? warehouse = null,
            string? kpn = null,
            string? mpn = null,
            string? odoType = null,
            string? projectNumber = null)
        {
            var connectionString = configuration["MySqlconnection:connectionString"];
            var results = new List<Dictionary<string, object?>>();

            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand("tras_getapcs_warehouse.Return_Open_DeliveryOrder_Report_withparameters_tras", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@p_DONumber", odoNumber ?? "");
            command.Parameters.AddWithValue("@p_CustomerName", customerName ?? "");
            command.Parameters.AddWithValue("@p_CustomerAliasName", customerAliasName ?? "");
            command.Parameters.AddWithValue("@p_LeadId", leadId ?? "");
            command.Parameters.AddWithValue("@p_IssuedTo", issuedTo ?? "");
            command.Parameters.AddWithValue("@p_Location", location ?? "");
            command.Parameters.AddWithValue("@p_Warehouse", warehouse ?? "");
            command.Parameters.AddWithValue("@p_KPN", kpn ?? "");
            command.Parameters.AddWithValue("@p_MPN", mpn ?? "");
            command.Parameters.AddWithValue("@p_ODOType", odoType ?? "");
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
