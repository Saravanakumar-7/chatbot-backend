using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WeightUomController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public WeightUomController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<WeightUomController>
        [HttpGet]
        public async Task<IActionResult> GetAllWeightUom()
        {
            try
            {
                var WeightUom = await _repository.WeightUomRepository.GetAllActiveWeightUom();
                _logger.LogInfo("Returned all WeightUom");
                var result = _mapper.Map<IEnumerable<WeightUom>>(WeightUom);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/<WeightUomController>/5
        [HttpGet("{id}")] 
        public async Task<IActionResult> GetWeightUomById(int id)
        {
            try
            {
                var weightUom = await _repository.WeightUomRepository.GetWeightUomById(id);
                if (weightUom == null)
                {
                    _logger.LogError($"weightUom with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<WeightUomDto>(weightUom);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetweightUomById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<WeightUomController>
        [HttpPost]
        public IActionResult CreateWeightUom([FromBody] WeightUomPostDto weightUom)
        {
            try
            {
                if (weightUom is null)
                {
                    _logger.LogError("weightUom object sent from client is null.");
                    return BadRequest("weightUom object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid weightUom object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var weightUoms = _mapper.Map<WeightUom>(weightUom);
                _repository.WeightUomRepository.CreateWeightUom(weightUoms);
                _repository.SaveAsync();


                return Created("GetWeightUomById", "Successfully Created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<WeightUomController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeightUom(int id, [FromBody] WeightUomDto uomDto)
        {
            try
            {
                if (uomDto is null)
                {
                    _logger.LogError("WeightUom object sent from client is null.");
                    return BadRequest("WeightUom object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid WeightUom object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var weightUom = await _repository.WeightUomRepository.GetWeightUomById(id);
                if (weightUom is null)
                {
                    _logger.LogError($"weightUom with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(uomDto, weightUom);
                string result = await _repository.WeightUomRepository.UpdateWeightUom(weightUom);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateWeight action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<WeightUomController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeightUom(int id)
        {
            try
            {
                var weightUom = await _repository.WeightUomRepository.GetWeightUomById(id);
                if (weightUom == null)
                {
                    _logger.LogError($"weightUom with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.WeightUomRepository.DeleteWeightUom(weightUom);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateWeightUom(int id)
        {
            try
            {
                var weightUom = await _repository.WeightUomRepository.GetWeightUomById(id);
                if (weightUom is null)
                {
                    _logger.LogError($"weightUom with id: {id}, hasn't been found in db.");
                    return BadRequest("weightUom object is null");
                }
                weightUom.IsActive = true;
                string result = await _repository.WeightUomRepository.UpdateWeightUom(weightUom);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateweightUom action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateWeightUom(int id)
        {
            try
            {
                var weightUom = await _repository.WeightUomRepository.GetWeightUomById(id);
                if (weightUom is null)
                {
                    _logger.LogError($"weightUom with id: {id}, hasn't been found in db.");
                    return BadRequest("weightUom object is null");
                }
                weightUom.IsActive = false;
                string result = await _repository.WeightUomRepository.UpdateWeightUom(weightUom);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateweightUom action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
