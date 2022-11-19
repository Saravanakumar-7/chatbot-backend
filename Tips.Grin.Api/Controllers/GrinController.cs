using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Entities;  
using AutoMapper;
using Tips.Grin.Api.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Net; 
using Entities;
using Newtonsoft.Json;
using Entities.DTOs;
using Tips.Grin.Api.Repository;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GrinController : ControllerBase
    {
        private GrinRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public GrinController(GrinRepository repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<GrinController>
        [HttpGet]

        public async Task<IActionResult> GetAllGrin([FromQuery] PagingParameter pagingParameter)

         {
            ServiceResponse<IEnumerable<GrinDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinDto>>();

            try
            {
                var listOfGrin = await _repository.GetAllGrin(pagingParameter);

                var metadata = new
                {
                    listOfGrin.TotalCount,
                    listOfGrin.PageSize,
                    listOfGrin.CurrentPage,
                    listOfGrin.HasNext,
                    listOfGrin.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all Grins");
                var result = _mapper.Map<IEnumerable<GrinDto>>(listOfGrin);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Grins Successfully";
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

        // GET api/<GrinController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGrinById(int id)
        {
            ServiceResponse<GrinDto> serviceResponse = new ServiceResponse<GrinDto>();

            try
            {
                var GrinDetails = await _repository.GetGrinById(id);

                if (GrinDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Grin with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Grin with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Grin with id: {id}");
                    var result = _mapper.Map<GrinDto>(GrinDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned Grin with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetGrinById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    

        // POST api/<GrinController>
        [HttpPost]
        public async Task<IActionResult> CreateGrin([FromBody] GrinPostDto grinPostDto)
        {
            ServiceResponse<GrinDto> serviceResponse = new ServiceResponse<GrinDto>();

            try
            {
                if (grinPostDto is null)
                {
                    _logger.LogError("Grin object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Grin object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Grin object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Grin object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var grin = _mapper.Map<IEnumerable<GrinParts>>(grinPostDto.GrinParts);
                var grins = _mapper.Map<Grins>(grinPostDto);

                grins.GrinParts = grin.ToList();

                _repository.CreateGrin(grins);

                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetGrinById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateGrin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<GrinController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrin(int id, [FromBody] GrinDto grinDto)
        {
            ServiceResponse<GrinDto> serviceResponse = new ServiceResponse<GrinDto>();

            try
            {
                if (grinDto is null)
                {
                    _logger.LogError("Update Grin object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Grin object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update Grin object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Grin object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updategrin = await _repository.GetGrinById(id);
                if (updategrin is null)
                {
                    _logger.LogError($"Update Grin with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update Grin with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var grinparts = _mapper.Map<IEnumerable<GrinParts>>(grinDto.GrinParts);
         
                var data = _mapper.Map(grinDto, updategrin);


                data.GrinParts = grinparts.ToList(); 

                string result = await _repository.UpdateGrin(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateGrin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<GrinController>/5
        [HttpDelete("{id}")] 
        public async Task<IActionResult> DeleteGrin(int id)
        {
            ServiceResponse<GrinDto> serviceResponse = new ServiceResponse<GrinDto>();

            try
            {
                var deletegrin = await _repository.GetGrinById(id);
                if (deletegrin == null)
                {
                    _logger.LogError($"Delete grin with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete grin with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteGrin(deletegrin);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteGrin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
