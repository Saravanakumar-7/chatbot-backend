using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Repository;
using Entities.DTOs;
using NuGet.Protocol.Core.Types;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    // Materialrequest Controller
    public class MaterialRequestController : ControllerBase
    {
        private IMaterialRequestRepository _materialRequestRepository;
        private IMapper _mapper;
        private ILoggerManager _logger;
        private readonly IHttpClientFactory _clientFactory;

        public MaterialRequestController(IMaterialRequestRepository materialRequestRepository, IMapper mapper, IHttpClientFactory clientFactory, ILoggerManager logger)
        {
            _clientFactory = clientFactory;
            _materialRequestRepository = materialRequestRepository;
            _mapper = mapper;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllMaterialRequest([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<MaterialRequestDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestDto>>();

            try
            {
                var getAllMaterialRequest = await _materialRequestRepository.GetAllMaterialRequest(pagingParameter, searchParammes);
                var metadata = new
                {
                    getAllMaterialRequest.TotalCount,
                    getAllMaterialRequest.PageSize,
                    getAllMaterialRequest.CurrentPage,
                    getAllMaterialRequest.HasNext,
                    getAllMaterialRequest.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogError("Returned all listOfmaterials");
                var result = _mapper.Map<IEnumerable<MaterialRequestDto>>(getAllMaterialRequest);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialRequest Successfully";
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
        public async Task<IActionResult> GetMaterialRequestById(int id)
        {
            ServiceResponse<MaterialRequestDto> serviceResponse = new ServiceResponse<MaterialRequestDto>();

            try
            {
                var materialRequestDetails = await _materialRequestRepository.GetMaterialRequestById(id);

                if (materialRequestDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"materialrequest with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"materialrequest with id: {id}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogError($"Returned materialrequest with id: {id}");

                    MaterialRequestDto materialRequestDto = _mapper.Map<MaterialRequestDto>(materialRequestDetails);
                   

                    List<MaterialRequestItemDto> materialRequestItemDtos = new List<MaterialRequestItemDto>();

                    if (materialRequestDetails.MaterialRequestItems != null)
                    {

                        foreach (var materialitemDetails in materialRequestDetails.MaterialRequestItems)
                        {
                            MaterialRequestItemDto materialRequestItemDto = _mapper.Map<MaterialRequestItemDto>(materialitemDetails);
                            materialRequestItemDtos.Add(materialRequestItemDto);
                        }
                    }

                    materialRequestDto.MaterialRequestItemDtos = materialRequestItemDtos;
                    serviceResponse.Data = materialRequestDto;
                    serviceResponse.Message = $"Returned materialrequest with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetmaterialRequestById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateMaterialRequest([FromBody] MaterialRequestPostDto materialRequestPostDto)
        {
            ServiceResponse<MaterialRequestDto> serviceResponse = new ServiceResponse<MaterialRequestDto>();

            try
            {
                if (materialRequestPostDto is null)
                {
                    _logger.LogError("MaterialRequest object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "MaterialRequest object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid MaterialRequest object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid MaterialRequest object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var createMaterialReq = _mapper.Map<MaterialRequest>(materialRequestPostDto);
                var materialReqDto = materialRequestPostDto.MaterialRequestItemPostDtos;

                var materialReqItemList = new List<MaterialRequestItem>();

                if (materialReqDto != null)
                {

                    for (int i = 0; i < materialReqDto.Count; i++)
                    {
                        MaterialRequestItem materialItemListDetail = _mapper.Map<MaterialRequestItem>(materialReqDto[i]);
                        materialReqItemList.Add(materialItemListDetail);

                    }
                }

                createMaterialReq.MaterialRequestItems = materialReqItemList;
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));



                //var newcount = await _materialRequestRepository.GetMRNumberAutoIncrementCount(date);

                //if (newcount > 0)
                //{
                //    var number = newcount + 1;
                //    string e = String.Format("{0:D4}", number);
                //    createMaterialReq.MRNumber = days + months + years + "MR" + (e);
                //}
                //else
                //{
                //    var count = 1;
                //    var e = count.ToString("D4");
                //    createMaterialReq.MRNumber = days + months + years + "MR" + (e);
                //}

                var dateFormat = days + months + years;
                var mrNumber = await _materialRequestRepository.GenerateMRNumber();
                createMaterialReq.MRNumber = dateFormat + mrNumber;

                await _materialRequestRepository.CreateMaterialRequest(createMaterialReq);

                _materialRequestRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialRequest Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetMaterialRequestById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateMaterialRequest action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterialRequest(int id, [FromBody] MaterialRequestUpdateDto materialRequestUpdateDto)
        {
            ServiceResponse<MaterialRequestDto> serviceResponse = new ServiceResponse<MaterialRequestDto>();

            try
            {
                if (materialRequestUpdateDto is null)
                {
                    _logger.LogError("MaterialRequest object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update MaterialRequest object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid MaterialRequest object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update MaterialRequest object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var getMaterialRequest = await _materialRequestRepository.GetMaterialRequestById(id);
                if (getMaterialRequest is null)
                {
                    _logger.LogError($"materialReq with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update materialReq with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var updateMaterialReqquest = _mapper.Map<MaterialRequest>(getMaterialRequest);

                var materialReqItemDto = materialRequestUpdateDto.MaterialRequestItemUpdateDtos;

                var materialReqItemList = new List<MaterialRequestItem>();

                for (int i = 0; i < materialReqItemDto.Count; i++)
                {
                    MaterialRequestItem materialItemDetail = _mapper.Map<MaterialRequestItem>(materialReqItemDto[i]);
                    materialReqItemList.Add(materialItemDetail);

                }

                var updateMaterialReq = _mapper.Map(materialRequestUpdateDto, getMaterialRequest);
                updateMaterialReq.MaterialRequestItems = materialReqItemList;
                string result = await _materialRequestRepository.UpdateMaterialRequest(updateMaterialReq);
                _materialRequestRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "materialReq Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateMaterialRequest action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialRequest(int id)
        {
            ServiceResponse<MaterialRequestDto> serviceResponse = new ServiceResponse<MaterialRequestDto>();

            try
            {
                var MaterialReqDetails = await _materialRequestRepository.GetMaterialRequestById(id);
                if (MaterialReqDetails == null)
                {
                    _logger.LogError($"materialReq with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete materialReq with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _materialRequestRepository.DeleteMaterialRequest(MaterialReqDetails);
                _logger.LogError(result);
                _materialRequestRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialReq Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteMaterialRequest action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOpenMRIdNoList()
        {
            ServiceResponse<IEnumerable<MaterialRequestIdNoDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestIdNoDto>>();
            try
            {
                var getAllMaterialReq = await _materialRequestRepository.GetAllOpenMRIdNoList();
                var result = _mapper.Map<IEnumerable<MaterialRequestIdNoDto>>(getAllMaterialReq);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All MaterialRequests";
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

        [HttpGet("{MRNumber}")]
        public async Task<IActionResult> GetMaterialReqByMRNumber(string MRNumber)
        {
            ServiceResponse<MaterialRequestDto> serviceResponse = new ServiceResponse<MaterialRequestDto>();

            try
            {
                var getMRbyMRNo = await _materialRequestRepository.GetMaterialReqByMRNumber(MRNumber);

                if (getMRbyMRNo == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GetMRNoDetailsById with id: {MRNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GetMRNoDetailsById with id: {MRNumber}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned GetMRNoDetailsById with id: {MRNumber}");

                    MaterialRequestDto materialRequestDto = _mapper.Map<MaterialRequestDto>(getMRbyMRNo);

                    List<MaterialRequestItemDto> materialRequestItemDtos = new List<MaterialRequestItemDto>();
                    foreach (var materialReqbyMRNo in getMRbyMRNo.MaterialRequestItems)
                    {
                        MaterialRequestItemDto materialRequestItemDto = _mapper.Map<MaterialRequestItemDto>(materialReqbyMRNo);
                        materialRequestItemDtos.Add(materialRequestItemDto);
                    }
                    materialRequestDto.MaterialRequestItemDtos = materialRequestItemDtos;
                    serviceResponse.Data = materialRequestDto;
                    serviceResponse.Message = $"Returned GetMRNoDetailsById with id: {MRNumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMRNoDetailsById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}