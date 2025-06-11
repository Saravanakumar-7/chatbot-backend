using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FieldInformationController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public FieldInformationController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllFieldInformation([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<FieldInformationDto>> serviceResponse = new ServiceResponse<IEnumerable<FieldInformationDto>>();
            try
            {

                var fieldInformationDetails = await _repository.FieldInformationRepository.GetAllFieldInformation(pagingParameter, searchParams);

                var metadata = new
                {
                    fieldInformationDetails.TotalCount,
                    fieldInformationDetails.PageSize,
                    fieldInformationDetails.CurrentPage,
                    fieldInformationDetails.HasNext,
                    fieldInformationDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all FieldInformation");
                var result = _mapper.Map<IEnumerable<FieldInformationDto>>(fieldInformationDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all FieldInformation  Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllFieldInformation API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllFieldInformation API : \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFieldInformationByIds(List<int> fieldIds)
        {
            ServiceResponse<FieldInformationDto> serviceResponse = new ServiceResponse<FieldInformationDto>();

            try
            {
                var fieldInformationDetails = await _repository.FieldInformationRepository.GetFieldInformationByIds(fieldIds);
                if (fieldInformationDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"FieldInformation with id: {fieldIds}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"FieldInformation with id: {fieldIds}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned FieldInformation with id: {fieldIds}");
                    var result = _mapper.Map<FieldInformationDto>(fieldInformationDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned FieldInformation successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetFieldInformationByIds API or the following fieldIds:{fieldIds} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetFieldInformationByIds API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
