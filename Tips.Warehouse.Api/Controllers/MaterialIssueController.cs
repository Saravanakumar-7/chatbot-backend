using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MaterialIssueController : ControllerBase
    {
        private IMaterialIssueRepository  _materialIssueRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public MaterialIssueController(IMaterialIssueRepository materialIssueRepository, ILoggerManager logger, IMapper mapper)
        {
            _materialIssueRepository = materialIssueRepository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<MaterialIssueController>
        [HttpGet]
        public async Task<IActionResult> GetAllMaterialIssue([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<MaterialIssueDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialIssueDto>>();
            try
            {
                var getAllMaterialIssue = await _materialIssueRepository.GetAllMaterialIssue(pagingParameter);
                var metadata = new
                {
                    getAllMaterialIssue.TotalCount,
                    getAllMaterialIssue.PageSize,
                    getAllMaterialIssue.CurrentPage,
                    getAllMaterialIssue.HasNext,
                    getAllMaterialIssue.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all MaterialIssues");
                var result = _mapper.Map<IEnumerable<MaterialIssueDto>>(getAllMaterialIssue);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SAShopOrderMaterialIssues";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        // GET api/<MaterialIssueController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaterialIssueById(int id)
        {
            ServiceResponse<MaterialIssueDto> serviceResponse = new ServiceResponse<MaterialIssueDto>();

            try
            {
                var getMaterialIssueById = await _materialIssueRepository.GetMaterialIssueById(id);
                if (getMaterialIssueById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MaterialIssue with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MaterialIssue with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<MaterialIssueDto>(getMaterialIssueById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned MaterialIssue with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMaterialIssueById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<MaterialIssueController>
        [HttpPost]
        public IActionResult CreateMaterialIssue([FromBody] MaterialIssueDtoPost materialIssueDtoPost)
        {
            ServiceResponse<MaterialIssueDto> serviceResponse = new ServiceResponse<MaterialIssueDto>();

            try
            {
                if (materialIssueDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "MaterialIssue object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("MaterialIssue object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid MaterialIssue object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid MaterialIssue object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var  createMaterialIssue = _mapper.Map<MaterialIssue>(materialIssueDtoPost);

                _materialIssueRepository.CreateMaterialIssue(createMaterialIssue);
                _materialIssueRepository.SaveAsync(); 
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialIssue Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;

                return Created("GetCommodityById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<MaterialIssueController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterialIssue(int id, [FromBody] MaterialIssueDtoUpdate materialIssueDtoUpdate)
        {
            ServiceResponse<MaterialIssueDto> serviceResponse = new ServiceResponse<MaterialIssueDto>();

            try
            {
                if (materialIssueDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "MaterialIssue object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("MaterialIssue object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid MaterialIssue object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid MaterialIssue object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var getMaterialIssueById = await _materialIssueRepository.GetMaterialIssueById(id);
                if (getMaterialIssueById is null)
                {
                    _logger.LogError($"MaterialIssue with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update MterialIssue with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var updateMaterialIssue = _mapper.Map(materialIssueDtoUpdate, getMaterialIssueById);
                _mapper.Map(materialIssueDtoUpdate, getMaterialIssueById);
                string result = await _materialIssueRepository.UpdateMaterialIssue(updateMaterialIssue);
                _logger.LogInfo(result);
                _materialIssueRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside UpdateCommodity action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<MaterialIssueController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialIssue(int id)
        {
            ServiceResponse<MaterialIssueDto> serviceResponse = new ServiceResponse<MaterialIssueDto>();

            try
            {
                var getMaterialIssueById = await _materialIssueRepository.GetMaterialIssueById(id);
                if (getMaterialIssueById == null)
                {
                    _logger.LogError($"Delete MaterialIssue with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete MaterialIssue with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _materialIssueRepository.DeleteMaterialIssue(getMaterialIssueById);
                _logger.LogInfo(result);
                _materialIssueRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Invoice Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteInvoice action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
