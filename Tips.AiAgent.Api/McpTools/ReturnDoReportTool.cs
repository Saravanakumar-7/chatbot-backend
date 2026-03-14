using ModelContextProtocol.Server;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Text.Json;

namespace Tips.AiAgent.Api.McpTools
{
    [McpServerToolType]
    public class ReturnDoReportTool
    {
        [McpServerTool, Description("Fetches the Return Delivery Order report by calling the returndeliveryorder_with_returntable_with_parameters_tras stored procedure. Returns report data as JSON.")]
        public static async Task<string> ReturnDoReport(
            IConfiguration configuration,
            string? returnBtoNumber = null,
            string? customerName = null,
            string? customerAliasName = null,
            string? customerLeadId = null,
            string? salesOrderNumber = null,
            string? productType = null,
            string? typeOfSolution = null,
            string? warehouse = null,
            string? location = null,
            string? kpn = null,
            string? mpn = null,
            string? projectNumber = null)
        {
            var connectionString = configuration["MySqlconnection:connectionString"];
            var results = new List<Dictionary<string, object?>>();

            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand("tras_getapcs_warehouse.returndeliveryorder_with_returntable_with_parameters_tras", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@p_ReturnBTONumber", returnBtoNumber ?? "");
            command.Parameters.AddWithValue("@p_CustomerName", customerName ?? "");
            command.Parameters.AddWithValue("@p_CustomerAliasName", customerAliasName ?? "");
            command.Parameters.AddWithValue("@p_CustomerLeadid", customerLeadId ?? "");
            command.Parameters.AddWithValue("@p_SalesOrderNumber", salesOrderNumber ?? "");
            command.Parameters.AddWithValue("@p_ProductType", productType ?? "");
            command.Parameters.AddWithValue("@p_TypeOfSolution", typeOfSolution ?? "");
            command.Parameters.AddWithValue("@p_Warehouse", warehouse ?? "");
            command.Parameters.AddWithValue("@p_Location", location ?? "");
            command.Parameters.AddWithValue("@p_KPN", kpn ?? "");
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
