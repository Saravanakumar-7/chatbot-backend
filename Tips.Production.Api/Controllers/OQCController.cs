using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Repository;

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OQCController : ControllerBase
    {
        private IOQCRepository _oQCRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IShopOrderRepository _shopOrderRepo;

        public OQCController(IOQCRepository oQCRepository, IShopOrderRepository shopOrderRepository, ILoggerManager logger, IMapper mapper)
        {
            _oQCRepository = oQCRepository;
            _logger = logger;
            _mapper = mapper;
            _shopOrderRepo = shopOrderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOQC([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<OQCDto>> serviceResponse = new ServiceResponse<IEnumerable<OQCDto>>();
            try
            {

                var oQCDetails = await _oQCRepository.GetAllOQC(pagingParameter,searchParamess);
                _logger.LogInfo("Returned all OQC");
                var result = _mapper.Map<IEnumerable<OQCDto>>(oQCDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OQCs Successfully";
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
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOQCById(int id)
        {
            ServiceResponse<OQCDto> serviceResponse = new ServiceResponse<OQCDto>();

            try
            {
                var oQCDetailsbyId = await _oQCRepository.GetOQCById(id);
                if (oQCDetailsbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OQC  hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"OQC with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned OQC with id: {id}");
                    var result = _mapper.Map<OQCDto>(oQCDetailsbyId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned OQC with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetOQCById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOQC([FromBody] OQCPostDto oQCPostDto)
        {
            ServiceResponse<OQCDto> serviceResponse = new ServiceResponse<OQCDto>();

            try
            {
                if (oQCPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OQC object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("OQC object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid OQC object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;

                    _logger.LogError("Invalid OQC object sent from client.");

                    return BadRequest(serviceResponse);
                }
                var oQCCreate = _mapper.Map<OQC>(oQCPostDto);
                var shopOrderNumber = oQCCreate.ShopOrderNumber;
                var shopOrderDetails = await _shopOrderRepo.GetShopOrderDetailsByShopOrderNo(shopOrderNumber);
                shopOrderDetails.OqcQty = shopOrderDetails.OqcQty + oQCCreate.AcceptedQty;
                _shopOrderRepo.SaveAsync();
                await _oQCRepository.CreateOQC(oQCCreate);
                _oQCRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "OQC Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateOQC action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOQC(int id, [FromBody] OQCUpdateDto oQCUpdateDto)
        {
            ServiceResponse<OQCDto> serviceResponse = new ServiceResponse<OQCDto>();

            try
            {
                if (oQCUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update OQC object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update OQC object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update OQC object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update OQC object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var oQCDetailsById = await _oQCRepository.GetOQCById(id);
                if (oQCDetailsById is null)
                {
                    _logger.LogError($"Update OQC with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update OQC hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(oQCUpdateDto, oQCDetailsById);
                string result = await _oQCRepository.UpdateOQC(oQCDetailsById);
                _logger.LogInfo(result);
                _oQCRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "OQC Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateOQC action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOQC(int id)
        {
            ServiceResponse<OQCDto> serviceResponse = new ServiceResponse<OQCDto>();

            try
            {
                var oQCDetailsById = await _oQCRepository.GetOQCById(id);
                if (oQCDetailsById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete OQC object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete OQC with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _oQCRepository.DeleteOQC(oQCDetailsById);
                _logger.LogInfo(result);
                _oQCRepository.SaveAsync();
                serviceResponse.Message = "OQC Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteOQC action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetShopOrderConfirmationItemNoByFGItemType()
        {
            ServiceResponse<IEnumerable<ShopOrderConfirmationItemNoListDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderConfirmationItemNoListDto>>();

            try
            {
                var shopOrderConfirmationItemNoList = await _oQCRepository.GetShopOrderConfirmationItemNoByFGItemType();
                if (shopOrderConfirmationItemNoList == null)
                {
                    _logger.LogError($"FGItemType hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"FGItemType hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned ShopOrderConfirmationItemNo By FGItemType");
                    var result = _mapper.Map<IEnumerable<ShopOrderConfirmationItemNoListDto>>(shopOrderConfirmationItemNoList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderConfirmationItemNoList By FGItemType  Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetShopOrderConfirmationItemNoByFGItemType action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetShopOrderConfirmationItemNoBySAItemType()
        {
            ServiceResponse<IEnumerable<ShopOrderConfirmationItemNoListDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderConfirmationItemNoListDto>>();

            try
            {
                var shopOrderConfirmationItemNoList = await _oQCRepository.GetShopOrderConfirmationItemNoBySAItemType();
                if (shopOrderConfirmationItemNoList == null)
                {
                    _logger.LogError($"SAItemType hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAItemType hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned ShopOrderConfirmationItemNo By SAItemType");
                    var result = _mapper.Map<IEnumerable<ShopOrderConfirmationItemNoListDto>>(shopOrderConfirmationItemNoList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderConfirmationItemNoList By SAItemType  Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetShopOrderConfirmationItemNoBySAItemType action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetShopOrderConfirmationFGSAItemTypeDetailsByItemNo(string itemNumber)
        {
            ServiceResponse<IEnumerable<ShopOrderConfirmationDetailsDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderConfirmationDetailsDto>>();

            try
            {
                var shopOrderConfirmationDeails = await _oQCRepository.GetShopOrderConfirmationDetailsByItemNo(itemNumber);
                if (shopOrderConfirmationDeails == null)
                {
                    _logger.LogError($"ShopOrderConfirmationItemNo hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrderConfirmationItemNo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned ShopOrderConfirmationDetails By ItemNo");
                    var result = _mapper.Map<IEnumerable<ShopOrderConfirmationDetailsDto>>(shopOrderConfirmationDeails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderConfirmationFGSAItemTypeDetails ByItemNo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetShopOrderConfirmationFGSAItemTypeDetailsByItemNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOQCIdNameList()
        {
            ServiceResponse<IEnumerable<OQCIdNameList>> serviceResponse = new ServiceResponse<IEnumerable<OQCIdNameList>>();
            try
            {
                var listOfAllOQCIdNames = await _oQCRepository.GetAllOQCIdNameList();
                var result = _mapper.Map<IEnumerable<OQCIdNameList>>(listOfAllOQCIdNames);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All listOfAllOQCIdNames";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllOQCIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
    }
}
