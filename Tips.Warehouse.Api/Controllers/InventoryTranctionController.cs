using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    public class InventoryTranctionController : ControllerBase
    {       
            private IInventoryTranctionRepository _inventoryTranctionRepository;
            private IMaterialIssueTrackerRepository _materialIssueTrackerRepository;
            private ILoggerManager _logger;
            private IMapper _mapper;

            public InventoryTranctionController(IMaterialIssueTrackerRepository materialIssueTrackerRepository,IInventoryTranctionRepository inventoryTranctionRepository, ILoggerManager logger, IMapper mapper)
            {
                _inventoryTranctionRepository = inventoryTranctionRepository;
                _materialIssueTrackerRepository = materialIssueTrackerRepository;
                _logger = logger;
                _mapper = mapper;
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
        public async Task<IActionResult> GetInventoryTranctionDetailsByGrinNoandGrinId(string GrinNo, int GrinPartsId,
                                                                                                string ItemNumber, string ProjectNumber)
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
            public IActionResult CreateInventoryTranction([FromBody] InventoryTranctionDtoPost inventoryTranctionDtoPost)
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

                    _inventoryTranctionRepository.CreateInventoryTransaction(createInventoryTranction);
                    _inventoryTranctionRepository.SaveAsync();
                    serviceResponse.Data = null;
                    serviceResponse.Message = "InventoryTranction Successfully Created";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok( serviceResponse);
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
                                .GetDetailsByShopOrderNOItemNoLotNo(inventoryTranctionDetail.PartNumber, inventoryTranctionDetail.shopOrderNo, inventoryTranctionDetail.LotNumber);

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

    }
}
