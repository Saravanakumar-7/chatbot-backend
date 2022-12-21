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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    // Materialrequest Controller
    public class MaterialRequestController : ControllerBase
    {
        private IMaterialRequestRepository _materialRequestRepository;
        private IMapper _mapper;
        private ILoggerManager _logger;


        public MaterialRequestController(IMaterialRequestRepository materialRequestRepository, IMapper mapper, ILoggerManager logger)
        {
            _materialRequestRepository = materialRequestRepository;
            _mapper = mapper;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllMaterialRequest([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<MaterialRequestDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestDto>>();

            try
            {
                var listOfmaterials = await _materialRequestRepository.GetAllMaterialRequest(pagingParameter);
                var metadata = new
                {
                    listOfmaterials.TotalCount,
                    listOfmaterials.PageSize,
                    listOfmaterials.CurrentPage,
                    listOfmaterials.HasNext,
                    listOfmaterials.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogError("Returned all listOfmaterials");
                var result = _mapper.Map<IEnumerable<MaterialRequestDto>>(listOfmaterials);
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
                var matreq = await _materialRequestRepository.GetMaterialRequestById(id);

                if (matreq == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"materialrequest with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"materialrequest with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogError($"Returned materialrequest with id: {id}");
                    MaterialRequestDto materialRequestDto = _mapper.Map<MaterialRequestDto>(matreq);//Main model mapping

                    //below mapping is child under child  

                    List<MaterialRequestItemDto> materialRequestItemDtos = new List<MaterialRequestItemDto>();

                    foreach (var materialitemDetails in matreq.MaterialRequestItemList)
                    {
                        MaterialRequestItemDto materialRequestItemDto = _mapper.Map<MaterialRequestItemDto>(materialitemDetails);
                        materialRequestItemDtos.Add(materialRequestItemDto);
                    }

                    materialRequestDto.MaterialRequestItems = materialRequestItemDtos;
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
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateMaterialRequest([FromBody] MaterialRequestDtoPost materialRequestDtoPost)
        {
            ServiceResponse<MaterialRequestDto> serviceResponse = new ServiceResponse<MaterialRequestDto>();

            try
            {
                if (materialRequestDtoPost is null)
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
                var materialReq = _mapper.Map<MaterialRequest>(materialRequestDtoPost);
                var materialdto = materialRequestDtoPost.MaterialRequestItems;

                var materialItemList = new List<MaterialRequestItem>();
                for (int i = 0; i < materialdto.Count; i++)
                {
                    MaterialRequestItem materialItemListDetail = _mapper.Map<MaterialRequestItem>(materialdto[i]);
                    materialItemList.Add(materialItemListDetail);

                }
                materialReq.MaterialRequestItemList = materialItemList;

                _materialRequestRepository.CreateMaterialRequest(materialReq);

                _materialRequestRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
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
        public async Task<IActionResult> UpdateMaterialRequest(int id, [FromBody] MaterialRequestDtoUpdate materialRequestDtoUpdate)
        {
            ServiceResponse<MaterialRequestDto> serviceResponse = new ServiceResponse<MaterialRequestDto>();

            try
            {
                if (materialRequestDtoUpdate is null)
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
                var materialReq = await _materialRequestRepository.GetMaterialRequestById(id);
                if (materialReq is null)
                {
                    _logger.LogError($"materialReq with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update materialReq with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var MaterialReqList = _mapper.Map<MaterialRequest>(materialReq);

                var MaterialItemDto = materialRequestDtoUpdate.MaterialRequestItems;

                var MaterialItemList = new List<MaterialRequestItem>();
                for (int i = 0; i < MaterialItemDto.Count; i++)
                {
                    MaterialRequestItem materialItemDetail = _mapper.Map<MaterialRequestItem>(MaterialItemDto[i]);
                    MaterialItemList.Add(materialItemDetail);

                }

                MaterialReqList.MaterialRequestItemList = MaterialItemList;
                var data = _mapper.Map(materialRequestDtoUpdate, MaterialReqList);

                string result = await _materialRequestRepository.UpdateMaterialRequest(data);
                _materialRequestRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
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
                var materialReq = await _materialRequestRepository.GetMaterialRequestById(id);
                if (materialReq == null)
                {
                    _logger.LogError($"materialReq with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete materialReq with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _materialRequestRepository.DeleteMaterialRequest(materialReq);
                _logger.LogError(result);
                _materialRequestRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
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
                var Mrnumbers = await _materialRequestRepository.GetAllOpenMRIdNoList();
                var result = _mapper.Map<IEnumerable<MaterialRequestIdNoDto>>(Mrnumbers);
                serviceResponse.Data = result;
                serviceResponse.Message = "Success";
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
        public async Task<IActionResult> GetMRNoDetailsById(string MRNumber)
        {
            ServiceResponse<MaterialRequestDto> serviceResponse = new ServiceResponse<MaterialRequestDto>();

            try
            {
                var GetMrNo = await _materialRequestRepository.GetMRNoDetailsById(MRNumber);

                if (GetMrNo == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GetMRNoDetailsById with id: {MRNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GetMRNoDetailsById with id: {MRNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned GetMRNoDetailsById with id: {MRNumber}");

                    MaterialRequestDto materialRequestDto = _mapper.Map<MaterialRequestDto>(GetMrNo);

                    List<MaterialRequestItemDto> materialRequestItemDtos = new List<MaterialRequestItemDto>();
                    foreach (var MrNoDetails in GetMrNo.MaterialRequestItemList)
                    {
                        MaterialRequestItemDto materialRequestItemDto = _mapper.Map<MaterialRequestItemDto>(MrNoDetails);
                        materialRequestItemDtos.Add(materialRequestItemDto);
                    }
                    materialRequestDto.MaterialRequestItems = materialRequestItemDtos;
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