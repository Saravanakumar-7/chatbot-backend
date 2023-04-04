using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MaterialRequestsController : ControllerBase
    {

        private IMaterialRequestsRepository _materialRequestRepository;
        private IMapper _mapper;
        private ILoggerManager _logger;


        public MaterialRequestsController(IMaterialRequestsRepository materialRequestRepository, IMapper mapper, ILoggerManager logger)
        {
            _materialRequestRepository = materialRequestRepository;
            _mapper = mapper;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllMaterialRequest([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParammes)
        {
            ServiceResponse<IEnumerable<MaterialRequestsDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestsDto>>();

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
                var result = _mapper.Map<IEnumerable<MaterialRequestsDto>>(getAllMaterialRequest);
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
            ServiceResponse<MaterialRequestsDto> serviceResponse = new ServiceResponse<MaterialRequestsDto>();

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

                    MaterialRequestsDto materialRequestDto = _mapper.Map<MaterialRequestsDto>(materialRequestDetails);


                    List<MaterialRequestItemsDto> materialRequestItemDtos = new List<MaterialRequestItemsDto>();

                    if (materialRequestDetails.MaterialRequestItems != null)
                    {

                        foreach (var materialitemDetails in materialRequestDetails.MaterialRequestItems)
                        {
                            MaterialRequestItemsDto materialRequestItemDto = _mapper.Map<MaterialRequestItemsDto>(materialitemDetails);
                            materialRequestItemDto.MRStockDetails = _mapper.Map<List<MRStockDetailsDto>>(materialitemDetails.MRStockDetail);

                            materialRequestItemDtos.Add(materialRequestItemDto);
                        }
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateMaterialRequest([FromBody] MaterialRequestsPostDto materialRequestPostDto)
        {
            ServiceResponse<MaterialRequestsDto> serviceResponse = new ServiceResponse<MaterialRequestsDto>();

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

                var createMaterialReq = _mapper.Map<MaterialRequests>(materialRequestPostDto);
                var materialReqDto = materialRequestPostDto.MaterialRequestItems;

                var materialReqItemList = new List<MaterialRequestItems>();

                if (materialReqDto != null)
                {

                    for (int i = 0; i < materialReqDto.Count; i++)
                    {
                        MaterialRequestItems materialItemListDetail = _mapper.Map<MaterialRequestItems>(materialReqDto[i]);
                        materialItemListDetail.MRStockDetail = _mapper.Map<List<MRStockDetails>>(materialReqDto[i].MRStockDetails);
                        materialReqItemList.Add(materialItemListDetail);

                    }
                }

                createMaterialReq.MaterialRequestItems = materialReqItemList;
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));



                var newcount = await _materialRequestRepository.GetMRNumberAutoIncrementCount(date);

                if (newcount > 0)
                {
                    var number = newcount + 1;
                    string e = String.Format("{0:D4}", number);
                    createMaterialReq.MRNumber = days + months + years + "MR" + (e);
                }
                else
                {
                    var count = 1;
                    var e = count.ToString("D4");
                    createMaterialReq.MRNumber = days + months + years + "MR" + (e);
                }
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
             ServiceResponse<MaterialRequestUpdateDto> serviceResponse = new ServiceResponse<MaterialRequestUpdateDto>();


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
                var updateMaterialReqquest = _mapper.Map<MaterialRequests>(getMaterialRequest);

                var materialReqItemDto = materialRequestUpdateDto.MaterialRequestItems;

                var materialReqItemList = new List<MaterialRequestItems>();

                for (int i = 0; i < materialReqItemDto.Count; i++)
                {
                    MaterialRequestItems materialItemDetail = _mapper.Map<MaterialRequestItems>(materialReqItemDto[i]);
                    materialItemDetail.MRStockDetail = _mapper.Map<List<MRStockDetails>>(materialReqItemDto[i].MRStockDetails);

                    materialReqItemList.Add(materialItemDetail);

                }


                updateMaterialReqquest.MaterialRequestItems = materialReqItemList;
                var updateMaterialReq = _mapper.Map(materialRequestUpdateDto, getMaterialRequest);
               // updateMaterialReq.MaterialRequestItems = materialReqItemList;
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
            ServiceResponse<MaterialRequestsDto> serviceResponse = new ServiceResponse<MaterialRequestsDto>();

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
            ServiceResponse<MaterialRequestsDto> serviceResponse = new ServiceResponse<MaterialRequestsDto>();

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

                    MaterialRequestsDto materialRequestDto = _mapper.Map<MaterialRequestsDto>(getMRbyMRNo);

                    List<MaterialRequestItemsDto> materialRequestItemDtos = new List<MaterialRequestItemsDto>();
                    foreach (var materialReqbyMRNo in getMRbyMRNo.MaterialRequestItems)
                    {
                        MaterialRequestItemsDto materialRequestItemDto = _mapper.Map<MaterialRequestItemsDto>(materialReqbyMRNo);
                        materialRequestItemDto.MRStockDetails = _mapper.Map<List<MRStockDetailsDto>>(materialReqbyMRNo.MRStockDetail);

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