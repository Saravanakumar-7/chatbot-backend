using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;


namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SOMaterialIssueTrackerController : ControllerBase
    {
        private IMaterialIssueTrackerRepository _materialIssueTrackerRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        public SOMaterialIssueTrackerController(IConfiguration config, HttpClient httpClient, IHttpClientFactory clientFactory,
            ILoggerManager logger, IMapper mapper, IMaterialIssueTrackerRepository materialIssueTrackerRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _materialIssueTrackerRepository = materialIssueTrackerRepository;
            _clientFactory = clientFactory;
        }


        [HttpGet]
        public async Task<IActionResult> SOMaterialIssueTrackerDetailsByShopOrderNo(string ShopOrderNo)
        {
            ServiceResponse<List<ShopOrderMaterialIssueTrackerDto>> serviceResponse = new ServiceResponse<List<ShopOrderMaterialIssueTrackerDto>>();
            try
            {
                var shopOrderMaterialIssueTrackers = await _materialIssueTrackerRepository.SOMaterialIssueTrackerDetailsByShopOrderNo(ShopOrderNo);

                //    var groupedMaterialIssueItemDtoList = shopOrderMaterialIssueTrackers
                //        .GroupBy(item => item.PartNumber)
                //        .Select(group => new ShopOrderMaterialIssueTrackerDto
                //        {
                //            PartNumber = group.Key,
                //            ShopOrderNumber = group.First().ShopOrderNumber,
                //            Description = group.First().Description,
                //            Bomversion = group.First().Bomversion,
                //            IssuedQty = group.Sum(item => item.IssuedQty),
                //            ConvertedToFgQty = group.Sum(item => item.ConvertedToFgQty),
                //            BalanceQty = group.Sum(item => item.BalanceQty),
                //            DataFrom = group.First().DataFrom
                //        })
                //.ToList();
                var groupedMaterialIssueItemDtoList = shopOrderMaterialIssueTrackers
    .Where(item => item.PartNumber != null) // Exclude items with null PartNumber
    .GroupBy(item => new { item.PartNumber, item.MRNumber, item.ShopOrderNumber, item.DataFrom }) // Group by multiple fields
    .Select(group => new ShopOrderMaterialIssueTrackerDto
    {
        PartNumber = group.Key.PartNumber,
        ShopOrderNumber = group.Key.ShopOrderNumber,
        Description = group.First().Description,
        Bomversion = group.First().Bomversion,
        MRNumber = group.First().MRNumber,
        IssuedQty = group.Sum(item => item.IssuedQty),
        ConvertedToFgQty = group.Sum(item => item.ConvertedToFgQty),
        BalanceQty = group.Sum(item => item.BalanceQty),
        DataFrom = group.Key.DataFrom // Use the DataFrom from the grouping key
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
                _logger.LogError($"Error Occured in GetCollectionTrackerById API for the following ShopOrderNo:{ShopOrderNo} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetCollectionTrackerById API for the following ShopOrderNo:{ShopOrderNo} \n {ex.Message}";
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
                        UOM = dtoForMaterialRequest[i].UOM,
                        Warehouse = dtoForMaterialRequest[i].Warehouse,
                        Location = dtoForMaterialRequest[i].Location,
                        Unit = dtoForMaterialRequest[i].Unit,
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
                _logger.LogError($"Error Occured in CreateMaterialRequestOnSOMaterialIssueTracker API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateMaterialRequestOnSOMaterialIssueTracker API : \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



    }
}