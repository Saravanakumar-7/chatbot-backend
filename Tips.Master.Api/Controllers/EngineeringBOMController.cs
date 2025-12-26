using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using AutoMapper;
using Contracts;
using Microsoft.EntityFrameworkCore;
using Entities.Migrations;
using System.Net;
using Newtonsoft.Json;
using Repository;
using NuGet.Packaging;
using System.Net.Http;
using Entities.Enums;
using System;
using MySqlX.XDevAPI.Common;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using NLog.Fluent;
using Org.BouncyCastle.Utilities;
using System.Collections.Generic;
using static Mysqlx.Notice.Warning.Types;
using MySqlX.XDevAPI;
using Microsoft.AspNetCore.StaticFiles;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class EngineeringBOMController : ControllerBase
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private IRepositoryWrapperForMaster _repository;
        private IReleaseEnggBomRepository _releaseEnggBomRepository;
        private ILoggerManager _logger;
        private IReleaseProductBomRepository _releaseProductBomRepository;
        private IMapper _mapper;
        private IReleaseCostBomRepository _releaseCostBomRepository;
        private IEnggBomRepository _enggBomRepository;
        private readonly IHttpClientFactory _clientFactory;
        public EngineeringBOMController(IHttpClientFactory clientFactory, HttpClient httpClient, IConfiguration config, IEnggBomRepository enggBomRepository,
            IRepositoryWrapperForMaster repository, IReleaseProductBomRepository releaseProductBomRepository, IReleaseCostBomRepository releaseCostBomRepository,
            IReleaseEnggBomRepository releaseEnggBomRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _httpClient = httpClient;
            _config = config;
            _mapper = mapper;
            _releaseCostBomRepository = releaseCostBomRepository;
            _releaseEnggBomRepository = releaseEnggBomRepository;
            _releaseProductBomRepository = releaseProductBomRepository;
            _enggBomRepository = enggBomRepository;
            _clientFactory = clientFactory;
        }
        // GET: api/<EngineeringBOMController>
        [HttpGet]
        public async Task<IActionResult> GetAllEnggBOM([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<EnggBomDto>> serviceResponse = new ServiceResponse<IEnumerable<EnggBomDto>>();

            try
            {
                var listOfBoms = await _repository.EnggBomRepository.GetAllEnggBOM(pagingParameter, searchParams);

                var metadata = new
                {
                    listOfBoms.TotalCount,
                    listOfBoms.PageSize,
                    listOfBoms.CurrentPage,
                    listOfBoms.HasNext,
                    listOfBoms.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));



                _logger.LogInfo("Returned all Boms");
                var bomDtoDetails = _mapper.Map<IEnumerable<EnggBomDto>>(listOfBoms);



                serviceResponse.Data = bomDtoDetails;
                serviceResponse.Message = "Returned all Engineering Boms Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllEnggBOM API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllEnggBOM API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetLatestBOMVersionNobyItemnumber([FromQuery] string ItemNumber)
        {
            ServiceResponse<decimal> serviceResponse = new ServiceResponse<decimal>();

            try
            {
                serviceResponse.Data = await _repository.EnggBomRepository.GetLatestBOMVersionNobyItemnumber(ItemNumber);
                serviceResponse.Message = $"Returned Latest BOM Version of {ItemNumber}";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetLatestBOMVersionNobyItemnumber API for Itemnuber:{ItemNumber}: \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = 0;
                serviceResponse.Message = $"Error Occured in GetLatestBOMVersionNobyItemnumber API for Itemnuber:{ItemNumber}: \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllParentBOMsofItemNumber([FromQuery] string ItemNumber)
        {
            ServiceResponse<List<EnggBomDetails>> serviceResponse = new ServiceResponse<List<EnggBomDetails>>();
            try
            {
                serviceResponse.Data = await _repository.EnggBomRepository.GetAllParentBOMsofItemNumber(ItemNumber);
                serviceResponse.Message = $"Returned all parent boms of ItemNumber:{ItemNumber}";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllParentBOMsofItemNumber API for Itemnuber:{ItemNumber}: \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllParentBOMsofItemNumber API for Itemnuber:{ItemNumber}: \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private async Task<List<EnggBomLevelSPReport>?> GetLevelSPReports(List<EnggBomLevelSPReport> SAspresent, int currentLevel)
        {

            var newlevel = new List<EnggBomLevelSPReport>();
            foreach (var Sa in SAspresent)
            {
                try
                {
                    var nextlevel = await _enggBomRepository.GetEnggLevelsSPReport(Sa.Child_ItemNumber);
                    if (nextlevel.Any())
                    { 
                        nextlevel.ForEach(x => x.Level = currentLevel + 1);
                        newlevel.AddRange(nextlevel);

                        if (nextlevel.Where(x => x.Child_PartType == PartType.SA).Count() > 0)
                        {
                            var otherlevels = await GetLevelSPReports(nextlevel.Where(x => x.Child_PartType == PartType.SA).ToList(), currentLevel + 1);
                            if (otherlevels.Any()) newlevel.AddRange(otherlevels);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing item {Sa.Child_ItemNumber}: {ex.Message}");
                    throw new InvalidOperationException($"Error with item {Sa.Child_ItemNumber}: {ex.Message}", ex);
                }
            }
            return newlevel;

        }
        [HttpGet]
        public async Task<IActionResult> GetAllEnggLevelsSPReport(string ItemNumber)
        {
            ServiceResponse<List<EnggBomLevelSPReport>> serviceResponse = new ServiceResponse<List<EnggBomLevelSPReport>>();
            try
            {
                var result = new List<EnggBomLevelSPReport>();
                var level1 = await _enggBomRepository.GetEnggLevelsSPReport(ItemNumber);
                if (level1.Count() == 0)
                {
                    _logger.LogError($"In GetAllEnggLevelsSPReport: Itemnumber: {ItemNumber} is not present in BOM");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"In GetAllEnggLevelsSPReport: Itemnumber: {ItemNumber} is not present in BOM";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return StatusCode(404, serviceResponse);
                }
                level1.ForEach(x => x.Level = 1);
                result.AddRange(level1);
                if (level1.Where(x => x.Child_PartType == PartType.SA).Count() > 0)
                {
                    var otherlevels = await GetLevelSPReports(level1.Where(x => x.Child_PartType == PartType.SA).ToList(), 1);
                    if(otherlevels.Any()) result.AddRange(otherlevels);
                }
                _logger.LogInfo("GetAllEnggLevelsSPReport was Successful");
                serviceResponse.Data = result.OrderBy(x => x.Level).ToList();
                serviceResponse.Message = $"GetAllEnggLevelsSPReport was Successful";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(200, serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllEnggLevelsSPReport API for the following ItemNumber : {ItemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllEnggLevelsSPReport API for the following ItemNumber : {ItemNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetEnggBomSPReportWithParam([FromBody] EnggBomSPReportDto enggBomSPReportDto)
        {
            ServiceResponse<IEnumerable<EnggBomSPReport>> serviceResponse = new ServiceResponse<IEnumerable<EnggBomSPReport>>();
            try
            {
                var products = await _enggBomRepository.GetEnggBomSPReportWithParam(enggBomSPReportDto.BOMId);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"EnggBomSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"EnggBomSPReport hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned EnggBomSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetEnggBomSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetEnggBomSPReportWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetEnggBomRevSPReportWithParam([FromBody] EnggBomRevSPReportDto enggBomRevSPReportDto)
        {
            ServiceResponse<IEnumerable<EnggBomSPReport>> serviceResponse = new ServiceResponse<IEnumerable<EnggBomSPReport>>();
            try
            {
                var products = await _enggBomRepository.GetEnggBomRevSPReportWithParam(enggBomRevSPReportDto.ItemNumber , enggBomRevSPReportDto.RevisionNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"EnggBomRevSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"EnggBomRevSPReport hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned EnggBomRevSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetEnggBomRevSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetEnggBomRevSPReportWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetEngganditsPP([FromQuery] string FGItemNumber, [FromQuery] decimal FGRevno, [FromBody] List<RfqSourcingPPdetailsforEngg> rfqSourcingPPdetails)
        {
            ServiceResponse<FGFinalLandedandMoqPrice> serviceResponse = new ServiceResponse<FGFinalLandedandMoqPrice>();
            try
            {
                FGFinalLandedandMoqPrice enggandpps = await _enggBomRepository.GetEngganditsPP(FGItemNumber, FGRevno, rfqSourcingPPdetails);
                serviceResponse.Data = enggandpps;
                serviceResponse.Message = "Returned all GetEngganditsPP Boms Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetEngganditsPP API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetEngganditsPP API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //coverage

        [HttpPost]
        public async Task<IActionResult> CoverageEnggBomChildDetails([FromBody] List<EnggBomCoverageDto> enggBomCoverageDtos)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                if (enggBomCoverageDtos == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "EnggBom object sent from the client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("EnggBom object sent from the client is null.");
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid EnggBom object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Enggbom object sent from the client.");
                    return BadRequest(serviceResponse);
                }
                foreach (var item in enggBomCoverageDtos)
                {
                    IEnumerable<CoverageEnggChildDto> coverageEnggChildDtos = await _enggBomRepository.GetEnggChildItemDetails(item.ItemNumber);
                    List<string> allChildItems = await GetChildItemsRecursive(item.ItemNumber);

                    //var orderItem = salesOrderItems.FirstOrDefault();
                    //orderItem.BalanceQty = orderItem.BalanceQty + item.DispatchQty;
                    //orderItem.DispatchQty -= item.DispatchQty;
                    //_salesOrderItemsRepository.UpdateSalesOrderItem(orderItem);
                }

                //_salesOrderItemsRepository.SaveAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CoverageEnggBomChildDetails API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CoverageEnggBomChildDetails API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //coverage recurssion Method

        private async Task<List<string>> GetChildItemsRecursive(string itemNumber)
        {
            List<string> allChildItems = new List<string>();


            // Retrieve the reference ID of the itemNumber from the enggbom table
            int enggBomId = await _enggBomRepository.GetEnggBomId(itemNumber);

            if (enggBomId == null)
            {
                return allChildItems; // ItemNumber not found in enggbom table, return an empty list
            }

            // Retrieve the child items of the enggbom table
            var childItems = await _enggBomRepository.GetEnggChildItemNumber(enggBomId);


            allChildItems.AddRange(childItems);

            foreach (var childItem in childItems)
            {
                // Recursively call the function to fetch child items of child items
                allChildItems.AddRange(await GetChildItemsRecursive(childItem));
            }

            return allChildItems;
        }

        //CoverageReport
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GeEnggBomChildByEnggBomId(int EnggBomId)
        //{
        //    ServiceResponse<EnggChildItem> serviceResponse = new ServiceResponse<EnggChildItem>();

        //    try
        //    {
        //        var enggChildDetails = await _enggBomRepository.GeEnggBomChildByEnggBomId(EnggBomId);

        //        if (enggChildDetails == null)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"EnggBomChildDetials hasn't been found in db.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            _logger.LogError($"EnggBomChildDetials with EnggBomId: {EnggBomId}, hasn't been found in db.");
        //            return Ok(serviceResponse);
        //        }
        //        else
        //        {
        //            _logger.LogInfo($"Returned EnggBomChildDetials with EnggBomId: {EnggBomId}");
        //            var result = _mapper.Map<EnggChildItem>(enggChildDetails);
        //            serviceResponse.Data = result;
        //            serviceResponse.Message = $"Returned EnggBomChildDetials";
        //            serviceResponse.Success = true;
        //            serviceResponse.StatusCode = HttpStatusCode.OK;
        //            return Ok(serviceResponse);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside GeEnggBomChildByEnggBomId action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Something went wrong. Please try again!";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}


        // GET api/<EngineeringBOMController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnggBomById(int id)
        {
            ServiceResponse<EnggBomDto> serviceResponse = new ServiceResponse<EnggBomDto>();

            try
            {
                var bomDetail = await _repository.EnggBomRepository.GetEnggBomById(id);



                if (bomDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Engineering Bom with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Engineering Bom with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Engineering Bom with id: {id}");
                    EnggBomDto enggBomDto = _mapper.Map<EnggBomDto>(bomDetail);
                    List<EnggChildItemDto> childItemsDtos = new List<EnggChildItemDto>();
                    enggBomDto.BomNREConsumableDto = _mapper.Map<List<BomNREConsumableDto>>(bomDetail.NREConsumable);

                    if (bomDetail.EnggChildItems != null)
                    {
                        foreach (var itemDetails in bomDetail.EnggChildItems)
                        {
                            EnggChildItemDto enggChildItemDto = _mapper.Map<EnggChildItemDto>(itemDetails);
                            enggChildItemDto.EnggAlternatesDtos = _mapper.Map<List<EnggAlternatesDto>>(itemDetails.EnggAlternates);
                            childItemsDtos.Add(enggChildItemDto);
                        }
                    }
                    enggBomDto.EnggChildItemDtos = childItemsDtos;
                    serviceResponse.Data = enggBomDto;
                    serviceResponse.Message = $"Returned Engineering Bom with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetEnggBomById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetEnggBomById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEnggBomByPartNumber(string itemNumber)
        {
            ServiceResponse<EnggBomDto> serviceResponse = new ServiceResponse<EnggBomDto>();

            try
            {
                var bomDetail = await _repository.EnggBomRepository.GetEnggBomByFgPartNumber(itemNumber);

                if (bomDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Engineering Bom with itemNumber: {itemNumber}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"Engineering Bom with id: {itemNumber}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Engineering Bom with itemNumber: {itemNumber}");
                    EnggBomDto enggBomDto = _mapper.Map<EnggBomDto>(bomDetail);
                    List<EnggChildItemDto> childItemsDtos = new List<EnggChildItemDto>();
                    enggBomDto.BomNREConsumableDto = _mapper.Map<List<BomNREConsumableDto>>(bomDetail.NREConsumable);

                    if (bomDetail.EnggChildItems != null)
                    {
                        foreach (var itemDetails in bomDetail.EnggChildItems)
                        {
                            EnggChildItemDto enggChildItemDto = _mapper.Map<EnggChildItemDto>(itemDetails);
                            enggChildItemDto.EnggAlternatesDtos = _mapper.Map<List<EnggAlternatesDto>>(itemDetails.EnggAlternates);
                            childItemsDtos.Add(enggChildItemDto);
                        }
                    }
                    enggBomDto.EnggChildItemDtos = childItemsDtos;
                    serviceResponse.Data = enggBomDto;
                    serviceResponse.Message = $"Returned Engineering Bom";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetEnggBomByPartNumber API for the following id : {itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetEnggBomByPartNumber API for the following id : {itemNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEnggChildBomQtyDetailsByFgItemNoAndRevNo(string itemNumber, decimal revisionNumber)
        {
            ServiceResponse<List<EnggChildBomQtyDetailsDto>> serviceResponse = new ServiceResponse<List<EnggChildBomQtyDetailsDto>>();

            try
            {
                var enggBomDetail = await _enggBomRepository
                          .GetEnggChildBomQtyDetailsByFgItemNoAndRevNo(itemNumber, revisionNumber);

                if (enggBomDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GetEnggChildBomQtyDetailsByFgItemNoAndRevNo hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GetEnggChildBomQtyDetailsByFgItemNoAndRevNo: {itemNumber},{revisionNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = enggBomDetail;
                    serviceResponse.Message = $"Returned GetEnggChildBomQtyDetailsByFgItemNoAndRevNo Successfully ";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetEnggChildBomQtyDetailsByFgItemNoAndRevNo API for the following itemNumber : {itemNumber} and revisionNumber:{revisionNumber}  \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetEnggChildBomQtyDetailsByFgItemNoAndRevNo API for the following itemNumber : {itemNumber} and revisionNumber:{revisionNumber}  \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEnggBomByItemNoAndRevNo(string itemNumber, decimal revisionNumber)
        {
            ServiceResponse<EnggBomDto> serviceResponse = new ServiceResponse<EnggBomDto>();

            try
            {
                var enggBomDetailByItemNoAndRevNo = await _repository.EnggBomRepository.GetEnggBomByItemNoAndRevNo(itemNumber, revisionNumber);

                if (enggBomDetailByItemNoAndRevNo == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Engineering Bom with ItemNoAndRevNo hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Engineering Bom with ItemNoAndRevNo: {itemNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Engineering Bom with ItemNoAndRevNo: {itemNumber},{revisionNumber}");
                    EnggBomDto enggBomDto = _mapper.Map<EnggBomDto>(enggBomDetailByItemNoAndRevNo);
                    List<EnggChildItemDto> childItemsDtos = new List<EnggChildItemDto>();
                    enggBomDto.BomNREConsumableDto = _mapper.Map<List<BomNREConsumableDto>>(enggBomDetailByItemNoAndRevNo.NREConsumable);

                    if (enggBomDetailByItemNoAndRevNo.EnggChildItems != null)
                    {
                        foreach (var itemDetails in enggBomDetailByItemNoAndRevNo.EnggChildItems)
                        {
                            EnggChildItemDto enggChildItemDto = _mapper.Map<EnggChildItemDto>(itemDetails);
                            enggChildItemDto.EnggAlternatesDtos = _mapper.Map<List<EnggAlternatesDto>>(itemDetails.EnggAlternates);
                            childItemsDtos.Add(enggChildItemDto);
                        }
                    }
                    enggBomDto.EnggChildItemDtos = childItemsDtos;
                    serviceResponse.Data = enggBomDto;
                    serviceResponse.Message = $"Returned EnggBomByItemNoAndRevNo Successfully ";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetEnggBomByItemNoAndRevNo API for the following itemNumber : {itemNumber} and revisionNumber:{revisionNumber}  \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetEnggBomByItemNoAndRevNo API for the following itemNumber : {itemNumber} and revisionNumber:{revisionNumber}  \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //get latest bom child Qty 


        [HttpGet]
        public async Task<IActionResult> GetLatestEnggProductionBomVersionDetailByItemNumber(string fgPartNumber)
        {
            ServiceResponse<decimal> serviceResponse = new ServiceResponse<decimal>();

            try
            {
                decimal revisionNo = await _releaseProductBomRepository.GetLatestProductionBomByItemNumber(fgPartNumber);

                if (revisionNo == null)
                {
                    serviceResponse.Data = 0;
                    serviceResponse.Message = $"Production Bom latest Version fgPartNumber: {fgPartNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Production Bom latest Version with fgPartNumber: {fgPartNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Production Bom latest Version with fgPartNumber: {fgPartNumber}");
                    serviceResponse.Data = revisionNo;
                    serviceResponse.Message = $"Production Bom latest Version with fgPartNumber: {fgPartNumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetLatestEnggProductionBomVersionDetailByItemNumber API for the following fgPartNumber : {fgPartNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = 0;
                serviceResponse.Message = $"Error Occured in GetLatestEnggProductionBomVersionDetailByItemNumber API for the following fgPartNumber : {fgPartNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetLatestEnggBomVersionDetailByItemNumber(string fgPartNumber)
        {
            ServiceResponse<EnggBomDto> serviceResponse = new ServiceResponse<EnggBomDto>();

            try
            {
                decimal revisionNo = await _releaseProductBomRepository.GetLatestProductionBomByItemNumber(fgPartNumber);

                var bom = await _repository.EnggBomRepository.GetLatestEnggBomVersionDetailByItemNumber(fgPartNumber, revisionNo);
                if (bom == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Engineering Bom with fgPartNumber: {fgPartNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Engineering Bom with fgPartNumber: {fgPartNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Engineering Bom with fgPartNumber: {fgPartNumber}");
                    EnggBomDto enggBomDto = _mapper.Map<EnggBomDto>(bom);
                    List<EnggChildItemDto> childItemsDtos = new List<EnggChildItemDto>();
                    foreach (var itemDetails in bom.EnggChildItems)
                    {
                        EnggChildItemDto enggChildItemDto = _mapper.Map<EnggChildItemDto>(itemDetails);
                        enggChildItemDto.EnggAlternatesDtos = _mapper.Map<List<EnggAlternatesDto>>(itemDetails.EnggAlternates);
                        childItemsDtos.Add(enggChildItemDto);
                    }
                    enggBomDto.EnggChildItemDtos = childItemsDtos;
                    serviceResponse.Data = enggBomDto;
                    serviceResponse.Message = $"Returned Engineering Bom with fgPartNumber: {fgPartNumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetLatestEnggBomVersionDetailByItemNumber API for the following id : {fgPartNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetLatestEnggBomVersionDetailByItemNumber API for the following id : {fgPartNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateEnggBom([FromBody] EnggBomPostDto enggBomPostDto)
        {
            ServiceResponse<EnggBomDto> serviceResponse = new ServiceResponse<EnggBomDto>();

            try
            {
                if (enggBomPostDto is null)
                {
                    _logger.LogError("Engineering Bom object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Engineering Bom object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Engineering Bom object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Engineering Bom object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var enggBomList = _mapper.Map<EnggBom>(enggBomPostDto);

                enggBomList.RevisionNumber = 1;

                var enggNre = enggBomPostDto.BomNREConsumablePostDto;
                var nreList = new List<NREConsumable>();
                for (int i = 0; i < enggNre.Count; i++)
                {
                    NREConsumable enggChildItemDetails = _mapper.Map<NREConsumable>(enggNre[i]);
                    nreList.Add(enggChildItemDetails);

                }
                enggBomList.NREConsumable = nreList;

                var enggChildItemDto = enggBomPostDto.EnggChildItemPosts;

                var enggChildItemList = new List<EnggChildItem>();
                if (enggChildItemDto != null)
                {
                    for (int i = 0; i < enggChildItemDto.Count; i++)
                    {
                        EnggChildItem enggChildItemDetail = _mapper.Map<EnggChildItem>(enggChildItemDto[i]);
                        enggChildItemDetail.EnggAlternates = _mapper.Map<List<EnggAlternates>>(enggChildItemDto[i].EnggAlternatesPostDtos);
                        enggChildItemList.Add(enggChildItemDetail);
                    }
                }
                else
                {
                    _logger.LogError("Engineering Bom Item object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Engineering Bom Items Object is Empty.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                enggBomList.EnggChildItems = enggChildItemList;

                await _repository.EnggBomRepository.CreateEnggBom(enggBomList);

                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateEnggBom API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateEnggBom API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateEnggBomWithValidation([FromBody] EnggBomPostDtoWithValidation enggBomPostDto)
        {
            ServiceResponse<EnggBomDto> serviceResponse = new ServiceResponse<EnggBomDto>();

            try
            {
                if (enggBomPostDto is null)
                {
                    _logger.LogError("Engineering Bom object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Engineering Bom object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Engineering Bom object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Engineering Bom object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (enggBomPostDto.EnggChildItemPosts.Count() < 1)
                {
                    _logger.LogError("Atleast 1 Child Item Is Required");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Atleast 1 Child Item Is Required";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotAcceptable;
                    return StatusCode(406, serviceResponse);
                }
                var bomitem = enggBomPostDto.EnggChildItemPosts.Where(x => x.ItemNumber == enggBomPostDto.ItemNumber).Count();
                if (bomitem > 0)
                {
                    _logger.LogError($"BOM can't contain itself");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BOM can't contain itself";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotAcceptable;
                    return StatusCode(406, serviceResponse);
                }
                var activeitems = enggBomPostDto.EnggChildItemPosts.Where(x => x.IsActive == true).Count();
                if (activeitems < 1)
                {
                    _logger.LogError($"Atleast 1 Active BOM Item Is Required");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Atleast 1 Active BOM Item Is Required";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotAcceptable;
                    return StatusCode(406, serviceResponse);
                }
                var duplicates = enggBomPostDto.EnggChildItemPosts.GroupBy(str => str.ItemNumber).Where(group => group.Count() > 1).Select(group => group.Key).ToList();
                if (duplicates.Count() > 0)
                {
                    string? items = null;
                    foreach (var item in duplicates)
                    {
                        if (items == null) items = item;
                        else items = items + "," + item;
                    }
                    _logger.LogError($"BOM Items : {items} Can't Repeat");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BOM Items : {items} Can't Repeat";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotAcceptable;
                    return StatusCode(406, serviceResponse);
                }
                var lowQtyitems = enggBomPostDto.EnggChildItemPosts.Where(x => x.Quantity <= 0).ToList();
                if (lowQtyitems.Count() > 0)
                {
                    string? items = null;
                    foreach (var item in lowQtyitems)
                    {
                        if (items == null) items = item.ItemNumber;
                        else items = items + "," + item.ItemNumber;
                    }
                    _logger.LogError($"BOM Items : {items} should have Quantity greater then 0");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BOM Items : {items} should have Quantity greater then 0";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotAcceptable;
                    return StatusCode(406, serviceResponse);
                }

                var ExistingBom = await _repository.EnggBomRepository.CheckEnggBomByItemNumber(enggBomPostDto.ItemNumber);
                if (ExistingBom)
                {
                    _logger.LogError($"BOM: {enggBomPostDto.ItemNumber} already Exists");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BOM: {enggBomPostDto.ItemNumber} already Exists";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotAcceptable;
                    return StatusCode(406, serviceResponse);
                }
                var itemmasterDetails = await _repository.ItemMasterRepository.GetItemMasterByItemNumber(enggBomPostDto.ItemNumber);
                if (itemmasterDetails == null)
                {
                    _logger.LogError($"BOM: {enggBomPostDto.ItemNumber} Doesnot Exists in ItemMater");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BOM: {enggBomPostDto.ItemNumber} Doesnot Exists in ItemMater";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotAcceptable;
                    return StatusCode(406, serviceResponse);
                }
                if (itemmasterDetails.ItemType != PartType.FG && itemmasterDetails.ItemType != PartType.SA)
                {
                    _logger.LogError($"BOM ItemType must be FinishedGood or SubAssembly");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BOM ItemType must be FinishedGood or SubAssembly";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotAcceptable;
                    return StatusCode(406, serviceResponse);
                }
                string? nonexistantitems = null;
                string? wrongTypeitems = null;
                foreach (var items in enggBomPostDto.EnggChildItemPosts)
                {
                    var childdetails = await _repository.ItemMasterRepository.GetItemMasterByItemNumber(items.ItemNumber);
                    if (childdetails == null)
                    {
                        if (nonexistantitems == null) nonexistantitems = items.ItemNumber;
                        else nonexistantitems = nonexistantitems + "," + items.ItemNumber;
                    }
                    else if (childdetails.ItemType != PartType.PurchasePart && childdetails.ItemType != PartType.SA)
                    {
                        if (wrongTypeitems == null) wrongTypeitems = items.ItemNumber;
                        else wrongTypeitems = wrongTypeitems + "," + items.ItemNumber;
                    }
                }
                if (nonexistantitems != null)
                {
                    _logger.LogError($"Item: {nonexistantitems} Doesnot Exists in ItemMater");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Item: {nonexistantitems} Doesnot Exists in ItemMater";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotAcceptable;
                    return StatusCode(406, serviceResponse);
                }
                if (wrongTypeitems != null)
                {
                    _logger.LogError($"Item: {wrongTypeitems} ItemType must be PurchasePart or SubAssembly");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Item: {wrongTypeitems} ItemType must be PurchasePart or SubAssembly";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotAcceptable;
                    return StatusCode(406, serviceResponse);
                }
                var enggBomList = _mapper.Map<EnggBom>(enggBomPostDto);

                enggBomList.RevisionNumber = 1;

                var enggNre = enggBomPostDto.BomNREConsumablePostDto;
                var nreList = new List<NREConsumable>();
                for (int i = 0; i < enggNre.Count; i++)
                {
                    NREConsumable enggChildItemDetails = _mapper.Map<NREConsumable>(enggNre[i]);
                    nreList.Add(enggChildItemDetails);

                }
                enggBomList.NREConsumable = nreList;

                var enggChildItemDto = enggBomPostDto.EnggChildItemPosts;

                var enggChildItemList = new List<EnggChildItem>();
                if (enggChildItemDto != null)
                {
                    for (int i = 0; i < enggChildItemDto.Count; i++)
                    {
                        EnggChildItem enggChildItemDetail = _mapper.Map<EnggChildItem>(enggChildItemDto[i]);
                        enggChildItemDetail.EnggAlternates = _mapper.Map<List<EnggAlternates>>(enggChildItemDto[i].EnggAlternatesPostDtos);
                        enggChildItemList.Add(enggChildItemDetail);
                    }
                }
                else
                {
                    _logger.LogError("Engineering Bom Item object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Engineering Bom Items Object is Empty.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                enggBomList.EnggChildItems = enggChildItemList;

                await _repository.EnggBomRepository.CreateEnggBom(enggBomList);

                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateEnggBomWithValidation API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateEnggBomWithValidation API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //PUT api/<EngineeringBOMController>/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateEnggBom(int id, [FromBody] EnggBomUpdateDto enggBomDto, [FromQuery] RevisionType revisionType)
        //{
        //    ServiceResponse<EnggBomDto> serviceResponse = new ServiceResponse<EnggBomDto>();

        //    try
        //    {
        //        if (enggBomDto is null)
        //        {
        //            _logger.LogError("Update EngineeringBom object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Update EngineeringBom object is null";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogError("Invalid Update EngineeringBom object sent from client.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid Update EngineeringBom object sent from client.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        var updateEnggBom = await _repository.EnggBomRepository.GetEnggBomById(id);
        //        if (updateEnggBom is null)
        //        {
        //            _logger.LogError($"Update EngineeringBom with id: {id}, hasn't been found in db.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"Update EngineeringBom with id: {id}, hasn't been found in db.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(serviceResponse);
        //        }


        //        var enggBomList = _mapper.Map<EnggBom>(updateEnggBom);

        //        if (revisionType == 0)
        //        {
        //            enggBomList.RevisionNumber = enggBomList.RevisionNumber + Convert.ToDecimal(0.1);
        //        }
        //        else
        //        {
        //            var revRound = Math.Round(enggBomList.RevisionNumber);
        //            enggBomList.RevisionNumber = revRound + Convert.ToDecimal(1.0);
        //        }


        //        var enggChildItemDto = enggBomDto.EnggChildItemUpdates;

        //        var enggChildItemList = new List<EnggChildItem>();
        //        for (int i = 0; i < enggChildItemDto.Count; i++)
        //        {
        //            EnggChildItem enggChildItemDetail = _mapper.Map<EnggChildItem>(enggChildItemDto[i]);
        //            enggChildItemDetail.EnggAlternates = _mapper.Map<List<EnggAlternates>>(enggChildItemDto[i].EnggAlternatesUpdateDtos);
        //            enggChildItemList.Add(enggChildItemDetail);

        //        }
        //        enggBomList.EnggChildItems = enggChildItemList;

        //        var data = _mapper.Map(enggBomDto, enggBomList);

        //        string result = await _repository.EnggBomRepository.UpdateEnggBom(data);



        //        _logger.LogInfo(result);
        //        _repository.SaveAsync();
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Update Successfully";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside UpdateEngineering Bom action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}

        [HttpPut]
        public async Task<IActionResult> UpdateEnggBom([FromBody] EnggBomUpdateDto enggBomUpdateDto, [FromQuery] RevisionType revisionType)
        {
            ServiceResponse<EnggBomDto> serviceResponse = new ServiceResponse<EnggBomDto>();

            try
            {
                if (enggBomUpdateDto is null)
                {
                    _logger.LogError("EngineeringBom object sent from client is null for update.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "EngineeringBom object is null for update";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid EngineeringBom object sent from client for update.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid EngineeringBom object sent from client for update.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var enggBomList = _mapper.Map<EnggBom>(enggBomUpdateDto);
                var enggNre = enggBomUpdateDto.BomNREConsumableUpdateDto;
                var nreList = new List<NREConsumable>();
                for (int i = 0; i < enggNre.Count; i++)
                {
                    NREConsumable enggChildItemDetails = _mapper.Map<NREConsumable>(enggNre[i]);
                    nreList.Add(enggChildItemDetails);

                }
                enggBomList.NREConsumable = nreList;
                var enggChildItemDto = enggBomUpdateDto.EnggChildItemUpdates;
                var enggChildItemList = new List<EnggChildItem>();
                if (enggChildItemDto != null)
                {
                    for (int i = 0; i < enggChildItemDto.Count; i++)
                    {
                        EnggChildItem enggChildItemDetail = _mapper.Map<EnggChildItem>(enggChildItemDto[i]);
                        enggChildItemDetail.EnggAlternates = _mapper.Map<List<EnggAlternates>>(enggChildItemDto[i].EnggAlternatesUpdateDtos);
                        enggChildItemList.Add(enggChildItemDetail);
                    }
                }
                enggBomList.EnggChildItems = enggChildItemList;
                await _repository.EnggBomRepository.UpdateEnggBomVersion(enggBomList,revisionType);
                _repository.SaveAsync();
                _logger.LogInfo("Engineering BOM Updated successfully");
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)    
            {
                _logger.LogError($"Error Occured in UpdateEnggBom API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateEnggBom API : \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<EngineeringBOMController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // GET: api/<ReleaseEnggBomController>
        [HttpGet]
        public async Task<IActionResult> GetAllReleaseEnggBom([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ReleaseEnggBomDto>> serviceResponse = new ServiceResponse<IEnumerable<ReleaseEnggBomDto>>();

            try
            {
                var listOfReleaseEnggBom = await _releaseEnggBomRepository.GetAllReleaseEnggBom(pagingParameter, searchParams);

                var metadata = new
                {
                    listOfReleaseEnggBom.TotalCount,
                    listOfReleaseEnggBom.PageSize,
                    listOfReleaseEnggBom.CurrentPage,
                    listOfReleaseEnggBom.HasNext,
                    listOfReleaseEnggBom.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ReleaseEnggBom");
                var releaseEnggBomDtoDetails = _mapper.Map<IEnumerable<ReleaseEnggBomDto>>(listOfReleaseEnggBom);
                serviceResponse.Data = releaseEnggBomDtoDetails;
                serviceResponse.Message = "Returned all ReleaseEnggBom  Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllReleaseEnggBom API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Error Occured in GetAllReleaseEnggBom API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET api/<ReleaseEnggBomController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReleaseEnggBomById(int id)
        {
            ServiceResponse<ReleaseEnggBomDto> serviceResponse = new ServiceResponse<ReleaseEnggBomDto>();

            try
            {
                //var releaseEnggBomDtoDetails = await _repository.releaseEnggBomRepository.GetReleaseEnggBomById(id);
                var releaseEnggBomDtoDetails = await _releaseEnggBomRepository.GetReleaseEnggBomById(id);


                if (releaseEnggBomDtoDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ReleaseEnggBomDetails hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ReleaseEnggBomDetails with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ReleaseEnggBomDetails with id: {id}");
                    var result = _mapper.Map<ReleaseEnggBomDto>(releaseEnggBomDtoDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned ReleaseEnggBomDetails with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetReleaseEnggBomById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetReleaseEnggBomById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<ReleaseEnggBomController>
        [HttpPost]
        public async Task<IActionResult> CreateReleaseEnggBom([FromBody] ReleaseEnggBomDtoPost releaseEnggBomDtoPost)
        {
            ServiceResponse<ReleaseEnggBomDtoPost> serviceResponse = new ServiceResponse<ReleaseEnggBomDtoPost>();

            try
            {
                if (releaseEnggBomDtoPost is null)
                {
                    _logger.LogError("ReleaseEnggBom object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ReleaseEnggBom object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ReleaseEnggBom object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ReleaseEnggBom object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var release = _mapper.Map<EngineeringBom>(releaseEnggBomDtoPost);
                release.IsReleaseCompleted = true;
                await _releaseEnggBomRepository.CreateReleaseEnggBom(release);
                await _enggBomRepository.ReleasedEnggBomByItemAndRevisionNumber(releaseEnggBomDtoPost.ItemNumber, releaseEnggBomDtoPost.ReleaseVersion);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ReleaseEnggBom Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetReleaseEnggBomById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateReleaseEnggBom API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateReleaseEnggBom API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // PUT api/<ReleaseEnggBomController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReleaseEnggBom(int id, [FromBody] ReleaseEnggBomDtoUpdate releaseEnggBomDtoUpdate)
        {
            ServiceResponse<ReleaseEnggBomDtoUpdate> serviceResponse = new ServiceResponse<ReleaseEnggBomDtoUpdate>();

            try
            {
                if (releaseEnggBomDtoUpdate is null)
                {
                    _logger.LogError("Update ReleaseEnggBom object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ReleaseEnggBom object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update ReleaseEnggBom object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update ReleaseEnggBom object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updateReleaseEnggBom = await _releaseEnggBomRepository.GetReleaseEnggBomById(id);
                if (updateReleaseEnggBom is null)
                {
                    _logger.LogError($" updateReleaseEnggBom with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $" updateReleaseEnggBom hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var data = _mapper.Map(releaseEnggBomDtoUpdate, updateReleaseEnggBom);
                string result = await _releaseEnggBomRepository.UpdateReleaseEnggBom(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ReleaseEnggBom Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdateReleaseEnggBom API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateReleaseEnggBom API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // DELETE api/<ReleaseEnggBomController>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReleaseEnggBom(int id)
        {
            ServiceResponse<ReleaseEnggBomDto> serviceResponse = new ServiceResponse<ReleaseEnggBomDto>();

            try
            {
                var deleteReleaseEnggBom = await _releaseEnggBomRepository.GetReleaseEnggBomById(id);
                if (deleteReleaseEnggBom == null)
                {
                    _logger.LogError($"deleteReleaseEnggBom  with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $" deleteReleaseEnggBom hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _releaseEnggBomRepository.DeleteReleaseEnggBom(deleteReleaseEnggBom);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ReleaseEnggBom Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeleteReleaseEnggBom API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteReleaseEnggBom API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCostingBOM([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CostingBom>> serviceResponse = new ServiceResponse<IEnumerable<CostingBom>>();

            try
            {
                var costingBomDetails = await _releaseCostBomRepository.GetAllCostingBom(pagingParameter, searchParams);

                var metadata = new
                {
                    costingBomDetails.TotalCount,
                    costingBomDetails.PageSize,
                    costingBomDetails.CurrentPage,
                    costingBomDetails.HasNext,
                    costingBomDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));



                _logger.LogInfo("Returned all Boms");
                var costingBomList = _mapper.Map<IEnumerable<CostingBom>>(costingBomDetails);



                serviceResponse.Data = costingBomList;
                serviceResponse.Message = "Returned all CostingBoms Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllCostingBOM API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllCostingBOM API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //by passing bom id get enggbom chid items
        [HttpGet]
        public async Task<IActionResult> GetEnggChildItemNumberByEnggbom(int bomId)
        {
            ServiceResponse<List<EnggChildItem>> serviceResponse = new ServiceResponse<List<EnggChildItem>>();
            try
            {
                var enggBomDetailsById = await _enggBomRepository.GetEnggChildItemNumberByEnggbom(bomId);

                if (enggBomDetailsById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"enggBomDetailsById hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"enggBomDetailsById with id: {bomId}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned enggBomDetailsById with id: {bomId}");
                    var result = _mapper.Map<List<EnggChildItem>>(enggBomDetailsById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned enggBomDetailsById Successfully.";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetEnggChildItemNumberByEnggbom API for the following id : {bomId} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetEnggChildItemNumberByEnggbom API for the following id : {bomId} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCostingBomById(int id)
        {
            ServiceResponse<CostingBom> serviceResponse = new ServiceResponse<CostingBom>();

            try
            {
                //var productionBomDetailsById = await _repository.releaseEnggBomRepository.GetReleaseEnggBomById(id);
                var costingBomDetailsById = await _releaseCostBomRepository.GetCostingBomById(id);


                if (costingBomDetailsById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CostingBomDetails hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CostingBomDetails with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned CostingBomDetails with id: {id}");
                    var result = _mapper.Map<CostingBom>(costingBomDetailsById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned CostingBomDetailsById Successfully.";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetCostingBomById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetCostingBomById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<ReleaseCostBomController>
        [HttpPost]
        public async Task<IActionResult> CreateReleaseCostBom([FromBody] CostingBomDtoPost releaseCostBomDtoPost)
        {
            ServiceResponse<CostingBomDtoPost> serviceResponse = new ServiceResponse<CostingBomDtoPost>();

            try
            {
                if (releaseCostBomDtoPost is null)
                {
                    _logger.LogError("ReleaseCostBom object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ReleaseCostBom object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ReleaseCostBom object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ReleaseCostBom object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var release = _mapper.Map<CostingBom>(releaseCostBomDtoPost);
                release.IsReleaseCostCompleted = true;
                await _releaseCostBomRepository.CreateReleaseCostBom(release);
                _repository.SaveAsync();
                await _releaseEnggBomRepository.ReleasedEnggBomByItemAndRevisionNumber(releaseCostBomDtoPost.ItemNumber, releaseCostBomDtoPost.ReleaseVersion);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ReleaseCostBom Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetReleaseCostBomById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateReleaseCostBom API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateReleaseCostBom API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCostingReleasedFGTGItemNoList()
        {
            ServiceResponse<IEnumerable<ReleasedCostingFGTGItemNoListDto>> serviceResponse = new ServiceResponse<IEnumerable<ReleasedCostingFGTGItemNoListDto>>();
            try
            {
                var releasedCostingfgTgItemNoList = await _repository.ReleaseCostBomRepository.GetAllCostingReleasedFGTGItemNoList();

                _logger.LogInfo("Returned all Released CostingBom FG and TG ItemNoList Sucessfully");
                serviceResponse.Data = releasedCostingfgTgItemNoList;
                serviceResponse.Message = "Returned all Released CostingBom FG and TG ItemNoList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)

            {
                _logger.LogError($"Error Occured in GetAllCostingReleasedFGTGItemNoList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllCostingReleasedFGTGItemNoList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEnggBOMItemNumber()
        {
            ServiceResponse<IEnumerable<EnggBomItemDto>> serviceResponse = new ServiceResponse<IEnumerable<EnggBomItemDto>>();
            try
            {
                var listOfBomItem = await _repository.EnggBomRepository.GetAllEnggBOMItemNumber();

                _logger.LogInfo("Returned all EnggBomsItems");
                var bomDtoDetails = _mapper.Map<IEnumerable<EnggBomItemDto>>(listOfBomItem);
                serviceResponse.Data = bomDtoDetails;
                serviceResponse.Message = "Returned all Engineering BomItems Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllEnggBOMItemNumber API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllEnggBOMItemNumber API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEnggBOMPPItemNumbers()
        {
            ServiceResponse<IEnumerable<EnggBomItemDto>> serviceResponse = new ServiceResponse<IEnumerable<EnggBomItemDto>>();
            try
            {
                var listOfBomItem = await _repository.EnggBomRepository.GetAllEnggBOMPPItemNumbers();

                _logger.LogInfo("Returned all EnggBomsItems");
                var bomDtoDetails = _mapper.Map<IEnumerable<EnggBomItemDto>>(listOfBomItem);
                serviceResponse.Data = bomDtoDetails;
                serviceResponse.Message = "Returned all Engineering pp BomItems Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllEnggBOMPPItemNumbers API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllEnggBOMPPItemNumbers API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllEnggBOMFGItemNumbers()
        {
            ServiceResponse<IEnumerable<EnggBomItemDto>> serviceResponse = new ServiceResponse<IEnumerable<EnggBomItemDto>>();
            try
            {
                var listOfBomItem = await _repository.EnggBomRepository.GetAllEnggBOMFGItemNumbers();

                _logger.LogInfo("Returned all EnggBomsItems");
                var bomDtoDetails = _mapper.Map<IEnumerable<EnggBomItemDto>>(listOfBomItem);
                serviceResponse.Data = bomDtoDetails;
                serviceResponse.Message = "Returned all Engineering FG BomItems Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllEnggBOMFGItemNumbers API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllEnggBOMFGItemNumbers API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllRevisionNumbersByEnggBOMItemNumber(string itemNumber)
        {
            ServiceResponse<IEnumerable<EnggBomItemDtoWithRevisions>> serviceResponse = new ServiceResponse<IEnumerable<EnggBomItemDtoWithRevisions>>();
            try
            {
                var listOfBomItem = await _repository.EnggBomRepository.GetAllRevisionNumbersByEnggBOMItemNumber(itemNumber);

                _logger.LogInfo("Returned all EnggBomItemDtoWithRevisions");
                var bomDtoDetails = _mapper.Map<IEnumerable<EnggBomItemDtoWithRevisions>>(listOfBomItem);
                serviceResponse.Data = bomDtoDetails;
                serviceResponse.Message = "Returned all Engineering EnggBomItemDtoWithRevisions Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllEnggBOMItemNumberWithRevisionNumbers API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllEnggBOMItemNumberWithRevisionNumbers API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



        //aravind
        //[HttpPost]
        //public async Task<IActionResult> GetFGBomItemsChildDetails([FromBody] List<string> itemNumber)
        //{
        //    ServiceResponse<List<EnggBomFGItemNumberWithQtyDto>> serviceResponse = new ServiceResponse<List<EnggBomFGItemNumberWithQtyDto>>();
        //    List<EnggBomFGItemNumberWithQtyDto> fgBomDetails = null;
        //    try
        //    {
        //        if (itemNumber is null)
        //        {
        //            _logger.LogError("ItemNumber object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Item Number object is null";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }

        //        fgBomDetails = await _enggBomRepository.GetFGBomItemsChildDetails(itemNumber);
        //        List<EnggBomFGItemNumberWithQtyDto> rfqCSDto = _mapper.Map<List<EnggBomFGItemNumberWithQtyDto>>(fgBomDetails);
        //        //rfqCSDto = itemMasterRouting;
        //        serviceResponse.Data = rfqCSDto;
        //        serviceResponse.Message = "List Of FG BOM Child ItemNumber ";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong FG BOM Child ItemNumber action: {ex.Message} {ex.InnerException}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}
        [HttpPost]
        public async Task<IActionResult> GetFGBomItemsChildDetails([FromBody] List<RfqEnggitemSourcingDto> itemNumber)
        {
            ServiceResponse<List<EnggBomFGItemNumberWithQtyDto>> serviceResponse = new ServiceResponse<List<EnggBomFGItemNumberWithQtyDto>>();
            List<EnggBomFGItemNumberWithQtyDto> fgBomDetails = null;
            try
            {
                if (itemNumber is null)
                {
                    _logger.LogError("ItemNumber object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Item Number object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                fgBomDetails = await _enggBomRepository.GetFGBomItemsChildDetails(itemNumber);
                List<EnggBomFGItemNumberWithQtyDto> rfqCSDto = _mapper.Map<List<EnggBomFGItemNumberWithQtyDto>>(fgBomDetails);
                //rfqCSDto = itemMasterRouting;
                serviceResponse.Data = rfqCSDto;
                serviceResponse.Message = "List Of FG BOM Child ItemNumber ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetFGBomItemsChildDetails API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetFGBomItemsChildDetails API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetKitBomChildDetails( string kitItemNumber ,decimal KitRevNo)
        {
            ServiceResponse<List<EnggBomKitItemNumberWithQtyDto>> serviceResponse = new ServiceResponse<List<EnggBomKitItemNumberWithQtyDto>>();
            
            try
            {
                if (kitItemNumber is null)
                {
                    _logger.LogError("KitItemNumber object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "KitItemNumber object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var kitBomDetails = await _enggBomRepository.GetKitBomChildDetails(kitItemNumber, KitRevNo);
                serviceResponse.Data = kitBomDetails;
                serviceResponse.Message = "List Of Kit BOM Child ItemNumber ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong Kit BOM Child ItemNumber action: {ex.Message} {ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetFGCostingDetailsByItemNumber(List<string> fgItemNumberList)
        {
            ServiceResponse<List<EnggBomFGCostItemNumberWithQtyDto>> serviceResponse = new ServiceResponse<List<EnggBomFGCostItemNumberWithQtyDto>>();

            try
            {
                List<EnggBomFGCostItemNumberWithQtyDto> fgBomDetails = await GetFgChildsWightedAvgCost(fgItemNumberList);
                serviceResponse.Data = fgBomDetails;
                serviceResponse.Message = "Returned GetFGCostingDetails Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetFGCostingDetailsByItemNumber API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetFGCostingDetailsByItemNumber API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetFGCostingDetails()
        {
            ServiceResponse<List<EnggBomFGCostItemNumberWithQtyDto>> serviceResponse = new ServiceResponse<List<EnggBomFGCostItemNumberWithQtyDto>>();

            try
            {
                var fgItemNumber = await _repository.ItemMasterRepository.GetAllFGItemNumberList();
                List<EnggBomFGCostItemNumberWithQtyDto> fgBomDetails = await GetFgChildsWightedAvgCost(fgItemNumber);

                serviceResponse.Data = fgBomDetails;
                serviceResponse.Message = "Returned GetFGCostingDetails Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetFGCostingDetails API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetFGCostingDetails API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private async Task<List<EnggBomFGCostItemNumberWithQtyDto>> GetFgChildsWightedAvgCost(IEnumerable<string> fgItemNumber)
        {
            List<EnggBomFGCostItemNumberWithQtyDto> fgBomDetails = new List<EnggBomFGCostItemNumberWithQtyDto>();
            foreach (var fgItem in fgItemNumber)
            {
                var fgEnggBomItemsDetails = await _enggBomRepository.GetFGBomItemsChildCostingDetails(fgItem);

                if (fgEnggBomItemsDetails.Count() > 0)
                {
                    foreach (var item in fgEnggBomItemsDetails)
                    {
                        if (item.ItemNumber != null)
                        {
                            var weightedAvgRateDetails = await _repository.WeightedAvgRateRepository.GetWeightedAvgRateDetailsByItemNumber(item.ItemNumber);
                            if (weightedAvgRateDetails != null)
                            {
                                item.WeightedAvg = item.QtyReq * weightedAvgRateDetails.Avg_cost;
                                fgBomDetails.Add(item);
                            }

                        }
                    }
                }
            }

            return fgBomDetails;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductionBOM([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ProductionBom>> serviceResponse = new ServiceResponse<IEnumerable<ProductionBom>>();

            try
            {
                var productionBomDetails = await _releaseProductBomRepository.GetAllProductionBom(pagingParameter, searchParams);

                var metadata = new
                {
                    productionBomDetails.TotalCount,
                    productionBomDetails.PageSize,
                    productionBomDetails.CurrentPage,
                    productionBomDetails.HasNext,
                    productionBomDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));



                _logger.LogInfo("Returned all Boms");
                var productionBomList = _mapper.Map<IEnumerable<ProductionBom>>(productionBomDetails);



                serviceResponse.Data = productionBomList;
                serviceResponse.Message = "Returned all ProductionBoms Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllProductionBOM API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllProductionBOM API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductionBomById(int id)
        {
            ServiceResponse<ProductionBom> serviceResponse = new ServiceResponse<ProductionBom>();

            try
            {

                var productionBomDetailsById = await _releaseProductBomRepository.GetProductionBomById(id);


                if (productionBomDetailsById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ProductionBomDetails hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ProductionBomDetails with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ProductionBomDetails with id: {id}");
                    var result = _mapper.Map<ProductionBom>(productionBomDetailsById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned ProductionBomDetailsById Successfully.";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetProductionBomById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetProductionBomById API for the following id : {id} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEnggBomFGItemNoListByItemNumber(string itemNumber)
        {
            ServiceResponse<IEnumerable<EnggBomFGItemNumber>> serviceResponse = new ServiceResponse<IEnumerable<EnggBomFGItemNumber>>();
            try
            {
                var enggBomFGItemNoDetails = await _enggBomRepository.GetAllEnggBomFGItemNoListByItemNumber(itemNumber);
                if (enggBomFGItemNoDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"EnggBom FGItemNumber is Invalid.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ProductionBomDetails with id: {itemNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    var result = _mapper.Map<IEnumerable<EnggBomFGItemNumber>>(enggBomFGItemNoDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned all EnggBomFGItemNumberList";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllEnggBomFGItemNoListByItemNumber API for the following itemNumber : {itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllEnggBomFGItemNoListByItemNumber API for the following itemNumber : {itemNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEnggBomChildFGItemNoListByItemNumber(string childItemNumber)
        {
            ServiceResponse<IEnumerable<EnggBomFGItemNumber>> serviceResponse = new ServiceResponse<IEnumerable<EnggBomFGItemNumber>>();
            try
            {
                var enggBomChildFGItemNoDetails = await _enggBomRepository.GetAllFgItemNumberListBySaItemNumber(childItemNumber);
                if (enggBomChildFGItemNoDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"EnggBom FGItemNumber is Invalid.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ProductionBomDetails with id: {childItemNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    var result = _mapper.Map<IEnumerable<EnggBomFGItemNumber>>(enggBomChildFGItemNoDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned all EnggBomChildFGItemNumberList";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllEnggBomChildFGItemNoListByItemNumber API for the following childItemNumber : {childItemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllEnggBomChildFGItemNoListByItemNumber API for the following childItemNumber : {childItemNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetProductionBomByItemAndBomVersionNo(string itemNumber, decimal bomVersionNo)
        {
            ServiceResponse<EnggBomDto> serviceResponse = new ServiceResponse<EnggBomDto>();

            try
            {

                var productionBomDetailsById = await _releaseProductBomRepository
                                               .GetProductionBomByItemAndBomVersionNo(itemNumber, bomVersionNo);


                if (productionBomDetailsById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ProductionBomDetails hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ProductionBomDetails with ItemNumber : {itemNumber} and BOM Version : {bomVersionNo}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ProductionBomDetails with ItemNumber : {itemNumber} and BOM Version : {bomVersionNo}");
                    var result = _mapper.Map<EnggBomDto>(productionBomDetailsById);
                    var enggChildItemList = _mapper.Map<List<EnggChildItemDto>>(productionBomDetailsById.EnggChildItems);
                    result.EnggChildItemDtos = enggChildItemList;
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned ProductionBomDetails Successfully.";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetProductionBomByItemAndBomVersionNo API for the following ItemNumber : {itemNumber} and BOM Version : {bomVersionNo} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetProductionBomByItemAndBomVersionNo API for the following ItemNumber : {itemNumber} and BOM Version : {bomVersionNo} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<ReleaseProductBomController>
        [HttpPost]
        public async Task<IActionResult> CreateReleaseProductBom([FromBody] ReleaseProductBomDtoPost releaseProductBomDtoPost)
        {
            ServiceResponse<ReleaseProductBomDtoPost> serviceResponse = new ServiceResponse<ReleaseProductBomDtoPost>();

            try
            {
                if (releaseProductBomDtoPost is null)
                {
                    _logger.LogError("ReleaseProductBom object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ReleaseProductBom object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ReleaseProductBom object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ReleaseProductBom object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var release = _mapper.Map<ProductionBom>(releaseProductBomDtoPost);
                release.IsReleaseProductCompleted = true;
                await _releaseProductBomRepository.CreateReleaseProductBom(release);
                _repository.SaveAsync();
                await _releaseCostBomRepository.ReleasedCostBomByItemAndRevisionNumber(releaseProductBomDtoPost.ItemNumber, releaseProductBomDtoPost.ReleaseVersion);
                _repository.SaveAsync();
                await _releaseEnggBomRepository.ReleasedEnggProductionByItemAndRevisionNumber(releaseProductBomDtoPost.ItemNumber, releaseProductBomDtoPost.ReleaseVersion);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ReleaseProductBom Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetReleaseProductBomById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateReleaseProductBom API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateReleaseProductBom API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // Get ListOf BomGroup Names
        [HttpGet]
        public async Task<IActionResult> GetAllBomGroupList()
        {
            ServiceResponse<IEnumerable<ListOfBomGroupDto>> serviceResponse = new ServiceResponse<IEnumerable<ListOfBomGroupDto>>();
            try
            {
                var bomGroupList = await _repository.EnggBomGroupRepository.GetAllBomGroupList();
                var result = _mapper.Map<IEnumerable<ListOfBomGroupDto>>(bomGroupList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all bomGroupList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)

            {
                _logger.LogError($"Error Occured in GetAllBomGroupList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllBomGroupList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        // GET: api/<EnggBomGroupController>
        [HttpGet]
        public async Task<IActionResult> GetAllEnggBomGroup([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<EnggBomGroupDto>> serviceResponse = new ServiceResponse<IEnumerable<EnggBomGroupDto>>();
            try
            {
                var listOfEnggBomGroup = await _repository.EnggBomGroupRepository.GetAllEnggBomGroup(pagingParameter, searchParams);

                var metadata = new
                {
                    listOfEnggBomGroup.TotalCount,
                    listOfEnggBomGroup.PageSize,
                    listOfEnggBomGroup.CurrentPage,
                    listOfEnggBomGroup.HasNext,
                    listOfEnggBomGroup.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all EnggBomGroup");
                var enggbomGroupEntity = _mapper.Map<IEnumerable<EnggBomGroupDto>>(listOfEnggBomGroup);
                serviceResponse.Data = enggbomGroupEntity;
                serviceResponse.Message = "Returned all EnggBomGroup";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllEnggBomGroup API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllEnggBomGroup API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET: api/<EnggBomGroupController>
        [HttpGet]
        public async Task<IActionResult> GetAllActiveEnggBomGroup()
        {
            ServiceResponse<IEnumerable<EnggBomGroupDto>> serviceResponse = new ServiceResponse<IEnumerable<EnggBomGroupDto>>();

            try
            {
                var enggBomGroupList = await _repository.EnggBomGroupRepository.GetAllActiveEnggBomGroup();
                _logger.LogInfo("Returned all ActiveEnggBomGroup");
                var enggbomGroupEntity = _mapper.Map<IEnumerable<EnggBomGroupDto>>(enggBomGroupList);
                serviceResponse.Data = enggbomGroupEntity;
                serviceResponse.Message = "Returned all  ActiveEnggBomGroup Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveEnggBomGroup API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveEnggBomGroup API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }

        // GET: api/<EnggBomGroupController>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnggBomGroupById(int id)
        {
            ServiceResponse<EnggBomGroupDto> serviceResponse = new ServiceResponse<EnggBomGroupDto>();

            try
            {
                var enggbomGroupList = await _repository.EnggBomGroupRepository.GetEnggBomGroupById(id);
                if (enggbomGroupList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"EnggBomGroup hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"EnggBomGroup with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var enggbomGroupEntity = _mapper.Map<EnggBomGroupDto>(enggbomGroupList);
                    serviceResponse.Data = enggbomGroupEntity;
                    serviceResponse.Message = "Returned EnggBomGroup Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetEnggBomGroupById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetEnggBomGroupById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST: api/<EnggBomGroupController>
        [HttpPost]
        public IActionResult CreateEnggBomGroup([FromBody] EnggBomGroupDtoPost enggbomGroupDtoPost)
        {
            ServiceResponse<EnggBomGroupDtoPost> serviceResponse = new ServiceResponse<EnggBomGroupDtoPost>();

            try
            {
                if (enggbomGroupDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "EnggBomGroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("EnggBomGroup object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid EnggBomGroup object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid EnggBomGroup object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var enggbomGroupEntity = _mapper.Map<EnggBomGroup>(enggbomGroupDtoPost);
                _repository.EnggBomGroupRepository.CreateEnggBomGroup(enggbomGroupEntity);
                _repository.SaveAsync();
                serviceResponse.Message = "EnggBomGroup Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateEnggBomGroup API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in CreateEnggBomGroup API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT: api/<EnggBomGroupController>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnggBomGroup(int id, [FromBody] EnggBomGroupDtoUpdate enggbomGroupDtoUpdate)
        {
            ServiceResponse<EnggBomGroupDtoUpdate> serviceResponse = new ServiceResponse<EnggBomGroupDtoUpdate>();

            try
            {
                if (enggbomGroupDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update EnggBomGroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update EnggBomGroup object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update EnggBomGroup object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update EnggBomGroup object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var enggbomGroupEntity = await _repository.EnggBomGroupRepository.GetEnggBomGroupById(id);
                if (enggbomGroupEntity is null)
                {
                    _logger.LogError($"Update EnggBomGroup with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " UpdateEnggBomGroup hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var dom = _mapper.Map<EnggBomGroup>(enggbomGroupEntity);
                _mapper.Map(dom, enggbomGroupEntity);
                string result = await _repository.EnggBomGroupRepository.UpdateEnggBomGroup(enggbomGroupEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "EnggBomGroup Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateEnggBomGroup API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateEnggBomGroup API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE: api/<EnggBomGroupController>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnggBomGroup(int id)
        {
            ServiceResponse<EnggBomGroupDto> serviceResponse = new ServiceResponse<EnggBomGroupDto>();

            try
            {
                var enggBomGroupList = await _repository.EnggBomGroupRepository.GetEnggBomGroupById(id);
                if (enggBomGroupList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete EnggBomGroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete EnggBomGroup with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.EnggBomGroupRepository.DeleteEnggBomGroup(enggBomGroupList);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "EnggBomGroup Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteEnggBomGroup API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteEnggBomGroup API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // GET: api/<EnggCustomFieldController>
        [HttpGet]
        public async Task<IActionResult> GetAllEnggCustomField()
        {
            ServiceResponse<IEnumerable<EnggCustomFieldDto>> serviceResponse = new ServiceResponse<IEnumerable<EnggCustomFieldDto>>();
            try
            {
                var listOfEnggCustomField = await _repository.EnggCustomFieldRepository.GetAllEnggCustomFields();
                //var metadata = new
                //{
                //    listOfEnggCustomField.TotalCount,
                //    listOfEnggCustomField.PageSize,
                //    listOfEnggCustomField.CurrentPage,
                //    listOfEnggCustomField.HasNext,
                //    listOfEnggCustomField.HasPreviuos
                //};

                //Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all EnggCustomField");
                var enggcustomFieldEntity = _mapper.Map<IEnumerable<EnggCustomFieldDto>>(listOfEnggCustomField);
                serviceResponse.Data = enggcustomFieldEntity;
                serviceResponse.Message = "Returned all EnggCustomField";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllEnggCustomField API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllEnggCustomField API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET: api/<EnggCustomFieldController>
        [HttpGet]
        public async Task<IActionResult> GetAllActiveEnggCustomField()
        {
            ServiceResponse<IEnumerable<EnggCustomFieldDto>> serviceResponse = new ServiceResponse<IEnumerable<EnggCustomFieldDto>>();

            try
            {
                var enggCustomFieldEntityList = await _repository.EnggCustomFieldRepository.GetAllActiveEnggCustomFields();
                _logger.LogInfo("Returned all EnggActiveCustomField");
                var enggCustomFieldEntity = _mapper.Map<IEnumerable<EnggCustomFieldDto>>(enggCustomFieldEntityList);
                serviceResponse.Data = enggCustomFieldEntity;
                serviceResponse.Message = "Returned all  ActiveEnggCustomField Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveEnggCustomField API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveEnggCustomField API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }

        //get: EnggCustomFiedDetails by BomgeoupName

        [HttpGet("{BomgroupName}")]
        public async Task<IActionResult> GetEnggCustomFieldByBomGroup(string BomgroupName)
        {
            ServiceResponse<IEnumerable<EnggCustomField>> serviceResponse = new ServiceResponse<IEnumerable<EnggCustomField>>();

            try
            {
                var enggCustomFieldDetails = await _repository.EnggCustomFieldRepository.GetEnggCustomFieldByBomGroup(BomgroupName);
                if (enggCustomFieldDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CustomFieldList with id: {BomgroupName}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"CustomFieldList with id: {BomgroupName}, hasn't been found.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned EnggCustomFieldDetails with id: {BomgroupName}");
                    var result = _mapper.Map<IEnumerable<EnggCustomField>>(enggCustomFieldDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned EnggCustomFieldDetails with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetEnggCustomFieldByBomGroup API for the following id : {BomgroupName} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetEnggCustomFieldByBomGroup API for the following id : {BomgroupName} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET: api/<EnggCustomFieldController>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnggCustomFieldById(int id)
        {
            ServiceResponse<EnggCustomFieldDto> serviceResponse = new ServiceResponse<EnggCustomFieldDto>();

            try
            {
                var enggcustomFieldList = await _repository.EnggCustomFieldRepository.GetEnggCustomFieldById(id);
                if (enggcustomFieldList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"EnggcustomField hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"EnggcustomField with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned EnggcustomField with id: {id}");
                    var enggcustomFieldEntity = _mapper.Map<EnggCustomFieldDto>(enggcustomFieldList);
                    serviceResponse.Data = enggcustomFieldEntity;
                    serviceResponse.Message = "Returned EnggcustomField Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetEnggCustomFieldById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetEnggCustomFieldById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST: api/<EnggCustomFieldController>
        [HttpPost]
        public IActionResult CreateEnggCustomField([FromBody] List<EnggCustomFieldDtoPost> enggcustomFieldDtoPost)
        {
            ServiceResponse<EnggCustomFieldDtoPost> serviceResponse = new ServiceResponse<EnggCustomFieldDtoPost>();

            try
            {
                if (enggcustomFieldDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "EnggCustomField object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("EnggCustomField object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid EnggCustomField object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid EnggcustomField object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var enggcustomFieldEntity = _mapper.Map<List<EnggCustomField>>(enggcustomFieldDtoPost);

                foreach (var enggCustomFielddetails in enggcustomFieldEntity)
                {
                    _repository.EnggCustomFieldRepository.CreateEnggCustomField(enggCustomFielddetails);


                }
                _repository.SaveAsync();
                serviceResponse.Message = "EnggCustomField Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateEnggBomGroup API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in CreateEnggBomGroup API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT: api/<EnggCustomFieldController>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnggCustomField(int id, [FromBody] EnggCustomFieldDtoUpdate enggcustomFieldDtoUpdate)
        {
            ServiceResponse<EnggCustomFieldDtoUpdate> serviceResponse = new ServiceResponse<EnggCustomFieldDtoUpdate>();

            try
            {
                if (enggcustomFieldDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update EnggCustomField object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update EnggCustomField object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update EnggCustomField object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update EnggCustomField object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var enggcustomFieldEntity = await _repository.EnggCustomFieldRepository.GetEnggCustomFieldById(id);
                if (enggcustomFieldEntity is null)
                {
                    _logger.LogError($"Update EnggCustomField with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " UpdateEnggCustomField hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(enggcustomFieldDtoUpdate, enggcustomFieldEntity);
                string result = await _repository.EnggCustomFieldRepository.UpdateEnggCustomField(enggcustomFieldEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "EnggCustomField Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateEnggCustomField API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateEnggCustomField API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE: api/<EnggCustomFieldController>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnggCustomField(int id)
        {
            ServiceResponse<EnggCustomFieldDto> serviceResponse = new ServiceResponse<EnggCustomFieldDto>();

            try
            {
                var enggCustomFieldList = await _repository.EnggCustomFieldRepository.GetEnggCustomFieldById(id);
                if (enggCustomFieldList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete EnggCustomField object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete EnggCustomField with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.EnggCustomFieldRepository.DeleteEnggCustomField(enggCustomFieldList);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "EnggCustomField Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteEnggCustomField API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteEnggCustomField API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEnggBomItemNumberVersionlist()
        {
            ServiceResponse<IEnumerable<EnggBomItemRevisionList>> serviceResponse = new ServiceResponse<IEnumerable<EnggBomItemRevisionList>>();
            try
            {
                var enggBomDetails = await _enggBomRepository.GetAllEnggBomItemNumberVersionList();
                var result = _mapper.Map<IEnumerable<EnggBomItemRevisionList>>(enggBomDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all EnggBomItemNumberVersionlist";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllEnggBomItemNumberVersionlist API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllEnggBomItemNumberVersionlist API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReleaseCostBomItemNumberVersionList()
        {
            ServiceResponse<IEnumerable<CostingBomItemRevisionList>> serviceResponse = new ServiceResponse<IEnumerable<CostingBomItemRevisionList>>();
            try
            {
                var releaseCostBomDetails = await _releaseCostBomRepository.GetAllReleaseCostBomItemNumberVersionList();
                var result = _mapper.Map<IEnumerable<CostingBomItemRevisionList>>(releaseCostBomDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ReleaseCostBomItemNumberVersionlist";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllReleaseCostBomItemNumberVersionList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllReleaseCostBomItemNumberVersionList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReleaseProductBomItemNumberVersionList()
        {
            ServiceResponse<IEnumerable<GetAllReleaseProductBomItemNumberVersionList>> serviceResponse = new ServiceResponse<IEnumerable<GetAllReleaseProductBomItemNumberVersionList>>();
            try
            {
                var releaseProductBomDetails = await _releaseProductBomRepository.GetAllReleaseProductBomItemNumberVersionList();
                var result = _mapper.Map<IEnumerable<GetAllReleaseProductBomItemNumberVersionList>>(releaseProductBomDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ReleaseProductBomItemNumberVersionlist";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllReleaseProductBomItemNumberVersionList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllReleaseProductBomItemNumberVersionList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProductionBomReleasedKitNoAndLatestVersion()
        {
            ServiceResponse<IEnumerable<ProductionBomKitRevNoDto>> serviceResponse = new ServiceResponse<IEnumerable<ProductionBomKitRevNoDto>>();
            try
            {
                var releaseProductkitBomDetails = await _releaseProductBomRepository.GetProductionBomReleasedKitNoAndLatestVersion();
                var result = _mapper.Map<IEnumerable<ProductionBomKitRevNoDto>>(releaseProductkitBomDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all KitNoAndLatestVersion";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetKitNoAndLatestVersion action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //[HttpGet("{itemNumber}")]
        //public async Task<IActionResult> GetAllEnggBomRevisionNumberList(string itemNumber)
        //{
        //    ServiceResponse<IEnumerable<ReleaseEnggBomDto>> serviceResponse = new ServiceResponse<IEnumerable<ReleaseEnggBomDto>>();
        //    try
        //    {
        //        var costingBomVersionDetails = await _enggBomRepository.GetAllEnggBomVersionListByItemNumber(itemNumber);
        //        var result = _mapper.Map<IEnumerable<ReleaseEnggBomDto>>(costingBomVersionDetails);
        //        serviceResponse.Data = result;
        //        serviceResponse.Message = "Returned all EnggBomRevisionNumberList";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong inside GetAllEnggBomRevisionNumberList action";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> GetLatestkitRevNo(string kitItemNumber)
        {
            ServiceResponse<EnggBomRevSPReportDto> serviceResponse = new ServiceResponse<EnggBomRevSPReportDto>();
            try
            {

                var kitRevNoDetail = await _enggBomRepository.GetLatestkitRevNo(kitItemNumber);
                var result = _mapper.Map<EnggBomRevSPReportDto>(kitRevNoDetail);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned  LatestkitRevNo";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetLatestkitRevNo action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEnggBomRevisionNumberList(string itemNumber)
        {
            ServiceResponse<IEnumerable<ReleaseEnggBomDto>> serviceResponse = new ServiceResponse<IEnumerable<ReleaseEnggBomDto>>();
            try
            {

                var costingBomVersionDetails = await _enggBomRepository.GetAllEnggBomVersionListByItemNumber(itemNumber);
                var result = _mapper.Map<IEnumerable<ReleaseEnggBomDto>>(costingBomVersionDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all EnggBomRevisionNumberList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllEnggBomRevisionNumberList API for the following itemNumber : {itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllEnggBomRevisionNumberList API for the following itemNumber : {itemNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCostingBomRevisionNumberList(string itemNumber)
        {
            ServiceResponse<IEnumerable<CostingBomDto>> serviceResponse = new ServiceResponse<IEnumerable<CostingBomDto>>();
            try
            {
                var costingBomVersionDetails = await _releaseCostBomRepository.GetAllCostingBomVersionListByItemNumber(itemNumber);
                var result = _mapper.Map<IEnumerable<CostingBomDto>>(costingBomVersionDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all CostingBomRevisionNumberList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllCostingBomRevisionNumberList API for the following itemNumber : {itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllCostingBomRevisionNumberList API for the following itemNumber : {itemNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductionBomRevisionNumberList(string itemNumber)
        {
            ServiceResponse<IEnumerable<ReleaseProductBomDto>> serviceResponse = new ServiceResponse<IEnumerable<ReleaseProductBomDto>>();
            try
            {
                var productionBomVersionDetails = await _releaseProductBomRepository.GetAllProductionBomVersionListByItemNumber(itemNumber);
                var result = _mapper.Map<IEnumerable<ReleaseProductBomDto>>(productionBomVersionDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ProductionBomRevisionNumber";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllProductionBomRevisionNumberList API for the following itemNumber : {itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllProductionBomRevisionNumberList API for the following itemNumber : {itemNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAllProductionBomFGListByItemNumber([FromBody] string itemNumber)
        {
            ServiceResponse<IEnumerable<ProductionBomRevisionNumber>> serviceResponse = new ServiceResponse<IEnumerable<ProductionBomRevisionNumber>>();
            try
            {
                var productionBomDetails = await _releaseProductBomRepository.GetAllProductionBomFGListByItemNumber(itemNumber);
                if (productionBomDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ProductionBom ItemType is Invalid.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ProductionBomDetails with id: {itemNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    var result = _mapper.Map<IEnumerable<ProductionBomRevisionNumber>>(productionBomDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned all ProductionBomFGList";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllProductionBomFGListByItemNumber API for the following itemNumber : {itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllProductionBomFGListByItemNumber API for the following itemNumber : {itemNumber} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //sa  

        [HttpGet]
        public async Task<IActionResult> GetSABomListByItemNumber(string fgPartNumber, string saItemNumber)
        {
            ServiceResponse<decimal?> serviceResponse = new ServiceResponse<decimal?>();
            try
            {
                var BomQuantity = await _enggBomRepository.GetSABomQuantity(fgPartNumber, saItemNumber);
                if (BomQuantity == null)
                {
                    serviceResponse.Data = 0;
                    serviceResponse.Message = $"BomQtyDetails is Invalid.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"BomQtyDetails with FG and SA part number: {fgPartNumber}{saItemNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    //var result = _mapper.Map<GetBomQuantityDto>(BomQuantity);
                    serviceResponse.Data = BomQuantity;
                    serviceResponse.Message = "Returned all BomQtyDetails";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetSABomListByItemNumber API for the following fgPartNumber : {fgPartNumber} and saItemNumber : {saItemNumber}\n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetSABomListByItemNumber API for the following fgPartNumber : {fgPartNumber} and saItemNumber : {saItemNumber}\n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllProductionBomSAListByItemNumber(string itemNumber)
        {
            ServiceResponse<ProductionBomRevisionNumberAndQty> serviceResponse = new ServiceResponse<ProductionBomRevisionNumberAndQty>();
            try
            {
                var productionBomDetails = await _releaseProductBomRepository.GetAllProductionBomSAListByItemNumber(itemNumber);
                if (productionBomDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ProductionBom ItemType is Invalid.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ProductionBomDetails with id: {itemNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    var result = _mapper.Map<ProductionBomRevisionNumberAndQty>(productionBomDetails);

                    //var result = _mapper.Map<IEnumerable<ProductionBomRevisionNumber>>(productionBomDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned all ProductionBomSAList";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllProductionBomSAListByItemNumber API for the following itemNumber : {itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllProductionBomSAListByItemNumber API for the following itemNumber : {itemNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetBomDetailsForCoverageReport(List<OpenSalesCoverageReportDto> openFGCoverageDetails)
        {
            ServiceResponse<List<BomCoverageReportChildItemReqQtyDto>> serviceResponse = new ServiceResponse<List<BomCoverageReportChildItemReqQtyDto>>();
            try
            {
                if (openFGCoverageDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Data Not found in this coverageReportChildItemReqQtyDtos Method.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Data Not found in this coverageReportChildItemReqQtyDtos Method");
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "coverageReportChildItemReqQtyDtosr object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid coverageReportChildItemReqQtyDtos object sent from the client.");
                    return BadRequest(serviceResponse);
                }
                List<BomCoverageReportChildItemReqQtyDto> bomCoverageList = new List<BomCoverageReportChildItemReqQtyDto>();
                if (openFGCoverageDetails != null)
                {

                    foreach (var item in openFGCoverageDetails)
                    {
                        var itemNo = item.ItemNumber;
                        var productionBomMaxVersion = await _releaseProductBomRepository.GetLatestProBomCountByItemNumber(itemNo);

                        //var enggDetail = _enggBomRepository.GetAllLatestRevBOMIsReleaseEnggBom(itemNo);
                        if (productionBomMaxVersion != null)
                        {
                            await ChildItemRequiredQtyForCoverage(bomCoverageList, item.ItemNumber, item.BalanceToOrder);
                        }
                    }
                    //changed

                }
                var itemsRequiredQtyGrouped = bomCoverageList
                        .GroupBy(item => item.ItemNumber)
                        .Select(group => new BomCoverageReportChildItemReqQtyDto
                        {
                            ItemNumber = group.Key,
                            PartType = group.First().PartType,
                            UOM = group.First().UOM,
                            RequiredQty = group.Sum(item => item.RequiredQty)
                        })
                        .ToList();

                serviceResponse.Data = itemsRequiredQtyGrouped;
                serviceResponse.Message = "Returned all ChildItemRequiredQtys";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetBomDetailsForCoverageReport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetBomDetailsForCoverageReport API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private async Task ChildItemRequiredQtyForCoverage(List<BomCoverageReportChildItemReqQtyDto> bomCoverageList, string itemNumber, decimal requiredQty)
        {
            var productionBomMaxVersion = await _releaseProductBomRepository
                                        .GetLatestProductionBomByItemNumber(itemNumber);
            Dictionary<string, decimal> saItemOpenStock = new Dictionary<string, decimal>();
            if (productionBomMaxVersion >= 0)
            {
                var enggBomDetail = await _enggBomRepository
                      .GetLatestEnggBomVersionDetailByItemNumber(itemNumber, productionBomMaxVersion);
                if (enggBomDetail != null)
                {
                    foreach (var enggChildItem in enggBomDetail?.EnggChildItems)
                    {
                        if (enggChildItem.PartType != PartType.SA)
                        {
                            BomCoverageReportChildItemReqQtyDto bomCoverageReportChildItemReqQty = new BomCoverageReportChildItemReqQtyDto
                            {
                                ItemNumber = enggChildItem.ItemNumber,
                                PartType = enggChildItem.PartType,
                                RequiredQty = enggChildItem.Quantity * requiredQty,
                                UOM = enggChildItem.UOM

                            };
                            bomCoverageList.Add(bomCoverageReportChildItemReqQty);
                        }
                        else
                        {
                            decimal openSAQty = 0;
                            string saItemNumber = enggChildItem.ItemNumber;
                            if (saItemOpenStock.ContainsKey(saItemNumber))
                            {
                                openSAQty = saItemOpenStock[saItemNumber];
                            }
                            else
                            {
                                //var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                                //  "GetTotalStockOfItemNumber?", "itemNumber=", saItemNumber));

                                var client = _clientFactory.CreateClient();
                                var token = HttpContext.Request.Headers["Authorization"].ToString();

                                var encodedItemNumber = Uri.EscapeDataString(saItemNumber);

                                var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                    $"GetTotalStockOfItemNumber?itemNumber={encodedItemNumber}"));
                                request.Headers.Add("Authorization", token);

                                var inventoryObjectResult = await client.SendAsync(request);

                                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                                dynamic inventoryObject = inventoryObjectData.data;
                                openSAQty = Convert.ToDecimal(inventoryObject) != null ? Convert.ToDecimal(inventoryObject) : 0;
                            }

                            // get stock from inventory
                            decimal requiredQtySA = enggChildItem.Quantity * requiredQty;
                            decimal newRequiredQtySA = requiredQtySA - openSAQty;
                            newRequiredQtySA = newRequiredQtySA <= 0 ? 0 : newRequiredQtySA;

                            //Get SA Stock from ODO
                            //var client1 = _clientFactory.CreateClient();
                            //var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                            //var encodedSAItemNumber = Uri.EscapeDataString(saItemNumber);

                            //var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ODOAPI"], $"GetListOfSAODOQtyByItemNo?saItemNumber={encodedSAItemNumber}"));
                            //request1.Headers.Add("Authorization", token1);

                            //var inventoryObjectResult1 = await client1.SendAsync(request1);

                            //var inventoryObjectString1 = await inventoryObjectResult1.Content.ReadAsStringAsync();
                            //var inventoryObjectData1 = JsonConvert.DeserializeObject<ODOItemDetailsForBomDto>(inventoryObjectString1);
                            //var inventoryObject1 = inventoryObjectData1.data;
                            //if (inventoryObject1.ODOQty> 0)
                            //{
                            //    newRequiredQtySA -= inventoryObject1.ODOQty;
                            //}

                            decimal newOpenSAQty = requiredQtySA >= openSAQty ? 0 : (openSAQty - requiredQtySA);
                            if (saItemOpenStock.ContainsKey(saItemNumber))
                            {
                                saItemOpenStock[saItemNumber] = newOpenSAQty;
                            }
                            else
                            {
                                saItemOpenStock.Add(saItemNumber, newOpenSAQty);
                            }

                            if (newRequiredQtySA <= 0)
                            {
                                continue;
                            }
                            await ChildItemRequiredQtyForCoverage(bomCoverageList, enggChildItem.ItemNumber, newRequiredQtySA);
                        }

                    }
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetBomDetailsByProjectNoForCoverageReport(List<OpenSalesCoverageReportByprojectNoDto> openFGCoverageDetails)
        {
            ServiceResponse<List<BomCoverageReportChildItemReqQtyByProjectNoDto>> serviceResponse = new ServiceResponse<List<BomCoverageReportChildItemReqQtyByProjectNoDto>>();
            try
            {
                if (openFGCoverageDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Data Not found in this coverageReportChildItemReqQtyDtos Method.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Data Not found in this coverageReportChildItemReqQtyDtos Method");
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "coverageReportChildItemReqQtyDtosr object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid coverageReportChildItemReqQtyDtos object sent from the client.");
                    return BadRequest(serviceResponse);
                }
                List<BomCoverageReportChildItemReqQtyByProjectNoDto> bomCoverageList = new List<BomCoverageReportChildItemReqQtyByProjectNoDto>();
                Dictionary<string,Dictionary<string, decimal>> saItemOpenStockProjectwise = new Dictionary<string, Dictionary<string, decimal>>();
                if (openFGCoverageDetails != null)
                {

                    foreach (var item in openFGCoverageDetails)
                    {
                        var itemNo = item.ItemNumber;
                        var productionBomMaxVersion = await _releaseProductBomRepository.GetLatestProBomCountByItemNumber(itemNo);

                        //var enggDetail = _enggBomRepository.GetAllLatestRevBOMIsReleaseEnggBom(itemNo);
                        if (productionBomMaxVersion != null)
                        {

                            await ChildItemRequiredQtyForCoverageReportByProjectNo(bomCoverageList, item.ItemNumber, item.BalanceToOrder, item.ProjectNumber,saItemOpenStockProjectwise);

                        }
                    }
                    //changed

                }
                var itemsRequiredQtyGrouped = bomCoverageList
                          .GroupBy(item => item.ItemNumber)
                          .Select(group => new BomCoverageReportChildItemReqQtyByProjectNoDto
                          {
                              ItemNumber = group.Key,
                              MftrItemNumber = group.First().MftrItemNumber,
                              Version = group.First().Version,
                              Description = group.First().Description,
                              UOM = group.First().UOM,
                              PartType = group.First().PartType,
                              RequiredQty = group.Sum(item => item.RequiredQty)
                          })
                          .ToList();

                serviceResponse.Data = itemsRequiredQtyGrouped;
                serviceResponse.Message = "Returned all ChildItemRequiredQtys";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetBomDetailsByProjectNoForCoverageReport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetBomDetailsByProjectNoForCoverageReport API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private async Task ChildItemRequiredQtyForCoverageReportByProjectNo(List<BomCoverageReportChildItemReqQtyByProjectNoDto> bomCoverageList, string itemNumber, decimal requiredQty,
                                                                                   string projectNo, Dictionary<string, Dictionary<string, decimal>> saItemOpenStockWithProject)
        {
            try
            {
                var productionBomMaxVersion = await _releaseProductBomRepository
                                            .GetLatestProductionBomByItemNumber(itemNumber);

                Dictionary<string, decimal> saItemOpenStock ;

                if (saItemOpenStockWithProject.ContainsKey(projectNo))
                {
                     saItemOpenStock = saItemOpenStockWithProject[projectNo];
                }
                else
                {
                    saItemOpenStock = new Dictionary<string, decimal>();
                }


                if (productionBomMaxVersion >= 0)
                {
                    var enggBomDetail = await _enggBomRepository
                          .GetLatestEnggBomVersionDetailByItemNumber(itemNumber, productionBomMaxVersion);
                    if (enggBomDetail != null)
                    {
                        foreach (var enggChildItem in enggBomDetail?.EnggChildItems)
                        {
                            if (enggChildItem.PartType != PartType.SA)
                            {
                                decimal requiredQuantity;
                                if (string.IsNullOrEmpty(enggChildItem.ScrapAllowance) || enggChildItem.ScrapAllowance == "0" || enggChildItem.ScrapAllowance == "-")
                                {
                                    requiredQuantity = enggChildItem.Quantity * requiredQty;

                                }

                                else
                                {
                                    decimal scrappercent = Convert.ToDecimal(enggChildItem.ScrapAllowance);
                                    if (enggChildItem.ScrapAllowanceType == "percentage")
                                    {
                                        decimal scrapvalue = scrappercent / 100;
                                        requiredQuantity = (enggChildItem.Quantity + (enggChildItem.Quantity * scrapvalue)) * requiredQty;

                                    }
                                    else if (enggChildItem.ScrapAllowanceType == "number")
                                    {
                                        requiredQuantity = (enggChildItem.Quantity * requiredQty) + scrappercent;
                                    }
                                    else
                                    {
                                        requiredQuantity = enggChildItem.Quantity * requiredQty;
                                    }

                                }

                                BomCoverageReportChildItemReqQtyByProjectNoDto bomCoverageReportChildItemReqQty = new BomCoverageReportChildItemReqQtyByProjectNoDto
                                {
                                    ItemNumber = enggChildItem.ItemNumber,
                                    Version = enggChildItem.Version,
                                    MftrItemNumber = enggChildItem.MftrItemNumbers,
                                    Description = enggChildItem.Description,
                                    UOM = enggChildItem.UOM,
                                    PartType = enggChildItem.PartType,
                                    RequiredQty = requiredQuantity

                                };

                                bomCoverageList.Add(bomCoverageReportChildItemReqQty);
                            }
                            else
                            {
                                decimal openSAStock = 0;
                                string saItemNumber = enggChildItem.ItemNumber;
                                if (saItemOpenStock.ContainsKey(saItemNumber))
                                {
                                    openSAStock = saItemOpenStock[saItemNumber];
                                }
                                else
                                {
                                    //var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                                    //  "GetTotalStockOfSAItemNumberAndProjectNo?", "itemNumber=", saItemNumber, "&ProjectNumber=", projectNo));

                                    var client = _clientFactory.CreateClient();
                                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                                    var encodedItemNumber = Uri.EscapeDataString(saItemNumber);
                                    var encodedProjectNumber = Uri.EscapeDataString(projectNo);

                                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                        $"GetTotalStockOfSAItemNumberAndProjectNo?saItemNumber={encodedItemNumber}&ProjectNumber={encodedProjectNumber}"));
                                    request.Headers.Add("Authorization", token);

                                    var inventoryObjectResult = await client.SendAsync(request);

                                    var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                    dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                                    dynamic inventoryObject = inventoryObjectData.data;
                                    openSAStock = Convert.ToDecimal(inventoryObject) != null ? Convert.ToDecimal(inventoryObject) : 0;


                                    var client1 = _clientFactory.CreateClient();
                                    var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                                    var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ShopOrderAPI"],
                                        $"GetSAShopOrderWipQtyByProjectNo?itemNumber={encodedItemNumber}&projectNo={encodedProjectNumber}"));
                                    request1.Headers.Add("Authorization", token1);

                                    var soConWipQtybjectResult1 = await client1.SendAsync(request1);

                                    var soConWipQtybjectString1 = await soConWipQtybjectResult1.Content.ReadAsStringAsync();
                                    var soConWipQtybjectData1 = JsonConvert.DeserializeObject<SoConWipQtyDetailsDto>(soConWipQtybjectString1);
                                    if (soConWipQtybjectData1.data != null)
                                    {
                                        var soConWipQtyObject1 = soConWipQtybjectData1.data;
                                        openSAStock = openSAStock + soConWipQtyObject1.WipQuantity;
                                        openSAStock = openSAStock <= 0 ? 0 : openSAStock;
                                    }
                                
                                }

                                // get stock from inventory
                                decimal requiredQtySA = enggChildItem.Quantity * requiredQty;
                                decimal newRequiredQtySA = requiredQtySA - openSAStock;
                                newRequiredQtySA = newRequiredQtySA <= 0 ? 0 : newRequiredQtySA;


                                decimal newOpenSAStock = requiredQtySA >= openSAStock ? 0 : (openSAStock - requiredQtySA);
                                if (saItemOpenStock.ContainsKey(saItemNumber))
                                {
                                    saItemOpenStock[saItemNumber] = newOpenSAStock;
                                   
                                }
                                else
                                {
                                    saItemOpenStock.Add(saItemNumber, newOpenSAStock);
                                    
                                }

                                if (saItemOpenStockWithProject.ContainsKey(projectNo))
                                {
                                    saItemOpenStockWithProject[projectNo] = saItemOpenStock;
                                }
                                else
                                {
                                    saItemOpenStockWithProject.Add(projectNo, saItemOpenStock);
                                }
                                if (newRequiredQtySA <= 0)
                                {
                                    continue;
                                }
                                await ChildItemRequiredQtyForCoverageReportByProjectNo(bomCoverageList, enggChildItem.ItemNumber, newRequiredQtySA, projectNo, saItemOpenStockWithProject);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetSABomDetailsByProjectNoForCoverageReport(List<OpenSalesCoverageReportByprojectNoDto> openFGCoverageDetails)
        {
            ServiceResponse<List<BomCoverageReportSAChildItemReqQtyByProjectNoDto>> serviceResponse = new ServiceResponse<List<BomCoverageReportSAChildItemReqQtyByProjectNoDto>>();
            try
            {
                if (openFGCoverageDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Data Not found in this coverageReportChildItemReqQtyDtos Method.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Data Not found in this coverageReportChildItemReqQtyDtos Method");
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "coverageReportChildItemReqQtyDtosr object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid coverageReportChildItemReqQtyDtos object sent from the client.");
                    return BadRequest(serviceResponse);
                }
                List<BomCoverageReportSAChildItemReqQtyByProjectNoDto> bomCoverageList = new List<BomCoverageReportSAChildItemReqQtyByProjectNoDto>();
                if (openFGCoverageDetails != null)
                {

                    foreach (var item in openFGCoverageDetails)
                    {
                        var itemNo = item.ItemNumber;
                        var productionBomMaxVersion = await _releaseProductBomRepository.GetLatestProBomCountByItemNumber(itemNo);

                        //var enggDetail = _enggBomRepository.GetAllLatestRevBOMIsReleaseEnggBom(itemNo);
                        if (productionBomMaxVersion != null)
                        {
                            await SAChildItemRequiredQtyForCoverageReportByProjectNo(bomCoverageList, item.ItemNumber, item.BalanceToOrder, item.ProjectNumber);
                        }
                    }
                    //changed

                }
                var itemsRequiredQtyGrouped = bomCoverageList
                        .GroupBy(item => item.ItemNumber)
                        .Select(group => new BomCoverageReportSAChildItemReqQtyByProjectNoDto
                        {
                            ItemNumber = group.Key,
                            MftrItemNumber = group.First().MftrItemNumber,
                            Version = group.First().Version,
                            Description = group.First().Description,
                            UOM = group.First().UOM,
                            PartType = group.First().PartType,
                            RequiredQty = group.Sum(item => item.RequiredQty),
                            ODOQty = group.Sum(item => item.ODOQty)
                        })
                        .ToList();

                serviceResponse.Data = itemsRequiredQtyGrouped;
                serviceResponse.Message = "Returned all ChildItemRequiredQtys";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetSABomDetailsByProjectNoForCoverageReport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetSABomDetailsByProjectNoForCoverageReport API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private async Task SAChildItemRequiredQtyForCoverageReportByProjectNo(List<BomCoverageReportSAChildItemReqQtyByProjectNoDto> bomCoverageList, string itemNumber, decimal requiredQty, string projectNo)
        {
            try
            {
                var productionBomMaxVersion = await _releaseProductBomRepository
                                            .GetLatestProductionBomByItemNumber(itemNumber);
                Dictionary<string, decimal> saItemOpenStock = new Dictionary<string, decimal>();
                if (productionBomMaxVersion >= 0)
                {
                    var enggBomDetail = await _enggBomRepository
                          .GetLatestEnggBomVersionDetailByItemNumber(itemNumber, productionBomMaxVersion);
                    if (enggBomDetail != null)
                    {
                        foreach (var enggChildItem in enggBomDetail?.EnggChildItems)
                        {
                            if (enggChildItem.PartType == PartType.SA)
                            {
                                decimal openSAStock = 0;
                                string saItemNumber = enggChildItem.ItemNumber;
                                if (saItemOpenStock.ContainsKey(saItemNumber))
                                {
                                    openSAStock = saItemOpenStock[saItemNumber];
                                }
                                else
                                {
                                    //var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                                    //  "GetTotalStockOfSAItemNumberAndProjectNo?", "itemNumber=", saItemNumber, "&ProjectNumber=", projectNo));

                                    //var client = _clientFactory.CreateClient();
                                    //var token = HttpContext.Request.Headers["Authorization"].ToString();

                                    //var encodedItemNumber = Uri.EscapeDataString(saItemNumber);
                                    //var encodedProjectNumber = Uri.EscapeDataString(projectNo);

                                    //var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                    //    $"GetTotalStockOfSAItemNumberAndProjectNo?saItemNumber={encodedItemNumber}&ProjectNumber={encodedProjectNumber}"));
                                    //request.Headers.Add("Authorization", token);

                                    //var inventoryObjectResult = await client.SendAsync(request);

                                    //var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                    //dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                                    //dynamic inventoryObject = inventoryObjectData.data;
                                    //openSAStock = Convert.ToDecimal(inventoryObject) != null ? Convert.ToDecimal(inventoryObject) : 0;
                                }

                                // get stock from inventory
                                decimal requiredQtySA = enggChildItem.Quantity * requiredQty;
                                //decimal newRequiredQtySA = requiredQtySA - openSAStock;
                                decimal newRequiredQtySA = requiredQtySA <= 0 ? 0 : requiredQtySA;

                                //Get SA Stock from ODO
                                var client1 = _clientFactory.CreateClient();
                                var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                                var encodedSAItemNumber = Uri.EscapeDataString(saItemNumber);
                                var encodedSAProjectNumber = Uri.EscapeDataString(projectNo);

                                var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ODOAPI"], $"GetListOfSAODOQtyByItemNo?saItemNumber={encodedSAItemNumber}&projectNumber={encodedSAProjectNumber}"));
                                request1.Headers.Add("Authorization", token1);

                                var oDOObjectResult1 = await client1.SendAsync(request1);

                                var oDOObjectString1 = await oDOObjectResult1.Content.ReadAsStringAsync();
                                var oDOObjectData1 = JsonConvert.DeserializeObject<ODOItemDetailsForBomDto>(oDOObjectString1);
                                if (oDOObjectData1.data != null)
                                {
                                    var oDOObject1 = oDOObjectData1.data;
                                    if (oDOObject1.ODOQty > 0)
                                    {

                                        BomCoverageReportSAChildItemReqQtyByProjectNoDto bomCoverageReportChildItemReqQty = new BomCoverageReportSAChildItemReqQtyByProjectNoDto
                                        {
                                            ItemNumber = enggChildItem.ItemNumber,
                                            PartType = enggChildItem.PartType,
                                            Version = enggChildItem.Version,
                                            MftrItemNumber = enggChildItem.MftrItemNumbers,
                                            Description = enggChildItem.Description,
                                            UOM = enggChildItem.UOM,
                                            RequiredQty = newRequiredQtySA,
                                            ODOQty = oDOObject1.ODOQty,

                                        };

                                        bomCoverageList.Add(bomCoverageReportChildItemReqQty);
                                    }
                                }
                                else
                                {
                                    BomCoverageReportSAChildItemReqQtyByProjectNoDto bomCoverageReportChildItemReqQty = new BomCoverageReportSAChildItemReqQtyByProjectNoDto
                                    {
                                        ItemNumber = enggChildItem.ItemNumber,
                                        PartType = enggChildItem.PartType,
                                        Version = enggChildItem.Version,
                                        MftrItemNumber = enggChildItem.MftrItemNumbers,
                                        Description = enggChildItem.Description,
                                        UOM = enggChildItem.UOM,
                                        RequiredQty = newRequiredQtySA,
                                        ODOQty = 0,

                                    };

                                    bomCoverageList.Add(bomCoverageReportChildItemReqQty);
                                }

                                decimal newOpenSAStock = requiredQtySA >= openSAStock ? 0 : (openSAStock - requiredQtySA);
                                if (saItemOpenStock.ContainsKey(saItemNumber))
                                {
                                    saItemOpenStock[saItemNumber] = newOpenSAStock;
                                }
                                else
                                {
                                    saItemOpenStock.Add(saItemNumber, newOpenSAStock);
                                }

                                if (newRequiredQtySA <= 0)
                                {
                                    continue;
                                }
                                await SAChildItemRequiredQtyForCoverageReportByProjectNo(bomCoverageList, enggChildItem.ItemNumber, newRequiredQtySA, projectNo);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }


        //[HttpPost]
        //public async Task<IActionResult> GetBomDetailsForCoverageReportByCustomerId(List<OpenSalesCoverageReportByprojectNoDto> openFGCoverageDetails)
        //{
        //    ServiceResponse<List<BomCoverageReportChildItemReqQtyByProjectNoDto>> serviceResponse = new ServiceResponse<List<BomCoverageReportChildItemReqQtyByProjectNoDto>>();
        //    try
        //    {
        //        if (openFGCoverageDetails == null)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Data Not found in this coverageReportChildItemReqQtyDtos Method.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            _logger.LogError("Data Not found in this coverageReportChildItemReqQtyDtos Method");
        //            return BadRequest(serviceResponse);
        //        }

        //        if (!ModelState.IsValid)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "coverageReportChildItemReqQtyDtosr object sent from the client.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            _logger.LogError("Invalid coverageReportChildItemReqQtyDtos object sent from the client.");
        //            return BadRequest(serviceResponse);
        //        }
        //        List<BomCoverageReportChildItemReqQtyByProjectNoDto> bomCoverageList = new List<BomCoverageReportChildItemReqQtyByProjectNoDto>();
        //        if (openFGCoverageDetails != null)
        //        {

        //            foreach (var item in openFGCoverageDetails)
        //            {
        //                var itemNo = item.ItemNumber;
        //                var productionBomMaxVersion = await _releaseProductBomRepository.GetLatestProBomCountByItemNumber(itemNo);

        //                //var enggDetail = _enggBomRepository.GetAllLatestRevBOMIsReleaseEnggBom(itemNo);
        //                if (productionBomMaxVersion != null)
        //                {
        //                    await ChildItemRequiredQtyForCoverageByCustomerId(bomCoverageList, item.ItemNumber, item.BalanceToOrder);
        //                }
        //            }
        //            //changed

        //        }
        //        var itemsRequiredQtyGrouped = bomCoverageList
        //                .GroupBy(item => item.ItemNumber)
        //                .Select(group => new BomCoverageReportChildItemReqQtyByProjectNoDto
        //                {
        //                    ItemNumber = group.Key,
        //                    MftrItemNumber = group.First().MftrItemNumber,
        //                    Version = group.First().Version,
        //                    Description = group.First().Description,
        //                    UOM = group.First().UOM,
        //                    PartType = group.First().PartType,
        //                    RequiredQty = group.Sum(item => item.RequiredQty)
        //                })
        //                .ToList();

        //        serviceResponse.Data = itemsRequiredQtyGrouped;
        //        serviceResponse.Message = "Returned all ChildItemRequiredQtys";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error in GetBomDetailsForCoverageReport {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong inside GetAllProductionBomSAListByItemNumber action";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}


        //private async Task ChildItemRequiredQtyForCoverageByCustomerId(List<BomCoverageReportChildItemReqQtyByProjectNoDto> bomCoverageList, string itemNumber, decimal requiredQty)
        //{
        //    try
        //    {
        //        var productionBomMaxVersion = await _releaseProductBomRepository
        //                                    .GetLatestProductionBomByItemNumber(itemNumber);
        //        Dictionary<string, decimal> saItemOpenStock = new Dictionary<string, decimal>();
        //        if (productionBomMaxVersion >= 0)
        //        {
        //            var enggBomDetail = await _enggBomRepository
        //                  .GetLatestEnggBomVersionDetailByItemNumber(itemNumber, productionBomMaxVersion);
        //            if (enggBomDetail != null)
        //            {
        //                foreach (var enggChildItem in enggBomDetail?.EnggChildItems)
        //                {
        //                    if (enggChildItem.PartType != PartType.SA)
        //                    {
        //                        decimal requiredQuantity;
        //                        if (string.IsNullOrEmpty(enggChildItem.ScrapAllowance) || enggChildItem.ScrapAllowance == "0" || enggChildItem.ScrapAllowance == "-")
        //                        {
        //                            requiredQuantity = enggChildItem.Quantity * requiredQty;

        //                        }

        //                        else
        //                        {
        //                            decimal scrappercent = Convert.ToDecimal(enggChildItem.ScrapAllowance);
        //                            if (enggChildItem.ScrapAllowanceType == "percentage")
        //                            {
        //                                decimal scrapvalue = scrappercent / 100;
        //                                requiredQuantity = (enggChildItem.Quantity + (enggChildItem.Quantity * scrapvalue)) * requiredQty;

        //                            }
        //                            else if (enggChildItem.ScrapAllowanceType == "number")
        //                            {
        //                                requiredQuantity = (enggChildItem.Quantity * requiredQty) + scrappercent;
        //                            }
        //                            else
        //                            {
        //                                requiredQuantity = enggChildItem.Quantity * requiredQty;
        //                            }

        //                        }

        //                        BomCoverageReportChildItemReqQtyByProjectNoDto bomCoverageReportChildItemReqQty = new BomCoverageReportChildItemReqQtyByProjectNoDto
        //                        {
        //                            ItemNumber = enggChildItem.ItemNumber,
        //                            Version = enggChildItem.Version,
        //                            MftrItemNumber = enggChildItem.MftrItemNumbers,
        //                            Description = enggChildItem.Description,
        //                            UOM = enggChildItem.UOM,
        //                            PartType = enggChildItem.PartType,
        //                            RequiredQty = requiredQuantity

        //                        };

        //                        bomCoverageList.Add(bomCoverageReportChildItemReqQty);
        //                    }
        //                    else
        //                    {
        //                        decimal openSAStock = 0;
        //                        string saItemNumber = enggChildItem.ItemNumber;
        //                        if (saItemOpenStock.ContainsKey(saItemNumber))
        //                        {
        //                            openSAStock = saItemOpenStock[saItemNumber];
        //                        }
        //                        else
        //                        {
        //                            //var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
        //                            //  "GetTotalStockOfItemNumber?", "itemNumber=", saItemNumber));

        //                            var client = _clientFactory.CreateClient();
        //                            var token = HttpContext.Request.Headers["Authorization"].ToString();

        //                            var encodedItemNumber = Uri.EscapeDataString(saItemNumber);

        //                            var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
        //                                $"GetTotalStockOfItemNumber?itemNumber={encodedItemNumber}"));
        //                            request.Headers.Add("Authorization", token);

        //                            var inventoryObjectResult = await client.SendAsync(request);

        //                            var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
        //                            dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
        //                            dynamic inventoryObject = inventoryObjectData.data;
        //                            openSAStock = Convert.ToDecimal(inventoryObject) != null ? Convert.ToDecimal(inventoryObject) : 0;
        //                        }

        //                        // get stock from inventory
        //                        decimal requiredQtySA = enggChildItem.Quantity * requiredQty;
        //                        decimal newRequiredQtySA = requiredQtySA - openSAStock;
        //                        newRequiredQtySA = newRequiredQtySA <= 0 ? 0 : newRequiredQtySA;
        //                        decimal newOpenSAStock = requiredQtySA >= openSAStock ? 0 : (openSAStock - requiredQtySA);
        //                        if (saItemOpenStock.ContainsKey(saItemNumber))
        //                        {
        //                            saItemOpenStock[saItemNumber] = newOpenSAStock;
        //                        }
        //                        else
        //                        {
        //                            saItemOpenStock.Add(saItemNumber, newOpenSAStock);
        //                        }

        //                        if (newRequiredQtySA <= 0)
        //                        {
        //                            continue;
        //                        }
        //                        await ChildItemRequiredQtyForCoverageByCustomerId(bomCoverageList, enggChildItem.ItemNumber, newRequiredQtySA);
        //                    }

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //    }
        //}

        private async Task<EnggBom> GetEnggBOM(string itemNumber)
        {
            EnggBom bomDetails = await _enggBomRepository.GetAllLatestRevAndIsReleaseEnggBom(itemNumber);

            if (bomDetails != null)
            {
                EnggBom enggBom = new EnggBom
                {
                    ItemNumber = bomDetails.ItemNumber,
                };

                return enggBom;
            }
            return null;
        }



        private async Task<List<CoverageReportDto>> CalculateTotalRequiredQtyRecursive(EnggBom enggBOM, decimal parentQty)
        {
            List<CoverageReportDto> result = new List<CoverageReportDto>();

            // Retrieve EnggBOM details
            var maxRevisionBOMs = enggBOM.EnggChildItems
                .Where(child => child.PartType == PartType.SA)
                .ToList();
            CoverageReportDto dto = new CoverageReportDto();

            foreach (var maxRevisionBOM in maxRevisionBOMs)
            {
                if (maxRevisionBOM != null)
                {
                    var childEnggBOM = await GetEnggBOM(maxRevisionBOM.ItemNumber);

                    if (childEnggBOM != null)
                    {


                        decimal SAStock = await GetSAStock(maxRevisionBOM.ItemNumber);

                        decimal adjustedParentQty = parentQty - SAStock;

                        // Recursively get child SA BOM details
                        decimal childSAQty = maxRevisionBOM.Quantity * adjustedParentQty;

                        var childSAChildQtyList = await CalculateTotalRequiredQtyRecursive(childEnggBOM, childSAQty);

                        dto.Stock = SAStock;

                        // Calculate OpenPoQty
                        //var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                        //              "GetAllOpenTGPoDetails?", "itemNumber=", maxRevisionBOM.ItemNumber));

                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var saItemNumber = maxRevisionBOM.ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(saItemNumber);

                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["PurchaseAPI"],
                                      $"GetAllOpenTGPoDetails?itemNumber={encodedItemNumber}"));
                        request.Headers.Add("Authorization", token);

                        var purchaseObjectResult = await client.SendAsync(request);


                        if (purchaseObjectResult != null && purchaseObjectResult.StatusCode == HttpStatusCode.OK)
                        {
                            var purchaseObjectResults = await purchaseObjectResult.Content.ReadAsStringAsync();
                            dynamic purchaseObjectData = JsonConvert.DeserializeObject(purchaseObjectResults);
                            dynamic purchaseObject = purchaseObjectData.data;

                            if (purchaseObject != null)
                            {
                                foreach (var purchase in purchaseObject)
                                {
                                    dto.OpenPoQty = dto.OpenPoQty + purchase.balanceQty;
                                }
                            }
                        }

                        foreach (var childSAChildQty in childSAChildQtyList)
                        {

                            dto.ChildItemNumber = maxRevisionBOM.ItemNumber;

                            dto.TotalRequiredQty = parentQty * childSAQty * childSAChildQty.TotalRequiredQty;
                            result.Add(dto);
                        }

                    }
                    else
                    {
                        if (maxRevisionBOM.PartType != PartType.SA)
                        {
                            decimal SAStock = await GetSAStock(maxRevisionBOM.ItemNumber);

                            dto.Stock = SAStock;

                            // Calculate OpenPoQty
                            //var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                            //              "GetAllOpenTGPoDetails?", "itemNumber=", maxRevisionBOM.ItemNumber));

                            var client = _clientFactory.CreateClient();
                            var token = HttpContext.Request.Headers["Authorization"].ToString();

                            var saItemNumber = maxRevisionBOM.ItemNumber;
                            var encodedItemNumber = Uri.EscapeDataString(saItemNumber);

                            var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["PurchaseAPI"],
                                          $"GetAllOpenTGPoDetails?itemNumber={encodedItemNumber}"));
                            request.Headers.Add("Authorization", token);

                            var purchaseObjectResult = await client.SendAsync(request);
                            if (purchaseObjectResult != null && purchaseObjectResult.StatusCode == HttpStatusCode.OK)
                            {
                                var purchaseObjectResults = await purchaseObjectResult.Content.ReadAsStringAsync();
                                dynamic purchaseObjectData = JsonConvert.DeserializeObject(purchaseObjectResults);
                                dynamic purchaseObject = purchaseObjectData.data;

                                if (purchaseObject != null)
                                {
                                    foreach (var purchase in purchaseObject)
                                    {
                                        dto.OpenPoQty = dto.OpenPoQty + purchase.balanceQty;
                                    }
                                }
                            }

                            dto.ChildItemNumber = maxRevisionBOM.ItemNumber;

                            dto.TotalRequiredQty = parentQty * maxRevisionBOM.Quantity;

                            dto.BalanceToOrderQtyChild = dto.TotalRequiredQty - (SAStock + dto.OpenPoQty); //dto.TotalRequiredQt for this get from step 2 need to check

                            result.Add(dto);
                        }
                    }
                }
            }

            return result;
        }



        //private async Task<List<CoverageReportDto>> CalculateTotalRequiredQtyRecursive(EnggBom enggBOM, decimal parentQty)
        //{
        //    decimal totalRequiredQty = 0;
        //    List<CoverageReportDto> result = new List<CoverageReportDto>();

        //    // Retrieve EnggBOM details
        //    var maxRevisionBOMs = enggBOM.EnggChildItems
        //        .Where(child => child.PartType == PartType.SA)
        //        .ToList();


        //    CoverageReportDto dto = new CoverageReportDto();

        //    foreach (var maxRevisionBOM in maxRevisionBOMs)
        //    {
        //        if (maxRevisionBOM != null)
        //        {
        //            var childEnggBOM = await GetEnggBOM(maxRevisionBOM.ItemNumber);

        //            if (childEnggBOM != null)
        //            {

        //                dto.ChildItemNumber = maxRevisionBOM.ItemNumber;

        //                decimal SAStock = await GetSAStock(maxRevisionBOM.ItemNumber);

        //                decimal adjustedParentQty = parentQty - SAStock;

        //                // Recursively get child SA BOM details
        //                decimal childSAQty = maxRevisionBOM.Quantity * adjustedParentQty;

        //                var childSAChildQty = await CalculateTotalRequiredQtyRecursive(childEnggBOM, childSAQty);

        //                dto.TotalRequiredQty = parentQty * childSAQty * childSAChildQty;

        //                result.Add(dto); 
        //                // Multiply BalanceToOrderQty * SA Qty * SA Child Qty

        //                //totalRequiredQty += parentQty * childSAQty * childSAChildQty;


        //                //totalRequiredQty += maxRevisionBOM.Quantity * childSAQty * childSAChildQty;

        //            }
        //            else
        //            {
        //                //foreach (var enggChildItem in enggBOM.EnggChildItems)
        //                //{
        //                if (maxRevisionBOM.PartType != PartType.SA)
        //                {
        //                    // Multiply BalanceToOrderQty * EnggChildItem.Quantity 

        //                    dto.TotalRequiredQty = parentQty * maxRevisionBOM.Quantity;

        //                    result.Add(dto);
        //                }

        //            }
        //        }
        //    }
        //    return result;
        //}



        //private async Task<decimal> CalculateTotalRequiredQtyRecursive(EnggBom enggBOM, decimal parentQty)
        //{
        //    decimal totalRequiredQty = 0;

        //    // Retrieve EnggBOM details
        //    var maxRevisionBOMs = enggBOM.EnggChildItems
        //        .Where(child => child.PartType == PartType.SA)
        //        .ToList();
        //    foreach (var maxRevisionBOM in maxRevisionBOMs)
        //    { 
        //    if (maxRevisionBOM != null)
        //    {
        //        var childEnggBOM = await GetEnggBOM(maxRevisionBOM.ItemNumber);

        //        if (childEnggBOM != null)
        //        {

        //            decimal SAStock = await GetSAStock(maxRevisionBOM.ItemNumber);

        //            decimal adjustedParentQty = parentQty - SAStock;

        //            // Recursively get child SA BOM details
        //            decimal childSAQty = maxRevisionBOM.Quantity * adjustedParentQty;
        //            decimal childSAChildQty = await CalculateTotalRequiredQtyRecursive(childEnggBOM, childSAQty);

        //            // Multiply BalanceToOrderQty * SA Qty * SA Child Qty

        //            totalRequiredQty += parentQty * childSAQty * childSAChildQty;


        //            //totalRequiredQty += maxRevisionBOM.Quantity * childSAQty * childSAChildQty;

        //        }
        //        else
        //        {
        //            //foreach (var enggChildItem in enggBOM.EnggChildItems)
        //            //{
        //                if (maxRevisionBOM.PartType != PartType.SA)
        //                {
        //                    // Multiply BalanceToOrderQty * EnggChildItem.Quantity

        //                    totalRequiredQty += parentQty * maxRevisionBOM.Quantity;

        //                    //totalRequiredQty += enggChildItem.Quantity * enggChildItem.Quantity;
        //                }
        //            //}
        //        }
        //    }
        //}
        //    return totalRequiredQty;
        //}

        private async Task<decimal> GetSAStock(string itemNumber)
        {
            //var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
            //                "GetInventoryBySAItemNo?", "itemNumber=", itemNumber));
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();

            var encodedItemNumber = Uri.EscapeDataString(itemNumber);

            var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                          $"GetInventoryBySAItemNo?itemNumber={encodedItemNumber}"));
            request.Headers.Add("Authorization", token);

            var inventoryObjectResult = await client.SendAsync(request);
            if (inventoryObjectResult.IsSuccessStatusCode)
            {
                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                dynamic inventoryObject = inventoryObjectData.data;

                if (inventoryObject != null && inventoryObject.balance_Quantity != null)
                {
                    return Convert.ToDecimal(inventoryObject.balance_Quantity);
                }
            }

            return 0;
        }
        //Below API is just to take from item master and put in enggchild items[
        [HttpPut]
        public async Task<IActionResult> GetAltenatesintoMftrNo()
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                var itemno = await _repository.EnggBomRepository.GetChildItemsLists();
                foreach (var item in itemno)
                {
                    var mftrs = await _repository.ItemMasterRepository.GetItemMasterByItemNumber(item.ItemNumber);
                    if (mftrs != null)
                    {
                        string? mftrno = null;
                        if (mftrs.ItemmasterAlternate != null)
                        {
                            foreach (var mftr in mftrs.ItemmasterAlternate)
                            {
                                if (mftrno == null)
                                    mftrno = mftr.ManufacturerPartNo;
                                else
                                    mftrno = mftrno + "," + mftr.ManufacturerPartNo;
                            }
                            item.MftrItemNumbers = mftrno;
                            await _repository.EnggChildItemsRepository.UpdateEnggChilditems(item);
                            _repository.SaveAsync();
                        }
                    }
                }

                serviceResponse.Data = "Successfull";
                serviceResponse.Message = "Returned all Engineering Boms Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAltenatesintoMftrNo API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAltenatesintoMftrNo API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetWeighted_AvgRateDetails([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<WeightedAvgRateDto>> serviceResponse = new ServiceResponse<IEnumerable<WeightedAvgRateDto>>();

            try
            {
                var avgDetails = await _repository.WeightedAvgRateRepository.GetWeighted_AvgRateDetails(searchParams);



                _logger.LogInfo("Returned all avgDetails");

                var result = _mapper.Map<IEnumerable<WeightedAvgRateDto>>(avgDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GrinsParts Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetWeighted_AvgRateDetails API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetWeighted_AvgRateDetails API : \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetFGCostingSPReportWithParam([FromBody] FGCostingSPReportDto fgCostingSPReportDto)

        {
            ServiceResponse<IEnumerable<FGCostingSPReport>> serviceResponse = new ServiceResponse<IEnumerable<FGCostingSPReport>>();
            try
            {
                var products = await _repository.EnggBomRepository.GetFGCostingSPReportWithParam(fgCostingSPReportDto.FGItemnumber,
                                                                                                    fgCostingSPReportDto.ShopOrderNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"FGCostingSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"FGCostingSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned FGCostingSPReportWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetFGCostingSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetFGCostingSPReportWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetBOMReleaseSPReportWithParamForTrans([FromBody] ReleaseProductionBomSPReportDto releaseProductionBomSPReportDto)

        {
            ServiceResponse<IEnumerable<ReleaseProductionBomSPReport>> serviceResponse = new ServiceResponse<IEnumerable<ReleaseProductionBomSPReport>>();
            try
            {
                var products = await _releaseProductBomRepository.GetBOMReleaseSPReportWithParamForTrans(releaseProductionBomSPReportDto.ItemNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BOMReleaseSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"BOMReleaseSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned BOMReleaseSPReportWithParamForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetBOMReleaseSPReportWithParamForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetBOMReleaseSPReportWithParamForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }





        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetBomLatestRevisionDataSpForGeeYes([FromBody] ReleaseProductionBomSPReportDto releaseProductionBomSPReportDto)

        {
            ServiceResponse<IEnumerable<BomLatestRevisionDataSp>> serviceResponse = new ServiceResponse<IEnumerable<BomLatestRevisionDataSp>>();
            try
            {
                var products = await _releaseProductBomRepository.GetBomLatestRevisionDataSpForGeeYes(releaseProductionBomSPReportDto.ItemNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BomLatestRevisionDataSp hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"BomLatestRevisionDataSp hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned BomLatestRevisionDataSp Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetBomLatestRevisionDataSpForGeeYes API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetBomLatestRevisionDataSpForGeeYes API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }




        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetBomReportWithAllRevisionsSpForGeeYes([FromBody] BomReportwithallRevisionsSpInput bomReportwithallRevisionsSpInput)

        {
            ServiceResponse<IEnumerable<BomReportwithallRevisionsSp>> serviceResponse = new ServiceResponse<IEnumerable<BomReportwithallRevisionsSp>>();
            try
            {
                var products = await _releaseProductBomRepository.GetBomReportWithAllRevisionsSpForGeeYes(bomReportwithallRevisionsSpInput.ItemNumber, bomReportwithallRevisionsSpInput.RevisionNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BomReportwithallRevisionsSp hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"BomReportwithallRevisionsSp hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned BomReportwithallRevisionsSp Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetBomReportWithAllRevisionsSpForGeeYes API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetBomReportWithAllRevisionsSpForGeeYes API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }




        [HttpPost]
        public async Task<IActionResult> CreateReleaseFileUpload([FromBody] List<ReleaseFileUploadPostDto> releaseFileUploadPostDtos)
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                if (releaseFileUploadPostDtos is null)
                {
                    _logger.LogError("ReleaseFileUpload object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ReleaseFileUpload object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ReleaseFileUpload object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                List<string?> ids = new List<string?>();
                foreach (var fileUploadDetail in releaseFileUploadPostDtos)
                {
                    Guid guid = Guid.NewGuid();
                    byte[] fileContent = Convert.FromBase64String(fileUploadDetail.FileByte);
                    string fileName = guid.ToString() + "_" + fileUploadDetail.FileName + "." + fileUploadDetail.FileExtension;
                    string fileExt = Path.GetExtension(fileName).ToUpper();

                    string uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "ReleaseFileUpload", fileName);

                    using (MemoryStream ms = new MemoryStream(fileContent))
                    {
                        ms.Position = 0;
                        using (var fileStream = new FileStream(uploadFolderPath, FileMode.Create, FileAccess.Write))
                        {
                            ms.WriteTo(fileStream);
                        }
                    }

                    var uploadedFile = new ReleaseFileUpload
                    {
                        FileName = fileName,
                        FileExtension = fileExt,
                        FilePath = uploadFolderPath,
                        ParentId = fileUploadDetail.ReleaseFor,
                        DocumentFrom = fileUploadDetail.ReleaseFor,
                        FileByte = fileUploadDetail.FileByte
                    };

                    await _repository.ReleaseFileUploadRepository.CreateReleaseFileUploadDocument(uploadedFile);
                    _repository.SaveAsync();
                    ids.Add(uploadedFile.Id.ToString());
                }


                serviceResponse.Data = ids;
                serviceResponse.Message = "ReleaseFileUpload documents successfully created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CreateReleaseFileUpload: \n{ex.Message} \n {ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error in CreateReleaseFileUpload: \n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }       
        [HttpGet]
        public async Task<IActionResult> GetReleaseDownloadUrlDetailsforItemFiles(string fileids)
        {
            ServiceResponse<List<ReleaseFileUploadDto>> serviceResponse = new ServiceResponse<List<ReleaseFileUploadDto>>();
            try
            {
                string serverKey = GetServerKey();
                var itemsFiles = await _repository.ReleaseFileUploadRepository.GetReleaseFileUploadDownloadUrlDetails(fileids);
                if (itemsFiles == null)
                {
                    _logger.LogError($"DownloadDetail with id: {fileids}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DownloadDetail with id: {fileids}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid release UploadDocument.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid release UploadDocument sent from client.");
                    return BadRequest(serviceResponse);
                }
                List<ReleaseFileUploadDto> fileUploads = new List<ReleaseFileUploadDto>();
                if (itemsFiles != null)
                {
                    foreach (var fileUploadDetails in itemsFiles)
                    {
                        ReleaseFileUploadDto fileUploadDto = _mapper.Map<ReleaseFileUploadDto>(fileUploadDetails);
                        var filename = Uri.EscapeDataString(fileUploadDto.FileName);
                        if (serverKey == "avision")
                        {
                            var baseUrl = $"{_config["ItemMasterBaseUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/apigateway/tips/EngineeringBOM/ReleaseDownloadFile?Filename={filename}";
                        }
                        else
                        {
                            var baseUrl = $"{_config["ItemMasterBaseUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/api/EngineeringBOM/ReleaseDownloadFile?Filename={filename}";
                        }

                        //fileUploadDto.FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "FileUpload", fileUploadDto.FileName);
                        fileUploads.Add(fileUploadDto);
                    }
                }
                _logger.LogInfo($"Returned DownloadDetail with id: {fileids}");
                //var result = _mapper.Map<IEnumerable<GetDownloadUrlDtos>>(getDownloadDetailByPoNumber);
                serviceResponse.Data = fileUploads;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetReleaseDownloadUrlDetailsforItemFiles API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetReleaseDownloadUrlDetailsforItemFiles API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<ActionResult> ReleaseDownloadFile(string Filename)
        {
            ServiceResponse<FileContentResult> serviceResponse = new ServiceResponse<FileContentResult>();
            var filename = Uri.UnescapeDataString(Filename);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "ReleaseFileUpload", filename);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var ContentType))
            {
                ContentType = "application/octet-stream";
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var DownloadFilename = filename.Split('_');
            var downloadFilename = string.IsNullOrWhiteSpace(DownloadFilename[1]) ? Path.GetFileName(filePath) : DownloadFilename[1];

            return File(bytes, ContentType, downloadFilename);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReleaseFile(int id)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                var fileToDelete = await _repository.ReleaseFileUploadRepository.GetReleaseFileUploadByIdAsync(id);

                if (fileToDelete == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"File with ID {id} not found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                // Delete file from database
                _repository.ReleaseFileUploadRepository.Delete(fileToDelete);
                _repository.SaveAsync();

                // Delete file from folder
                if (System.IO.File.Exists(fileToDelete.FilePath))
                {
                    System.IO.File.Delete(fileToDelete.FilePath);
                }

                serviceResponse.Data = $"File with ID {id} deleted successfully";
                serviceResponse.Message = "File deleted successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeleteReleaseFile API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteReleaseFile API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private string GetServerKey()
        {
            var serverName = Environment.MachineName;
            var serverConfiguration = _config.GetSection("ServerConfiguration");

            if (serverConfiguration.GetValue<bool?>("Server1:EnableKeus") == true)
            {
                return "keus";
            }
            else if (serverConfiguration.GetValue<bool?>("Server1:EnableAvision") == true)
            {
                return "avision";

            }
            else
            {
                return "trasccon";
            }
        }
        //[HttpPost] 
        //public async Task<IActionResult> GetBomDetailsSPReportWithParam([FromBody] BOMSPReportDto bomSPReportDto)

        //{
        //    ServiceResponse<EnggBomDto> serviceResponse = new ServiceResponse<EnggBomDto>();
        //    try
        //    {
        //        var products = await _repository.EnggBomRepository.GetBomDetailsSPReportWithParam(bomSPReportDto.Itemnumber);

        //        if (result == null)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"BOMSPReport hasn't been found.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            _logger.LogError($"BOMSPReport hasn't been found in db.");
        //            return NotFound(serviceResponse);
        //        }
        //        else
        //        {

        //            serviceResponse.Data = result;
        //            serviceResponse.Message = "Returned BomDetailsSPReport Details";
        //            serviceResponse.Success = true;
        //            serviceResponse.StatusCode = HttpStatusCode.OK;
        //            return Ok(serviceResponse);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong inside GetBomDetailsSPReportWithParam action";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}
    }

}
