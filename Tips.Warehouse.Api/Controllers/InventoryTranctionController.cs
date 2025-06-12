using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Net;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class InventoryTranctionController : ControllerBase
    {
        private IInventoryTranctionRepository _inventoryTranctionRepository;
        private IMaterialIssueTrackerRepository _materialIssueTrackerRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly IHttpClientFactory _clientFactory;
        public InventoryTranctionController(IMaterialIssueTrackerRepository materialIssueTrackerRepository, IHttpClientFactory clientFactory, IInventoryTranctionRepository inventoryTranctionRepository, ILoggerManager logger, IMapper mapper)
        {
            _inventoryTranctionRepository = inventoryTranctionRepository;
            _materialIssueTrackerRepository = materialIssueTrackerRepository;
            _logger = logger;
            _mapper = mapper;
            _clientFactory = clientFactory;
        }

        // GET: api/<InventoryTranctionController>
        [HttpGet]
        public async Task<IActionResult> GetAllInventoryTranctions([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<InventoryTranctionDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryTranctionDto>>();
            try
            {
                var getAllInventoryTranctions = await _inventoryTranctionRepository.GetAllInventoryTranction(pagingParameter, searchParams);
                var metadata = new
                {
                    getAllInventoryTranctions.TotalCount,
                    getAllInventoryTranctions.PageSize,
                    getAllInventoryTranctions.CurrentPage,
                    getAllInventoryTranctions.HasNext,
                    getAllInventoryTranctions.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all InventoryTrancton");
                var result = _mapper.Map<IEnumerable<InventoryTranctionDto>>(getAllInventoryTranctions);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all InventoryTrancton";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET api/<InventoryTranctionController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventoryTranctionById(int id)
        {
            ServiceResponse<InventoryTranctionDto> serviceResponse = new ServiceResponse<InventoryTranctionDto>();

            try
            {
                var getInventoryTranctionById = await _inventoryTranctionRepository.GetInventoryTranctionById(id);
                if (getInventoryTranctionById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventoryTranction with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"InventoryTranction with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned InventoryTranction with id: {id}");
                    var result = _mapper.Map<InventoryTranctionDto>(getInventoryTranctionById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned InventoryTranction with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInventoryTranctionById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetInventoryTranctionDetailsByGrinNoandGrinId(string GrinNo, int GrinPartsId,string ItemNumber, string ProjectNumber)
        {
            ServiceResponse<InventoryTranctionDto> serviceResponse = new ServiceResponse<InventoryTranctionDto>();
            try
            {
                var inventoryTranctionDetailsByGrinNoandGrinId = await _inventoryTranctionRepository.GetInventoryTranctionDetailsByGrinNoandGrinId
                                                                                                (GrinNo, GrinPartsId, ItemNumber, ProjectNumber);
                if (inventoryTranctionDetailsByGrinNoandGrinId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventoryTranction with id: {GrinNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with id: {GrinNo}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned InventoryTranction with id: {GrinNo}");
                    var result = _mapper.Map<InventoryTranctionDto>(inventoryTranctionDetailsByGrinNoandGrinId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned InventoryTranction Details Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInventoryTranctionDetailsByGrinNoandGrinId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        // POST api/<InventoryTranctionController>
        [HttpPost]
        public async Task<IActionResult> CreateInventoryTranction([FromBody] InventoryTranctionDtoPost inventoryTranctionDtoPost)
        {
            ServiceResponse<InventoryTranctionDto> serviceResponse = new ServiceResponse<InventoryTranctionDto>();

            try
            {
                if (inventoryTranctionDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "InventoryTranction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("InventoryTranction object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid InventoryTranction object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid InventoryTranction object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var createInventoryTranction = _mapper.Map<InventoryTranction>(inventoryTranctionDtoPost);

                await _inventoryTranctionRepository.CreateInventoryTransaction(createInventoryTranction);
                _inventoryTranctionRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "InventoryTranction Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside CreateInventoryTranction action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInventoryTranctionStockByItemAndProjectNo(string itemNumber, string projectNumber)
        {
            ServiceResponse<List<InventoryTranctionBalanceQtyMaterialIssue>> serviceResponse = new ServiceResponse<List<InventoryTranctionBalanceQtyMaterialIssue>>();
            try
            {
                var InventoryTranctionDetails = await _inventoryTranctionRepository.GetInventoryTranctionStockByItemAndProjectNo(itemNumber, projectNumber);
                if (InventoryTranctionDetails.Count() == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventoryTranction with itemNumber and ProjectNumber: {itemNumber} {projectNumber}, is invalid";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"InventoryTranction with itemNumber and ProjectNumber: {itemNumber} {projectNumber}, is invalid");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned InventoryTranction with Itemnumber and ProjectNumber: {itemNumber} {projectNumber}");
                    var result = _mapper.Map<List<InventoryTranctionBalanceQtyMaterialIssue>>(InventoryTranctionDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned InventoryTranction with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventoryTranction action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Invalid inventoryTranction{ex.Message},{ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //Create InventoryTranction From Grin Data
        [HttpPost]
        public IActionResult CreateInventoryTranctionFromGrin([FromBody] InventoryTranctionGrinDtoPost inventoryTranctionDto)
        {
            ServiceResponse<InventoryTranctionDto> serviceResponse = new ServiceResponse<InventoryTranctionDto>();

            try
            {
                if (inventoryTranctionDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "InventoryTranction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("InventoryTranction object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid InventoryTranction object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid InventoryTranction object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var createInventoryTranction = _mapper.Map<InventoryTranction>(inventoryTranctionDto);

                _inventoryTranctionRepository.CreateInventoryTransaction(createInventoryTranction);
                _inventoryTranctionRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "InventoryTranction Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateInventoryTranction action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<InventoryTranctionController>/5
        [HttpPut]
        public async Task<IActionResult> UpdateInventoryTranction(int id, [FromBody] InventoryTranctionDtoUpdate inventoryTranctionDtoUpdate)
        {
            ServiceResponse<InventoryTranctionDto> serviceResponse = new ServiceResponse<InventoryTranctionDto>();

            try
            {
                if (inventoryTranctionDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "InventoryTranction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("InventoryTranction object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid InventoryTranction object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid InventoryTranction object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var getInventoryTranctionById = await _inventoryTranctionRepository.GetInventoryTranctionById(id);
                if (getInventoryTranctionById is null)
                {
                    _logger.LogError($"InventoryTranction with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update InventoryTranction with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var updateInventoryTranction = _mapper.Map(inventoryTranctionDtoUpdate, getInventoryTranctionById);
                _mapper.Map(inventoryTranctionDtoUpdate, getInventoryTranctionById);
                string result = await _inventoryTranctionRepository.UpdateInventoryTraction(updateInventoryTranction);
                _logger.LogInfo(result);
                _inventoryTranctionRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside UpdateCommodity action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<InventoryTranctionController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryTranction(int id)
        {
            ServiceResponse<InventoryTranctionDto> serviceResponse = new ServiceResponse<InventoryTranctionDto>();

            try
            {
                var getMaterialIssueById = await _inventoryTranctionRepository.GetInventoryTranctionById(id);
                if (getMaterialIssueById == null)
                {
                    _logger.LogError($"Delete InventoryTranction with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete InventoryTranction with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _inventoryTranctionRepository.DeleteInventoryTranction(getMaterialIssueById);
                _logger.LogInfo(result);
                _inventoryTranctionRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "InventoryTranction Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteInventoryTranction action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //update inventoryTranction on shoporder confirmation 
        [HttpPost]
        public async Task<IActionResult> UpdateInventoryTranctionOnShopOrderConfirmation(List<InventoryTranctionDtoForShopOrderConfirmation> dtoForShopOrderConfirmation)
        {
            ServiceResponse<InventoryTranctionDto> serviceResponse = new ServiceResponse<InventoryTranctionDto>();
            try
            {
                foreach (var item in dtoForShopOrderConfirmation)
                {

                    var inventoryTranctionDetails = await _inventoryTranctionRepository
                        .GetWIPInventoryTranctionDetailsByItemNo(item.PartNumber, item.ShopOrderNumber);

                    if (inventoryTranctionDetails == null || inventoryTranctionDetails.Count == 0)
                    {
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"InventoryTranction Details hasn't been found";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        _logger.LogError($"InventoryTranction with itemNumber: {item.PartNumber}, is not available");
                        return StatusCode(500, serviceResponse);
                    }

                    decimal producedQty = item.NewConvertedToFgQty; // value get from payload

                    //decimal revisionNo = await _releaseProductBomRepository.GetLatestProductionBomByItemNumber(fgPartNumber);

                    //var bom = await _repository.EnggBomRepository.GetLatestEnggBomVersionDetailByItemNumber(fgPartNumber, revisionNo);

                    //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterAPI"], "GetLatestEnggBomVersionDetailByItemNumber?", "&fgPartNumber=", itemNumber));

                    //var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    //dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    //dynamic itemMasterObject = itemMasterObjectData.data;

                    //decimal producedQty = item.WipConfirmedQty * ;
                    for (int i = 0; i < inventoryTranctionDetails.Count; i++)
                    {
                        decimal balanceqty = inventoryTranctionDetails[i].Issued_Quantity;
                        decimal lotNoWiseProducedQty = 0;
                        if (inventoryTranctionDetails[i].Issued_Quantity <= producedQty)
                        {
                            inventoryTranctionDetails[i].Issued_Quantity = 0;
                            inventoryTranctionDetails[i].IsStockAvailable = false;
                            lotNoWiseProducedQty = balanceqty;
                            producedQty -= balanceqty;
                            balanceqty = 0;
                        }
                        else
                        {
                            inventoryTranctionDetails[i].Issued_Quantity -= producedQty;
                            lotNoWiseProducedQty = producedQty;

                            producedQty = 0;
                        }

                        string result = await _inventoryTranctionRepository.UpdateInventoryTraction(inventoryTranctionDetails[i]);

                        /*********************************** Update data to Material Issue Tracker *************************/
                        await UpdateDataToMaterialIssueTracker(item, inventoryTranctionDetails[i]);
                        /*********************************** End of Add data to Material Issue Tracker *************************/

                        if (producedQty <= 0)
                        {
                            break;
                        }
                    }
                }

                _inventoryTranctionRepository.SaveAsync();
                _materialIssueTrackerRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);


            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventoryTranction action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private async Task UpdateDataToMaterialIssueTracker(InventoryTranctionDtoForShopOrderConfirmation item, InventoryTranction inventoryTranctionDetail)
        {
            // Retrieve the existing entry from the repository based on the ShopOrderNumber, PartNumber, and LotNumber

            List<ShopOrderMaterialIssueTracker> materialIssueTrackerList = await _materialIssueTrackerRepository
                                .GetDetailsByShopOrderNOItemNoLotNo(inventoryTranctionDetail.PartNumber, inventoryTranctionDetail.shopOrderNo, inventoryTranctionDetail.LotNumber,"");

            if (materialIssueTrackerList != null || materialIssueTrackerList.Count > 0)
            {
                decimal newConvertedToFgQty = item.NewConvertedToFgQty;

                foreach (var materialIssueTrack in materialIssueTrackerList)
                {
                    decimal balanceqtyToConvert = (materialIssueTrack.IssuedQty - materialIssueTrack.ConvertedToFgQty);
                    if (balanceqtyToConvert <= newConvertedToFgQty)
                    {
                        materialIssueTrack.ConvertedToFgQty += balanceqtyToConvert;
                        newConvertedToFgQty -= balanceqtyToConvert;
                        balanceqtyToConvert = 0;
                    }
                    else
                    {
                        materialIssueTrack.ConvertedToFgQty += newConvertedToFgQty;
                        balanceqtyToConvert -= newConvertedToFgQty;
                        newConvertedToFgQty = 0;
                    }

                    await _materialIssueTrackerRepository.UpdateMaterialIssueTracker(materialIssueTrack);
                    if (newConvertedToFgQty <= 0)
                    {
                        break;
                    }
                }

            }
        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetInventoryTranctionSPReports()
        {
            ServiceResponse<IEnumerable<InventoryTranctionSPReport>> serviceResponse = new ServiceResponse<IEnumerable<InventoryTranctionSPReport>>();
            try
            {
                var products = await _inventoryTranctionRepository.GetInventoryTranctionSPReports();

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventoryTranctionSPReports hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"InventoryTranctionSPReports hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned InventoryTranctionSPReports Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetInventoryTranctionSPReports action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportInventoryTranctionSPReportToExcel()
        {
            try
            {
                // Get data from repository using stored procedure
                var inventoryTransactionSPReportDetails = await _inventoryTranctionRepository.GetInventoryTranctionSPReports();

                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("InventoryTranctionSPReport");

                // Set header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Part Number");
                headerRow.CreateCell(1).SetCellValue("Manufacturer Part Number");
                headerRow.CreateCell(2).SetCellValue("Description");
                headerRow.CreateCell(3).SetCellValue("Project Number");
                headerRow.CreateCell(4).SetCellValue("Issued Quantity");
                headerRow.CreateCell(5).SetCellValue("UOM");
                headerRow.CreateCell(6).SetCellValue("Issued DateTime");
                headerRow.CreateCell(7).SetCellValue("Issued By");
                headerRow.CreateCell(8).SetCellValue("Shop Order Id");
                headerRow.CreateCell(9).SetCellValue("Reference ID");
                headerRow.CreateCell(10).SetCellValue("Reference ID From");
                headerRow.CreateCell(11).SetCellValue("BOM Version No");
                headerRow.CreateCell(12).SetCellValue("From Location");
                headerRow.CreateCell(13).SetCellValue("To Location");
                headerRow.CreateCell(14).SetCellValue("Modified Status");
                headerRow.CreateCell(15).SetCellValue("Unit");
                headerRow.CreateCell(16).SetCellValue("Grin Material Type");
                headerRow.CreateCell(17).SetCellValue("Remarks");
                headerRow.CreateCell(18).SetCellValue("Created By");
                headerRow.CreateCell(19).SetCellValue("Created On");
                headerRow.CreateCell(20).SetCellValue("Last Modified On");
                headerRow.CreateCell(21).SetCellValue("Part Type");
                headerRow.CreateCell(22).SetCellValue("Lot Number");
                headerRow.CreateCell(23).SetCellValue("Is Stock Available");
                headerRow.CreateCell(24).SetCellValue("Warehouse");
                headerRow.CreateCell(25).SetCellValue("Grin No");
                headerRow.CreateCell(26).SetCellValue("Grin Part Id");
                headerRow.CreateCell(27).SetCellValue("Shop Order No");

                // Populate data rows
                int rowIndex = 1;
                foreach (var item in inventoryTransactionSPReportDetails)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.PartNumber);
                    row.CreateCell(1).SetCellValue(item.MftrPartNumber);
                    row.CreateCell(2).SetCellValue(item.Description);
                    row.CreateCell(3).SetCellValue(item.ProjectNumber);
                    row.CreateCell(4).SetCellValue(Convert.ToDouble(item.Issued_Quantity ?? 0)); // Assuming Issued_Quantity is nullable decimal
                    row.CreateCell(5).SetCellValue(item.UOM);
                    row.CreateCell(6).SetCellValue(item.Issued_DateTime.HasValue ? item.Issued_DateTime.Value.ToString("MM/dd/yyyy HH:mm:ss") : ""); // Assuming Issued_DateTime is nullable DateTime
                    row.CreateCell(7).SetCellValue(item.Issued_By);
                    row.CreateCell(8).SetCellValue(item.ShopOrderId);
                    row.CreateCell(9).SetCellValue(item.ReferenceID);
                    row.CreateCell(10).SetCellValue(item.ReferenceIDFrom);
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(item.BOM_Version_No ?? 0)); // Assuming BOM_Version_No is nullable decimal
                    row.CreateCell(12).SetCellValue(item.From_Location);
                    row.CreateCell(13).SetCellValue(item.TO_Location);
                    row.CreateCell(14).SetCellValue(item.ModifiedStatus != null ? item.ModifiedStatus.ToString() : ""); // Assuming ModifiedStatus is nullable bool
                    row.CreateCell(15).SetCellValue(item.Unit);
                    row.CreateCell(16).SetCellValue(item.GrinMaterialType);
                    row.CreateCell(17).SetCellValue(item.Remarks);
                    row.CreateCell(18).SetCellValue(item.CreatedBy);
                    row.CreateCell(19).SetCellValue(item.CreatedOn.HasValue ? item.CreatedOn.Value.ToString("MM/dd/yyyy HH:mm:ss") : ""); // Assuming CreatedOn is nullable DateTime
                    row.CreateCell(20).SetCellValue(item.LastModifiedOn.HasValue ? item.LastModifiedOn.Value.ToString("MM/dd/yyyy HH:mm:ss") : ""); // Assuming LastModifiedOn is nullable DateTime
                    row.CreateCell(21).SetCellValue(item.PartType != null ? item.PartType.ToString() : ""); // Assuming PartType is nullable enum
                    row.CreateCell(22).SetCellValue(item.LotNumber);
                    row.CreateCell(23).SetCellValue(item.IsStockAvailable);
                    row.CreateCell(24).SetCellValue(item.Warehouse);
                    row.CreateCell(25).SetCellValue(item.GrinNo);
                    row.CreateCell(26).SetCellValue(Convert.ToDouble(item.GrinPartId ?? 0)); // Assuming GrinPartId is nullable int
                    row.CreateCell(27).SetCellValue(item.shopOrderNo);
                }

                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "InventoryTranctionSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        //MaterialRequest
        //[HttpPost]
        //public async Task<IActionResult> MaterialInventoryTranctionBalanceQty([FromBody] List<UpdateInventoryTranctionBalanceQty> updateInventoryTranctionBalanceQty)
        //{
        //    try
        //    {
        //        foreach (var materialIssueQty in updateInventoryTranctionBalanceQty)
        //        {
        //            foreach (var Location in materialIssueQty.MRNWarehouseList)
        //            {
        //                decimal issuedQty = Location.Qty;
        //                IEnumerable<InventoryTranction> inventories = await _inventoryTranctionRepository.GetInventoryTranctionDetailsByItemNoandLocationandwarehouse(materialIssueQty.PartNumber, Location.Location, Location.Warehouse, materialIssueQty.ProjectNumber);
        //                foreach (var invItem in inventories)
        //                {
        //                    decimal stock = invItem.Issued_Quantity;
        //                    if (stock <= issuedQty)
        //                    {

        //                        invItem.Issued_Quantity = 0;
        //                        invItem.IsStockAvailable = false;
        //                        issuedQty -= stock;

        //                    }
        //                    else
        //                    {
        //                        invItem.Issued_Quantity -= issuedQty;
        //                        issuedQty = 0;
        //                    }
        //                    await _inventoryTranctionRepository.UpdateInventoryTraction(invItem);
        //                    if (issuedQty <= 0)
        //                    {
        //                        break;
        //                    }
        //                }

        //            }
        //        }
        //        _inventoryTranctionRepository.SaveAsync();
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {

        //        return StatusCode(500);
        //    }
        //}

    }
}
