using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities; 
using Tips.Production.Api.Entities.Enums;

using Entities;
using Entities.Enums;
using Newtonsoft.Json;
using System.Net.Http;
using Tips.Production.Api.Repository;
using Microsoft.AspNetCore.Authorization;

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SAShopOrderController : ControllerBase
    {
        private ISAShopOrderRepository _sashopOrderRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IMaterialIssueRepository _materialIssueRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        public SAShopOrderController(ISAShopOrderRepository sashopOrderRepository, IHttpClientFactory clientFactory, IMaterialIssueRepository materialIssueRepository, ILoggerManager logger, IConfiguration config, HttpClient httpClient, IMapper mapper)
        {
            _logger = logger;
            _sashopOrderRepository = sashopOrderRepository;
            _materialIssueRepository = materialIssueRepository;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSAShopOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<SAShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<SAShopOrderDto>>();

            try
            {
                var sAShopOrderDetails = await _sashopOrderRepository.GetAllSAShopOrders(pagingParameter, searchParamess);
                _logger.LogInfo("Returned all SAShopOrders");
                var result = _mapper.Map<IEnumerable<SAShopOrderDto>>(sAShopOrderDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "SAShopOrders Successfully Returned";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSAShopOrderById(int id)
        {
            ServiceResponse<SAShopOrderDto> serviceResponse = new ServiceResponse<SAShopOrderDto>();

            try
            {
                var sAShopOrderDetailById = await _sashopOrderRepository.GetSAShopOrderById(id);
                if (sAShopOrderDetailById == null)
                {
                    _logger.LogError($"SAShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAShopOrder with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned SAShopOrder with id: {id}");
                    var result = _mapper.Map<SAShopOrderDto>(sAShopOrderDetailById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "SAShopOrderById Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSAshopOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public IActionResult CreateSAShopOrder([FromBody] SAShopOrderPostDto sAShopOrderPostDto)
        {
            ServiceResponse<SAShopOrderPostDto> serviceResponse = new ServiceResponse<SAShopOrderPostDto>();

            try
            {
                if (sAShopOrderPostDto == null)
                {
                    _logger.LogError("SAShopOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SAShopOrder object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid SAShopOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SAShopOrder object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var sAShopOrder = _mapper.Map<SAShopOrder>(sAShopOrderPostDto);

                _sashopOrderRepository.CreateSAShopOrder(sAShopOrder);
                _sashopOrderRepository.SaveAsync();

               CreateSAMaterialIssueDetails(sAShopOrder);

                serviceResponse.Data = null;
                serviceResponse.Message = "SAShopOrder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetSAShopOrderById", serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateSAShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private async Task<IActionResult> CreateSAMaterialIssueDetails(SAShopOrder SaShopOrder)
        {
            ServiceResponse<MaterialIssueDto> serviceResponse = new ServiceResponse<MaterialIssueDto>();
            try
            {
                dynamic bomData = null;
                for (int i = 0; i < SaShopOrder.ShopOrderItems.Count(); i++)
                {
                    if (i == 0)
                    {
                        //var fgNumber = SaShopOrder.SAItemNumber;
                        //decimal bomversion = SaShopOrder.BomRevisionNo;
                        //var bomDetails = await _httpClient.GetAsync(string.Concat(_config["EngineeringBomAPI"], "GetProductionBomByItemAndBomVersionNo?", "ItemNumber=", fgNumber, "&bomVersionNo=", bomversion));

                        var client2 = _clientFactory.CreateClient();
                        var token2 = HttpContext.Request.Headers["Authorization"].ToString();

                        var fgNumber = SaShopOrder.SAItemNumber;
                        var encodedItemNo = Uri.EscapeDataString(fgNumber);
                        decimal bomversion = SaShopOrder.BomRevisionNo;

                        var request2 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EngineeringBomAPI"],
                            $"GetProductionBomByItemAndBomVersionNo?ItemNumber={encodedItemNo}&bomVersionNo={bomversion}"));
                        request2.Headers.Add("Authorization", token2);

                        var bomDetails = await client2.SendAsync(request2);
                        var bomDetailsString = await bomDetails.Content.ReadAsStringAsync();
                        dynamic bomDetailsData = JsonConvert.DeserializeObject(bomDetailsString);
                        bomData = bomDetailsData.data;
                    }

                    if (bomData != null)
                    {
                        MaterialIssue materialIssue = new MaterialIssue();
                        materialIssue.ShopOrderNumber = SaShopOrder.SAShopOrderNumber;
                        materialIssue.ShopOrderDate = SaShopOrder.CreatedOn;
                        materialIssue.ProjectType = SaShopOrder.ProjectType;
                        materialIssue.ItemType = SaShopOrder.ItemType;
                        materialIssue.ShopOrderQty = SaShopOrder.TotalSOReleaseQty;
                        materialIssue.ItemNumber = SaShopOrder.SAItemNumber;
                        materialIssue.MaterialIssuedStatus = IssuedStatus.Open;
                        List<MaterialIssueItem> materialIssueItemList = new List<MaterialIssueItem>();
                        foreach (var bom in bomData.enggChildItemDtos)
                        {
                            var projectNo = SaShopOrder.ShopOrderItems[i].ProjectNumber;
                            MaterialIssueItem materialIssueItem = new MaterialIssueItem();
                            materialIssueItem.PartNumber = bom.itemNumber;
                            materialIssueItem.Description = bom.description;
                            materialIssueItem.ProjectNumber = projectNo;
                            materialIssueItem.PartType = bom.partType;
                            materialIssueItem.UOM = bom.uom;
                            materialIssueItem.RequiredQty = (bom.quantity * SaShopOrder.TotalSOReleaseQty);
                            materialIssueItem.IssuedQty = 0;
                            materialIssueItem.MaterialIssuedStatus = IssuedStatus.Open;
                            //materialIssueItem.CreatedBy = "Admin";
                            //materialIssueItem.CreatedOn = DateTime.Now;
                            //materialIssueItem.LastModifiedBy = "Admin";
                            //materialIssueItem.LastModifiedOn = DateTime.Now;
                            materialIssueItemList.Add(materialIssueItem);
                        }

                        var groupedMaterialIssueItems = materialIssueItemList
                            .GroupBy(item => item.PartNumber)
                            .Select(group => new MaterialIssueItem
                            {
                                PartNumber = group.Key,
                                Description = group.First().Description,
                                ProjectNumber = group.First().ProjectNumber,
                                PartType = group.First().PartType,
                                UOM = group.First().UOM,
                                RequiredQty = group.Sum(item => item.RequiredQty),
                                IssuedQty = 0, // Assuming IssuedQty remains 0 for grouped items
                                MaterialIssuedStatus = IssuedStatus.Open,
                                //CreatedBy = "Admin",
                                //CreatedOn = DateTime.Now,
                                //LastModifiedBy = "Admin",
                                //LastModifiedOn = DateTime.Now
                            })
                            .ToList();
                        materialIssue.materialIssueItems = groupedMaterialIssueItems;
                        await _materialIssueRepository.CreateMaterialIssue(materialIssue);
                        _materialIssueRepository.SaveAsync();
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateSAShopOrder action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong {ex.Message},{ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSAShopOrder(int id, [FromBody] SAShopOrderDto sAShopOrderUpdateDto)
        {
            ServiceResponse<SAShopOrderDto> serviceResponse = new ServiceResponse<SAShopOrderDto>();

            try
            {
                if (sAShopOrderUpdateDto is null)
                {
                    _logger.LogError("UpdateSAShopOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update SAShopOrder object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid SAShopOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SAShopOrder object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var sAShopOrderDetailById = await _sashopOrderRepository.GetSAShopOrderById(id);
                if (sAShopOrderDetailById is null)
                {
                    _logger.LogError($"SAShopOrder with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }

                var sAShopOrder = _mapper.Map(sAShopOrderUpdateDto, sAShopOrderDetailById);

                string result = await _sashopOrderRepository.UpdateSAShopOrder(sAShopOrder);
                _logger.LogInfo(result);
                _sashopOrderRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "SAShopOrder Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateSAShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSAShopOrder(int id)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var sAShopOrderDetailsById = await _sashopOrderRepository.GetSAShopOrderById(id);
                if (sAShopOrderDetailsById == null)
                {
                    _logger.LogError($"SAShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAShopOrder with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                sAShopOrderDetailsById.IsDeleted = true;
                string result = await _sashopOrderRepository.UpdateSAShopOrder(sAShopOrderDetailsById);
                _sashopOrderRepository.SaveAsync();
                serviceResponse.Message = "SAShopOrder Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteSAShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        
        [HttpGet("salesOrderNo")]
        public async Task<IActionResult> GetSAShopOrderBySalesOrderNo(string salesOrderNo)
        {
            ServiceResponse<SAShopOrderDto> serviceResponse = new ServiceResponse<SAShopOrderDto>();

            try
            {
                var sAShopOrderBySalesOrderNo = await _sashopOrderRepository.GetSAShopOrderBySalesOrderNo(salesOrderNo);
                if (sAShopOrderBySalesOrderNo == null)
                {
                    _logger.LogError($"SAShopOrder with salesOrderNo: {salesOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAShopOrder with salesOrderNo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned SAShopOrder with salesOrderNo: {salesOrderNo}");
                    var result = _mapper.Map<SAShopOrderDto>(sAShopOrderBySalesOrderNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "SAShopOrderBySalesOrderNumber Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSAShopOrderBySalesOrderNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("SAshopOrderNo")]
        public async Task<IActionResult> GetSAShopOrderByShopOrderNo(string SAshopOrderNo)
        {
            ServiceResponse<SAShopOrderDto> serviceResponse = new ServiceResponse<SAShopOrderDto>();

            try
            {
                var sAShopOrderByShopOrderNo = await _sashopOrderRepository.GetSAShopOrderBySAShopOrderNo(SAshopOrderNo);
                if (sAShopOrderByShopOrderNo == null)
                {
                    _logger.LogError($"SAShopOrder with SAshopOrderNo: {SAshopOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAShopOrder with SAshopOrderNo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned SAShopOrder with SAshopOrderNo: {SAshopOrderNo}");
                    var result = _mapper.Map<SAShopOrderDto>(sAShopOrderByShopOrderNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "SAShopOrderByShopOrderNo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSAShopOrderByShopOrderNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllOpenSAShopOrders()
        {
            ServiceResponse<IEnumerable<SAShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<SAShopOrderDto>>();

            try
            {
                var openSAShopOrderDetails = await _sashopOrderRepository.GetAllOpenSAShopOrders();
                _logger.LogInfo("Returned all SAShopOrders");
                var result = _mapper.Map<IEnumerable<SAShopOrderDto>>(openSAShopOrderDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenSAShopOrders Successfully";
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

        [HttpGet("{id}")]
        public async Task<IActionResult> ShortCloseSAShopOrder(int id)
        {
            ServiceResponse<SAShopOrderDto> serviceResponse = new ServiceResponse<SAShopOrderDto>();

            try
            {
                var shortCloseSAShopOrder = await _sashopOrderRepository.GetSAShopOrderById(id);
                if (shortCloseSAShopOrder == null)
                {
                    _logger.LogError($"SAShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAShopOrder with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                shortCloseSAShopOrder.IsShortClosed = true;
                shortCloseSAShopOrder.ShorClosedBy = "Admin";
                shortCloseSAShopOrder.ShortClosedOn = DateTime.Now;
                string result = await _sashopOrderRepository.UpdateSAShopOrder(shortCloseSAShopOrder);
                _sashopOrderRepository.SaveAsync();
                serviceResponse.Message = "SAShopOrder have been closed";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ShortCloseSAShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
