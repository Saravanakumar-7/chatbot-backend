using System.Net;
using System.Net.Http;
using System.Text;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;


namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CoverageReportController : ControllerBase
    {
        private ICollectionTrackerRepository _repository;
        private ICoverageReportRepository _coverageRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public CoverageReportController(ICollectionTrackerRepository repository, HttpClient httpClient, IConfiguration config,ICoverageReportRepository coverageReportRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _coverageRepository = coverageReportRepository;
        }
        //test aravind
        //public async Task<List<CoverageReport>> GenerateCoverageReportAsync()
        //{
        //    var coverageReports = new Dictionary<string, CoverageReport>();

        //    // Get all sales orders where status is 'Forecast'
        //    var salesOrders = await _coverageRepository.GetAllForecastSalesOrderDetails();


        //    foreach (var salesOrder in salesOrders)
        //    {
        //        int salesOrderId = salesOrder.Id;
        //        // Get the sales order items for the current sales order
        //        var salesOrderItems = await _coverageRepository.GetAllSalesOrderItemDetails(salesOrderId);

        //        foreach (var salesOrderItem in salesOrderItems)
        //        {
        //            // Get the BOM for the current sales order item recursively and calculate required quantities
        //            await GetBomAndCalculateRequiredQuantitiesRecursivelyAsync(salesOrderItem.ItemNumber, salesOrderItem.BalanceQty, coverageReports);
        //        }
        //    }

        //    // At this point, 'coverageReports' will have the coverage report for each item number
        //    return coverageReports.Values.ToList();
        //}

        //private async Task GetBomAndCalculateRequiredQuantitiesRecursivelyAsync(string itemNumber, decimal requiredQtyMultiplier, Dictionary<string, CoverageReport> coverageReports)
        //{
        //    // Get the BOM for the current item
        //    var enggBoms = await _context.EnggBoms
        //        .Where(eb => eb.ItemNumber == itemNumber)
        //        .ToListAsync();

        //    foreach (var enggBom in enggBoms)
        //    {
        //        // Get all the child items recursively
        //        var enggChildItems = await GetEnggChildItemsRecursivelyAsync(enggBom.Id);

        //        foreach (var childItem in enggChildItems)
        //        {
        //            var totalRequiredQty = requiredQtyMultiplier * childItem.RequiredQty;

        //            // Check if the coverage report for this item number already exists
        //            if (!coverageReports.TryGetValue(childItem.ItemNumber, out var coverageReport))
        //            {
        //                coverageReport = new CoverageReport
        //                {
        //                    ItemNumber = childItem.ItemNumber
        //                };
        //                coverageReports[childItem.ItemNumber] = coverageReport;
        //            }

        //            coverageReport.TotalRequiredQty += totalRequiredQty;

        //            // Get the inventory for the current child item
        //            var inventory = await _context.Inventory
        //                .FirstOrDefaultAsync(i => i.ItemNumber == childItem.ItemNumber && i.IsStockAvailable);
        //            if (inventory != null)
        //            {
        //                coverageReport.InventoryQty += inventory.Qty;
        //            }

        //            // Get the open purchase order items for the current child item
        //            var openPoItems = await _context.PoItems
        //                .Include(pi => pi.PurchaseOrder)
        //                .Where(pi => pi.ItemNumber == childItem.ItemNumber &&
        //                             (pi.PurchaseOrder.Status == "Open" || pi.PurchaseOrder.Status == "Partially Closed"))
        //                .ToListAsync();
        //            coverageReport.OpenPOQty += openPoItems.Sum(pi => pi.Qty);

        //            // Calculate the balance quantity to order
        //            coverageReport.BalanceQtyToOrder = coverageReport.TotalRequiredQty - (coverageReport.InventoryQty + coverageReport.OpenPOQty);

        //            // If the part type is 'SA', continue the recursion
        //            if (enggBom.PartType == "SA")
        //            {
        //                await GetBomAndCalculateRequiredQuantitiesRecursivelyAsync(childItem.ItemNumber, totalRequiredQty, coverageReports);
        //            }
        //        }
        //    }
        //}

        //private async Task<List<EnggChildItem>> GetEnggChildItemsRecursivelyAsync(int parentId)
        //{
        //    var childItems = new List<EnggChildItem>();
        //    var directChildItems = await _context.EnggChildItems.Where(eci => eci.ParentId == parentId).ToListAsync();
        //    childItems.AddRange(directChildItems);

        //    foreach (var directChildItem in directChildItems)
        //    {
        //        var indirectChildItems = await GetEnggChildItemsRecursivelyAsync(directChildItem.Id);
        //        childItems.AddRange(indirectChildItems);
        //    }

        //    return childItems;
        //}

        [HttpGet]
        public async Task<IActionResult> GetCoverageReport()
        {
            ServiceResponse<IEnumerable<CoverageReportDto>> serviceResponse = new ServiceResponse<IEnumerable<CoverageReportDto>>();

            try
            {
                var getAllForecastLpCosting = await _coverageRepository.GetAllSalesOrderDetails();

                _logger.LogInfo("Returned Coverage Report");
                var result = _mapper.Map<List<CoverageReportDto>>(getAllForecastLpCosting);
                //bom details
                var json = JsonConvert.SerializeObject(result);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(string.Concat(_config["EngineeringBomAPI"], "CoverageEnggBomChildDetails"), data);
                
                //var inventoryObjectResult = await _httpClient.PostAsync(string.Concat(_config["ItemMasterAPI"], "GetItemsRoutingDetailsForLpCosting"), content);
                //var itemsRoutingDetailsJsonString = await inventoryObjectResult.Content.ReadAsStringAsync();
                //dynamic itemsRoutingDetailsJson = JsonConvert.DeserializeObject(itemsRoutingDetailsJsonString);
                //var data = itemsRoutingDetailsJson.data;

                //serviceResponse.Data = result;
                serviceResponse.Message = "Returned Coverage Report Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // Get all ForeCastEngg 
    }
}
