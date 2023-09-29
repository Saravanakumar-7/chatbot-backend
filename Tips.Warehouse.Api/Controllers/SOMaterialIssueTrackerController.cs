using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
 

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SOMaterialIssueTrackerController : ControllerBase
    {
        private IMaterialIssueTrackerRepository _materialIssueTrackerRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public SOMaterialIssueTrackerController(IConfiguration config, HttpClient httpClient,
            ILoggerManager logger, IMapper mapper, IMaterialIssueTrackerRepository materialIssueTrackerRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _materialIssueTrackerRepository = materialIssueTrackerRepository;
        }


        [HttpGet]
        public async Task<IActionResult> SOMaterialIssueTrackerDetailsByShopOrderNo(string ShopOrderNo)
        {
            ServiceResponse<List<ShopOrderMaterialIssueTrackerDto>> serviceResponse = new ServiceResponse<List<ShopOrderMaterialIssueTrackerDto>>();
            try
            {
                var shopOrderMaterialIssueTrackers = await _materialIssueTrackerRepository.SOMaterialIssueTrackerDetailsByShopOrderNo(ShopOrderNo);

                var groupedMaterialIssueItemDtoList = shopOrderMaterialIssueTrackers
                    .GroupBy(item => item.PartNumber)
                    .Select(group => new ShopOrderMaterialIssueTrackerDto 
                    {
                        PartNumber = group.Key,
                        ShopOrderNumber = group.First().ShopOrderNumber,
                        Description = group.First().Description, 
                        Bomversion = group.First().Bomversion,
                        IssuedQty = group.Sum(item => item.IssuedQty),
                        ConvertedToFgQty = group.Sum(item => item.ConvertedToFgQty),
                        BalanceQty = group.Sum(item => item.BalanceQty),
                        DataFrom = group.First().DataFrom
                    })
            .ToList();
                
                   // if (shopOrderMaterialIssueTrackers.Count() == 0)
                    if (groupedMaterialIssueItemDtoList.Count() == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SOMaterialTrackerIssue with itemNumber: {ShopOrderNo}, is invalid";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SOMaterialTrackerIssue with itemNumber: {ShopOrderNo}, is invalid");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned SOMaterialTrackerIssue with Itemnumber: {ShopOrderNo}");
                    var result = _mapper.Map<List<ShopOrderMaterialIssueTrackerDto>>(groupedMaterialIssueItemDtoList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned shopOrderMaterialIssueTrackers with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid shopOrderMaterialIssueTrackers action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Invalid shopOrderMaterialIssueTrackers{ex.Message},{ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



        //Add material request data to material issue tracker

        [HttpPost]
        public async Task<IActionResult> CreateMaterialRequestOnSOMaterialIssueTracker(List<ShopOrderDtoForMaterialRequest> dtoForMaterialRequest)
        {
            ServiceResponse<ShopOrderDtoForMaterialRequest> serviceResponse = new ServiceResponse<ShopOrderDtoForMaterialRequest>();
            try
            {
                if (dtoForMaterialRequest == null)
                {
                    _logger.LogError("MaterialRequest object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invoice object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid MaterialRequest object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                for (int i = 0; i < dtoForMaterialRequest.Count; i++)
                {
                    ShopOrderMaterialIssueTracker shopOrderMaterialIssueTracker = new ShopOrderMaterialIssueTracker
                    {
                        ShopOrderNumber = dtoForMaterialRequest[i].ShopOrderNumber,
                        PartNumber = dtoForMaterialRequest[i].PartNumber,
                        LotNumber = "",
                        MftrPartNumber = dtoForMaterialRequest[i].MftrPartNumber,
                        Description = dtoForMaterialRequest[i].Description,
                        ProjectNumber = dtoForMaterialRequest[i].ProjectNumber,
                        IssuedQty = dtoForMaterialRequest[i].IssueQty,
                        ConvertedToFgQty = 0,
                        UOM = "",
                        Warehouse = "",
                        Location = "",
                        Unit = "",
                        PartType = dtoForMaterialRequest[i].PartType,
                        DataFrom = "MR",
                        MRNumber = dtoForMaterialRequest[i].MRNumber
                    };

                    await _materialIssueTrackerRepository.CreateMaterialIssueTracker(shopOrderMaterialIssueTracker);
                    _materialIssueTrackerRepository.SaveAsync(); 
                } 
                 
                serviceResponse.Data = null;
                serviceResponse.Message = "CreateMaterialIssueTracker Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetInvoiceById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateMaterialIssueTracker action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



    }
}