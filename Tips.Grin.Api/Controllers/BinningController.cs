using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Entities;
using Entities;
using Newtonsoft.Json;

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BinningController : ControllerBase
    {
        private IBinningRepository _binningRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public BinningController(IBinningRepository binningRepository, ILoggerManager logger, IMapper mapper)
        {
            _logger = logger;
            _binningRepository = binningRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBinningDetails()
        {
            ServiceResponse<IEnumerable<BinningDto>> serviceResponse = new ServiceResponse<IEnumerable<BinningDto>>();

            try
            {
                var binningList = await _binningRepository.GetAllBinningDetails();

                _logger.LogInfo("Returned all Binning details()s");
                var result = _mapper.Map<IEnumerable<BinningDto>>(binningList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned BinningDetails";
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
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{grinNo}")]
        public async Task<IActionResult> GetBinningDetailsByGrinNo(string grinNo)
        {
            ServiceResponse<IEnumerable<BinningDto>> serviceResponse = new ServiceResponse<IEnumerable<BinningDto>>();

            try
            {
                var binningList = await _binningRepository.GetBinningDetailsByGrinNo(grinNo);
                if (binningList == null)
                {
                    _logger.LogError($"Binning Details with GrinNumber: {grinNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Binning Details with GrinNumber hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned Binning Details with id: {grinNo}");
                    var result = _mapper.Map<IEnumerable<BinningDto>>(binningList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Successfully Returned BinningDetailsByGrinNo";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside BinningByGrinNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBinning(int id, [FromBody] BinningUpdateDto BinningUpdateDto)
        {
            ServiceResponse<BinningUpdateDto> serviceResponse = new ServiceResponse<BinningUpdateDto>();

            try
            {
                if (BinningUpdateDto is null)
                {
                    _logger.LogError("Binning details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Binning details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid BinningUpdate details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var BinningList = await _binningRepository.GetBinningDetailsbyId(id);
                if (BinningList is null)
                {
                    _logger.LogError($"Binning details with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }

                var binningList = _mapper.Map<Binning>(BinningUpdateDto);

                var binningitemdto = BinningUpdateDto.binningItems;

                var binningItemList = new List<BinningItems>();
                for (int i = 0; i < binningitemdto.Count; i++)
                {
                    BinningItems binningItemDetail = _mapper.Map<BinningItems>(binningitemdto[i]);
                    binningItemDetail.binningLocations = _mapper.Map<List<BinningLocation>>(binningitemdto[i].binningLocations);
                    binningItemList.Add(binningItemDetail);

                }
                var data = _mapper.Map(BinningUpdateDto, BinningList);

                data.binningItems = binningItemList;

                var BinningEntity = _mapper.Map(BinningUpdateDto, BinningList);

                string result = await _binningRepository.UpdateBinning(BinningEntity);
                _logger.LogInfo(result);
                _binningRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Binning Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Update Update action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBinning([FromBody] BinningPostDto binningPostDto)
        {
            ServiceResponse<BinningPostDto> serviceResponse = new ServiceResponse<BinningPostDto>();

            try
            {
                if (binningPostDto == null)
                {
                    _logger.LogError("Binning details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Binning details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Binning details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var binningList = _mapper.Map<Binning>(binningPostDto);
                var binningsDto = binningPostDto.binningItems;

                var binningItemList = new List<BinningItems>();
                for (int i = 0; i < binningsDto.Count; i++)
                {
                    BinningItems binningItemListDetail = _mapper.Map<BinningItems>(binningsDto[i]);
                    binningItemListDetail.binningLocations = _mapper.Map<List<BinningLocation>>(binningsDto[i].binningLocations);
                    binningItemList.Add(binningItemListDetail);

                }
                binningList.binningItems = binningItemList;

                _binningRepository.CreateBinning(binningList);
                _binningRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Binning Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("BinningById",serviceResponse);
            }
            catch (Exception ex)
            {

                _logger.LogError($"Something went wrong inside Create Binning action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);


            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBinningDetailsbyId(int id)
        {
            ServiceResponse<BinningDto> serviceResponse = new ServiceResponse<BinningDto>();

            try
            {
                var binningList = await _binningRepository.GetBinningDetailsbyId(id);
                if (binningList == null)
                {
                    _logger.LogError($"Binning details with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Binning details with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Binnings with id: {id}");
                    //var result = _mapper.Map<BinningDto>(rfqsourcing);
                    BinningDto binningDto = _mapper.Map<BinningDto>(binningList);//Main model mapping

                    //below mapping is child under child  

                    List<BinningItemsDto> binningItemDtos = new List<BinningItemsDto>();

                    foreach (var binningitemDetails in binningList.binningItems)
                    {
                        BinningItemsDto binningItemDto = _mapper.Map<BinningItemsDto>(binningitemDetails);
                        binningItemDto.binningLocations = _mapper.Map<List<BinningLocationDto>>(binningitemDetails.binningLocations);
                        binningItemDtos.Add(binningItemDto);
                    }

                    binningDto.binningItems = binningItemDtos;
                    serviceResponse.Data = binningDto;
                    serviceResponse.Message = $"Returned BinningbyId";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside BinningById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBinning(int id)
        {
            ServiceResponse<BinningDto> serviceResponse = new ServiceResponse<BinningDto>();

            try
            {
                var shopOrders = await _binningRepository.GetBinningDetailsbyId(id);
                if (shopOrders == null)
                {
                    _logger.LogError($"Confirmation with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Binning with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                shopOrders.IsDeleted = true;
                string result = await _binningRepository.UpdateBinning(shopOrders);
                serviceResponse.Message = "Binning Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Delete Binning action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}